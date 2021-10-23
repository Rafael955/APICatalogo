using APICatalogo.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalogo.DTOs.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Produto, ProdutoDTO>().ForMember(x => x.ProdutoId, map => map.MapFrom(src => src.Id));
            CreateMap<ProdutoDTO, Produto>().ForMember(x => x.Id, map => map.MapFrom(src => src.ProdutoId));

            CreateMap<Categoria, CategoriaDTO>().ForMember(x => x.CategoriaId, map => map.MapFrom(src => src.Id));
            CreateMap<CategoriaDTO, Categoria>().ForMember(x => x.Id, map => map.MapFrom(src => src.CategoriaId));
        }
    }
}
