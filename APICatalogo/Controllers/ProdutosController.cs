﻿using APICatalogo.Context;
using APICatalogo.DTOs;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repository.Interfaces;
using AutoMapper;
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

        [HttpGet("menor-preco")]
        public ActionResult<IEnumerable<ProdutoDTO>> GetProdutosPrecos()
        {
            return _mapper.Map<List<ProdutoDTO>>(_uow.ProdutoRepository.GetProdutosPorPreco().ToList());
        }

        [HttpGet]
        //[ServiceFilter(typeof(ApiLoggingFilter))]
        public ActionResult<IEnumerable<ProdutoDTO>> Get([FromQuery] ProdutosParameters produtosParameters)
        {
            var produtos = _uow.ProdutoRepository.GetProdutos(produtosParameters);

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

        [HttpGet("{id:int}", Name = "ObterProduto")]
        public ActionResult<ProdutoDTO> Get(int id)
        {
            ////throw new Exception("Exception ao retornar produto pelo id!");
            //string[] teste = null;
            //if(teste.Length > 0)
            //{

            //}

            var produto = _uow.ProdutoRepository.GetById(x => x.Id == id);

            if (produto == null)
                return NotFound();

            return _mapper.Map<ProdutoDTO>(produto);
        }

        [HttpPost]
        public ActionResult Post([FromBody] ProdutoDTO produtoDto)
        {
            //NÃO É NECESSÁRIO VERIRICAR SE MODELSTATE É VÁLIDO POR CONTA DO ATTRIBUTE [ApiController]")]
            //if (!ModelState.IsValid)
            //    return BadRequest(ModelState);
            var produto = _mapper.Map<Produto>(produtoDto);

            _uow.ProdutoRepository.Add(produto);
            _uow.Commit();

            var produtoDTO = _mapper.Map<ProdutoDTO>(produto);

            return new CreatedAtRouteResult("ObterProduto", new { id = produtoDTO.ProdutoId }, produtoDTO);
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id, [FromBody] ProdutoDTO produtoDto)
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

            return Ok();
        }

        [HttpDelete("{id:int}")]
        public ActionResult<ProdutoDTO> Delete(int id)
        {
            //var produto = _uow.Produtos.FirstOrDefault(x => x.Id == id);
            var produto = _uow.ProdutoRepository.GetById(x => x.Id == id); //Obs: Vantagem do método Find, ele busca primeiro em memória, se não achar ele vai até o banco de dados, FirstOrDefault vai direto ao banco.

            if (produto == null)
                return NotFound();

            _uow.ProdutoRepository.Delete(produto);
            _uow.Commit();

            return _mapper.Map<ProdutoDTO>(produto);
        }
    }
}
