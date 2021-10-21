using APICatalogo.Context;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Repository.Interfaces;
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
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        //private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _uow;

        public ProdutosController(IUnitOfWork uof)
        {
            _uow = uof;
        }

        [HttpGet("menor-preco")]
        public ActionResult<IEnumerable<Produto>> GetProdutosPrecos()
        {
            return _uow.ProdutoRepository.GetProdutosPorPreco().ToList();
        }

        [HttpGet]
        //[ServiceFilter(typeof(ApiLoggingFilter))]
        public ActionResult<IEnumerable<Produto>> Get()
        {
            return _uow.ProdutoRepository.Get().AsNoTracking().ToList();
        }

        [HttpGet("{id:int}", Name = "ObterProduto")]
        public ActionResult<Produto> Get(int id)
        {
            ////throw new Exception("Exception ao retornar produto pelo id!");
            //string[] teste = null;
            //if(teste.Length > 0)
            //{

            //}

            var produto = _uow.ProdutoRepository.GetById(x => x.Id == id);

            if (produto == null)
                return NotFound();

            return produto;
        }

        [HttpPost]
        public ActionResult Post([FromBody] Produto produto)
        {
            //NÃO É NECESSÁRIO VERIRICAR SE MODELSTATE É VÁLIDO POR CONTA DO ATTRIBUTE [ApiController]")]
            //if (!ModelState.IsValid)
            //    return BadRequest(ModelState);

            _uow.ProdutoRepository.Add(produto);
            _uow.Commit();

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
            //var _produto = _uow.Produtos.Find(id);

            //_produto.Descricao = produto.Descricao;
            //_produto.ImagemUrl = produto.ImagemUrl;
            //_produto.Nome = produto.Nome;
            //_produto.Preco = produto.Preco;
            //_produto.Estoque = produto.Estoque;
            //_produto.CategoriaId = produto.CategoriaId;

            //_uow.Produtos.Update(_produto);
            //_uow.SaveChanges();

            //Versão do professor
            _uow.ProdutoRepository.Update(produto);

            return Ok();
        }

        [HttpDelete("{id:int}")]
        public ActionResult<Produto> Delete(int id)
        {
            //var produto = _uow.Produtos.FirstOrDefault(x => x.Id == id);
            var produto = _uow.ProdutoRepository.GetById(x =>x.Id == id); //Obs: Vantagem do método Find, ele busca primeiro em memória, se não achar ele vai até o banco de dados, FirstOrDefault vai direto ao banco.

            if (produto == null)
                return NotFound();

            _uow.ProdutoRepository.Delete(produto);
            _uow.Commit();

            return produto;
        }
    }
}
