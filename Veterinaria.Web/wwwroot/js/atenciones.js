// Transaccion principal: Registro de Atenciones
const tabla = document.getElementById('tabla');
const formFiltros = document.getElementById('form-filtros');
const formAtencion = document.getElementById('form-atencion');
const modal = new bootstrap.Modal(document.getElementById('modal'));

const selCliente = document.getElementById('cliente');
const selMascota = document.getElementById('mascota');
const selTipo = document.getElementById('tipo');
const inputFecha = document.getElementById('fecha');
const inputDescripcion = document.getElementById('descripcion');

let atenciones = [];
let tiposAtencion = [];
let idEditando = null;

const textoCliente = c => `${c.apellido}, ${c.nombre} (${c.codigo})`;
const textoMascota = m => `${m.nombre} - ${m.tipoMascota.nombre}`;

async function cargarCombos() {
    const [clientes, tipos] = await Promise.all([api('/clientes'), api('/tipos-atencion')]);
    tiposAtencion = tipos;

    llenarSelect(document.getElementById('filtro-cliente'), clientes, textoCliente, 'Todos');
    llenarSelect(document.getElementById('filtro-tipo'), tipos, t => t.nombre, 'Todos');
    llenarSelect(selCliente, clientes, textoCliente);
}

// ===== Seleccion dependiente: cliente -> mascotas activas de ESE cliente -> tipo =====
async function cargarMascotasDelCliente(idCliente) {
    selMascota.disabled = true;
    selTipo.disabled = true;
    selTipo.value = '';

    if (!idCliente) {
        selMascota.innerHTML = '<option value="">Primero seleccione un cliente</option>';
        return;
    }

    const mascotas = await api(`/clientes/${idCliente}/mascotas`);
    if (mascotas.length === 0) {
        selMascota.innerHTML = '<option value="">El cliente no tiene mascotas activas</option>';
        return;
    }
    llenarSelect(selMascota, mascotas, textoMascota);
    selMascota.disabled = false;
}

selCliente.addEventListener('change', () => {
    cargarMascotasDelCliente(selCliente.value).catch(error => mostrarAlerta(error.message, 'danger'));
});

// El tipo de atencion se habilita recien cuando hay una mascota elegida
selMascota.addEventListener('change', () => {
    selTipo.disabled = !selMascota.value;
});

// Filtro de mascota del listado: tambien depende del cliente
document.getElementById('filtro-cliente').addEventListener('change', async (e) => {
    const filtroMascota = document.getElementById('filtro-mascota');
    filtroMascota.innerHTML = '<option value="">Todas</option>';
    filtroMascota.disabled = true;
    if (e.target.value) {
        const mascotas = await api(`/mascotas?idCliente=${e.target.value}&incluirInactivos=true`);
        llenarSelect(filtroMascota, mascotas, m => m.nombre, 'Todas');
        filtroMascota.disabled = false;
    }
    cargarAtenciones();
});

// ===== Listado =====
async function cargarAtenciones() {
    const params = new URLSearchParams();
    const valores = {
        idCliente: document.getElementById('filtro-cliente').value,
        idMascota: document.getElementById('filtro-mascota').value,
        idTipoAtencion: document.getElementById('filtro-tipo').value,
        desde: document.getElementById('filtro-desde').value,
        hasta: document.getElementById('filtro-hasta').value
    };
    for (const clave in valores) {
        if (valores[clave]) params.append(clave, valores[clave]);
    }
    params.append('incluirInactivas', document.getElementById('filtro-inactivas').checked);

    try {
        atenciones = await api('/atenciones?' + params.toString());
        mostrarTabla();
    } catch (error) {
        mostrarAlerta(error.message, 'danger');
    }
}

function mostrarTabla() {
    if (atenciones.length === 0) {
        tabla.innerHTML = '<tr><td colspan="8" class="text-center text-muted py-4">No hay atenciones para mostrar.</td></tr>';
    } else {
        tabla.innerHTML = atenciones.map(a => `
            <tr class="${a.activo ? '' : 'fila-inactiva'}">
                <td>${formatearFecha(a.fecha)}</td>
                <td>${escaparHtml(a.mascota.cliente.apellido)}, ${escaparHtml(a.mascota.cliente.nombre)}</td>
                <td>${escaparHtml(a.mascota.nombre)} <small class="text-muted">(${escaparHtml(a.mascota.tipoMascota.nombre)})</small></td>
                <td>${escaparHtml(a.tipoAtencion.nombre)}</td>
                <td>${escaparHtml(a.descripcion)}</td>
                <td class="text-end">${formatearMoneda(a.importe)}</td>
                <td>${badgeEstado(a.activo)}</td>
                <td class="text-end acciones">${botonesAcciones(a)}</td>
            </tr>`).join('');
    }

    // El total solo suma las atenciones activas
    const activas = atenciones.filter(a => a.activo);
    const total = activas.reduce((suma, a) => suma + a.importe, 0);
    document.getElementById('total-cantidad').textContent = `${activas.length} atención(es) activa(s)`;
    document.getElementById('total-importe').textContent = formatearMoneda(total);
}

