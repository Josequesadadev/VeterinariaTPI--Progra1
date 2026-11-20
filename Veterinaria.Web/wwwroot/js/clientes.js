let clientes = [];
let idEditando = null;
const modal = new bootstrap.Modal(document.getElementById('modal'));

async function cargarClientes() {
    const texto = document.getElementById('filtro-texto').value;
    const inactivos = document.getElementById('filtro-inactivos').checked;

    try {
        clientes = await llamarApi('/clientes?texto=' + texto + '&incluirInactivos=' + inactivos);
        mostrarClientes();
    } catch (error) {
        alert(error.message);
    }
}

function mostrarClientes() {
    const tabla = document.getElementById('tabla');
    tabla.innerHTML = '';

    if (clientes.length == 0) {
        tabla.innerHTML = '<tr><td colspan="6" class="text-center">No hay clientes</td></tr>';
        return;
    }

    for (const c of clientes) {
        let botones = '';
        if (c.activo) {
            botones += '<button class="btn btn-warning btn-sm" onclick="editarCliente(' + c.id + ')">Editar</button> ';
        }
        if (esAdmin() && c.activo) {
            botones += '<button class="btn btn-danger btn-sm" onclick="bajaCliente(' + c.id + ')">Baja</button>';
        }
        if (esAdmin() && !c.activo) {
            botones += '<button class="btn btn-info btn-sm" onclick="reactivarCliente(' + c.id + ')">Reactivar</button>';
        }

        tabla.innerHTML += '<tr>' +
            '<td>' + c.codigo + '</td>' +
            '<td>' + c.apellido + ', ' + c.nombre + '</td>' +
            '<td>' + c.sexo + '</td>' +
            '<td>' + (c.telefono ?? '') + '</td>' +
            '<td>' + estado(c.activo) + '</td>' +
            '<td>' + botones + '</td>' +
            '</tr>';
    }
}

function nuevoCliente() {
    idEditando = null;
    document.getElementById('modal-titulo').textContent = 'Nuevo cliente';
    document.getElementById('codigo').value = '';
    document.getElementById('nombre').value = '';
    document.getElementById('apellido').value = '';
    document.getElementById('sexo').value = '';
    document.getElementById('telefono').value = '';
    modal.show();
}

function editarCliente(id) {
    const c = clientes.find(x => x.id == id);
    idEditando = id;
    document.getElementById('modal-titulo').textContent = 'Editar cliente';
    document.getElementById('codigo').value = c.codigo;
    document.getElementById('nombre').value = c.nombre;
    document.getElementById('apellido').value = c.apellido;
    document.getElementById('telefono').value = c.telefono ?? '';

    if (c.sexo == 'Masculino') document.getElementById('sexo').value = 'M';
    if (c.sexo == 'Femenino') document.getElementById('sexo').value = 'F';
    if (c.sexo == 'Otro') document.getElementById('sexo').value = 'X';

    modal.show();
}

async function guardarCliente() {
    const cliente = {
        codigo: document.getElementById('codigo').value,
        nombre: document.getElementById('nombre').value,
        apellido: document.getElementById('apellido').value,
        sexo: document.getElementById('sexo').value,
        telefono: document.getElementById('telefono').value
    };

    if (cliente.codigo == '' || cliente.nombre == '' || cliente.apellido == '' || cliente.sexo == '') {
        alert('Complete código, nombre, apellido y sexo.');
        return;
    }

    try {
        if (idEditando == null) {
            await llamarApi('/clientes', 'POST', cliente);
        } else {
            await llamarApi('/clientes/' + idEditando, 'PUT', cliente);
        }
        modal.hide();
        cargarClientes();
    } catch (error) {
        alert(error.message);
    }
}

async function bajaCliente(id) {
    if (!confirm('¿Seguro que quiere dar de baja el cliente?')) {
        return;
    }
    try {
        await llamarApi('/clientes/' + id, 'DELETE');
        cargarClientes();
    } catch (error) {
        alert(error.message);
    }
}

async function reactivarCliente(id) {
    try {
        await llamarApi('/clientes/' + id + '/reactivar', 'PATCH');
        cargarClientes();
    } catch (error) {
        alert(error.message);
    }
}

cargarClientes();
