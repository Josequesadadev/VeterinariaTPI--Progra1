using Veterinaria.Logica.Models;

namespace Veterinaria.Logica.Repositories.Interfaces
{
    public interface ITipoAtencionRepository
    {
        List<TipoAtencion> GetAll(bool incluirInactivos);
        TipoAtencion? GetById(int id);
        void Add(TipoAtencion tipo);
        void Update(TipoAtencion tipo);
        bool ExisteNombre(string nombre, int idExcluir);
        bool TieneAtencionesActivas(int idTipo);
    }
}
