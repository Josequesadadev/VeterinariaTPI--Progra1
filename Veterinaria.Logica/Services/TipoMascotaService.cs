using Veterinaria.Logica.Dtos;
using Veterinaria.Logica.Exceptions;
using Veterinaria.Logica.Models;
using Veterinaria.Logica.Repositories.Interfaces;
using Veterinaria.Logica.Services.Interfaces;

namespace Veterinaria.Logica.Services
{
    public class TipoMascotaService : ITipoMascotaService
    {
        private readonly ITipoMascotaRepository repo;

        public TipoMascotaService(ITipoMascotaRepository repo)
        {
            this.repo = repo;
        }

        public List<TipoMascota> GetAll(bool incluirInactivos)
        {
            return repo.GetAll(incluirInactivos);
        }

        public TipoMascota? GetById(int id)
        {
            return repo.GetById(id);
        }

        public TipoMascota Crear(TipoDto dto)
        {
            TipoMascota tipo = new TipoMascota(0, dto.Nombre.Trim());
            tipo.Validar();

            if (repo.ExisteNombre(tipo.Nombre, 0))
            {
                throw new NegocioException("Ya existe un tipo de mascota con ese nombre.");
            }

            repo.Add(tipo);
            return tipo;
        }

        public TipoMascota? Modificar(int id, TipoDto dto)
        {
            TipoMascota? tipo = repo.GetById(id);
            if (tipo == null)
            {
                return null;
            }

            tipo.Nombre = dto.Nombre.Trim();
            tipo.Validar();

            if (repo.ExisteNombre(tipo.Nombre, id))
            {
                throw new NegocioException("Ya existe un tipo de mascota con ese nombre.");
            }

            repo.Update(tipo);
            return tipo;
        }

        public bool DarDeBaja(int id)
        {
            TipoMascota? tipo = repo.GetById(id);
            if (tipo == null)
            {
                return false;
            }

            // no se puede dar de baja si hay mascotas activas de este tipo
            if (repo.TieneMascotasActivas(id))
            {
                throw new NegocioException("No se puede dar de baja " + tipo.Nombre + " porque tiene mascotas activas.");
            }

            tipo.Activo = false;
            repo.Update(tipo);
            return true;
        }

        public bool Reactivar(int id)
        {
            TipoMascota? tipo = repo.GetById(id);
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
