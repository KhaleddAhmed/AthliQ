using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AthliQ.Core.DTOs.Auth;
using AthliQ.Core.DTOs.Category;
using AthliQ.Core.DTOs.Test;
using AthliQ.Core.Entities;
using AthliQ.Core.Entities.Models;
using AutoMapper;

namespace AthliQ.Core.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterDto, AthliQUser>();
            CreateMap<CreateCategoryDto, Category>();
            CreateMap<Category, GetCategoryDto>().ReverseMap();
            CreateMap<CreateTestDto, Test>();
            CreateMap<Test, GetTestDto>()
                .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name));
            CreateMap<Test, GetAllTestDto>();
            CreateMap<UpdateTestDto, Test>();
        }
    }
}
