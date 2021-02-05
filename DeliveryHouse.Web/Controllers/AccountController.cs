using DeliveryHouse.Common.Entities;
using DeliveryHouse.Common.Enums;
using DeliveryHouse.Common.Responses;
using DeliveryHouse.Web.Data;
using DeliveryHouse.Web.Data.Entities;
using DeliveryHouse.Web.Helpers;
using DeliveryHouse.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
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
        private readonly IEmailHelper _emailHelper;

        public AccountController(DataContext context, IUserHelper userHelper, ICombosHelper combosHelper, IImageHelper imageHelper,
                                 IEmailHelper emailHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _combosHelper = combosHelper;
            _imageHelper = imageHelper;
            _emailHelper = emailHelper;
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

                string myToken = await _userHelper.GenerateEmailConfirmatioTokenAsync(user);
                string tokenLink = Url.Action("ConfirmEmail", "Account", new
                {
                    userid = user.Id,
                    token = myToken
                }, protocol: HttpContext.Request.Scheme);

                Response response = _emailHelper.SednMail(model.UserName, "Email Confirmation",
                                                          $"<h1> Email Confirmation </h1>" +
                                                          $"To alloe the user, " +
                                                          $"please click in this link:<p><a href = \"{tokenLink}\"> Confirm Email </a></p>");

                if (response.IsSuccess)
                {
                    ViewBag.Message = "The instruction to allow your user has been sent to email.";

                    return View(model);
                }

                ModelState.AddModelError(string.Empty, response.Message);

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

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            User user = await _userHelper.GetUserAsync(new Guid(userId));

            if (user == null)
            {
                return NotFound();
            }

            IdentityResult result = await _userHelper.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                return NotFound();
            }

            return View();
        }

        public IActionResult RecoverPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userHelper.GetUserAsync(model.Email);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "The email doesn't correspont to a registered user.");
                    return View(model);
                }

                string myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);
                string link = Url.Action("ResetPassword", "Account", new
                {
                    token = myToken
                }, protocol: HttpContext.Request.Scheme);

                _emailHelper.SednMail(model.Email, "Password Reset", $"<h1> Password Reset </h1>" +
                                      $"To reset the password click in this link:<p>" +
                                      $"<a href = \"{link}\"> Reset Password </a></p>");

                ViewBag.Message = "The instructions to recover your password has been sent to email.";

                return View();
            }

            return View(model);
        }

        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            User user = await _userHelper.GetUserAsync(model.UserName);

            if (user !=  null)
            {
                IdentityResult result = await _userHelper.ResetPasswordAsync(user, model.Token, model.Password);

                if (result.Succeeded)
                {
                    ViewBag.Message = "Password reset successful";
                    return View();
                }

                ViewBag.Message = "Error while resetting the password";
                return View(model);
            }

            ViewBag.Message = "User not found";

            return View(model);
        }
    }
}
