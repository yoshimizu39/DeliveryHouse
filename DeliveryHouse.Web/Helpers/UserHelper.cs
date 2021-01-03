using DeliveryHouse.Common.Enums;
using DeliveryHouse.Web.Data;
using DeliveryHouse.Web.Data.Entities;
using DeliveryHouse.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace DeliveryHouse.Web.Helpers
{
    public class UserHelper : IUserHelper
    {
        private readonly DataContext _dataContext;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;

        public UserHelper(DataContext dataContext, 
                          UserManager<User> userManager, 
                          RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager)
        {
            _dataContext = dataContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public async Task<IdentityResult> AddUserAsync(User user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<User> AddUserAsync(AddUserViewModel model, string path, UserType userType)
        {
            User user = new User
            {
                Address = model.Address,
                Email = model.UserName,
                FirstName = model.FirstName,
                LastName = model.LastName,
                ImageUser = path,
                PhoneNumber = model.PhoneNumber,
                City = await _dataContext.Cities.FindAsync(model.IdCity),
                UserName = model.UserName,
                UserType = userType
            };

            IdentityResult identityResult = await _userManager.CreateAsync(user, model.Password);

            if (identityResult != IdentityResult.Success)
            {
                return null;
            }

            User newUser = await GetUserAsync(model.UserName);
            await AddUserToRoleAsync(newUser, user.UserType.ToString());

            return newUser;
        }

        public async Task AddUserToRoleAsync(User user, string roleName)
        {
            await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        }

        public async Task CheckRoleAsync(string roleName)
        {
            bool roleExists = await _roleManager.RoleExistsAsync(roleName);

            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole
                {
                    Name = roleName
                });
            }
        }

        public async Task<IdentityResult> ConfirmEmailAsync(User user, string token)
        {
            return await _userManager.ConfirmEmailAsync(user, token);
        }

        public async Task<string> GenerateEmailConfirmatioTokenAsync(User user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<User> GetUserAsync(string email)
        {
            return await _dataContext.Users.Include(u => u.City)
                                           .FirstOrDefaultAsync(m => m.Email == email);
        }

        public async Task<User> GetUserAsync(Guid userId)
        {
            return await _dataContext.Users.Include(u => u.City)
                                           .FirstOrDefaultAsync(m => m.Id == userId.ToString());   
        }

        public async Task<bool> IsUserInRoleAsync(User user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }

        public async Task<SignInResult> LoginAsync(LoginViewModel loginViewModel)
        {
            return await _signInManager.PasswordSignInAsync(
                    loginViewModel.UserName,
                    loginViewModel.Password,
                    loginViewModel.RememberMe,
                    false);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            return await _userManager.UpdateAsync(user);
        }

        public async Task<SignInResult> ValidatePasswordAsync(User user, string password)
        {
            return await _signInManager.CheckPasswordSignInAsync(user, password, false);
        }
    }
}
