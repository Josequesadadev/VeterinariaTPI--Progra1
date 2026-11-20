// Si ya tiene una sesion valida, va directo a la pagina principal
if (sesion.estaLogueado()) {
    window.location.href = 'principal.html';
}

const form = document.getElementById('form-login');
const errorLogin = document.getElementById('error-login');
const btnIngresar = document.getElementById('btn-ingresar');

form.addEventListener('submit', async (e) => {
    e.preventDefault();
    errorLogin.classList.add('d-none');

    if (!form.checkValidity()) {
        form.classList.add('was-validated');
        return;
    }

    btnIngresar.disabled = true;
    btnIngresar.textContent = 'Ingresando...';

    try {
        const datos = await api('/auth/login', 'POST', {
            nombreUsuario: document.getElementById('usuario').value.trim(),
            password: document.getElementById('password').value
        });
        sesion.guardar(datos);
        window.location.href = 'principal.html';
    } catch (error) {
        errorLogin.textContent = error.message;
        errorLogin.classList.remove('d-none');
    } finally {
        btnIngresar.disabled = false;
        btnIngresar.textContent = 'Ingresar';
    }
});
