using DeliveryHouse.Common.Entities;
using DeliveryHouse.Common.Enums;
using DeliveryHouse.Web.Data;
using DeliveryHouse.Web.Data.Entities;
using DeliveryHouse.Web.Helpers;
using DeliveryHouse.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryHouse.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly ICombosHelper _combosHelper;
        private readonly IImageHelper _imageHelper;

        public AccountController(DataContext context, IUserHelper userHelper, ICombosHelper combosHelper, IImageHelper imageHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _combosHelper = combosHelper;
            _imageHelper = imageHelper;
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                Microsoft.AspNetCore.Identity.SignInResult signInResult = await _userHelper.LoginAsync(model);
                if (signInResult.Succeeded)
                {
                    if (Request.Query.Keys.Contains("ReturnURL"))
                    {
                        return Redirect(Request.Query["ReturnURL"].First());
                    }

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Email o Password incorrectos");
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult NotAuthorized()
        {
            return View();
        }

        public IActionResult Register()
        {
            AddUserViewModel model = new AddUserViewModel
            {
                Countries = _combosHelper.GetComboCountries(),
                Departments = _combosHelper.GetComboDepartments(0),
                Cities = _combosHelper.GetComboCities(0)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                string path = string.Empty;

                if (model.ImageFile != null)
                {
                    path = await _imageHelper.UploadImageAsync(model.ImageFile, "users");
                }

                User user = await _userHelper.AddUserAsync(model, path, UserType.User);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "This email is already used.");
                    model.Countries = _combosHelper.GetComboCountries();
                    model.Departments = _combosHelper.GetComboDepartments(model.IdCountry);
                    model.Cities = _combosHelper.GetComboCities(model.IdDepartment);

                    return View(model);
                }

                LoginViewModel loginViewModel = new LoginViewModel
                {
                    UserName = model.UserName,
                    Password = model.Password,
                    RememberMe = false
                };

                var user2 = await _userHelper.LoginAsync(loginViewModel);

                if (user2.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            model.Countries = _combosHelper.GetComboCountries();
            model.Departments = _combosHelper.GetComboDepartments(model.IdCountry);
            model.Cities = _combosHelper.GetComboCities(model.IdDepartment);

            return View(model);
        }

        public JsonResult GetDepartments(int idCountry)
        {
            Country country = _context.Countries.Include(c => c.Departments)
                                                .FirstOrDefault(c => c.Id == idCountry);

            if (country == null)
            {
                return null;
            }

            return Json(country.Departments.OrderBy(d => d.Name));
        }

        public JsonResult GetCities(int idDepartment)
        {
            Department department = _context.Departments.Include(d => d.Cities)
                                                        .FirstOrDefault(d => d.Id == idDepartment);

            if (department == null)
            {
                return null;
            }

            return Json(department.Cities.OrderBy(c => c.Name));
        }

        public async Task<IActionResult> ChangeUser()
        {
            User user = await _userHelper.GetUserAsync(User.Identity.Name);

            if (user == null)
            {
                return NotFound();
            }

            Department department = await _context.Departments.FirstOrDefaultAsync(d => d.Cities.FirstOrDefault(c => c.Id == user.City.Id) != null);

            if (department == null)
            {
                return NotAuthorized();
                //department = await _context.Departments.FirstOrDefaultAsync();
            }

            Country country = await _context.Countries.FirstOrDefaultAsync(c => c.Departments.FirstOrDefault(d => d.Id == department.Id) != null);

            if (country == null)
            {
                return NotAuthorized();
                //country = await _context.Countries.FirstOrDefaultAsync();
            }

            EditUserViewModel model = new EditUserViewModel
            {
                Address = user.Address,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                ImageUser = user.ImageUser,
                Cities = _combosHelper.GetComboCities(department.Id),
                IdCity = user.City.Id,
                Departments = _combosHelper.GetComboDepartments(country.Id),
                IdDepartment = department.Id,
                Countries = _combosHelper.GetComboCountries(),
                IdCountry = country.Id,
                Id = user.Id
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                string path = model.ImageUser;

                if (model.ImageFile != null)
                {
                    path = await _imageHelper.UploadImageAsync(model.ImageFile, "users");
                }

                User user = await _userHelper.GetUserAsync(User.Identity.Name);

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Address = model.Address;
                user.PhoneNumber = model.PhoneNumber;
                user.ImageUser = path;
                user.City = await _context.Cities.FindAsync(model.IdCity);

                await _userHelper.UpdateUserAsync(user);

                return RedirectToAction("Index", "Home");
            }

            model.Countries = _combosHelper.GetComboCountries();
            model.Departments = _combosHelper.GetComboDepartments(model.IdCountry);
            model.Cities = _combosHelper.GetComboCities(model.IdCity);

            return View(model);
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userHelper.GetUserAsync(User.Identity.Name);

                if (user != null)
                {
                    var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("ChangeUser");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User no found");
                }
            }

            return View(model);
        }
    }
}
