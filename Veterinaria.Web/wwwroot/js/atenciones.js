let atenciones = [];
let tiposAtencion = [];
let idEditando = null;
const modal = new bootstrap.Modal(document.getElementById('modal'));

async function cargarCombos() {
    try {
        const clientes = await llamarApi('/clientes');
        tiposAtencion = await llamarApi('/tipos-atencion');

        llenarCombo(document.getElementById('filtro-cliente'), clientes, c => c.apellido + ', ' + c.nombre, 'Todos');
        llenarCombo(document.getElementById('filtro-tipo'), tiposAtencion, t => t.nombre, 'Todos');
        llenarCombo(document.getElementById('cliente'), clientes, c => c.apellido + ', ' + c.nombre + ' (' + c.codigo + ')', 'Seleccione...');
    } catch (error) {
        alert(error.message);
    }
}

// ===== LISTADO =====

async function cargarAtenciones() {
    let url = '/atenciones?incluirInactivas=' + document.getElementById('filtro-inactivas').checked;

    const idCliente = document.getElementById('filtro-cliente').value;
    const idMascota = document.getElementById('filtro-mascota').value;
    const idTipo = document.getElementById('filtro-tipo').value;
    const desde = document.getElementById('filtro-desde').value;
    const hasta = document.getElementById('filtro-hasta').value;

    if (idCliente != '') url += '&idCliente=' + idCliente;
    if (idMascota != '') url += '&idMascota=' + idMascota;
    if (idTipo != '') url += '&idTipoAtencion=' + idTipo;
    if (desde != '') url += '&desde=' + desde;
    if (hasta != '') url += '&hasta=' + hasta;

    try {
        atenciones = await llamarApi(url);
        mostrarAtenciones();
    } catch (error) {
        alert(error.message);
    }
}

function mostrarAtenciones() {
    const tabla = document.getElementById('tabla');
    tabla.innerHTML = '';
    let total = 0;

    if (atenciones.length == 0) {
        tabla.innerHTML = '<tr><td colspan="8" class="text-center">No hay atenciones</td></tr>';
    }

    for (const a of atenciones) {
        let botones = '';
        if (a.activo) {
            botones += '<button class="btn btn-warning btn-sm" onclick="editarAtencion(' + a.id + ')">Editar</button> ';
            total += a.importe;
        }
        if (esAdmin() && a.activo) {
            botones += '<button class="btn btn-danger btn-sm" onclick="bajaAtencion(' + a.id + ')">Baja</button>';
        }
        if (esAdmin() && !a.activo) {
            botones += '<button class="btn btn-info btn-sm" onclick="reactivarAtencion(' + a.id + ')">Reactivar</button>';
        }

        tabla.innerHTML += '<tr>' +
            '<td>' + formatoFecha(a.fecha) + '</td>' +
            '<td>' + a.mascota.cliente.apellido + ', ' + a.mascota.cliente.nombre + '</td>' +
            '<td>' + a.mascota.nombre + ' (' + a.mascota.tipoMascota.nombre + ')</td>' +
            '<td>' + a.tipoAtencion.nombre + '</td>' +
            '<td>' + a.descripcion + '</td>' +
            '<td>' + formatoPrecio(a.importe) + '</td>' +
            '<td>' + estado(a.activo) + '</td>' +
            '<td>' + botones + '</td>' +
            '</tr>';
    }

    document.getElementById('total').textContent = formatoPrecio(total);
}

// el filtro de mascota depende del cliente elegido
async function cambioFiltroCliente() {
    const idCliente = document.getElementById('filtro-cliente').value;
    const filtroMascota = document.getElementById('filtro-mascota');

    if (idCliente == '') {
        filtroMascota.innerHTML = '<option value="">Todas</option>';
        filtroMascota.disabled = true;
    } else {
        const mascotas = await llamarApi('/mascotas?incluirInactivos=true&idCliente=' + idCliente);
        llenarCombo(filtroMascota, mascotas, m => m.nombre, 'Todas');
        filtroMascota.disabled = false;
    }
    cargarAtenciones();
}

// ===== FORMULARIO (cliente -> mascotas del cliente -> tipo) =====

