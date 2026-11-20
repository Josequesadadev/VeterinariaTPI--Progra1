using Veterinaria.Logica.Dtos;
using Veterinaria.Logica.Exceptions;
using Veterinaria.Logica.Models;
using Veterinaria.Logica.Repositories.Interfaces;
using Veterinaria.Logica.Services.Interfaces;

namespace Veterinaria.Logica.Services
{
    public class AtencionService : IAtencionService
    {
        private readonly IAtencionRepository repo;
        private readonly IClienteRepository clienteRepo;
        private readonly IMascotaRepository mascotaRepo;
        private readonly ITipoAtencionRepository tipoRepo;

        public AtencionService(IAtencionRepository repo, IClienteRepository clienteRepo, IMascotaRepository mascotaRepo, ITipoAtencionRepository tipoRepo)
        {
            this.repo = repo;
            this.clienteRepo = clienteRepo;
            this.mascotaRepo = mascotaRepo;
            this.tipoRepo = tipoRepo;
        }

        public List<Atencion> GetAll(FiltroAtencionDto filtro)
        {
            if (filtro.Desde != null && filtro.Hasta != null && filtro.Desde > filtro.Hasta)
            {
                throw new NegocioException("La fecha desde no puede ser mayor a la fecha hasta.");
            }
            return repo.GetAll(filtro);
        }

        public Atencion? GetById(int id)
        {
            return repo.GetById(id);
        }

        public Atencion Crear(AtencionDto dto)
        {
            Atencion atencion = new Atencion(0, dto.IdMascota, dto.IdTipoAtencion, dto.Fecha, dto.Descripcion.Trim(), dto.Importe);
            atencion.Validar();
            ValidarDatos(dto);

            repo.Add(atencion);
            return repo.GetById(atencion.Id)!;
        }

        public Atencion? Modificar(int id, AtencionDto dto)
        {
            Atencion? atencion = repo.GetById(id);
            if (atencion == null)
            {
                return null;
            }

            if (atencion.Activo == false)
            {
                throw new NegocioException("No se puede modificar una atencion dada de baja.");
            }

            atencion.IdMascota = dto.IdMascota;
            atencion.IdTipoAtencion = dto.IdTipoAtencion;
            atencion.Fecha = dto.Fecha;
            atencion.Descripcion = dto.Descripcion.Trim();
            atencion.Importe = dto.Importe;
            atencion.Validar();
            ValidarDatos(dto);

            atencion.Mascota = mascotaRepo.GetById(dto.IdMascota);
            atencion.TipoAtencion = tipoRepo.GetById(dto.IdTipoAtencion);

            repo.Update(atencion);
            return atencion;
        }

        public bool DarDeBaja(int id)
        {
            Atencion? atencion = repo.GetById(id);
            if (atencion == null)
            {
                return false;
            }

            atencion.Activo = false;
            repo.Update(atencion);
            return true;
        }

        public bool Reactivar(int id)
        {
            Atencion? atencion = repo.GetById(id);
            if (atencion == null)
            {
                return false;
            }

            if (atencion.Mascota != null && atencion.Mascota.Activo == false)
            {
                throw new NegocioException("No se puede reactivar la atencion porque la mascota esta dada de baja.");
            }

            atencion.Activo = true;
            repo.Update(atencion);
            return true;
        }

        // se valida tambien en el backend, no solo en el formulario
        private void ValidarDatos(AtencionDto dto)
        {
            Cliente? cliente = clienteRepo.GetById(dto.IdCliente);
            if (cliente == null || cliente.Activo == false)
            {
                throw new NegocioException("El cliente no existe o esta dado de baja.");
            }

            Mascota? mascota = mascotaRepo.GetById(dto.IdMascota);
            if (mascota == null || mascota.Activo == false)
            {
                throw new NegocioException("La mascota no existe o esta dada de baja.");
            }

            if (!mascota.PerteneceA(cliente.Id))
            {
                throw new NegocioException("La mascota " + mascota.Nombre + " no pertenece al cliente " + cliente.Apellido + ", " + cliente.Nombre + ".");
            }

            TipoAtencion? tipo = tipoRepo.GetById(dto.IdTipoAtencion);
            if (tipo == null || tipo.Activo == false)
            {
                throw new NegocioException("El tipo de atencion no existe o esta dado de baja.");
            }
        }
    }
}
