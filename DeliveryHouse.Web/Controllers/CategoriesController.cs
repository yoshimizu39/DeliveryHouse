using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DeliveryHouse.Common.Entities;
using DeliveryHouse.Web.Data;
using DeliveryHouse.Web.Models;
using DeliveryHouse.Web.Helpers;

namespace DeliveryHouse.Web.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly DataContext _context;
        private readonly IImageHelper _imageHelper;
        private readonly IConverterHelper _converterHelper;

        public CategoriesController(DataContext context, IImageHelper imageHelper, IConverterHelper converterHelper)
        {
            _context = context;
            _imageHelper = imageHelper;
            _converterHelper = converterHelper;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.Include(c => c.Products)
                                                 .ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Category category = await _context.Categories.Include(c => c.Products)
                                                         .FirstOrDefaultAsync(m => m.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            //CategoryViewModel model = _converterHelper.ToCategoryViewModel(category);

            return View(category);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                string path = string.Empty;

                if (model.ImageFile != null)
                {
                    path = await _imageHelper.UploadImageAsync(model.ImageFile, "categories");
                }

                Category category = _converterHelper.ToCategoryEntity(model, path, true);
                _context.Add(category);

                try
                {
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    if (ex.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, $"Category {model.Name} is exists");
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

            Category category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            CategoryViewModel model = _converterHelper.ToCategoryViewModel(category);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                string image = model.ImageCategory;

                if (model.ImageFile != null)
                {
                    image = await _imageHelper.UploadImageAsync(model.ImageFile, "categories");
                }

                try
                {
                    Category category = _converterHelper.ToCategoryEntity(model, image, false);
                    _context.Update(category);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException db)
                {
                    if (db.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, $"Category {model.Name} is duplicate");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, db.InnerException.Message);
                    }
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            return View(model);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Category category = await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            try
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
