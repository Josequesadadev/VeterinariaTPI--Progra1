using Veterinaria.Logica.Dtos;
using Veterinaria.Logica.Exceptions;
using Veterinaria.Logica.Models;
using Veterinaria.Logica.Repositories.Interfaces;
using Veterinaria.Logica.Services.Interfaces;

namespace Veterinaria.Logica.Services
{
    // Servicio de la transaccion principal: Registro de Atenciones
    public class AtencionService : IAtencionService
    {
        private readonly IAtencionRepository repo;
        private readonly IClienteRepository clienteRepo;
        private readonly IMascotaRepository mascotaRepo;
        private readonly ICatalogoRepository<TipoAtencion> tipoRepo;

        public AtencionService(IAtencionRepository repo, IClienteRepository clienteRepo,
                               IMascotaRepository mascotaRepo, ICatalogoRepository<TipoAtencion> tipoRepo)
        {
            this.repo = repo;
            this.clienteRepo = clienteRepo;
            this.mascotaRepo = mascotaRepo;
            this.tipoRepo = tipoRepo;
        }

        public List<Atencion> GetAll(AtencionFiltroDto filtro)
        {
            if (filtro.Desde.HasValue && filtro.Hasta.HasValue && filtro.Desde > filtro.Hasta)
            {
                throw new NegocioException("La fecha 'desde' no puede ser mayor que la fecha 'hasta'.");
            }
            return repo.GetByFiltro(filtro);
        }

        public Atencion GetById(int id)
        {
            Atencion? atencion = repo.GetById(id);
            if (atencion == null)
            {
                throw new NoEncontradoException($"No existe la atención con id {id}.");
            }
            return atencion;
        }

        public Atencion Crear(AtencionDto dto)
        {
            Atencion atencion = new Atencion(0, dto.IdMascota, dto.IdTipoAtencion, dto.Fecha,
                                             (dto.Descripcion ?? string.Empty).Trim(), dto.Importe);
            atencion.Validar();
            ValidarRelaciones(dto);

            repo.Add(atencion);
            return GetById(atencion.Id);
        }

        public Atencion Modificar(int id, AtencionDto dto)
        {
            Atencion atencion = GetById(id);

            if (!atencion.Activo)
            {
                throw new NegocioException("No se puede modificar una atención dada de baja.");
            }

            atencion.IdMascota = dto.IdMascota;
            atencion.IdTipoAtencion = dto.IdTipoAtencion;
            atencion.Fecha = dto.Fecha;
            atencion.Descripcion = (dto.Descripcion ?? string.Empty).Trim();
            atencion.Importe = dto.Importe;
            atencion.Validar();
            ValidarRelaciones(dto);

            atencion.Mascota = mascotaRepo.GetById(dto.IdMascota);
            atencion.TipoAtencion = tipoRepo.GetById(dto.IdTipoAtencion);

            repo.Update(atencion);
            return atencion;
        }

        public void DarDeBaja(int id)
        {
            Atencion atencion = GetById(id);
            atencion.DarDeBaja();
            repo.Update(atencion);
        }

        public void Reactivar(int id)
        {
            Atencion atencion = GetById(id);

            if (atencion.Mascota != null && !atencion.Mascota.Activo)
            {
                throw new NegocioException("No se puede reactivar la atención porque la mascota está dada de baja.");
            }

            atencion.Reactivar();
            repo.Update(atencion);
        }

        // Validaciones del backend: no confiamos solo en lo que filtra el formulario
        private void ValidarRelaciones(AtencionDto dto)
        {
            Cliente? cliente = clienteRepo.GetById(dto.IdCliente);
            if (cliente == null || !cliente.Activo)
            {
                throw new NegocioException("El cliente seleccionado no existe o está dado de baja.");
            }

            Mascota? mascota = mascotaRepo.GetById(dto.IdMascota);
            if (mascota == null || !mascota.Activo)
            {
                throw new NegocioException("La mascota seleccionada no existe o está dada de baja.");
            }

            if (!mascota.PerteneceA(cliente.Id))
            {
                throw new NegocioException($"La mascota {mascota.Nombre} no pertenece al cliente {cliente.NombreCompleto}.");
            }

            TipoAtencion? tipo = tipoRepo.GetById(dto.IdTipoAtencion);
            if (tipo == null || !tipo.Activo)
            {
                throw new NegocioException("El tipo de atención seleccionado no existe o está dado de baja.");
            }
        }
    }
}
