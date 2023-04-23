using AutoMapper;
using TodoApi.Models;
using TodoApi.Models.Dto;

namespace TodoApi
{
    public class MappingConfig:Profile
    {

        public MappingConfig()
        {
            CreateMap<Todo,TodoDTO>().ReverseMap();
            CreateMap<Todo,TodoCreateDTO>().ReverseMap();
            CreateMap<Todo,TodoUpdateDTO>().ReverseMap();
        }
    }
}
