using AutoMapper;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;

namespace MagicVilla_Web
{
    public class Mappingconfig : Profile
    {
        public Mappingconfig()
        {
            CreateMap<VillaDto, VillaCreateDTO>().ReverseMap();
            CreateMap<VillaDto, VillaUpdateDTO>().ReverseMap();

            CreateMap<VillaNumberDTO , VillaNumberUpdateDTO>().ReverseMap();
            CreateMap<VillaNumberDTO , VillaNumberCreateDTO>().ReverseMap();
        }
    }
}