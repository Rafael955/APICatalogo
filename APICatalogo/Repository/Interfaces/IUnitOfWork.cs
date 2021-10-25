using System.Threading.Tasks;

namespace APICatalogo.Repository.Interfaces
{
    public interface IUnitOfWork
    {
        IProdutoRepository ProdutoRepository { get; }

        ICategoriaRepository CategoriaRepository { get; }

        Task Commit();
    }
}
