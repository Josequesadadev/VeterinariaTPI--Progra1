const API_URL = 'http://localhost:5080/api';

// si no hay sesion o el token vencio, vuelve al login
if (localStorage.getItem('token') == null || new Date(localStorage.getItem('expira')) < new Date()) {
    localStorage.clear();
    window.location.href = 'login.html';
}

// muestra el usuario logueado en el menu
document.addEventListener('DOMContentLoaded', () => {
    const span = document.getElementById('usuario-logueado');
    if (span) {
        span.textContent = localStorage.getItem('usuario') + ' (' + localStorage.getItem('rol') + ')';
    }
});

function cerrarSesion() {
    localStorage.clear();
    window.location.href = 'index.html';
}

function esAdmin() {
    return localStorage.getItem('rol') == 'Admin';
}

// funcion para llamar a la api con el token
async function llamarApi(ruta, metodo = 'GET', datos = null) {
    const opciones = {
        method: metodo,
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + localStorage.getItem('token')
        }
    };
    if (datos != null) {
        opciones.body = JSON.stringify(datos);
    }

    const respuesta = await fetch(API_URL + ruta, opciones);

    if (respuesta.status == 401) {
        cerrarSesion();
        return;
    }
    if (respuesta.status == 403) {
        throw new Error('No tiene permisos para hacer esto (solo el administrador).');
    }
    if (respuesta.status == 204) {
        return null;
    }

    const json = await respuesta.json();
    if (!respuesta.ok) {
        throw new Error(json.mensaje);
    }
    return json;
}

function llenarCombo(select, lista, texto, primeraOpcion) {
    select.innerHTML = '<option value="">' + primeraOpcion + '</option>';
    for (const item of lista) {
        select.innerHTML += '<option value="' + item.id + '">' + texto(item) + '</option>';
    }
}

function formatoPrecio(numero) {
    return '$ ' + numero.toLocaleString('es-AR', { minimumFractionDigits: 2 });
}

// "2026-11-08T00:00:00" -> "08/11/2026"
function formatoFecha(fecha) {
    const partes = fecha.substring(0, 10).split('-');
    return partes[2] + '/' + partes[1] + '/' + partes[0];
}

// fecha de hoy en formato yyyy-mm-dd
function fechaHoy() {
    const hoy = new Date();
    const mes = String(hoy.getMonth() + 1).padStart(2, '0');
    const dia = String(hoy.getDate()).padStart(2, '0');
    return hoy.getFullYear() + '-' + mes + '-' + dia;
}

function estado(activo) {
    if (activo) {
        return '<span class="badge bg-success">Activo</span>';
    }
    return '<span class="badge bg-secondary">Inactivo</span>';
}
