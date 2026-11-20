using Veterinaria.Logica.Dtos;
using Veterinaria.Logica.Exceptions;
using Veterinaria.Logica.Models;
using Veterinaria.Logica.Repositories.Interfaces;
using Veterinaria.Logica.Services.Interfaces;

namespace Veterinaria.Logica.Services
{
    public class TipoAtencionService : ITipoAtencionService
    {
        private readonly ITipoAtencionRepository repo;

        public TipoAtencionService(ITipoAtencionRepository repo)
        {
            this.repo = repo;
        }

        public List<TipoAtencion> GetAll(bool incluirInactivos)
        {
            return repo.GetAll(incluirInactivos);
        }

        public TipoAtencion? GetById(int id)
        {
            return repo.GetById(id);
        }

        public TipoAtencion Crear(TipoDto dto)
        {
            TipoAtencion tipo = new TipoAtencion(0, dto.Nombre.Trim());
            tipo.Validar();

            if (repo.ExisteNombre(tipo.Nombre, 0))
            {
                throw new NegocioException("Ya existe un tipo de atencion con ese nombre.");
            }

            repo.Add(tipo);
            return tipo;
        }

        public TipoAtencion? Modificar(int id, TipoDto dto)
        {
            TipoAtencion? tipo = repo.GetById(id);
            if (tipo == null)
            {
                return null;
            }

            tipo.Nombre = dto.Nombre.Trim();
            tipo.Validar();

            if (repo.ExisteNombre(tipo.Nombre, id))
            {
                throw new NegocioException("Ya existe un tipo de atencion con ese nombre.");
            }

            repo.Update(tipo);
            return tipo;
        }

        public bool DarDeBaja(int id)
        {
            TipoAtencion? tipo = repo.GetById(id);
            if (tipo == null)
            {
                return false;
            }

            // no se puede dar de baja si hay atenciones activas de este tipo
            if (repo.TieneAtencionesActivas(id))
            {
                throw new NegocioException("No se puede dar de baja " + tipo.Nombre + " porque tiene atenciones activas.");
            }

            tipo.Activo = false;
            repo.Update(tipo);
            return true;
        }

        public bool Reactivar(int id)
        {
            TipoAtencion? tipo = repo.GetById(id);
            if (tipo == null)
            {
                return false;
            }

            tipo.Activo = true;
            repo.Update(tipo);
            return true;
        }
    }
}
