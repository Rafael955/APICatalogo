using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace APICatalogo.Repository
{
    public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
    {
        public CategoriaRepository(ApplicationDbContext context) : base(context)
        {

        }

        public IEnumerable<Categoria> GetCategoriaProdutos()
        {
            return Get().Include(x => x.Produtos);
        }
    }

}
