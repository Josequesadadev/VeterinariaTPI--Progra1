using Veterinaria.Logica.Dtos;
using Veterinaria.Logica.Exceptions;
using Veterinaria.Logica.Models;
using Veterinaria.Logica.Repositories.Interfaces;
using Veterinaria.Logica.Services.Interfaces;

namespace Veterinaria.Logica.Services
{
    public class MascotaService : IMascotaService
    {
        private readonly IMascotaRepository repo;
        private readonly IClienteRepository clienteRepo;
        private readonly ICatalogoRepository<TipoMascota> tipoRepo;

        public MascotaService(IMascotaRepository repo, IClienteRepository clienteRepo, ICatalogoRepository<TipoMascota> tipoRepo)
        {
            this.repo = repo;
            this.clienteRepo = clienteRepo;
            this.tipoRepo = tipoRepo;
        }

        public List<Mascota> GetAll(int? idCliente, int? idTipoMascota, string? nombre, bool incluirInactivos)
        {
            return repo.GetByFiltro(idCliente, idTipoMascota, nombre, incluirInactivos);
        }

        public Mascota GetById(int id)
        {
            Mascota? mascota = repo.GetById(id);
            if (mascota == null)
            {
                throw new NoEncontradoException($"No existe la mascota con id {id}.");
            }
            return mascota;
        }

        public Mascota Crear(MascotaDto dto)
        {
            Mascota mascota = new Mascota(0, (dto.Nombre ?? string.Empty).Trim(), dto.Edad, dto.IdTipoMascota, dto.IdCliente);
            mascota.Validar();
            ValidarRelaciones(mascota);

            repo.Add(mascota);
            return GetById(mascota.Id);
        }

        public Mascota Modificar(int id, MascotaDto dto)
        {
            Mascota mascota = GetById(id);
            mascota.Nombre = (dto.Nombre ?? string.Empty).Trim();
            mascota.Edad = dto.Edad;
            mascota.IdTipoMascota = dto.IdTipoMascota;
            mascota.IdCliente = dto.IdCliente;
            mascota.Validar();
            ValidarRelaciones(mascota);

            // Actualizamos tambien las navegaciones para que EF no se confunda con las FK
            mascota.Cliente = clienteRepo.GetById(dto.IdCliente);
            mascota.TipoMascota = tipoRepo.GetById(dto.IdTipoMascota);

            repo.Update(mascota);
            return mascota;
        }

        public void DarDeBaja(int id)
        {
            Mascota mascota = GetById(id);
            mascota.DarDeBaja();
            repo.Update(mascota);
        }

        public void Reactivar(int id)
        {
            Mascota mascota = GetById(id);

            if (mascota.Cliente != null && !mascota.Cliente.Activo)
            {
                throw new NegocioException("No se puede reactivar la mascota porque su cliente está dado de baja.");
            }

            mascota.Reactivar();
            repo.Update(mascota);
        }

        // El cliente y el tipo tienen que existir y estar activos
        private void ValidarRelaciones(Mascota mascota)
        {
            Cliente? cliente = clienteRepo.GetById(mascota.IdCliente);
            if (cliente == null || !cliente.Activo)
            {
                throw new NegocioException("El cliente seleccionado no existe o está dado de baja.");
            }

            TipoMascota? tipo = tipoRepo.GetById(mascota.IdTipoMascota);
            if (tipo == null || !tipo.Activo)
            {
                throw new NegocioException("El tipo de mascota seleccionado no existe o está dado de baja.");
            }
        }
    }
}
