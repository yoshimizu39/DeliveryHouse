using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DeliveryHouse.Common.Entities;
using DeliveryHouse.Web.Data;
using DeliveryHouse.Web.Helpers;
using DeliveryHouse.Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace DeliveryHouse.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CountriesController : Controller
    {
        private readonly DataContext _context;
        private readonly IImageHelper _imageHelper;
        private readonly IConverterHelper _converterHelper;

        public CountriesController(DataContext context, IImageHelper imageHelper, IConverterHelper converterHelper)
        {
            _context = context;
            _imageHelper = imageHelper;
            _converterHelper = converterHelper;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Countries.Include(c => c.Departments)
                                                .ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Country country = await _context.Countries.Include(c => c.Departments)
                                                      .ThenInclude(d => d.Cities)
                                                      .FirstOrDefaultAsync(m => m.Id == id);

            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CountryViewModel model)
        {
            if (ModelState.IsValid)
            {
                string path = string.Empty;

                if (model.ImageFile != null)
                {
                    path = await _imageHelper.UploadImageAsync(model.ImageFile, "countries");
                }

                Country country = _converterHelper.ToCountryEntity(model, path, true);
                _context.Add(country);

                try
                {
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    if (ex.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, $"El país {country.Name} ya existe");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, ex.InnerException.Message);
                    }
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Country country = await _context.Countries.FindAsync(id);

            if (country == null)
            {
                return NotFound();
            }

            CountryViewModel model = _converterHelper.ToCountryViewModel(country);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CountryViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                string image = model.ImageCountry;

                if (model.ImageFile != null)
                {
                    image = await _imageHelper.UploadImageAsync(model.ImageFile, "countries");
                }
                try
                {
                    Country entity = _converterHelper.ToCountryEntity(model, image, false);
                    _context.Update(entity);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, $"Datos del País {model.Name} actualizados");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"País {model.Name} ya existe");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Country country = await _context.Countries.FirstOrDefaultAsync(m => m.Id == id);

            if (country == null)
            {
                return NotFound();
            }

            try
            {
                _context.Countries.Remove(country);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }

            return RedirectToAction(nameof(Index));
        }



        //-------------------------------DEPARMENT----------------------------------//
        public async Task<IActionResult> AddDepartment(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Country country = await _context.Countries.FindAsync(id);

            if (country == null)
            {
                return NotFound();
            }

            Department model = new Department { IdCountry = country.Id };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDepartment(Department department)
        {
            if (ModelState.IsValid)
            {
                Country country = await _context.Countries.Include(c => c.Departments)
                                                          .FirstOrDefaultAsync(c => c.Id == department.IdCountry);

                if (country == null)
                {
                    return NotFound();
                }

                try
                {
                    department.Id = 0;
                    country.Departments.Add(department);
                    _context.Update(country);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Details", "Countries", new { Id = department.IdCountry });
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, $"Ya existe un registro con el nombre {department.Name}");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            return View(department);
        }

        public async Task<IActionResult> EditDepartment(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Department department = await _context.Departments.FindAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            Country country = await _context.Countries.FirstOrDefaultAsync(c => c.Departments.FirstOrDefault(d => d.Id == department.Id) != null);
            department.IdCountry = country.Id;

            return View(department);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDepartment(Department department)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(department);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Details", "Countries", new { Id = department.IdCountry });

                }
                catch (DbUpdateConcurrencyException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, $"Departamento {department.Name} actualizados");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Departamento {department.Name} ya existe");
                }
            }

            return View(department);
        }

        public async Task<IActionResult> DeleteDepartment(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Department department = await _context.Departments.Include(d => d.Cities)
                                                              .FirstOrDefaultAsync(d => d.Id == id);

            if (department == null)
            {
                return NotFound();
            }

            Country country = await _context.Countries.FirstOrDefaultAsync(c => c.Departments.FirstOrDefault(d => d.Id == department.Id) != null);
            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Countries", new { Id = country.Id });
        }

        public async Task<IActionResult> DetailsDepartmen(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Department department = await _context.Departments.Include(d => d.Cities)
                                                              .ThenInclude(c => c.Stores)
                                                              .FirstOrDefaultAsync(m => m.Id == id);

            if (department == null)
            {
                return NotFound();
            }

            Country country = await _context.Countries.FirstOrDefaultAsync(c => c.Departments.FirstOrDefault(d => d.Id == department.Id) != null);
            department.IdCountry = country.Id;

            return View(department);
        }



        //------------------------------CITY--------------------------------------------//

        public async Task<IActionResult> AddCity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Department department = await _context.Departments.FindAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            City city = new City { IdDepartment = department.Id };

            return View(city);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCity(City city)
        {
            if (ModelState.IsValid)
            {
                Department department = await _context.Departments.Include(d => d.Cities)
                                                                  .FirstOrDefaultAsync(m => m.Id == city.IdDepartment);

                if (department == null)
                {
                    return NotFound();
                }

                try
                {
                    city.Id = 0;
                    department.Cities.Add(city);
                    _context.Update(department);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("DetailsDepartmen", "Countries", new { Id = department.Id });
                }
                catch (DbUpdateException db)
                {
                    if (db.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, $"La ciudad {city.Name} ya existe");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, db.InnerException.Message);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            return View(city);
        }

        public async Task<IActionResult> EditCity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            City city = await _context.Cities.FindAsync(id);

            if (city == null)
            {
                return NotFound();
            }

            Department department = await _context.Departments.FirstOrDefaultAsync(d => d.Cities.FirstOrDefault(c => c.Id == city.Id) != null);
            city.IdDepartment = department.Id;

            return View(city);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCity(City city)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(city);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("DetailsDepartmen", "Countries", new { Id = city.IdDepartment });
                }
                catch (DbUpdateException db)
                {
                    if (db.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, $"La ciudad {city.Name} ya existe");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, db.InnerException.Message);
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, $"La ciudad {city.Name} ya existe");
                }
            }

            return View(city);
        }

        public async Task<IActionResult> DeleteCity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            City city = await _context.Cities.Include(c => c.Stores)
                                             .FirstOrDefaultAsync(m => m.Id == id);

            if (city == null)
            {
                return NotFound();
            }

            Department department = await _context.Departments.FirstOrDefaultAsync(d => d.Cities.FirstOrDefault(c => c.Id == city.Id) != null);
            _context.Remove(city);
            await _context.SaveChangesAsync();

            return RedirectToAction("DetailsDepartmen", "Countries", new { Id = department.Id });
        }
    }
}
