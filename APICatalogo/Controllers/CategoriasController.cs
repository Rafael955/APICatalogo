using APICatalogo.Context;
using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repository.Interfaces;
using APICatalogo.Servicos.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalogo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        //private readonly ApplicationDbContext _uow;
        private readonly IUnitOfWork _uow;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public CategoriasController(IUnitOfWork uow, IConfiguration config, ILogger<CategoriasController> logger, IMapper mapper)
        {
            _uow = uow;
            _configuration = config;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("autor")]
        public string GetAutor()
        {
            var autor = _configuration["autor"];
            //Pegando a string de conexão
            //var connectionString = _configuration["ConnectionStrings:DefaultConnection"];

            return $"Autor: {autor}";
        }

        [HttpGet("saudacao/{nome:maxlength(15)}")]
        public ActionResult<string> GetSaudacao([FromServices] IMeuServico meuservico, string nome)
        {
            return meuservico.Saudacao(nome);
        }

        [HttpGet]
        public ActionResult<IEnumerable<CategoriaDTO>> Get([FromQuery] CategoriasParameters categoriasParameters)
        {
            try
            {
                var categorias = _uow.CategoriaRepository.GetCategorias(categoriasParameters);

                var metadata = new
                {
                    categorias.TotalCount,
                    categorias.PageSize,
                    categorias.CurrentPage,
                    categorias.TotalPages,
                    categorias.NasNext,
                    categorias.HasPrevious
                };

                //Inclui dados no Header do Http Response
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                var categoriaDto = _mapper.Map<List<CategoriaDTO>>(categorias);

                return categoriaDto;
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao tentar obter as categorias do banco de dados!");
            }
        }

        [HttpGet("produtos")]
        public ActionResult<IEnumerable<CategoriaDTO>> GetCategoriasProdutos()
        {
            try
            {
                //_logger.LogInformation(" ============== GET api/categorias/produtos ============== ");
                var categoriasProdutoes = _uow.CategoriaRepository.GetCategoriaProdutos().ToList();

                return _mapper.Map<List<CategoriaDTO>>(categoriasProdutoes);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao tentar obter as categorias do banco de dados!");
            }
        }

        //api/categorias/[numero inteiro > 0]
        [HttpGet("{id:int:min(1)}", Name = "ObterCategoria")] //min(1) - estipula um ID mínimo igual a 1
        public ActionResult<CategoriaDTO> Get(int id)
        {
            try
            {
                //_logger.LogInformation($" ============== GET api/categorias/produtos/{id} ============== ");

                var categoria = _uow.CategoriaRepository.GetById(x => x.Id == id);

                if (categoria == null)
                {
                   //_logger.LogInformation($" ============== GET api/categorias/produtos/{id} NOT FOUND ============== ");
                    return NotFound($"Categoria com ID {id} não foi encontrada!");
                }

                return _mapper.Map<CategoriaDTO>(categoria);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao tentar obter a categoria do banco de dados!");
            }
        }

        [HttpGet("/primeiro")] // A barra(/) ignora a rota estabelecida no atributo [Route] decorando a classe
        public ActionResult<CategoriaDTO> GetPrimeiro()
        {
            try
            {
                return _mapper.Map<CategoriaDTO>(_uow.CategoriaRepository.Get().FirstOrDefault());
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao tentar obter a categoria do banco de dados!");
            }
        }

        [HttpPost]
        public ActionResult Post([FromBody] CategoriaDTO categoriaDto)
        {
            try
            {
                _uow.CategoriaRepository.Add(_mapper.Map<Categoria>(categoriaDto));
                _uow.Commit();

                return new CreatedAtRouteResult("ObterCategoria", new { id = categoriaDto.CategoriaId }, categoriaDto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao tentar criar nova categoria!");
            }
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id, [FromBody] CategoriaDTO categoriaDto)
        {
            try
            {
                if (id != categoriaDto.CategoriaId)
                    return BadRequest($"Não foi possível atualizar a categoria com ID {id}");

                _uow.CategoriaRepository.Update(_mapper.Map<Categoria>(categoriaDto));
                _uow.Commit();

                var categoriaAlt = _uow.CategoriaRepository.GetById(x => x.Id == id);

                return Ok(categoriaAlt);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível atualizar a categoria com ID {id}");
            }
        }

        [HttpDelete("{id:int}")]
        public ActionResult<CategoriaDTO> Delete(int id)
        {
            try
            {
                var categoria = _uow.CategoriaRepository.GetById(x => x.Id == id);

                if (categoria == null)
                    return NotFound($"A categoria com ID {id} não foi encontrada!");

                _uow.CategoriaRepository.Delete(categoria);
                _uow.Commit();

                return _mapper.Map<CategoriaDTO>(categoria);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível excluir a categoria com ID {id}");
            }
        }
    }
}
