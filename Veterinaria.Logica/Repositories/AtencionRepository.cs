using Microsoft.EntityFrameworkCore;
using Veterinaria.Logica.Data;
using Veterinaria.Logica.Dtos;
using Veterinaria.Logica.Models;
using Veterinaria.Logica.Repositories.Interfaces;

namespace Veterinaria.Logica.Repositories
{
    public class AtencionRepository : Repository<Atencion>, IAtencionRepository
    {
        public AtencionRepository(VeterinariaDbContext db) : base(db) { }

        protected override IQueryable<Atencion> Query()
        {
            return db.Atenciones
                .Include(a => a.TipoAtencion)
                .Include(a => a.Mascota).ThenInclude(m => m!.Cliente)
                .Include(a => a.Mascota).ThenInclude(m => m!.TipoMascota);
        }

        public override List<Atencion> GetAll(bool incluirInactivos)
        {
            return GetByFiltro(new AtencionFiltroDto { IncluirInactivas = incluirInactivos });
        }

        public List<Atencion> GetByFiltro(AtencionFiltroDto filtro)
        {
            IQueryable<Atencion> consulta = Query().Where(a => filtro.IncluirInactivas || a.Activo);

            if (filtro.IdCliente.HasValue)
            {
                consulta = consulta.Where(a => a.Mascota!.IdCliente == filtro.IdCliente.Value);
            }
            if (filtro.IdMascota.HasValue)
            {
                consulta = consulta.Where(a => a.IdMascota == filtro.IdMascota.Value);
            }
            if (filtro.IdTipoAtencion.HasValue)
            {
                consulta = consulta.Where(a => a.IdTipoAtencion == filtro.IdTipoAtencion.Value);
            }
            consulta = FiltrarFechas(consulta, filtro.Desde, filtro.Hasta);

            return consulta.OrderByDescending(a => a.Fecha).ThenByDescending(a => a.Id).ToList();
        }

        // ===== Consultas para los dashboards (solo atenciones activas) =====

        public List<FacturacionMensualDto> GetFacturacionMensual(DateTime desde, DateTime hasta)
        {
            return FiltrarFechas(db.Atenciones.Where(a => a.Activo), desde, hasta)
                .GroupBy(a => new { a.Fecha.Year, a.Fecha.Month })
                .Select(g => new FacturacionMensualDto
                {
                    Anio = g.Key.Year,
                    Mes = g.Key.Month,
                    Cantidad = g.Count(),
                    Total = g.Sum(a => a.Importe)
                })
                .ToList();
        }

        public List<AtencionesPorTipoDto> GetCantidadPorTipo(DateTime? desde, DateTime? hasta)
        {
            return FiltrarFechas(db.Atenciones.Where(a => a.Activo), desde, hasta)
                .GroupBy(a => a.TipoAtencion!.Nombre)
                .Select(g => new AtencionesPorTipoDto
                {
                    Tipo = g.Key,
                    Cantidad = g.Count(),
                    Importe = g.Sum(a => a.Importe)
                })
                .OrderByDescending(x => x.Cantidad)
                .ToList();
        }

        // Une Atenciones -> Mascotas -> Clientes, agrupa por cliente y ordena
        public List<TopClienteDto> GetTopClientes(DateTime? desde, DateTime? hasta, int cantidad)
        {
            return FiltrarFechas(db.Atenciones.Where(a => a.Activo), desde, hasta)
                .GroupBy(a => new
                {
                    a.Mascota!.Cliente!.Id,
                    a.Mascota.Cliente.Codigo,
                    a.Mascota.Cliente.Apellido,
                    a.Mascota.Cliente.Nombre
                })
                .Select(g => new TopClienteDto
                {
                    IdCliente = g.Key.Id,
                    Codigo = g.Key.Codigo,
                    Cliente = g.Key.Apellido + ", " + g.Key.Nombre,
                    Cantidad = g.Count(),
                    Importe = g.Sum(a => a.Importe)
                })
                .OrderByDescending(x => x.Cantidad)
                .ThenByDescending(x => x.Importe)
                .Take(cantidad)
                .ToList();
        }

        // "hasta" es inclusive: se compara contra el dia siguiente a las 00:00
        private static IQueryable<Atencion> FiltrarFechas(IQueryable<Atencion> consulta, DateTime? desde, DateTime? hasta)
        {
            if (desde.HasValue)
            {
                DateTime d = desde.Value.Date;
                consulta = consulta.Where(a => a.Fecha >= d);
            }
            if (hasta.HasValue)
            {
                DateTime h = hasta.Value.Date.AddDays(1);
                consulta = consulta.Where(a => a.Fecha < h);
            }
            return consulta;
        }
    }
}
