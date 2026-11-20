const API_URL = 'http://localhost:5080/api';

document.getElementById('form-login').addEventListener('submit', async (e) => {
    e.preventDefault();

    const usuario = document.getElementById('usuario').value;
    const password = document.getElementById('password').value;
    const error = document.getElementById('error');

    if (usuario == '' || password == '') {
        error.textContent = 'Complete el usuario y la contraseña.';
        return;
    }

    try {
        const respuesta = await fetch(API_URL + '/auth/login', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ nombreUsuario: usuario, password: password })
        });

        const datos = await respuesta.json();

        if (!respuesta.ok) {
            error.textContent = datos.mensaje;
            return;
        }

        // guardo el token para usarlo en las otras paginas
        localStorage.setItem('token', datos.token);
        localStorage.setItem('usuario', datos.nombreUsuario);
        localStorage.setItem('rol', datos.rol);
        localStorage.setItem('expira', datos.expira);

        window.location.href = 'principal.html';
    } catch {
        error.textContent = 'No se pudo conectar con la API.';
    }
});
