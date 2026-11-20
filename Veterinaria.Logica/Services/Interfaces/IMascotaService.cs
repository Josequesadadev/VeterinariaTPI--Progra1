using Veterinaria.Logica.Dtos;
using Veterinaria.Logica.Models;

namespace Veterinaria.Logica.Services.Interfaces
{
    public interface IMascotaService
    {
        List<Mascota> GetAll(int? idCliente, int? idTipoMascota, string? nombre, bool incluirInactivos);
        Mascota? GetById(int id);
        Mascota Crear(MascotaDto dto);
        Mascota? Modificar(int id, MascotaDto dto);
        bool DarDeBaja(int id);
        bool Reactivar(int id);
    }
}
