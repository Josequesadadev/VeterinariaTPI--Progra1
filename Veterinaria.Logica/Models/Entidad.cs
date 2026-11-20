namespace Veterinaria.Logica.Models
{
    // clase base de todas las entidades
    public abstract class Entidad
    {
        public int Id { get; set; }

        public bool Activo { get; set; } = true;

        // cada clase implementa sus propias validaciones
        public abstract void Validar();
    }
}
