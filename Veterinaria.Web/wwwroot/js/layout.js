// Arma el menu de navegacion de las paginas internas (usuario logueado)
requiereLogin();

function armarMenu() {
    const paginaActual = window.location.pathname.split('/').pop();
    const activo = pagina => paginaActual === pagina ? 'active' : '';

    const nav = document.createElement('nav');
    nav.className = 'navbar navbar-expand-lg navbar-dark navbar-vet shadow-sm';
    nav.innerHTML = `
        <div class="container">
            <a class="navbar-brand fw-bold" href="principal.html"><i class="bi bi-heart-pulse"></i> Veterinaria</a>
            <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#menu">
                <span class="navbar-toggler-icon"></span>
            </button>
            <div class="collapse navbar-collapse" id="menu">
                <ul class="navbar-nav me-auto">
                    <li class="nav-item dropdown">
                        <a class="nav-link dropdown-toggle" href="#" data-bs-toggle="dropdown">Soporte</a>
                        <ul class="dropdown-menu">
                            <li><a class="dropdown-item ${activo('clientes.html')}" href="clientes.html">Clientes</a></li>
                            <li><a class="dropdown-item ${activo('mascotas.html')}" href="mascotas.html">Mascotas</a></li>
                            <li><hr class="dropdown-divider"></li>
                            <li><a class="dropdown-item ${activo('tipos-mascota.html')}" href="tipos-mascota.html">Tipos de mascota</a></li>
                            <li><a class="dropdown-item ${activo('tipos-atencion.html')}" href="tipos-atencion.html">Tipos de atención</a></li>
                        </ul>
                    </li>
                    <li class="nav-item dropdown">
                        <a class="nav-link dropdown-toggle" href="#" data-bs-toggle="dropdown">Transacción</a>
                        <ul class="dropdown-menu">
                            <li><a class="dropdown-item ${activo('atenciones.html')}" href="atenciones.html">Registro de atenciones</a></li>
                        </ul>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link ${activo('principal.html')}" href="principal.html">Dashboard</a>
                    </li>
                    <li class="nav-item dropdown">
                        <a class="nav-link dropdown-toggle" href="#" data-bs-toggle="dropdown">Acerca de</a>
                        <ul class="dropdown-menu">
                            <li><a class="dropdown-item ${activo('acerca.html')}" href="acerca.html">Equipo de desarrollo</a></li>
                        </ul>
                    </li>
                </ul>
                <div class="d-flex align-items-center gap-2 text-white">
                    <i class="bi bi-person-circle"></i>
                    <span>${escaparHtml(sesion.usuario())}</span>
                    <span class="badge ${sesion.esAdmin() ? 'text-bg-warning' : 'text-bg-light'}">${escaparHtml(sesion.rol())}</span>
                    <button id="btn-salir" class="btn btn-sm btn-outline-light ms-2"><i class="bi bi-box-arrow-right"></i> Salir</button>
                </div>
            </div>
        </div>`;
    document.body.prepend(nav);

    const footer = document.createElement('footer');
    footer.className = 'text-center text-muted py-3';
    footer.textContent = 'Veterinaria · Trabajo Práctico Integrador · Programación II · UTN FRC';
    document.body.appendChild(footer);

    document.getElementById('btn-salir').addEventListener('click', () => {
        sesion.cerrar();
        window.location.href = 'index.html';
    });
}

armarMenu();
