using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalogo.DTOs
{
    public class CategoriaDTO
    {
        [Key]
        public int CategoriaId { get; set; }

        public string Nome { get; set; }

        public string ImagemUrl { get; set; }

        public ICollection<ProdutoDTO> Produtos { get; set; }
    }
}
