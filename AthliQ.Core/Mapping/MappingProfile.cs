using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AthliQ.Core.DTOs.Auth;
using AthliQ.Core.DTOs.Category;
using AthliQ.Core.DTOs.Sport;
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

            #region Category
            CreateMap<CreateCategoryDto, Category>();
            CreateMap<Category, GetCategoryDto>().ReverseMap();
            CreateMap<Category, GetCategoryWithSportsDto>().ForMember(dto => dto.SportsName,
                                                                      options => options.MapFrom(c => c.Sports.Select(s => s.Name)));
            CreateMap<Category, GetAllCategoriesDto>();
            CreateMap<UpdateCategoryDto, Category>();
            #endregion

            #region Test
            CreateMap<CreateTestDto, Test>();
            CreateMap<Test, GetTestDto>()
                .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name));
            CreateMap<Test, GetAllTestDto>();
            CreateMap<UpdateTestDto, Test>();
            #endregion

            #region Sport
            CreateMap<CreateSportDto, Sport>();
            CreateMap<Sport , GetSportDto>().ForMember(dto => dto.CategoryName,
                                                       options => options.MapFrom(s => s.Category.Name));
            CreateMap<Sport, GetAllSportsDto>();
            CreateMap<UpdateSportDto , Sport>();
            #endregion
        }
    }
}
