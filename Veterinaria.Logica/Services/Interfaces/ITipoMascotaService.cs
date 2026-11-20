using Veterinaria.Logica.Dtos;
using Veterinaria.Logica.Models;

namespace Veterinaria.Logica.Services.Interfaces
{
    public interface ITipoMascotaService
    {
        List<TipoMascota> GetAll(bool incluirInactivos);
        TipoMascota? GetById(int id);
        TipoMascota Crear(TipoDto dto);
        TipoMascota? Modificar(int id, TipoDto dto);
        bool DarDeBaja(int id);
        bool Reactivar(int id);
    }
}
