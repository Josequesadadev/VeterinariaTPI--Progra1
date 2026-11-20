using Veterinaria.Logica.Dtos;
using Veterinaria.Logica.Models;

namespace Veterinaria.Logica.Services.Interfaces
{
    public interface ICatalogoService<T> where T : Catalogo
    {
        List<T> GetAll(bool incluirInactivos);
        T GetById(int id);
        T Crear(CatalogoDto dto);
        T Modificar(int id, CatalogoDto dto);
        void DarDeBaja(int id);
        void Reactivar(int id);
    }

    public interface IClienteService
    {
        List<Cliente> GetAll(string? texto, bool incluirInactivos);
        Cliente GetById(int id);
        Cliente Crear(ClienteDto dto);
        Cliente Modificar(int id, ClienteDto dto);
        void DarDeBaja(int id);
        void Reactivar(int id);
    }

    public interface IMascotaService
    {
        List<Mascota> GetAll(int? idCliente, int? idTipoMascota, string? nombre, bool incluirInactivos);
        Mascota GetById(int id);
        Mascota Crear(MascotaDto dto);
        Mascota Modificar(int id, MascotaDto dto);
        void DarDeBaja(int id);
        void Reactivar(int id);
    }

    public interface IAtencionService
    {
        List<Atencion> GetAll(AtencionFiltroDto filtro);
        Atencion GetById(int id);
        Atencion Crear(AtencionDto dto);
        Atencion Modificar(int id, AtencionDto dto);
        void DarDeBaja(int id);
        void Reactivar(int id);
    }

    public interface IDashboardService
    {
        List<FacturacionMensualDto> GetFacturacionMensual(int? anio);
        List<AtencionesPorTipoDto> GetCantidadPorTipo(DateTime? desde, DateTime? hasta);
        List<TopClienteDto> GetTopClientes(DateTime? desde, DateTime? hasta);
    }

    public interface IAuthService
    {
        Usuario? ValidarCredenciales(LoginRequestDto dto);
        void CrearUsuariosIniciales();
    }
}
