using AutoMapper;
using ResourceStorageService.Application.Models;
using ResourceStorageService.Infrastructure.Models;

namespace ResourceStorageService.Application.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ResourceStorageEntity, ResourceDto>().ReverseMap();
        }
    }
}