// ===== Alta / edicion =====
async function abrirModal(atencion = null) {
    formAtencion.reset();
    formAtencion.classList.remove('was-validated');
    idEditando = atencion ? atencion.id : null;
    document.getElementById('modal-titulo').textContent = atencion ? 'Editar atención' : 'Nueva atención';

    inputFecha.max = hoyIso();
    llenarSelect(selTipo, tiposAtencion, t => t.nombre);

    if (atencion) {
        selCliente.value = atencion.mascota.idCliente;
        await cargarMascotasDelCliente(atencion.mascota.idCliente);
        selMascota.value = atencion.idMascota;
        selTipo.disabled = false;
        selTipo.value = atencion.idTipoAtencion;
        inputFecha.value = atencion.fecha.substring(0, 10);
        document.getElementById('importe').value = atencion.importe;
        inputDescripcion.value = atencion.descripcion;
    } else {
        await cargarMascotasDelCliente('');
        inputFecha.value = hoyIso();
    }
    document.getElementById('contador').textContent = inputDescripcion.value.length;
    modal.show();
}

inputDescripcion.addEventListener('input', () => {
    document.getElementById('contador').textContent = inputDescripcion.value.length;
});

formAtencion.addEventListener('submit', async (e) => {
    e.preventDefault();

    // Validaciones del lado del cliente (el backend vuelve a validar todo)
    inputFecha.setCustomValidity(inputFecha.value && inputFecha.value > hoyIso() ? 'futura' : '');
    inputDescripcion.setCustomValidity(inputDescripcion.value.trim() === '' ? 'vacia' : '');

    if (!formAtencion.checkValidity()) {
        formAtencion.classList.add('was-validated');
        return;
    }

    const dto = {
        idCliente: Number(selCliente.value),
        idMascota: Number(selMascota.value),
        idTipoAtencion: Number(selTipo.value),
        fecha: inputFecha.value,
        descripcion: inputDescripcion.value.trim(),
        importe: Number(document.getElementById('importe').value)
    };

    try {
        if (idEditando) {
            await api(`/atenciones/${idEditando}`, 'PUT', dto);
            mostrarAlerta('Atención modificada correctamente.');
        } else {
            await api('/atenciones', 'POST', dto);
            mostrarAlerta('Atención registrada correctamente.');
        }
        modal.hide();
        cargarAtenciones();
    } catch (error) {
        mostrarAlerta(error.message, 'danger');
    }
});

// ===== Acciones de la tabla =====
tabla.addEventListener('click', async (e) => {
    const boton = e.target.closest('button[data-accion]');
    if (!boton) return;

    const id = Number(boton.dataset.id);
    const atencion = atenciones.find(a => a.id === id);

    try {
        if (boton.dataset.accion === 'editar') {
            await abrirModal(atencion);
        } else if (boton.dataset.accion === 'baja') {
            if (!confirm(`¿Dar de baja la atención del ${formatearFecha(atencion.fecha)} (${atencion.descripcion})?`)) return;
            await api(`/atenciones/${id}`, 'DELETE');
            mostrarAlerta('Atención dada de baja.');
            cargarAtenciones();
        } else if (boton.dataset.accion === 'reactivar') {
            await api(`/atenciones/${id}/reactivar`, 'PATCH');
            mostrarAlerta('Atención reactivada.');
            cargarAtenciones();
        }
    } catch (error) {
        mostrarAlerta(error.message, 'danger');
    }
});

formFiltros.addEventListener('submit', (e) => {
    e.preventDefault();
    cargarAtenciones();
});
['filtro-mascota', 'filtro-tipo', 'filtro-inactivas'].forEach(id =>
    document.getElementById(id).addEventListener('change', cargarAtenciones));

document.getElementById('btn-limpiar').addEventListener('click', () => {
    formFiltros.reset();
    const filtroMascota = document.getElementById('filtro-mascota');
    filtroMascota.innerHTML = '<option value="">Todas</option>';
    filtroMascota.disabled = true;
    cargarAtenciones();
});

document.getElementById('btn-nuevo').addEventListener('click', () => {
    abrirModal().catch(error => mostrarAlerta(error.message, 'danger'));
});

cargarCombos()
    .then(cargarAtenciones)
    .catch(error => mostrarAlerta(error.message, 'danger'));