async function cargarMascotasDelCliente(idCliente) {
    const comboMascota = document.getElementById('mascota');
    const comboTipo = document.getElementById('tipo');
    comboMascota.disabled = true;
    comboTipo.disabled = true;
    comboTipo.value = '';

    if (idCliente == '') {
        comboMascota.innerHTML = '<option value="">Primero elija un cliente</option>';
        return;
    }

    // solo trae las mascotas activas de ese cliente
    const mascotas = await llamarApi('/clientes/' + idCliente + '/mascotas');

    if (mascotas.length == 0) {
        comboMascota.innerHTML = '<option value="">El cliente no tiene mascotas activas</option>';
        return;
    }

    llenarCombo(comboMascota, mascotas, m => m.nombre + ' - ' + m.tipoMascota.nombre, 'Seleccione...');
    comboMascota.disabled = false;
}

async function cambioCliente() {
    try {
        await cargarMascotasDelCliente(document.getElementById('cliente').value);
    } catch (error) {
        alert(error.message);
    }
}

function cambioMascota() {
    // el tipo se habilita recien cuando se elige la mascota
    document.getElementById('tipo').disabled = document.getElementById('mascota').value == '';
}

async function nuevaAtencion() {
    idEditando = null;
    document.getElementById('modal-titulo').textContent = 'Nueva atención';
    document.getElementById('cliente').value = '';
    llenarCombo(document.getElementById('tipo'), tiposAtencion, t => t.nombre, 'Seleccione...');
    await cargarMascotasDelCliente('');
    document.getElementById('fecha').value = fechaHoy();
    document.getElementById('fecha').max = fechaHoy();
    document.getElementById('descripcion').value = '';
    document.getElementById('importe').value = '';
    modal.show();
}

async function editarAtencion(id) {
    const a = atenciones.find(x => x.id == id);
    idEditando = id;
    document.getElementById('modal-titulo').textContent = 'Editar atención';

    try {
        document.getElementById('cliente').value = a.mascota.idCliente;
        await cargarMascotasDelCliente(a.mascota.idCliente);
        document.getElementById('mascota').value = a.idMascota;

        llenarCombo(document.getElementById('tipo'), tiposAtencion, t => t.nombre, 'Seleccione...');
        document.getElementById('tipo').disabled = false;
        document.getElementById('tipo').value = a.idTipoAtencion;

        document.getElementById('fecha').value = a.fecha.substring(0, 10);
        document.getElementById('fecha').max = fechaHoy();
        document.getElementById('descripcion').value = a.descripcion;
        document.getElementById('importe').value = a.importe;
        modal.show();
    } catch (error) {
        alert(error.message);
    }
}

async function guardarAtencion() {
    const atencion = {
        idCliente: Number(document.getElementById('cliente').value),
        idMascota: Number(document.getElementById('mascota').value),
        idTipoAtencion: Number(document.getElementById('tipo').value),
        fecha: document.getElementById('fecha').value,
        descripcion: document.getElementById('descripcion').value.trim(),
        importe: Number(document.getElementById('importe').value)
    };

    // validaciones del formulario (la api tambien valida todo)
    if (atencion.idCliente == 0 || atencion.idMascota == 0 || atencion.idTipoAtencion == 0) {
        alert('Elija el cliente, la mascota y el tipo de atención.');
        return;
    }
    if (atencion.fecha == '' || atencion.fecha > fechaHoy()) {
        alert('La fecha es obligatoria y no puede ser futura.');
        return;
    }
    if (atencion.descripcion == '') {
        alert('La descripción es obligatoria.');
        return;
    }
    if (atencion.importe <= 0) {
        alert('El importe tiene que ser mayor a 0.');
        return;
    }

    try {
        if (idEditando == null) {
            await llamarApi('/atenciones', 'POST', atencion);
        } else {
            await llamarApi('/atenciones/' + idEditando, 'PUT', atencion);
        }
        modal.hide();
        cargarAtenciones();
    } catch (error) {
        alert(error.message);
    }
}

async function bajaAtencion(id) {
    if (!confirm('¿Seguro que quiere dar de baja la atención?')) {
        return;
    }
    try {
        await llamarApi('/atenciones/' + id, 'DELETE');
        cargarAtenciones();
    } catch (error) {
        alert(error.message);
    }
}

async function reactivarAtencion(id) {
    try {
        await llamarApi('/atenciones/' + id + '/reactivar', 'PATCH');
        cargarAtenciones();
    } catch (error) {
        alert(error.message);
    }
}

cargarCombos();
cargarAtenciones();
