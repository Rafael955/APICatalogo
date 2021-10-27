using APICatalogo.DTOs;
using APICatalogo.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APICatalogo.Models
{
    [Table("Produtos")]
    public class Produto : Entity<int>, IValidatableObject
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(20, ErrorMessage = "O nome deverá ter entre {2} e {1} caracteres", MinimumLength = 5)]
        //[PrimeiraLetraMaiuscula]
        public string Nome { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatório")]
        [StringLength(100, ErrorMessage = "A descrição deverá ter no máximo {1} caracteres")]
        public string Descricao { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(8,2)")]
        [Range(1, 10000, ErrorMessage = "O preço deverá estar entre {1} e {2}")]
        public decimal Preco { get; set; }

        [Required]
        [StringLength(300, ErrorMessage = "A url da imagem do produto deverá ter entre {2} e {1} caracteres", MinimumLength = 10)]
        public string ImagemUrl { get; set; }

        public float Estoque { get; set; }

        public DateTime DataCadastro { get; private set; } = DateTime.Now;

        //EF
        //[ForeignKey("CategoriaId")]
        public virtual Categoria Categoria { get; set; }

        public int CategoriaId { get; set; }

        //Alternativa nativa do c# ao AutoMapper
        //public static implicit operator Produto(ProdutoDTO produtoDto)
        //{
        //    return new Produto
        //    {
        //        Id = produtoDto.ProdutoId,
        //        Descricao = produtoDto.Descricao,
        //        CategoriaId = produtoDto.CategoriaId,
        //        ImagemUrl = produtoDto.ImagemUrl,
        //        Nome = produtoDto.Nome,
        //        Preco = produtoDto.Preco
        //    };
        //}

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(Nome))
            {
                var primeiraLetra = Nome[0].ToString();

                if (primeiraLetra != primeiraLetra.ToUpper())
                {
                    yield return
                        new ValidationResult("A primeira letra do nome do produto deverá ser maiuscula!",
                        new[]
                        {
                            nameof(Nome)
                        });
                }
            }

            if (Estoque <= 0)
            {
                yield return
                    new ValidationResult("O estoque deverá ser maior do que zero!",
                    new[]
                    {
                        nameof(Estoque)
                    });
            }
        }
    }
}
