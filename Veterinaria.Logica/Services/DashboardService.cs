using System.Globalization;
using Veterinaria.Logica.Dtos;
using Veterinaria.Logica.Exceptions;
using Veterinaria.Logica.Repositories.Interfaces;
using Veterinaria.Logica.Services.Interfaces;

namespace Veterinaria.Logica.Services
{
    public class DashboardService : IDashboardService
    {
        private const int CantidadTopClientes = 5;
        private readonly IAtencionRepository repo;

        public DashboardService(IAtencionRepository repo)
        {
            this.repo = repo;
        }

        // Sin año: los ultimos 12 meses (incluyendo el actual). Con año: de enero a diciembre de ese año.
        public List<FacturacionMensualDto> GetFacturacionMensual(int? anio)
        {
            DateTime desde;
            if (anio.HasValue)
            {
                if (anio < 2000 || anio > DateTime.Today.Year)
                {
                    throw new NegocioException("El año indicado no es válido.");
                }
                desde = new DateTime(anio.Value, 1, 1);
            }
            else
            {
                DateTime primeroDelMes = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                desde = primeroDelMes.AddMonths(-11);
            }
            DateTime hasta = desde.AddMonths(12).AddDays(-1);

            List<FacturacionMensualDto> datos = repo.GetFacturacionMensual(desde, hasta);

            // Completamos los meses sin atenciones con 0 para que el grafico muestre los 12
            CultureInfo cultura = new CultureInfo("es-AR");
            List<FacturacionMensualDto> resultado = new List<FacturacionMensualDto>();
            for (int i = 0; i < 12; i++)
            {
                DateTime mes = desde.AddMonths(i);
                FacturacionMensualDto? encontrado = datos.FirstOrDefault(d => d.Anio == mes.Year && d.Mes == mes.Month);
                string nombreMes = cultura.TextInfo.ToTitleCase(mes.ToString("MMMM", cultura)).Substring(0, 3);

                resultado.Add(new FacturacionMensualDto
                {
                    Anio = mes.Year,
                    Mes = mes.Month,
                    Etiqueta = $"{nombreMes} {mes:yy}",
                    Cantidad = encontrado?.Cantidad ?? 0,
                    Total = encontrado?.Total ?? 0
                });
            }
            return resultado;
        }

        public List<AtencionesPorTipoDto> GetCantidadPorTipo(DateTime? desde, DateTime? hasta)
        {
            ValidarRango(desde, hasta);
            return repo.GetCantidadPorTipo(desde, hasta);
        }

        public List<TopClienteDto> GetTopClientes(DateTime? desde, DateTime? hasta)
        {
            ValidarRango(desde, hasta);
            return repo.GetTopClientes(desde, hasta, CantidadTopClientes);
        }

        private static void ValidarRango(DateTime? desde, DateTime? hasta)
        {
            if (desde.HasValue && hasta.HasValue && desde > hasta)
            {
                throw new NegocioException("La fecha 'desde' no puede ser mayor que la fecha 'hasta'.");
            }
        }
    }
}
