using DeliveryHouse.Common.Enums;
using DeliveryHouse.Web.Data.Entities;
using DeliveryHouse.Web.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace DeliveryHouse.Web.Helpers
{
    public interface IUserHelper
    {
        Task<User> GetUserAsync(string email);
        Task<IdentityResult> AddUserAsync(User user, string password);
        Task CheckRoleAsync(string roleName);
        Task AddUserToRoleAsync(User user, string roleName);
        Task<bool> IsUserInRoleAsync(User user, string roleName);
        Task<SignInResult> LoginAsync(LoginViewModel loginViewModel);
        Task LogoutAsync();
        Task<SignInResult> ValidatePasswordAsync(User user, string password);
        Task<User> AddUserAsync(AddUserViewModel model, string path, UserType userType);
        Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword);
        Task<IdentityResult> UpdateUserAsync(User user);
        Task<User> GetUserAsync(Guid userId);
    }
}
