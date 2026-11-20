using Veterinaria.Logica.Dtos;
using Veterinaria.Logica.Models;

namespace Veterinaria.Logica.Repositories.Interfaces
{
    // Repositorio generico: operaciones comunes a todas las entidades
    public interface IRepository<T> where T : Entidad
    {
        List<T> GetAll(bool incluirInactivos);
        T? GetById(int id);
        void Add(T entidad);
        void Update(T entidad);
    }

    // Repositorio para las tablas de soporte con solo nombre
    public interface ICatalogoRepository<T> : IRepository<T> where T : Catalogo
    {
        bool ExisteNombre(string nombre, int idExcluir);
        bool TieneRegistrosActivos(int id);
    }

    public interface IClienteRepository : IRepository<Cliente>
    {
        List<Cliente> GetByFiltro(string? texto, bool incluirInactivos);
        bool ExisteCodigo(string codigo, int idExcluir);
        bool TieneMascotasActivas(int idCliente);
    }

    public interface IMascotaRepository : IRepository<Mascota>
    {
        List<Mascota> GetByFiltro(int? idCliente, int? idTipoMascota, string? nombre, bool incluirInactivos);
    }

    public interface IAtencionRepository : IRepository<Atencion>
    {
        List<Atencion> GetByFiltro(AtencionFiltroDto filtro);
        List<FacturacionMensualDto> GetFacturacionMensual(DateTime desde, DateTime hasta);
        List<AtencionesPorTipoDto> GetCantidadPorTipo(DateTime? desde, DateTime? hasta);
        List<TopClienteDto> GetTopClientes(DateTime? desde, DateTime? hasta, int cantidad);
    }

    public interface IUsuarioRepository : IRepository<Usuario>
    {
        Usuario? GetByNombreUsuario(string nombreUsuario);
        bool HayUsuarios();
    }
}
