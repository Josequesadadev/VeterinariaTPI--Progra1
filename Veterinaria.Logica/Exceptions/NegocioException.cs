namespace Veterinaria.Logica.Exceptions
{
    // Excepcion propia para las reglas de negocio.
    // El controller la atrapa y devuelve un 400 (BadRequest) con el mensaje.
    public class NegocioException : Exception
    {
        public NegocioException(string mensaje) : base(mensaje) { }
    }
}
