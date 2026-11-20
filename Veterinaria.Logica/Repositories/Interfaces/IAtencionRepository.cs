using Veterinaria.Logica.Dtos;
using Veterinaria.Logica.Models;

namespace Veterinaria.Logica.Repositories.Interfaces
{
    public interface IAtencionRepository
    {
        List<Atencion> GetAll(FiltroAtencionDto filtro);
        Atencion? GetById(int id);
        void Add(Atencion atencion);
        void Update(Atencion atencion);

        // dashboards
        List<FacturacionMensualDto> GetFacturacionMensual(DateTime desde, DateTime hasta);
        List<AtencionesPorTipoDto> GetCantidadPorTipo(DateTime? desde, DateTime? hasta);
        List<TopClienteDto> GetTopClientes(DateTime? desde, DateTime? hasta);
    }
}
