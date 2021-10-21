using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Repository.Interfaces;
using APICatalogo.Servicos.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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

        public CategoriasController(IUnitOfWork uow, IConfiguration config, ILogger<CategoriasController> logger)
        {
            _uow = uow;
            _configuration = config;
            _logger = logger;
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
        public ActionResult<IEnumerable<Categoria>> Get()
        {
            try
            {
                return _uow.CategoriaRepository.Get().AsNoTracking().ToList();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao tentar obter as categorias do banco de dados!");
            }
        }

        [HttpGet("produtos")]
        public ActionResult<IEnumerable<Categoria>> GetCategoriasProdutos()
        {
            try
            {
                _logger.LogInformation(" ============== GET api/categorias/produtos ============== ");

                return _uow.CategoriaRepository.GetCategoriaProdutos().ToList();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao tentar obter as categorias do banco de dados!");
            }
        }

        //api/categorias/[numero inteiro > 0]
        [HttpGet("{id:int:min(1)}", Name = "ObterCategoria")] //min(1) - estipula um ID mínimo igual a 1
        public ActionResult<Categoria> Get(int id)
        {
            try
            {
                _logger.LogInformation($" ============== GET api/categorias/produtos/{id} ============== ");

                var categoria = _uow.CategoriaRepository.GetById(x => x.Id == id);

                if (categoria == null)
                {
                    _logger.LogInformation($" ============== GET api/categorias/produtos/{id} NOT FOUND ============== ");
                    return NotFound($"Categoria com ID {id} não foi encontrada!");
                }

                return categoria;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao tentar obter a categoria do banco de dados!");
            }
        }

        [HttpGet("/primeiro")] // A barra(/) ignora a rota estabelecida no atributo [Route] decorando a classe
        public ActionResult<Categoria> GetPrimeiro()
        {
            try
            {
                return _uow.CategoriaRepository.Get().FirstOrDefault();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao tentar obter a categoria do banco de dados!");
            }
        }

        [HttpPost]
        public ActionResult Post([FromBody] Categoria categoria)
        {
            try
            {
                _uow.CategoriaRepository.Add(categoria);
                _uow.Commit();

                return new CreatedAtRouteResult("ObterCategoria", new { id = categoria.Id }, categoria);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao tentar criar nova categoria!");
            }
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id, [FromBody] Categoria categoria)
        {
            try
            {
                if (id != categoria.Id)
                    return BadRequest($"Não foi possível atualizar a categoria com ID {id}");

                _uow.CategoriaRepository.Update(categoria);
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
        public ActionResult<Categoria> Delete(int id)
        {
            try
            {
                var categoria = _uow.CategoriaRepository.GetById(x => x.Id == id);

                if (categoria == null)
                    return NotFound($"A categoria com ID {id} não foi encontrada!");

                _uow.CategoriaRepository.Delete(categoria);
                _uow.Commit();

                return categoria;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível excluir a categoria com ID {id}");
            }
        }
    }
}
