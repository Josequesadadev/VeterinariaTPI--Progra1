const tabla = document.getElementById('tabla');
const formFiltros = document.getElementById('form-filtros');
const formMascota = document.getElementById('form-mascota');
const modal = new bootstrap.Modal(document.getElementById('modal'));

let mascotas = [];
let idEditando = null;

// Carga los combos con clientes y tipos activos
async function cargarCombos() {
    const [clientes, tipos] = await Promise.all([api('/clientes'), api('/tipos-mascota')]);
    const textoCliente = c => `${c.apellido}, ${c.nombre} (${c.codigo})`;

    llenarSelect(document.getElementById('filtro-cliente'), clientes, textoCliente, 'Todos');
    llenarSelect(document.getElementById('filtro-tipo'), tipos, t => t.nombre, 'Todos');
    llenarSelect(document.getElementById('cliente'), clientes, textoCliente);
    llenarSelect(document.getElementById('tipo'), tipos, t => t.nombre);
}

async function cargarMascotas() {
    const params = new URLSearchParams();
    const idCliente = document.getElementById('filtro-cliente').value;
    const idTipo = document.getElementById('filtro-tipo').value;
    const nombre = document.getElementById('filtro-nombre').value.trim();
    if (idCliente) params.append('idCliente', idCliente);
    if (idTipo) params.append('idTipoMascota', idTipo);
    if (nombre) params.append('nombre', nombre);
    params.append('incluirInactivos', document.getElementById('filtro-inactivos').checked);

    try {
        mascotas = await api('/mascotas?' + params.toString());
        mostrarTabla();
    } catch (error) {
        mostrarAlerta(error.message, 'danger');
    }
}

function mostrarTabla() {
    if (mascotas.length === 0) {
        tabla.innerHTML = '<tr><td colspan="6" class="text-center text-muted py-4">No hay mascotas para mostrar.</td></tr>';
        return;
    }
    tabla.innerHTML = mascotas.map(m => `
        <tr class="${m.activo ? '' : 'fila-inactiva'}">
            <td>${escaparHtml(m.nombre)}</td>
            <td>${escaparHtml(m.tipoMascota.nombre)}</td>
            <td>${m.edad} ${m.edad === 1 ? 'año' : 'años'}</td>
            <td>${escaparHtml(m.cliente.apellido)}, ${escaparHtml(m.cliente.nombre)}</td>
            <td>${badgeEstado(m.activo)}</td>
            <td class="text-end acciones">${botonesAcciones(m)}</td>
        </tr>`).join('');
}

function abrirModal(mascota = null) {
    formMascota.reset();
    formMascota.classList.remove('was-validated');
    idEditando = mascota ? mascota.id : null;
    document.getElementById('modal-titulo').textContent = mascota ? 'Editar mascota' : 'Nueva mascota';

    if (mascota) {
        document.getElementById('nombre').value = mascota.nombre;
        document.getElementById('edad').value = mascota.edad;
        document.getElementById('tipo').value = mascota.idTipoMascota;
        document.getElementById('cliente').value = mascota.idCliente;
    }
    modal.show();
}

formMascota.addEventListener('submit', async (e) => {
    e.preventDefault();
    if (!formMascota.checkValidity()) {
        formMascota.classList.add('was-validated');
        return;
    }

    const dto = {
        nombre: document.getElementById('nombre').value.trim(),
        edad: Number(document.getElementById('edad').value),
        idTipoMascota: Number(document.getElementById('tipo').value),
        idCliente: Number(document.getElementById('cliente').value)
    };

    try {
        if (idEditando) {
            await api(`/mascotas/${idEditando}`, 'PUT', dto);
            mostrarAlerta('Mascota modificada correctamente.');
        } else {
            await api('/mascotas', 'POST', dto);
            mostrarAlerta('Mascota creada correctamente.');
        }
        modal.hide();
        cargarMascotas();
    } catch (error) {
        mostrarAlerta(error.message, 'danger');
    }
});

tabla.addEventListener('click', async (e) => {
    const boton = e.target.closest('button[data-accion]');
    if (!boton) return;

    const id = Number(boton.dataset.id);
    const mascota = mascotas.find(m => m.id === id);

    try {
        if (boton.dataset.accion === 'editar') {
            abrirModal(mascota);
        } else if (boton.dataset.accion === 'baja') {
            if (!confirm(`¿Dar de baja a ${mascota.nombre}?`)) return;
            await api(`/mascotas/${id}`, 'DELETE');
            mostrarAlerta('Mascota dada de baja.');
            cargarMascotas();
        } else if (boton.dataset.accion === 'reactivar') {
            await api(`/mascotas/${id}/reactivar`, 'PATCH');
            mostrarAlerta('Mascota reactivada.');
            cargarMascotas();
        }
    } catch (error) {
        mostrarAlerta(error.message, 'danger');
    }
});

formFiltros.addEventListener('submit', (e) => {
    e.preventDefault();
    cargarMascotas();
});
document.getElementById('filtro-cliente').addEventListener('change', cargarMascotas);
document.getElementById('filtro-tipo').addEventListener('change', cargarMascotas);
document.getElementById('filtro-inactivos').addEventListener('change', cargarMascotas);
document.getElementById('btn-nuevo').addEventListener('click', () => abrirModal());

cargarCombos()
    .then(cargarMascotas)
    .catch(error => mostrarAlerta(error.message, 'danger'));
