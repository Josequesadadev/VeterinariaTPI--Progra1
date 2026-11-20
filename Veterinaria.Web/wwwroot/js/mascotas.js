let mascotas = [];
let idEditando = null;
const modal = new bootstrap.Modal(document.getElementById('modal'));

async function cargarCombos() {
    try {
        const clientes = await llamarApi('/clientes');
        const tipos = await llamarApi('/tipos-mascota');

        llenarCombo(document.getElementById('filtro-cliente'), clientes, c => c.apellido + ', ' + c.nombre, 'Todos los clientes');
        llenarCombo(document.getElementById('filtro-tipo'), tipos, t => t.nombre, 'Todos los tipos');
        llenarCombo(document.getElementById('cliente'), clientes, c => c.apellido + ', ' + c.nombre, 'Seleccione...');
        llenarCombo(document.getElementById('tipo'), tipos, t => t.nombre, 'Seleccione...');
    } catch (error) {
        alert(error.message);
    }
}

async function cargarMascotas() {
    const idCliente = document.getElementById('filtro-cliente').value;
    const idTipo = document.getElementById('filtro-tipo').value;
    const nombre = document.getElementById('filtro-nombre').value;
    const inactivos = document.getElementById('filtro-inactivos').checked;

    let url = '/mascotas?incluirInactivos=' + inactivos;
    if (idCliente != '') url += '&idCliente=' + idCliente;
    if (idTipo != '') url += '&idTipoMascota=' + idTipo;
    if (nombre != '') url += '&nombre=' + nombre;

    try {
        mascotas = await llamarApi(url);
        mostrarMascotas();
    } catch (error) {
        alert(error.message);
    }
}

function mostrarMascotas() {
    const tabla = document.getElementById('tabla');
    tabla.innerHTML = '';

    if (mascotas.length == 0) {
        tabla.innerHTML = '<tr><td colspan="6" class="text-center">No hay mascotas</td></tr>';
        return;
    }

    for (const m of mascotas) {
        let botones = '';
        if (m.activo) {
            botones += '<button class="btn btn-warning btn-sm" onclick="editarMascota(' + m.id + ')">Editar</button> ';
        }
        if (esAdmin() && m.activo) {
            botones += '<button class="btn btn-danger btn-sm" onclick="bajaMascota(' + m.id + ')">Baja</button>';
        }
        if (esAdmin() && !m.activo) {
            botones += '<button class="btn btn-info btn-sm" onclick="reactivarMascota(' + m.id + ')">Reactivar</button>';
        }

        tabla.innerHTML += '<tr>' +
            '<td>' + m.nombre + '</td>' +
            '<td>' + m.tipoMascota.nombre + '</td>' +
            '<td>' + m.edad + '</td>' +
            '<td>' + m.cliente.apellido + ', ' + m.cliente.nombre + '</td>' +
            '<td>' + estado(m.activo) + '</td>' +
            '<td>' + botones + '</td>' +
            '</tr>';
    }
}

function nuevaMascota() {
    idEditando = null;
    document.getElementById('modal-titulo').textContent = 'Nueva mascota';
    document.getElementById('nombre').value = '';
    document.getElementById('edad').value = '';
    document.getElementById('tipo').value = '';
    document.getElementById('cliente').value = '';
    modal.show();
}

function editarMascota(id) {
    const m = mascotas.find(x => x.id == id);
    idEditando = id;
    document.getElementById('modal-titulo').textContent = 'Editar mascota';
    document.getElementById('nombre').value = m.nombre;
    document.getElementById('edad').value = m.edad;
    document.getElementById('tipo').value = m.idTipoMascota;
    document.getElementById('cliente').value = m.idCliente;
    modal.show();
}

async function guardarMascota() {
    const mascota = {
        nombre: document.getElementById('nombre').value,
        edad: Number(document.getElementById('edad').value),
        idTipoMascota: Number(document.getElementById('tipo').value),
        idCliente: Number(document.getElementById('cliente').value)
    };

    if (mascota.nombre == '' || document.getElementById('edad').value == '' || mascota.idTipoMascota == 0 || mascota.idCliente == 0) {
        alert('Complete todos los datos.');
        return;
    }
    if (mascota.edad < 0) {
        alert('La edad no puede ser negativa.');
        return;
    }

    try {
        if (idEditando == null) {
            await llamarApi('/mascotas', 'POST', mascota);
        } else {
            await llamarApi('/mascotas/' + idEditando, 'PUT', mascota);
        }
        modal.hide();
        cargarMascotas();
    } catch (error) {
        alert(error.message);
    }
}

async function bajaMascota(id) {
    if (!confirm('¿Seguro que quiere dar de baja la mascota?')) {
        return;
    }
    try {
        await llamarApi('/mascotas/' + id, 'DELETE');
        cargarMascotas();
    } catch (error) {
        alert(error.message);
    }
}

async function reactivarMascota(id) {
    try {
        await llamarApi('/mascotas/' + id + '/reactivar', 'PATCH');
        cargarMascotas();
    } catch (error) {
        alert(error.message);
    }
}

cargarCombos();
cargarMascotas();
