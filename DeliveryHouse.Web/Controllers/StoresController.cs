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

namespace DeliveryHouse.Web.Controllers
{
    public class StoresController : Controller
    {
        private readonly DataContext _context;
        private readonly IImageHelper _imageHelper;
        private readonly IConverterHelper _converterHelper;

        public StoresController(DataContext context, IImageHelper imageHelper, IConverterHelper converterHelper)
        {
            _context = context;
            _imageHelper = imageHelper;
            _converterHelper = converterHelper;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Stores.ToListAsync());
        }

        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    Store store = await _context.Stores.Include(s => s.Categories)
        //                                       .FirstOrDefaultAsync(m => m.Id == id);

        //    if (store == null)
        //    {
        //        return NotFound();
        //    }

        //    //StoreViewModel model = _converterHelper.ToStoreViewModel(store);

        //    return View(store);
        //}

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StoreViewModel model)
        {
            if (ModelState.IsValid)
            {
                string path = string.Empty;

                if (model.ImageFile != null)
                {
                    path = await _imageHelper.UploadImageAsync(model.ImageFile, "stores");
                }

                Store store = _converterHelper.ToStoreEntity(model, path, true);
                _context.Add(store);

                try
                {
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    if (ex.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, $"El país {store.Name} ya existe");
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

            Store store = await _context.Stores.FindAsync(id);

            if (store == null)
            {
                return NotFound();
            }

            StoreViewModel model = _converterHelper.ToStoreViewModel(store);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StoreViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                string path = model.ImageStore;

                if (model.ImageFile != null)
                {
                    path = await _imageHelper.UploadImageAsync(model.ImageFile, "stores");
                }

                try
                {
                    Store store = _converterHelper.ToStoreEntity(model, path, false);
                    _context.Update(store);
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

        // GET: Stores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Store store = await _context.Stores.FirstOrDefaultAsync(m => m.Id == id);

            if (store == null)
            {
                return NotFound();
            }

            try
            {
                _context.Stores.Remove(store);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }

            return RedirectToAction(nameof(Index));
        }




        //--------------------------Category--------------------------------//
        public async Task<IActionResult> AddCategory(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Store store = await _context.Stores.FindAsync(id);

            if (store == null)
            {
                return NotFound();
            }

            CategoryViewModel model = new CategoryViewModel 
            {
                Store = store,
                IdStore = store.Id 
            };

            return View(model);
        }

        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public async Task<IActionResult> AddCategory(CategoryViewModel model)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            string path = string.Empty;

        //            Store store = await _context.Stores.Include(s => s.Categories)
        //                                               .FirstOrDefaultAsync(m => m.Id == model.IdStore);

        //            if (store == null)
        //            {
        //                return NotFound();
        //            }

        //            if (model.ImageFile != null)
        //            {
        //                path = await _imageHelper.UploadImageAsync(model.ImageFile, "categories");
        //            }

        //            Category category = _converterHelper.ToCategoryEntity(model, path, true);

        //            if (category == null)
        //            {
        //                return NotFound();
        //            }

        //            try
        //            {
        //                category.Id = 0;
        //                store.Categories.Add(category);
        //                _context.Update(store);
        //                await _context.SaveChangesAsync();

        //                return RedirectToAction("Details", "Stores", new { Id = model.IdStore });
        //            }
        //            catch (DbUpdateException db)
        //            {
        //                if (db.InnerException.Message.Contains("duplicate"))
        //                {
        //                    ModelState.AddModelError(string.Empty, $"El nombre {model.Name} es ya existe");
        //                }
        //                else
        //                {
        //                    ModelState.AddModelError(string.Empty, db.InnerException.Message);
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                ModelState.AddModelError(string.Empty, ex.Message);
        //            }
        //        }

        //        return View(model);
        //    }
    }
}
