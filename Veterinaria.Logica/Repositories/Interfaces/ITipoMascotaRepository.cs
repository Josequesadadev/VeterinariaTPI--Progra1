using Veterinaria.Logica.Models;

namespace Veterinaria.Logica.Repositories.Interfaces
{
    public interface ITipoMascotaRepository
    {
        List<TipoMascota> GetAll(bool incluirInactivos);
        TipoMascota? GetById(int id);
        void Add(TipoMascota tipo);
        void Update(TipoMascota tipo);
        bool ExisteNombre(string nombre, int idExcluir);
        bool TieneMascotasActivas(int idTipo);
    }
}
