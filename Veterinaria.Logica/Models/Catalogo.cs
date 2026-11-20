using Veterinaria.Logica.Exceptions;

namespace Veterinaria.Logica.Models
{
    // Base para las tablas de soporte que solo tienen un nombre (TiposMascota y TiposAtencion).
    public abstract class Catalogo : Entidad
    {
        public string Nombre { get; set; } = string.Empty;

        protected Catalogo() { }

        protected Catalogo(int id, string nombre)
        {
            Id = id;
            Nombre = nombre;
        }

        public override void Validar()
        {
            if (string.IsNullOrWhiteSpace(Nombre))
            {
                throw new NegocioException("El nombre es obligatorio.");
            }
            if (Nombre.Trim().Length > 50)
            {
                throw new NegocioException("El nombre no puede superar los 50 caracteres.");
            }
        }

        public override string ToString()
        {
            return Nombre;
        }
    }
}
