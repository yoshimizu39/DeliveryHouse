using DeliveryHouse.Common.Entities;
using DeliveryHouse.Web.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryHouse.Web.Helpers
{
    public class ComboHelper : ICombosHelper
    {
        private readonly DataContext _context;

        public ComboHelper(DataContext context)
        {
            _context = context;
        }
        public IEnumerable<SelectListItem> GetComboCities(int IdDepartment)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            Department department = _context.Departments.Include(d => d.Cities)
                                                        .FirstOrDefault(m => m.Id == IdDepartment);

            if (department !=  null)
            {
                listItems = department.Cities.Select(d => new SelectListItem
                {
                    Text = d.Name,
                    Value = $"{d.Id}"
                })
                    .OrderBy(d => d.Text)
                    .ToList();
            }

            listItems.Insert(0, new SelectListItem
            {
                Text = "[Select a city...]",
                Value = "0"
            });

            return listItems;
        }

        public IEnumerable<SelectListItem> GetComboCountries()
        {
            List<SelectListItem> listItems = _context.Countries.Select(t => new SelectListItem
            {
                Text = t.Name,
                Value = $"{t.Id}"
            })
                .OrderBy(t => t.Text)
                .ToList();

            listItems.Insert(0, new SelectListItem
            {
                Text = "[Select a country...]",
                Value = "0"
            });

            return listItems;
        }

        public IEnumerable<SelectListItem> GetComboDepartments(int IdCountry)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            Country country = _context.Countries.Include(c => c.Departments)
                                                      .FirstOrDefault(m => m.Id == IdCountry);

            if (country != null)
            {
                listItems = country.Departments.Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = $"{c.Id}"
                })
                    .OrderBy(c => c.Text)
                    .ToList();
            }

            listItems.Insert(0, new SelectListItem
            {
                Text = "[Select a Department...]",
                Value = "0"
            });

            return listItems;
        }
    }
}
