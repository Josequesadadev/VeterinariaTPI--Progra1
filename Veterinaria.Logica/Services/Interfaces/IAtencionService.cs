using Veterinaria.Logica.Dtos;
using Veterinaria.Logica.Models;

namespace Veterinaria.Logica.Services.Interfaces
{
    public interface IAtencionService
    {
        List<Atencion> GetAll(FiltroAtencionDto filtro);
        Atencion? GetById(int id);
        Atencion Crear(AtencionDto dto);
        Atencion? Modificar(int id, AtencionDto dto);
        bool DarDeBaja(int id);
        bool Reactivar(int id);
    }
}
