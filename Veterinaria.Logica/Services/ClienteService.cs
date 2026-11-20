using Veterinaria.Logica.Dtos;
using Veterinaria.Logica.Exceptions;
using Veterinaria.Logica.Models;
using Veterinaria.Logica.Repositories.Interfaces;
using Veterinaria.Logica.Services.Interfaces;

namespace Veterinaria.Logica.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository repo;

        public ClienteService(IClienteRepository repo)
        {
            this.repo = repo;
        }

        public List<Cliente> GetAll(string? texto, bool incluirInactivos)
        {
            return repo.GetAll(texto, incluirInactivos);
        }

        public Cliente? GetById(int id)
        {
            return repo.GetById(id);
        }

        public Cliente Crear(ClienteDto dto)
        {
            Cliente cliente = new Cliente();
            CargarDatos(cliente, dto);
            cliente.Validar();

            if (repo.ExisteCodigo(cliente.Codigo, 0))
            {
                throw new NegocioException("Ya existe un cliente con el codigo " + cliente.Codigo + ".");
            }

            repo.Add(cliente);
            return cliente;
        }

        public Cliente? Modificar(int id, ClienteDto dto)
        {
            Cliente? cliente = repo.GetById(id);
            if (cliente == null)
            {
                return null;
            }

            CargarDatos(cliente, dto);
            cliente.Validar();

            if (repo.ExisteCodigo(cliente.Codigo, id))
            {
                throw new NegocioException("Ya existe un cliente con el codigo " + cliente.Codigo + ".");
            }

            repo.Update(cliente);
            return cliente;
        }

        public bool DarDeBaja(int id)
        {
            Cliente? cliente = repo.GetById(id);
            if (cliente == null)
            {
                return false;
            }

            if (repo.TieneMascotasActivas(id))
            {
                throw new NegocioException("No se puede dar de baja el cliente porque tiene mascotas activas.");
            }

            cliente.Activo = false;
            repo.Update(cliente);
            return true;
        }

        public bool Reactivar(int id)
        {
            Cliente? cliente = repo.GetById(id);
            if (cliente == null)
            {
                return false;
            }

            cliente.Activo = true;
            repo.Update(cliente);
            return true;
        }

        private void CargarDatos(Cliente cliente, ClienteDto dto)
        {
            cliente.Codigo = dto.Codigo.Trim().ToUpper();
            cliente.Nombre = dto.Nombre.Trim();
            cliente.Apellido = dto.Apellido.Trim();
            cliente.Telefono = string.IsNullOrWhiteSpace(dto.Telefono) ? null : dto.Telefono.Trim();

            if (dto.Sexo == "M")
            {
                cliente.Sexo = Sexo.Masculino;
            }
            else if (dto.Sexo == "F")
            {
                cliente.Sexo = Sexo.Femenino;
            }
            else if (dto.Sexo == "X")
            {
                cliente.Sexo = Sexo.Otro;
            }
            else
            {
                throw new NegocioException("El sexo tiene que ser M, F o X.");
            }
        }
    }
}
