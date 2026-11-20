using Veterinaria.Logica.Models;

namespace Veterinaria.Logica.Repositories.Interfaces
{
    public interface IClienteRepository
    {
        List<Cliente> GetAll(string? texto, bool incluirInactivos);
        Cliente? GetById(int id);
        void Add(Cliente cliente);
        void Update(Cliente cliente);
        bool ExisteCodigo(string codigo, int idExcluir);
        bool TieneMascotasActivas(int idCliente);
    }
}
