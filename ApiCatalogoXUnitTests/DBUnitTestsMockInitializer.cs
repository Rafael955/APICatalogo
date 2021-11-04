using APICatalogo.Context;
using APICatalogo.Models;

namespace ApiCatalogoXUnitTests
{
    public class DBUnitTestsMockInitializer
    {
        public DBUnitTestsMockInitializer()
        {

        }

        public void Seed(ApplicationDbContext context)
        {
            context.Categorias.Add(new Categoria { Id = 999, Nome = "Bebidas999", ImagemUrl = "bebidas999.jpg" });

            context.Categorias.Add(new Categoria { Id = 8, Nome = "Sucos", ImagemUrl = "sucos.jpg" });

            context.Categorias.Add(new Categoria { Id = 9, Nome = "Doces", ImagemUrl = "doces.jpg" });

            context.Categorias.Add(new Categoria { Id = 10, Nome = "Salgados", ImagemUrl = "salgados.jpg" });

            context.Categorias.Add(new Categoria { Id = 11, Nome = "Tortas", ImagemUrl = "tortas.jpg" });
        }
    }
}
