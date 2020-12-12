using DeliveryHouse.Common.Entities;
using DeliveryHouse.Web.Data;
using DeliveryHouse.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryHouse.Web.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        private readonly DataContext _context;

        public ConverterHelper(DataContext context)
        {
            _context = context;
        }

        public Category ToCategoryEntity(CategoryViewModel model, string path, bool isnew)
        {
            return new Category
            {
                Id = isnew ? 0 : model.Id,
                Name = model.Name,
                ImageCategory = path
            };
        }

        public CategoryViewModel ToCategoryViewModel(Category entity)
        {
            return new CategoryViewModel
            {
                Id = entity.Id,
                Name = entity.Name,
                ImageCategory = entity.ImageCategory
            };
        }

        public Country ToCountryEntity(CountryViewModel model, string path, bool isnew)
        {
            return new Country
            {
                Id = isnew ? 0 : model.Id,
                ImageCountry = path,
                Name = model.Name,
            };
        }

        public CountryViewModel ToCountryViewModel(Country entity)
        {
            return new CountryViewModel
            {
                Id = entity.Id,
                ImageCountry = entity.ImageCountry,
                Name = entity.Name
            };
        }

        public Store ToStoreEntity(StoreViewModel model, string path, bool isnew)
        {
            return new Store
            {
                Id = isnew ? 0 : model.Id,
                Name = model.Name,
                Direction = model.Direction,
                Email = model.Email,
                ImageStore = path,
                Telephone = model.Telephone
                //Categories = model.Categories
            };
        }

        public StoreViewModel ToStoreViewModel(Store entity)
        {
            return new StoreViewModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Direction = entity.Direction,
                Email = entity.Email,
                ImageStore = entity.ImageStore,
                Telephone = entity.Telephone
                //Categories = entity.Categories
            };
        }
    }
}
