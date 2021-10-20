using APICatalogo.Models;
using System.Collections.Generic;

namespace APICatalogo.Repository.Interfaces
{
    public interface ICategoriaRepository : IRepository<Categoria>
    {
        IEnumerable<Categoria> GetCategoriaProdutos();
    }
}
