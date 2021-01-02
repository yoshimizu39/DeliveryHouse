using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryHouse.Web.Helpers
{
    public interface ICombosHelper
    {
        IEnumerable<SelectListItem> GetComboCountries();
        IEnumerable<SelectListItem> GetComboDepartments(int IdCountry);
        IEnumerable<SelectListItem> GetComboCities(int IdDepartment);
    }
}
