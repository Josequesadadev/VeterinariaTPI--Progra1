namespace Veterinaria.Logica.Exceptions
{
    // Se lanza cuando se busca un registro por id y no existe.
    // El controller la atrapa y devuelve un 404 (NotFound).
    public class NoEncontradoException : Exception
    {
        public NoEncontradoException(string mensaje) : base(mensaje) { }
    }
}
