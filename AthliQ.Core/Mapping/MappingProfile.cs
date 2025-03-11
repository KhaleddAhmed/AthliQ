﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AthliQ.Core.DTOs.Auth;
using AthliQ.Core.DTOs.Category;
using AthliQ.Core.DTOs.Child;
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
            MapUser();
            MapCategory();
            MapTest();
            MapSport();
            MapChild();
        }

        private void MapUser()
        {
            CreateMap<RegisterDto, AthliQUser>();
        }

        private void MapCategory()
        {
            CreateMap<CreateCategoryDto, Category>();
            CreateMap<Category, GetCategoryDto>().ReverseMap();
            CreateMap<Category, GetCategoryWithSportsDto>()
                .ForMember(
                    dto => dto.SportsName,
                    options => options.MapFrom(c => c.Sports.Select(s => s.Name))
                );
            CreateMap<Category, GetAllCategoriesDto>();
            CreateMap<UpdateCategoryDto, Category>();
        }

        private void MapTest()
        {
            CreateMap<CreateTestDto, Test>();
            CreateMap<Test, GetTestDto>()
                .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name));
            CreateMap<Test, GetAllTestDto>();
            CreateMap<UpdateTestDto, Test>();
        }

        private void MapSport()
        {
            CreateMap<CreateSportDto, Sport>();
            CreateMap<Sport, GetSportDto>()
                .ForMember(
                    dto => dto.CategoryName,
                    options => options.MapFrom(s => s.Category.Name)
                );
            CreateMap<Sport, GetAllSportsDto>();
            CreateMap<UpdateSportDto, Sport>();
        }

        private void MapChild()
        {
            CreateMap<CreateChildDto, Child>();
        }
    }
}
