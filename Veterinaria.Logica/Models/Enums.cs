namespace Veterinaria.Logica.Models
{
    // En la base se guarda como CHAR(1): 'M', 'F' o 'X'
    public enum Sexo
    {
        Masculino,
        Femenino,
        Otro
    }

    // En la base se guarda como texto: 'Admin' o 'Empleado'
    public enum Rol
    {
        Admin,
        Empleado
    }
}
