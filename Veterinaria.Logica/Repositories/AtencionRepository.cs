using Microsoft.EntityFrameworkCore;
using Veterinaria.Logica.Data;
using Veterinaria.Logica.Dtos;
using Veterinaria.Logica.Models;
using Veterinaria.Logica.Repositories.Interfaces;

namespace Veterinaria.Logica.Repositories
{
    public class AtencionRepository : IAtencionRepository
    {
        private readonly VeterinariaDbContext db;

        public AtencionRepository(VeterinariaDbContext db)
        {
            this.db = db;
        }

        public List<Atencion> GetAll(FiltroAtencionDto filtro)
        {
            IQueryable<Atencion> consulta = db.Atenciones
                .Include(a => a.TipoAtencion)
                .Include(a => a.Mascota).ThenInclude(m => m!.Cliente)
                .Include(a => a.Mascota).ThenInclude(m => m!.TipoMascota);

            if (!filtro.IncluirInactivas)
            {
                consulta = consulta.Where(a => a.Activo == true);
            }
            if (filtro.IdCliente != null)
            {
                consulta = consulta.Where(a => a.Mascota!.IdCliente == filtro.IdCliente);
            }
            if (filtro.IdMascota != null)
            {
                consulta = consulta.Where(a => a.IdMascota == filtro.IdMascota);
            }
            if (filtro.IdTipoAtencion != null)
            {
                consulta = consulta.Where(a => a.IdTipoAtencion == filtro.IdTipoAtencion);
            }
            consulta = FiltrarFechas(consulta, filtro.Desde, filtro.Hasta);

            return consulta.OrderByDescending(a => a.Fecha).ThenByDescending(a => a.Id).ToList();
        }

        public Atencion? GetById(int id)
        {
            return db.Atenciones
                .Include(a => a.TipoAtencion)
                .Include(a => a.Mascota).ThenInclude(m => m!.Cliente)
                .Include(a => a.Mascota).ThenInclude(m => m!.TipoMascota)
                .FirstOrDefault(a => a.Id == id);
        }

        public void Add(Atencion atencion)
        {
            db.Atenciones.Add(atencion);
            db.SaveChanges();
        }

        public void Update(Atencion atencion)
        {
            db.Entry(atencion).State = EntityState.Modified;
            db.SaveChanges();
        }

        // ===== DASHBOARDS (solo atenciones activas) =====

        public List<FacturacionMensualDto> GetFacturacionMensual(DateTime desde, DateTime hasta)
        {
            IQueryable<Atencion> consulta = FiltrarFechas(db.Atenciones.Where(a => a.Activo == true), desde, hasta);

            return consulta
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
            IQueryable<Atencion> consulta = FiltrarFechas(db.Atenciones.Where(a => a.Activo == true), desde, hasta);

            return consulta
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

        public List<TopClienteDto> GetTopClientes(DateTime? desde, DateTime? hasta)
        {
            IQueryable<Atencion> consulta = FiltrarFechas(db.Atenciones.Where(a => a.Activo == true), desde, hasta);

            // atenciones -> mascotas -> clientes, agrupado por cliente
            return consulta
                .GroupBy(a => new { a.Mascota!.Cliente!.Id, a.Mascota.Cliente.Codigo, a.Mascota.Cliente.Apellido, a.Mascota.Cliente.Nombre })
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
                .Take(5)
                .ToList();
        }

        private IQueryable<Atencion> FiltrarFechas(IQueryable<Atencion> consulta, DateTime? desde, DateTime? hasta)
        {
            if (desde != null)
            {
                DateTime d = desde.Value.Date;
                consulta = consulta.Where(a => a.Fecha >= d);
            }
            if (hasta != null)
            {
                // se suma un dia para que incluya todo el dia "hasta"
                DateTime h = hasta.Value.Date.AddDays(1);
                consulta = consulta.Where(a => a.Fecha < h);
            }
            return consulta;
        }
    }
}
