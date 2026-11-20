const tabla = document.getElementById('tabla');
const formFiltros = document.getElementById('form-filtros');
const formCliente = document.getElementById('form-cliente');
const modal = new bootstrap.Modal(document.getElementById('modal'));
const letraSexo = { Masculino: 'M', Femenino: 'F', Otro: 'X' };

let clientes = [];
let idEditando = null;

async function cargarClientes() {
    const texto = document.getElementById('filtro-texto').value.trim();
    const inactivos = document.getElementById('filtro-inactivos').checked;
    try {
        clientes = await api(`/clientes?texto=${encodeURIComponent(texto)}&incluirInactivos=${inactivos}`);
        mostrarTabla();
    } catch (error) {
        mostrarAlerta(error.message, 'danger');
    }
}

function mostrarTabla() {
    if (clientes.length === 0) {
        tabla.innerHTML = '<tr><td colspan="6" class="text-center text-muted py-4">No hay clientes para mostrar.</td></tr>';
        return;
    }
    tabla.innerHTML = clientes.map(c => `
        <tr class="${c.activo ? '' : 'fila-inactiva'}">
            <td>${escaparHtml(c.codigo)}</td>
            <td>${escaparHtml(c.apellido)}, ${escaparHtml(c.nombre)}</td>
            <td>${escaparHtml(c.sexo)}</td>
            <td>${escaparHtml(c.telefono ?? '-')}</td>
            <td>${badgeEstado(c.activo)}</td>
            <td class="text-end acciones">${botonesAcciones(c)}</td>
        </tr>`).join('');
}

function abrirModal(cliente = null) {
    formCliente.reset();
    formCliente.classList.remove('was-validated');
    idEditando = cliente ? cliente.id : null;
    document.getElementById('modal-titulo').textContent = cliente ? 'Editar cliente' : 'Nuevo cliente';

    if (cliente) {
        document.getElementById('codigo').value = cliente.codigo;
        document.getElementById('nombre').value = cliente.nombre;
        document.getElementById('apellido').value = cliente.apellido;
        document.getElementById('sexo').value = letraSexo[cliente.sexo];
        document.getElementById('telefono').value = cliente.telefono ?? '';
    }
    modal.show();
}

formCliente.addEventListener('submit', async (e) => {
    e.preventDefault();
    if (!formCliente.checkValidity()) {
        formCliente.classList.add('was-validated');
        return;
    }

    const dto = {
        codigo: document.getElementById('codigo').value.trim(),
        nombre: document.getElementById('nombre').value.trim(),
        apellido: document.getElementById('apellido').value.trim(),
        sexo: document.getElementById('sexo').value,
        telefono: document.getElementById('telefono').value.trim() || null
    };

    try {
        if (idEditando) {
            await api(`/clientes/${idEditando}`, 'PUT', dto);
            mostrarAlerta('Cliente modificado correctamente.');
        } else {
            await api('/clientes', 'POST', dto);
            mostrarAlerta('Cliente creado correctamente.');
        }
        modal.hide();
        cargarClientes();
    } catch (error) {
        mostrarAlerta(error.message, 'danger');
    }
});

// Un solo listener para todos los botones de la tabla
tabla.addEventListener('click', async (e) => {
    const boton = e.target.closest('button[data-accion]');
    if (!boton) return;

    const id = Number(boton.dataset.id);
    const cliente = clientes.find(c => c.id === id);

    try {
        if (boton.dataset.accion === 'editar') {
            abrirModal(cliente);
        } else if (boton.dataset.accion === 'baja') {
            if (!confirm(`¿Dar de baja al cliente ${cliente.apellido}, ${cliente.nombre}?`)) return;
            await api(`/clientes/${id}`, 'DELETE');
            mostrarAlerta('Cliente dado de baja.');
            cargarClientes();
        } else if (boton.dataset.accion === 'reactivar') {
            await api(`/clientes/${id}/reactivar`, 'PATCH');
            mostrarAlerta('Cliente reactivado.');
            cargarClientes();
        }
    } catch (error) {
        mostrarAlerta(error.message, 'danger');
    }
});

formFiltros.addEventListener('submit', (e) => {
    e.preventDefault();
    cargarClientes();
});
document.getElementById('filtro-inactivos').addEventListener('change', cargarClientes);
document.getElementById('btn-nuevo').addEventListener('click', () => abrirModal());

cargarClientes();
