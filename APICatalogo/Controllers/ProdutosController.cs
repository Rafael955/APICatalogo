using APICatalogo.Context;
using APICatalogo.DTOs;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repository.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalogo.Controllers
{
    //[Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        //private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ProdutosController(IUnitOfWork uof, IMapper mapper)
        {
            _uow = uof;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtém os produtos ordenados por preço na ordem ascendente
        /// </summary>
        /// <param name="none"></param>
        /// <returns>Lista de objetos Produtos ordenados por preço</returns>
        [HttpGet("menor-preco")]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutosPrecos()
        {
            return _mapper.Map<List<ProdutoDTO>>(await _uow.ProdutoRepository.GetProdutosPorPreco());
        }

        //[ServiceFilter(typeof(ApiLoggingFilter))]
        /// <summary>
        /// Exibe uma relação dos produtos
        /// </summary>
        /// <returns>Retorna uma lista de objetos Produto</returns>
        // api/produtos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get([FromQuery] ProdutosParameters produtosParameters)
        {
            var produtos = await _uow.ProdutoRepository.GetProdutos(produtosParameters);

            //informações para se incluir no header do HTTP response;
            var metadata = new
            {
                produtos.TotalCount,
                produtos.PageSize,
                produtos.CurrentPage,
                produtos.TotalPages,
                produtos.NasNext,
                produtos.HasPrevious
            };

            //Inclui dados no Header do Http Response
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            var produtosDto = _mapper.Map<List<ProdutoDTO>>(produtos);

            return produtosDto;
        }

        /// <summary>
        /// Obtem o produto pelo seu identificado produtoId
        /// </summary>
        /// <param name="id">Código do produto</param>
        /// <returns>Um objeto Produto</returns>
        [HttpGet("{id:int}", Name = "ObterProduto")]
        [EnableCors("PermitirApiRequest")]
        public async Task<ActionResult<ProdutoDTO>> Get(int id)
        {
            ////throw new Exception("Exception ao retornar produto pelo id!");
            //string[] teste = null;
            //if(teste.Length > 0)
            //{

            //}

            var produto = await _uow.ProdutoRepository.GetById(x => x.Id == id);

            if (produto == null)
                return NotFound();

            return _mapper.Map<ProdutoDTO>(produto);
        }

        /// <summary>
        /// Incluir um novo produto
        /// </summary>
        /// <param name="produtoDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] ProdutoDTO produtoDto)
        {
            //NÃO É NECESSÁRIO VERIRICAR SE MODELSTATE É VÁLIDO POR CONTA DO ATTRIBUTE [ApiController]")]
            //if (!ModelState.IsValid)
            //    return BadRequest(ModelState);
            var produto = _mapper.Map<Produto>(produtoDto);

            _uow.ProdutoRepository.Add(produto);
            await _uow.Commit();

            var produtoDTO = _mapper.Map<ProdutoDTO>(produto);

            return new CreatedAtRouteResult("ObterProduto", new { id = produtoDTO.ProdutoId }, produtoDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] ProdutoDTO produtoDto)
        {
            //if (!ModelState.IsValid)
            //    return BadRequest(ModelState);

            if (id != produtoDto.ProdutoId)
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
            _uow.ProdutoRepository.Update(_mapper.Map<Produto>(produtoDto));
            await _uow.Commit();

            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ProdutoDTO>> Delete(int id)
        {
            //var produto = _uow.Produtos.FirstOrDefault(x => x.Id == id);
            var produto = await _uow.ProdutoRepository.GetById(x => x.Id == id); //Obs: Vantagem do método Find, ele busca primeiro em memória, se não achar ele vai até o banco de dados, FirstOrDefault vai direto ao banco.

            if (produto == null)
                return NotFound();

            _uow.ProdutoRepository.Delete(produto);
            await _uow.Commit();

            return _mapper.Map<ProdutoDTO>(produto);
        }
    }
}
