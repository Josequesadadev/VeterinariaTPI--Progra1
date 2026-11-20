using Veterinaria.Logica.Dtos;
using Veterinaria.Logica.Exceptions;
using Veterinaria.Logica.Repositories.Interfaces;
using Veterinaria.Logica.Services.Interfaces;

namespace Veterinaria.Logica.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IAtencionRepository repo;
        private readonly string[] meses = { "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic" };

        public DashboardService(IAtencionRepository repo)
        {
            this.repo = repo;
        }

        // si no viene el año muestra los ultimos 12 meses, si viene muestra de enero a diciembre
        public List<FacturacionMensualDto> GetFacturacionMensual(int? anio)
        {
            DateTime desde;
            if (anio != null)
            {
                desde = new DateTime(anio.Value, 1, 1);
            }
            else
            {
                desde = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-11);
            }
            DateTime hasta = desde.AddMonths(12).AddDays(-1);

            List<FacturacionMensualDto> datos = repo.GetFacturacionMensual(desde, hasta);

            // armo los 12 meses, los que no tienen atenciones quedan en 0
            List<FacturacionMensualDto> resultado = new List<FacturacionMensualDto>();
            for (int i = 0; i < 12; i++)
            {
                DateTime fecha = desde.AddMonths(i);
                FacturacionMensualDto? mes = datos.FirstOrDefault(d => d.Anio == fecha.Year && d.Mes == fecha.Month);

                FacturacionMensualDto item = new FacturacionMensualDto();
                item.Anio = fecha.Year;
                item.Mes = fecha.Month;
                item.Etiqueta = meses[fecha.Month - 1] + " " + fecha.ToString("yy");
                item.Cantidad = mes != null ? mes.Cantidad : 0;
                item.Total = mes != null ? mes.Total : 0;
                resultado.Add(item);
            }
            return resultado;
        }

        public List<AtencionesPorTipoDto> GetCantidadPorTipo(DateTime? desde, DateTime? hasta)
        {
            ValidarFechas(desde, hasta);
            return repo.GetCantidadPorTipo(desde, hasta);
        }

        public List<TopClienteDto> GetTopClientes(DateTime? desde, DateTime? hasta)
        {
            ValidarFechas(desde, hasta);
            return repo.GetTopClientes(desde, hasta);
        }

        private void ValidarFechas(DateTime? desde, DateTime? hasta)
        {
            if (desde != null && hasta != null && desde > hasta)
            {
                throw new NegocioException("La fecha desde no puede ser mayor a la fecha hasta.");
            }
        }
    }
}
