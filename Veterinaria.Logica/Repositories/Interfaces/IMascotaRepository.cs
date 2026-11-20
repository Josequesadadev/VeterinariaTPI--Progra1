using Veterinaria.Logica.Models;

namespace Veterinaria.Logica.Repositories.Interfaces
{
    public interface IMascotaRepository
    {
        List<Mascota> GetAll(int? idCliente, int? idTipoMascota, string? nombre, bool incluirInactivos);
        Mascota? GetById(int id);
        void Add(Mascota mascota);
        void Update(Mascota mascota);
    }
}
