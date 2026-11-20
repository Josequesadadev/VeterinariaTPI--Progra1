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
            return repo.GetByFiltro(texto, incluirInactivos);
        }

        public Cliente GetById(int id)
        {
            Cliente? cliente = repo.GetById(id);
            if (cliente == null)
            {
                throw new NoEncontradoException($"No existe el cliente con id {id}.");
            }
            return cliente;
        }

        public Cliente Crear(ClienteDto dto)
        {
            Cliente cliente = new Cliente();
            CargarDatos(cliente, dto);
            cliente.Validar();
            ValidarCodigoUnico(cliente.Codigo, 0);

            repo.Add(cliente);
            return cliente;
        }

        public Cliente Modificar(int id, ClienteDto dto)
        {
            Cliente cliente = GetById(id);
            CargarDatos(cliente, dto);
            cliente.Validar();
            ValidarCodigoUnico(cliente.Codigo, id);

            repo.Update(cliente);
            return cliente;
        }

        public void DarDeBaja(int id)
        {
            Cliente cliente = GetById(id);

            if (repo.TieneMascotasActivas(id))
            {
                throw new NegocioException("No se puede dar de baja el cliente porque tiene mascotas activas.");
            }

            cliente.DarDeBaja();
            repo.Update(cliente);
        }

        public void Reactivar(int id)
        {
            Cliente cliente = GetById(id);
            cliente.Reactivar();
            repo.Update(cliente);
        }

        private void CargarDatos(Cliente cliente, ClienteDto dto)
        {
            cliente.Codigo = (dto.Codigo ?? string.Empty).Trim().ToUpper();
            cliente.Nombre = (dto.Nombre ?? string.Empty).Trim();
            cliente.Apellido = (dto.Apellido ?? string.Empty).Trim();
            cliente.Telefono = string.IsNullOrWhiteSpace(dto.Telefono) ? null : dto.Telefono.Trim();
            cliente.Sexo = ConvertirSexo(dto.Sexo);
        }

        // Acepta "M", "F", "X" o el nombre del enum ("Masculino", "Femenino", "Otro")
        private static Sexo ConvertirSexo(string? valor)
        {
            switch ((valor ?? string.Empty).Trim().ToUpper())
            {
                case "M":
                case "MASCULINO":
                    return Sexo.Masculino;
                case "F":
                case "FEMENINO":
                    return Sexo.Femenino;
                case "X":
                case "OTRO":
                    return Sexo.Otro;
                default:
                    throw new NegocioException("El sexo debe ser M, F o X.");
            }
        }

        private void ValidarCodigoUnico(string codigo, int idExcluir)
        {
            if (repo.ExisteCodigo(codigo, idExcluir))
            {
                throw new NegocioException($"Ya existe un cliente con el código {codigo}.");
            }
        }
    }
}
