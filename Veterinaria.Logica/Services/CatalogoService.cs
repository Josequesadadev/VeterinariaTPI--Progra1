using Veterinaria.Logica.Dtos;
using Veterinaria.Logica.Exceptions;
using Veterinaria.Logica.Models;
using Veterinaria.Logica.Repositories.Interfaces;
using Veterinaria.Logica.Services.Interfaces;

namespace Veterinaria.Logica.Services
{
    // Servicio generico: el mismo codigo sirve para TiposMascota y TiposAtencion
    public class CatalogoService<T> : ICatalogoService<T> where T : Catalogo, new()
    {
        private readonly ICatalogoRepository<T> repo;

        public CatalogoService(ICatalogoRepository<T> repo)
        {
            this.repo = repo;
        }

        public List<T> GetAll(bool incluirInactivos)
        {
            return repo.GetAll(incluirInactivos);
        }

        public T GetById(int id)
        {
            T? entidad = repo.GetById(id);
            if (entidad == null)
            {
                throw new NoEncontradoException($"No existe el registro con id {id}.");
            }
            return entidad;
        }

        public T Crear(CatalogoDto dto)
        {
            T nuevo = new T { Nombre = dto.Nombre?.Trim() ?? string.Empty };
            nuevo.Validar();
            ValidarNombreUnico(nuevo.Nombre, 0);

            repo.Add(nuevo);
            return nuevo;
        }

        public T Modificar(int id, CatalogoDto dto)
        {
            T entidad = GetById(id);
            entidad.Nombre = dto.Nombre?.Trim() ?? string.Empty;
            entidad.Validar();
            ValidarNombreUnico(entidad.Nombre, id);

            repo.Update(entidad);
            return entidad;
        }

        public void DarDeBaja(int id)
        {
            T entidad = GetById(id);

            if (repo.TieneRegistrosActivos(id))
            {
                throw new NegocioException($"No se puede dar de baja \"{entidad.Nombre}\" porque tiene registros activos asociados.");
            }

            entidad.DarDeBaja();
            repo.Update(entidad);
        }

        public void Reactivar(int id)
        {
            T entidad = GetById(id);
            entidad.Reactivar();
            repo.Update(entidad);
        }

        private void ValidarNombreUnico(string nombre, int idExcluir)
        {
            if (repo.ExisteNombre(nombre, idExcluir))
            {
                throw new NegocioException($"Ya existe un registro con el nombre \"{nombre}\".");
            }
        }
    }
}
