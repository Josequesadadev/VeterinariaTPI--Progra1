using Veterinaria.Logica.Dtos;

namespace Veterinaria.Logica.Services.Interfaces
{
    public interface IDashboardService
    {
        List<FacturacionMensualDto> GetFacturacionMensual(int? anio);
        List<AtencionesPorTipoDto> GetCantidadPorTipo(DateTime? desde, DateTime? hasta);
        List<TopClienteDto> GetTopClientes(DateTime? desde, DateTime? hasta);
    }
}
