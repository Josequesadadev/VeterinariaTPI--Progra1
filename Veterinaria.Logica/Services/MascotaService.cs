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
        private readonly ITipoMascotaRepository tipoRepo;

        public MascotaService(IMascotaRepository repo, IClienteRepository clienteRepo, ITipoMascotaRepository tipoRepo)
        {
            this.repo = repo;
            this.clienteRepo = clienteRepo;
            this.tipoRepo = tipoRepo;
        }

        public List<Mascota> GetAll(int? idCliente, int? idTipoMascota, string? nombre, bool incluirInactivos)
        {
            return repo.GetAll(idCliente, idTipoMascota, nombre, incluirInactivos);
        }

        public Mascota? GetById(int id)
        {
            return repo.GetById(id);
        }

        public Mascota Crear(MascotaDto dto)
        {
            Mascota mascota = new Mascota(0, dto.Nombre.Trim(), dto.Edad, dto.IdTipoMascota, dto.IdCliente);
            mascota.Validar();
            ValidarClienteYTipo(dto.IdCliente, dto.IdTipoMascota);

            repo.Add(mascota);
            return repo.GetById(mascota.Id)!;
        }

        public Mascota? Modificar(int id, MascotaDto dto)
        {
            Mascota? mascota = repo.GetById(id);
            if (mascota == null)
            {
                return null;
            }

            mascota.Nombre = dto.Nombre.Trim();
            mascota.Edad = dto.Edad;
            mascota.IdTipoMascota = dto.IdTipoMascota;
            mascota.IdCliente = dto.IdCliente;
            mascota.Validar();
            ValidarClienteYTipo(dto.IdCliente, dto.IdTipoMascota);

            // actualizo tambien los objetos relacionados para que coincidan con los id
            mascota.Cliente = clienteRepo.GetById(dto.IdCliente);
            mascota.TipoMascota = tipoRepo.GetById(dto.IdTipoMascota);

            repo.Update(mascota);
            return mascota;
        }

        public bool DarDeBaja(int id)
        {
            Mascota? mascota = repo.GetById(id);
            if (mascota == null)
            {
                return false;
            }

            mascota.Activo = false;
            repo.Update(mascota);
            return true;
        }

        public bool Reactivar(int id)
        {
            Mascota? mascota = repo.GetById(id);
            if (mascota == null)
            {
                return false;
            }

            if (mascota.Cliente != null && mascota.Cliente.Activo == false)
            {
                throw new NegocioException("No se puede reactivar la mascota porque el cliente esta dado de baja.");
            }

            mascota.Activo = true;
            repo.Update(mascota);
            return true;
        }

        private void ValidarClienteYTipo(int idCliente, int idTipo)
        {
            Cliente? cliente = clienteRepo.GetById(idCliente);
            if (cliente == null || cliente.Activo == false)
            {
                throw new NegocioException("El cliente no existe o esta dado de baja.");
            }

            TipoMascota? tipo = tipoRepo.GetById(idTipo);
            if (tipo == null || tipo.Activo == false)
            {
                throw new NegocioException("El tipo de mascota no existe o esta dado de baja.");
            }
        }
    }
}
