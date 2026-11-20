using Veterinaria.Logica.Exceptions;

namespace Veterinaria.Logica.Models
{
    // Clase base abstracta de todas las entidades del sistema.
    // Centraliza el Id y la baja logica (campo activo).
    public abstract class Entidad
    {
        public int Id { get; set; }

        public bool Activo { get; set; } = true;

        public void DarDeBaja()
        {
            if (!Activo)
            {
                throw new NegocioException("El registro ya se encuentra dado de baja.");
            }
            Activo = false;
        }

        public void Reactivar()
        {
            if (Activo)
            {
                throw new NegocioException("El registro ya se encuentra activo.");
            }
            Activo = true;
        }

        // Cada entidad define sus propias reglas de validacion (polimorfismo).
        public abstract void Validar();
    }
}
