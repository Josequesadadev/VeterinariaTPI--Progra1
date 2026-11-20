using Veterinaria.Logica.Dtos;
using Veterinaria.Logica.Models;

namespace Veterinaria.Logica.Services.Interfaces
{
    public interface IClienteService
    {
        List<Cliente> GetAll(string? texto, bool incluirInactivos);
        Cliente? GetById(int id);
        Cliente Crear(ClienteDto dto);
        Cliente? Modificar(int id, ClienteDto dto);
        bool DarDeBaja(int id);
        bool Reactivar(int id);
    }
}
