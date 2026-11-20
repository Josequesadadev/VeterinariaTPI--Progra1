namespace Veterinaria.Logica.Exceptions
{
    // se lanza cuando no se cumple una regla de negocio
    public class NegocioException : Exception
    {
        public NegocioException(string mensaje) : base(mensaje)
        {
        }
    }
}
