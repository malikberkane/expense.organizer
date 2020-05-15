using AutoMapper;
using expense.manager.Data;
using expense.manager.Models;
using expense.manager.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace expense.manager.Mapping
{
    public class ExpenseManagerMapperProfile:Profile
    {

        public ExpenseManagerMapperProfile()
        {
            CreateMap<Expense, ExpenseVm>().ReverseMap();
            CreateMap<Expense, ExpenseData>().ReverseMap();

            CreateMap<Category,CategoryVm>().ReverseMap();

            CreateMap<Category, CategoryData>().ReverseMap();

            CreateMap<Tag, TagData>().ReverseMap();

            CreateMap<Tag, TagVm>().ReverseMap();

            CreateMap<Budget, BudgetData>().ReverseMap();




        }
    }

   
    public static class MapperHelper
    {
        public static IMapper Mapper { get; set; }


        public static void Intialize()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<ExpenseManagerMapperProfile>());

            Mapper = new Mapper(config);
        }

        public static TDestination Map<TSource, TDestination>(this TSource source)
        {
            return Mapper.Map<TSource, TDestination>(source);
        }
    }


}