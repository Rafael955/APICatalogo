using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APICatalogo.Models
{
    [Table("Produtos")]
    public class Produto : Entity<int>
    {
        [Required]
        [MaxLength(80)]
        public string Nome { get; set; }

        [Required]
        [MaxLength(300)]
        public string Descricao { get; set; }

        [Required]
        [Range(0, 9999999999999999.99, ErrorMessage = "Custo total não poderá exceder um valor de 18 digitos")]
        public decimal Preco { get; set; }

        [Required]
        [MaxLength(500)]
        public string ImagemUrl { get; set; }

        public float Estoque { get; set; }

        public DateTime DataCadastro { get; private set; } = DateTime.Now;

        //EF
        //[ForeignKey("CategoriaId")]
        public virtual Categoria Categoria { get; set; }

        public int CategoriaId { get; set; }
    }
}
