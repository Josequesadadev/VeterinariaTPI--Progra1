// ===== Configuracion y funciones comunes para hablar con la Web API =====

const API_URL = 'http://localhost:5080/api';

// ----- Sesion (el token JWT se guarda en localStorage) -----
const sesion = {
    guardar(datos) {
        localStorage.setItem('token', datos.token);
        localStorage.setItem('usuario', datos.nombreUsuario);
        localStorage.setItem('rol', datos.rol);
        localStorage.setItem('expira', datos.expira);
    },
    cerrar() {
        localStorage.removeItem('token');
        localStorage.removeItem('usuario');
        localStorage.removeItem('rol');
        localStorage.removeItem('expira');
    },
    token() {
        return localStorage.getItem('token');
    },
    usuario() {
        return localStorage.getItem('usuario');
    },
    rol() {
        return localStorage.getItem('rol');
    },
    esAdmin() {
        return localStorage.getItem('rol') === 'Admin';
    },
    estaLogueado() {
        const token = localStorage.getItem('token');
        const expira = localStorage.getItem('expira');
        return token !== null && expira !== null && new Date(expira) > new Date();
    }
};

// Si no hay sesion valida, vuelve al login
function requiereLogin() {
    if (!sesion.estaLogueado()) {
        sesion.cerrar();
        window.location.href = 'login.html';
    }
}

// ----- Llamada generica a la API: agrega el token y maneja los errores -----
async function api(ruta, metodo = 'GET', cuerpo = null) {
    const opciones = {
        method: metodo,
        headers: { 'Content-Type': 'application/json' }
    };

    if (sesion.token()) {
        opciones.headers['Authorization'] = 'Bearer ' + sesion.token();
    }
    if (cuerpo !== null) {
        opciones.body = JSON.stringify(cuerpo);
    }

    let respuesta;
    try {
        respuesta = await fetch(API_URL + ruta, opciones);
    } catch (error) {
        throw new Error('No se pudo conectar con la API. Verifique que el proyecto Veterinaria.API esté ejecutándose.');
    }

    // 401: token vencido o invalido -> al login (salvo en el propio login)
    if (respuesta.status === 401 && ruta !== '/auth/login') {
        sesion.cerrar();
        window.location.href = 'login.html';
        throw new Error('La sesión expiró.');
    }
    if (respuesta.status === 403) {
        throw new Error('No tiene permisos para realizar esta acción (solo el administrador).');
    }
    if (respuesta.status === 204) {
        return null;
    }

    const datos = await respuesta.json().catch(() => null);

    if (!respuesta.ok) {
        throw new Error(datos && datos.mensaje ? datos.mensaje : 'Error ' + respuesta.status);
    }
    return datos;
}

// ----- Utilidades -----
function mostrarAlerta(mensaje, tipo = 'success') {
    let contenedor = document.getElementById('contenedor-alertas');
    if (!contenedor) {
        contenedor = document.createElement('div');
        contenedor.id = 'contenedor-alertas';
        document.body.appendChild(contenedor);
    }
    const alerta = document.createElement('div');
    alerta.className = `alert alert-${tipo} alert-dismissible fade show shadow-sm`;
    alerta.innerHTML = `${escaparHtml(mensaje)}<button type="button" class="btn-close" data-bs-dismiss="alert"></button>`;
    contenedor.appendChild(alerta);
    setTimeout(() => alerta.remove(), 5000);
}

function escaparHtml(texto) {
    const div = document.createElement('div');
    div.textContent = texto ?? '';
    return div.innerHTML;
}

function formatearMoneda(valor) {
    return Number(valor).toLocaleString('es-AR', { style: 'currency', currency: 'ARS' });
}

// "2026-11-08T00:00:00" -> "08/11/2026"
function formatearFecha(fechaIso) {
    const [anio, mes, dia] = fechaIso.substring(0, 10).split('-');
    return `${dia}/${mes}/${anio}`;
}

// Fecha de hoy en formato yyyy-mm-dd (para los input type="date")
function hoyIso() {
    const hoy = new Date();
    hoy.setMinutes(hoy.getMinutes() - hoy.getTimezoneOffset());
    return hoy.toISOString().substring(0, 10);
}

function badgeEstado(activo) {
    return activo
        ? '<span class="badge text-bg-success">Activo</span>'
        : '<span class="badge text-bg-secondary">Inactivo</span>';
}

// Llena un <select> con una lista de objetos
function llenarSelect(select, lista, textoOpcion, textoVacio = 'Seleccione...') {
    select.innerHTML = `<option value="">${textoVacio}</option>`;
    lista.forEach(item => {
        const opcion = document.createElement('option');
        opcion.value = item.id;
        opcion.textContent = textoOpcion(item);
        select.appendChild(opcion);
    });
}

// Botones de editar / baja / reactivar. La baja y la reactivacion solo las ve el administrador.
function botonesAcciones(item) {
    let html = `<button class="btn btn-sm btn-outline-primary me-1" data-accion="editar" data-id="${item.id}" title="Editar"
                    ${item.activo ? '' : 'disabled'}><i class="bi bi-pencil"></i></button>`;
    if (sesion.esAdmin()) {
        html += item.activo
            ? `<button class="btn btn-sm btn-outline-danger" data-accion="baja" data-id="${item.id}" title="Dar de baja"><i class="bi bi-trash"></i></button>`
            : `<button class="btn btn-sm btn-outline-success" data-accion="reactivar" data-id="${item.id}" title="Reactivar"><i class="bi bi-arrow-counterclockwise"></i></button>`;
    }
    return html;
}
