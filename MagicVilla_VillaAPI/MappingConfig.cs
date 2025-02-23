using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;

namespace MagicVilla_VillaAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<VillaDto, VillaCreateDTO>().ReverseMap();
            CreateMap<VillaDto, VillaUpdateDTO>().ReverseMap();
            CreateMap<Villa, VillaDto>().ReverseMap();
            CreateMap<Villa, VillaUpdateDTO>().ReverseMap();
            CreateMap<Villa, VillaCreateDTO>().ReverseMap();


            CreateMap<VillaNumber , VillaNumberCreateDTO>().ReverseMap();
            CreateMap<VillaNumber, VillaNumberUpdateDTO>().ReverseMap();
            CreateMap<VillaNumberCreateDTO, VillaNumberUpdateDTO>().ReverseMap();
            CreateMap<VillaNumber , VillaNumberDTO>().ReverseMap();


            CreateMap<ApplicationUser , UserDTO>().ReverseMap();


        }
    }
}
