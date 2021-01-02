using DeliveryHouse.Common.Entities;
using DeliveryHouse.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryHouse.Web.Helpers
{
    public interface IConverterHelper
    {
        Country ToCountryEntity(CountryViewModel model, string path, bool isnew);
        CountryViewModel ToCountryViewModel(Country entity);


        Store ToStoreEntity(StoreViewModel model, string path, bool isnew);
        StoreViewModel ToStoreViewModel(Store entity);

        Category ToCategoryEntity(CategoryViewModel model, string path, bool isnew);
        CategoryViewModel ToCategoryViewModel(Category entity);

        Product ToProductEntity(ProductViewModel model, string path, bool isNew);
        ProductViewModel ToProductViewModel(Product entity);

    }
}
