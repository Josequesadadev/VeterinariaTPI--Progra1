let tipos = [];
let idEditando = null;
const modal = new bootstrap.Modal(document.getElementById('modal'));

async function cargarTipos() {
    const inactivos = document.getElementById('filtro-inactivos').checked;
    try {
        tipos = await llamarApi('/tipos-mascota?incluirInactivos=' + inactivos);
        mostrarTipos();
    } catch (error) {
        alert(error.message);
    }
}

function mostrarTipos() {
    const tabla = document.getElementById('tabla');
    tabla.innerHTML = '';

    for (const t of tipos) {
        let botones = '';
        if (t.activo) {
            botones += '<button class="btn btn-warning btn-sm" onclick="editarTipo(' + t.id + ')">Editar</button> ';
        }
        if (esAdmin() && t.activo) {
            botones += '<button class="btn btn-danger btn-sm" onclick="bajaTipo(' + t.id + ')">Baja</button>';
        }
        if (esAdmin() && !t.activo) {
            botones += '<button class="btn btn-info btn-sm" onclick="reactivarTipo(' + t.id + ')">Reactivar</button>';
        }

        tabla.innerHTML += '<tr>' +
            '<td>' + t.id + '</td>' +
            '<td>' + t.nombre + '</td>' +
            '<td>' + estado(t.activo) + '</td>' +
            '<td>' + botones + '</td>' +
            '</tr>';
    }
}

function nuevoTipo() {
    idEditando = null;
    document.getElementById('modal-titulo').textContent = 'Nuevo tipo de mascota';
    document.getElementById('nombre').value = '';
    modal.show();
}

function editarTipo(id) {
    const t = tipos.find(x => x.id == id);
    idEditando = id;
    document.getElementById('modal-titulo').textContent = 'Editar tipo de mascota';
    document.getElementById('nombre').value = t.nombre;
    modal.show();
}

async function guardarTipo() {
    const tipo = { nombre: document.getElementById('nombre').value };

    if (tipo.nombre == '') {
        alert('Ingrese el nombre.');
        return;
    }

    try {
        if (idEditando == null) {
            await llamarApi('/tipos-mascota', 'POST', tipo);
        } else {
            await llamarApi('/tipos-mascota/' + idEditando, 'PUT', tipo);
        }
        modal.hide();
        cargarTipos();
    } catch (error) {
        alert(error.message);
    }
}

async function bajaTipo(id) {
    if (!confirm('¿Seguro que quiere dar de baja este tipo?')) {
        return;
    }
    try {
        await llamarApi('/tipos-mascota/' + id, 'DELETE');
        cargarTipos();
    } catch (error) {
        alert(error.message);
    }
}

async function reactivarTipo(id) {
    try {
        await llamarApi('/tipos-mascota/' + id + '/reactivar', 'PATCH');
        cargarTipos();
    } catch (error) {
        alert(error.message);
    }
}

cargarTipos();
