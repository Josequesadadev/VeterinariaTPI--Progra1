# Veterinaria – Trabajo Práctico Integrador (Programación II)

Sistema web para registrar las atenciones de las mascotas de una veterinaria (consultas, tratamientos, vacunas y controles), con ABM de clientes, mascotas y tipos, autenticación con JWT y dashboards de facturación.

**Tecnicatura Universitaria en Programación – UTN Facultad Regional Córdoba**

## Integrantes

| Nombre | Legajo |
|---|---|
| José Quesada | 430181 |
| Francisco Baravalle | 421435 |
| Baltasar Ferreyra Peuser | 421676 |
| Santiago Alonso | 412654 |

## Estructura de la solución

| Proyecto | Tipo | Contenido |
|---|---|---|
| `Veterinaria.Logica` | Librería de clases | Entidades, `DbContext` (Entity Framework), repositorios, servicios con las reglas de negocio, DTOs y hash de contraseñas |
| `Veterinaria.API` | ASP.NET Core Web API | Controllers REST, autenticación y autorización con JWT, Swagger, CORS |
| `Veterinaria.Web` | ASP.NET Core (archivos estáticos) | Páginas HTML, CSS y JavaScript que consumen la API |
| `BaseDeDatos/` | – | Script de creación de la base (`script_db.sql`) y diagrama E-R |

Dependencias: `Veterinaria.API` → `Veterinaria.Logica`. La web solo se comunica con la API por HTTP.

## Cómo ejecutarlo

1. **Base de datos:** abrir `BaseDeDatos/script_db.sql` en SQL Server Management Studio y ejecutarlo (crea `db_veterinaria` con datos de ejemplo).
2. **Cadena de conexión:** si el servidor no es `.\SQLEXPRESS`, cambiar `ConnectionStrings:DefaultConnection` en `Veterinaria.API/appsettings.json`.
3. **Visual Studio:** abrir `VeterinariaTPI.sln`, elegir el perfil de inicio **"API + Web"** (arranca los dos proyectos) y presionar F5.
   - Si no aparece ese perfil: clic derecho en la solución → *Configurar proyectos de inicio* → *Varios proyectos de inicio* → `Veterinaria.API` y `Veterinaria.Web` en *Iniciar*.
4. Abrir **http://localhost:5090** (web). La API queda en **http://localhost:5080/swagger**.

Desde consola:

```bash
dotnet run --project Veterinaria.API --launch-profile http
dotnet run --project Veterinaria.Web --launch-profile http
```

## Usuarios de prueba

Se crean automáticamente al iniciar la API si la tabla `Usuarios` está vacía:

| Usuario | Contraseña | Rol | Permisos |
|---|---|---|---|
| `admin` | `Admin123` | Admin | Todo, incluidas las bajas y reactivaciones |
| `empleado` | `Empleado123` | Empleado | Consultar, crear y modificar (no puede dar de baja) |

## Funcionalidades

- **Inicio** con descripción del sistema y acceso al login.
- **Login** con validación contra la base y token JWT.
- **Soporte:** ABM de clientes, mascotas, tipos de mascota y tipos de atención (baja lógica).
- **Transacción – Registro de atenciones:** selección dependiente cliente → mascotas activas del cliente → tipo de atención; alta, edición y baja lógica, con filtros.
- **Dashboard:** facturación mensual (12 meses / por año), cantidad e importe por tipo de atención y top 5 de clientes, con filtros de fechas.
- **Acerca de:** integrantes del grupo.
