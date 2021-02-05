using DeliveryHouse.Common.Entities;
using DeliveryHouse.Web.Data;
using DeliveryHouse.Web.Helpers;
using DeliveryHouse.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryHouse.Web.Controllers
{
    [Authorize(Roles = "Admin")]
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
                    ModelState.AddModelError(string.Empty, $"Category {model.Name} ya existe");
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



        //------------------------PRODUCT------------------------//
        public async Task<IActionResult> DetailsProduct(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        public async Task<IActionResult> AddProduct(int? id)
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

            ProductViewModel model = new ProductViewModel
            {
                Category = category,
                IdCategory = category.Id,
                IsActive = true
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                string path = string.Empty;

                Category category = await _context.Categories.Include(c => c.Products)
                                                             .FirstOrDefaultAsync(m => m.Id == model.IdCategory);

                if (category == null)
                {
                    return NotFound();  
                }

                if (model.ImageFile != null)
                {
                    path = await _imageHelper.UploadImageAsync(model.ImageFile, "products");
                }

                Product product = _converterHelper.ToProductEntity(model, path, true);

                if (product == null)
                {
                    return NotFound();
                }

                try
                {
                    product.Id = 0;
                    category.Products.Add(product);
                    _context.Update(category);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Details", "Categories", new { Id = model.IdCategory });
                }
                catch(DbUpdateException dbu)
                {
                    if (dbu.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, $"Product {model.Name} duplicado");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbu.InnerException.Message);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> EditProduct(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            ProductViewModel model = _converterHelper.ToProductViewModel(product);
            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Products.FirstOrDefault(p => p.Id == model.Id) != null);
            model.IdCategory = category.Id;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                string path = model.ImageProduct;

                if (model.ImageFile != null)
                {
                    path = await _imageHelper.UploadImageAsync(model.ImageFile, "products");
                }

                Product product = _converterHelper.ToProductEntity(model, path, false);

                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Details", "Categories", new { Id = model.IdCategory });
                }
                catch(DbUpdateException db)
                {
                    if (db.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, $"Prodcuto {product.Name} ya existe");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, db.InnerException.Message);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.InnerException.Message);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> DeleteProduct(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product product = await _context.Products.FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Products.FirstOrDefault(p => p.Id == product.Id) != null);

            try
            {
                _context.Remove(product);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.InnerException.Message);
            }

            return RedirectToAction("Details", "Categories", new { Id = category.Id });

        }
    }
}
