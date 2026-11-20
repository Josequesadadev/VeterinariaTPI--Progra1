# TPI Veterinaria - Programación II

Trabajo práctico integrador de Programación II (TUP - UTN FRC).

Sistema para registrar las atenciones de las mascotas de una veterinaria, con ABM de clientes, mascotas y tipos, login con JWT y dashboard.

## Integrantes

- José Quesada - 430181
- Francisco Baravalle - 421435
- Baltasar Ferreyra Peuser - 421676
- Santiago Alonso - 412654

## Proyectos

- **Veterinaria.Logica**: librería de clases con los modelos, el DbContext, los repositorios y los servicios.
- **Veterinaria.API**: Web API con los controllers y el login con JWT.
- **Veterinaria.Web**: páginas HTML, CSS y JavaScript que consumen la API.
- **BaseDeDatos**: script de la base y diagrama E-R.

## Cómo ejecutarlo

1. Ejecutar `BaseDeDatos/script_db.sql` en SQL Server (crea la base `db_veterinaria` con datos de prueba).
2. Si el servidor no es `.\SQLEXPRESS`, cambiar la cadena de conexión en `Veterinaria.API/appsettings.json`.
3. Abrir `VeterinariaTPI.sln` y elegir el perfil **API + Web** para que arranquen los dos proyectos (o en propiedades de la solución poner varios proyectos de inicio).
4. Web: http://localhost:5090 - API (Swagger): http://localhost:5080/swagger

## Usuarios

- admin / Admin123 (rol Admin)
- empleado / Empleado123 (rol Empleado, no puede dar de baja)
