using APICatalogo.Context;
using APICatalogo.Controllers;
using APICatalogo.DTOs;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Repository;
using APICatalogo.Repository.Interfaces;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ApiCatalogoXUnitTests
{
    public class CategoriasUnitTestController
    {
        private IMapper mapper;
        private IUnitOfWork repository;

        public static DbContextOptions<ApplicationDbContext> dbContextOptions { get; }

        public static string connectionString = "Server=localhost;Database=CatalogoDB;Uid=root;Pwd=1234";


        //Construtores estáticos são inicializados automaticamente antes dos outros.
        static CategoriasUnitTestController() 
        {
            dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                .Options;
        }

        public CategoriasUnitTestController()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });

            mapper = config.CreateMapper();

            var context = new ApplicationDbContext(dbContextOptions);

            //DBUnitTestsMockInitializer db = new DBUnitTestsMockInitializer();
            //db.Seed(context);

            repository = new UnitOfWork(context);
        }

        //testes unitários

        //TESTE - MÉTODO GET
        [Fact]
        public void GetCategorias_Returns_OkResult()
        {
            //Arrange
            var controller = new CategoriasController(repository, mapper);

            //Act
            var data = controller.Get();

            //Assert
            Assert.IsType<List<CategoriaDTO>>(data.Value);
        }

        //TESTE - MÉTODO GET BY ID
        [Fact]
        public async Task GetCategoriasById_Returns_OkResult()
        {
            //Arrange
            var controller = new CategoriasController(repository, mapper);
            var categoriaId = 2;

            //Act
            var data = await controller.Get(categoriaId);

            //Assert
            Assert.IsType<CategoriaDTO>(data.Value);
        }

        //TESTE - MÉTODO GET BY ID - NOT FOUND
        [Fact]
        public async Task GetCategoriasById_Returns_NotFoundResult()
        {
            //Arrange
            var controller = new CategoriasController(repository, mapper);
            var categoriaId = 999;

            //Act
            var data = await controller.Get(categoriaId);

            //Assert
            Assert.IsType<NotFoundObjectResult>(data.Result);
        }

        //TESTE - MÉTODO GET - BADREQUEST
        [Fact]
        public void GetCategorias_Returns_BadRequest()
        {
            //Arrange
            var controller = new CategoriasController(repository, mapper);

            //Act
            var data = controller.Get();

            //Assert
            Assert.IsType<BadRequestResult>(data.Result);
        }

        //TESTE - MÉTODO GET - RETORNO DE UMA LISTA DE OBJETOS CATEGORIA
        [Fact]
        public void GetCategorias_MatchResult()
        {
            //Arrange
            var controller = new CategoriasController(repository, mapper);

            //Act
            var data = controller.Get();

            //Assert
            Assert.IsType<List<CategoriaDTO>>(data.Value);
            var cat = data.Value.Should().BeAssignableTo<List<CategoriaDTO>>().Subject;

            Assert.Equal("Bebidas alteradas", cat[0].Nome);
            Assert.Equal("http://www.macoratti.net/Imagens/1.jpg", cat[0].ImagemUrl);

            Assert.Equal("Sobremesas", cat[2].Nome);
            Assert.Equal("http://www.macoratti.net/Imagens/3.jpg", cat[2].ImagemUrl);
        }

        //TESTE - MÉTODO POST
        [Fact]
        public async Task Post_Categoria_AddValidData_Returns_CreatedResult()
        {
            //Arrange
            var controller = new CategoriasController(repository, mapper);

            var cat = new CategoriaDTO
            {
                Nome = "Teste Unitario Inclusao",
                ImagemUrl = "testeunitario.jpg"
            };

            //Act
            var data = await controller.Post(cat);

            //Assert
            Assert.IsType<CreatedAtRouteResult>(data);
        }

        //TESTE - MÉTODO PUT
        [Fact]
        public async Task Put_Categoria_UpdateValidData_Returns_OkResult()
        {
            //Arrange
            var controller = new CategoriasController(repository, mapper);
            var catId = 2;

            //Act
            var existingPost = await controller.Get(catId);
            CategoriaDTO result = existingPost.Value.Should().BeAssignableTo<CategoriaDTO>().Subject;

            var catDto = new CategoriaDTO();
            catDto.CategoriaId = catId;
            catDto.Nome = "Categoria Atualizada - Testes 1";
            catDto.ImagemUrl = result.ImagemUrl;

            var updatedData = await controller.Put(catId, catDto);

            //Assert
            Assert.IsType<OkObjectResult>(updatedData);
        }

        //TESTE - MÉTODO DELETE
        [Fact]
        public async Task Delete_Categoria_Returns_OkResult()
        {
            //Arrange
            var controller = new CategoriasController(repository, mapper);
            var catId = 9;

            //Act
            var deletedData = await controller.Delete(catId);

            //Assert
            Assert.IsType<CategoriaDTO>(deletedData.Value);
        }
    }
}
