using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalogo.Controllers
{
    [Route("api/[controller]")]
    //[ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProdutosController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> Get()
        {
            return await _context.Produtos.AsNoTracking().ToListAsync();
        }

        [HttpGet("{id:int}", Name = "ObterProduto")]
        public ActionResult<Produto> Get(int id)
        {
            var produto = _context.Produtos.AsNoTracking().FirstOrDefault(p => p.Id == id);

            if (produto == null)
                return NotFound();

            return produto;
        }

        [HttpPost]
        public ActionResult Post([FromBody] Produto produto)
        {
            //NÃO É NECESSÁRIO VERIRICAR SE MODELSTATE É VÁLIDO POR CONTA DO ATTRIBUTE [ApiController]")]
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Produtos.Add(produto);
            _context.SaveChanges();

            return new CreatedAtRouteResult("ObterProduto", new { id = produto.Id }, produto);
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id, [FromBody] Produto produto)
        {
            //if (!ModelState.IsValid)
            //    return BadRequest(ModelState);

            if (id != produto.Id)
                return BadRequest();
            //Minha versão
            //var _produto = _context.Produtos.Find(id);

            //_produto.Descricao = produto.Descricao;
            //_produto.ImagemUrl = produto.ImagemUrl;
            //_produto.Nome = produto.Nome;
            //_produto.Preco = produto.Preco;
            //_produto.Estoque = produto.Estoque;
            //_produto.CategoriaId = produto.CategoriaId;

            //_context.Produtos.Update(_produto);
            //_context.SaveChanges();

            //Versão do professor
            _context.Entry(produto).State = EntityState.Modified;
            _context.SaveChanges();

            return Ok();
        }

        [HttpDelete("{id:int}")]
        public ActionResult<Produto> Delete(int id)
        {
            //var produto = _context.Produtos.FirstOrDefault(x => x.Id == id);
            var produto = _context.Produtos.Find(id); //Obs: Vantagem do método Find, ele busca primeiro em memória, se não achar ele vai até o banco de dados, FirstOrDefault vai direto ao banco.

            if (produto == null)
                return NotFound();

            _context.Produtos.Remove(produto);
            _context.SaveChanges();

            return produto;
        }
    }
}
