using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APICatalogo.Models
{
    [Table("Produtos")]
    public class Produto : Entity<int>
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(20, ErrorMessage = "O nome deverá ter entre {2} e {1} caracteres", MinimumLength = 5)]
        public string Nome { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatório")]
        [StringLength(10, ErrorMessage = "A descrição deverá ter no máximo {1} caracteres")]
        public string Descricao { get; set; }

        [Required]
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
    }
}
