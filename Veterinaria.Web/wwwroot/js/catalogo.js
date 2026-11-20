// JS comun para las tablas de soporte con solo nombre (tipos de mascota y tipos de atencion).
// Cada pagina indica en el <body> el endpoint y el nombre a mostrar.
const endpoint = document.body.dataset.endpoint;
const nombreEntidad = document.body.dataset.nombre;

const tabla = document.getElementById('tabla');
const formCatalogo = document.getElementById('form-catalogo');
const modal = new bootstrap.Modal(document.getElementById('modal'));

let registros = [];
let idEditando = null;

async function cargar() {
    const inactivos = document.getElementById('filtro-inactivos').checked;
    try {
        registros = await api(`${endpoint}?incluirInactivos=${inactivos}`);
        mostrarTabla();
    } catch (error) {
        mostrarAlerta(error.message, 'danger');
    }
}

function mostrarTabla() {
    if (registros.length === 0) {
        tabla.innerHTML = '<tr><td colspan="4" class="text-center text-muted py-4">No hay registros para mostrar.</td></tr>';
        return;
    }
    tabla.innerHTML = registros.map(r => `
        <tr class="${r.activo ? '' : 'fila-inactiva'}">
            <td>${r.id}</td>
            <td>${escaparHtml(r.nombre)}</td>
            <td>${badgeEstado(r.activo)}</td>
            <td class="text-end acciones">${botonesAcciones(r)}</td>
        </tr>`).join('');
}

function abrirModal(registro = null) {
    formCatalogo.reset();
    formCatalogo.classList.remove('was-validated');
    idEditando = registro ? registro.id : null;
    document.getElementById('modal-titulo').textContent = (registro ? 'Editar ' : 'Nuevo ') + nombreEntidad;
    if (registro) {
        document.getElementById('nombre').value = registro.nombre;
    }
    modal.show();
}

formCatalogo.addEventListener('submit', async (e) => {
    e.preventDefault();
    if (!formCatalogo.checkValidity()) {
        formCatalogo.classList.add('was-validated');
        return;
    }

    const dto = { nombre: document.getElementById('nombre').value.trim() };
    try {
        if (idEditando) {
            await api(`${endpoint}/${idEditando}`, 'PUT', dto);
            mostrarAlerta('Registro modificado correctamente.');
        } else {
            await api(endpoint, 'POST', dto);
            mostrarAlerta('Registro creado correctamente.');
        }
        modal.hide();
        cargar();
    } catch (error) {
        mostrarAlerta(error.message, 'danger');
    }
});

tabla.addEventListener('click', async (e) => {
    const boton = e.target.closest('button[data-accion]');
    if (!boton) return;

    const id = Number(boton.dataset.id);
    const registro = registros.find(r => r.id === id);

    try {
        if (boton.dataset.accion === 'editar') {
            abrirModal(registro);
        } else if (boton.dataset.accion === 'baja') {
            if (!confirm(`¿Dar de baja "${registro.nombre}"?`)) return;
            await api(`${endpoint}/${id}`, 'DELETE');
            mostrarAlerta('Registro dado de baja.');
            cargar();
        } else if (boton.dataset.accion === 'reactivar') {
            await api(`${endpoint}/${id}/reactivar`, 'PATCH');
            mostrarAlerta('Registro reactivado.');
            cargar();
        }
    } catch (error) {
        mostrarAlerta(error.message, 'danger');
    }
});

document.getElementById('filtro-inactivos').addEventListener('change', cargar);
document.getElementById('btn-nuevo').addEventListener('click', () => abrirModal());

cargar();
