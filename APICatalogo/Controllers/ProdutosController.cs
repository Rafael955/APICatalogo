using APICatalogo.Context;
using APICatalogo.DTOs;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repository.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
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
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Produces("application/json")]
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
        /// Obtem o produto pelo seu identificador produtoId
        /// </summary>
        /// <param name="id">Código do produto</param>
        /// <returns>Um objeto Produto</returns>
        //[ProducesResponseType(typeof(ProdutoDTO), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id:int}", Name = "ObterProduto")]
        [EnableCors("PermitirApiRequest")]
        public async Task<ActionResult<ProdutoDTO>> Get(int id)
        {
            var produto = await _uow.ProdutoRepository.GetById(x => x.Id == id);

            if (produto == null)
                return NotFound();

            return _mapper.Map<ProdutoDTO>(produto);
        }

        /// <summary>
        /// Inclui um novo produto
        /// </summary>
        /// <param name="produtoDto">objeto Produto</param>
        /// <returns>O objeto Produto incluido</returns>
        /// <remarks>Retorna um objeto Produto incluído</remarks>
        //[ProducesResponseType(StatusCodes.Status201Created)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] ProdutoDTO produtoDto)
        {
            var produto = _mapper.Map<Produto>(produtoDto);

            _uow.ProdutoRepository.Add(produto);
            await _uow.Commit();

            var produtoDTO = _mapper.Map<ProdutoDTO>(produto);

            return new CreatedAtRouteResult("ObterProduto", new { id = produtoDTO.ProdutoId }, produtoDTO);
        }

        /// <summary>
        /// Atualiza um produto pelo seu identificador produtoId
        /// </summary>
        /// <param name="id"></param>
        /// <param name="produtoDto"></param>
        /// <returns></returns>
        [HttpPut("{id:int}")]
        //[ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        public async Task<ActionResult> Put(int id, [FromBody] ProdutoDTO produtoDto)
        {
            if (id != produtoDto.ProdutoId)
                return BadRequest();

            _uow.ProdutoRepository.Update(_mapper.Map<Produto>(produtoDto));
            await _uow.Commit();

            return Ok();
        }

        /// <summary>
        /// Remove um produto pelo seu identificador produtoId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:int}")]
        //[ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        public async Task<ActionResult<ProdutoDTO>> Delete(int id)
        {
            var produto = await _uow.ProdutoRepository.GetById(x => x.Id == id);

            if (produto == null)
                return NotFound();

            _uow.ProdutoRepository.Delete(produto);
            await _uow.Commit();

            return _mapper.Map<ProdutoDTO>(produto);
        }
    }
}
