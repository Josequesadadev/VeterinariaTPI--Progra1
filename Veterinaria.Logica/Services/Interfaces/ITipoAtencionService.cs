using Veterinaria.Logica.Dtos;
using Veterinaria.Logica.Models;

namespace Veterinaria.Logica.Services.Interfaces
{
    public interface ITipoAtencionService
    {
        List<TipoAtencion> GetAll(bool incluirInactivos);
        TipoAtencion? GetById(int id);
        TipoAtencion Crear(TipoDto dto);
        TipoAtencion? Modificar(int id, TipoDto dto);
        bool DarDeBaja(int id);
        bool Reactivar(int id);
    }
}
