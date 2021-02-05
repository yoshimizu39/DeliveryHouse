using DeliveryHouse.Common.Entities;
using DeliveryHouse.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryHouse.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly DataContext _context;

        public CategoriesController(DataContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> GetCategoriesProducts()
        {
            List<Category> categories = await _context.Categories.Include(c => c.Products)
                                          .OrderBy(c => c.Name)
                                          .ToListAsync();

            return Ok(categories);
        }
    }
}
