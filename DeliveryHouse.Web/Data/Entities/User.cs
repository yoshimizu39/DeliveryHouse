using DeliveryHouse.Common.Entities;
using DeliveryHouse.Common.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace DeliveryHouse.Web.Data.Entities
{
    public class User : IdentityUser
    {
        //[StringLength(30, MinimumLength = 6)]
        //[Required]
        //public string Document { get; set; }

        [Display(Name = "First Name")]
        [StringLength(20, MinimumLength = 6)]
        [Required]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(20, MinimumLength = 6)]
        [Required]
        public string LastName { get; set; }

        [StringLength(100, MinimumLength = 10)]
        public string Address { get; set; }

        [Display(Name = "Image")]
        public string ImageUser { get; set; }

        [Display(Name = "Login Type")]
        public LoginType LoginType { get; set; }

        public string ImageFacebook { get; set; }

        [Display(Name = "Image")]
        public string ImageFullPath
        {
            get
            {
                if (LoginType == LoginType.Facebook && string.IsNullOrEmpty(ImageFacebook) ||
                    LoginType == LoginType.OnSale && ImageUser == string.Empty)
                {
                    return $"https://localhost:44352/images/noimage.png";
                }

                if (LoginType == LoginType.Facebook)
                {
                    return ImageFacebook;
                }

                return $"https://localhost:44352/images/users/{ImageUser}";
            }
        }

        [Display(Name = "User Type")]
        public UserType UserType { get; set; }

        public City City { get; set; }

        [Display(Name = "User")]
        public string FullName => $"{FirstName} {LastName}";

        [Display(Name = "User")]
        public string FullNameWithDocument => $"{FirstName} {LastName}";

        //public string FullNameWithDocument => $"{FirstName} {LastName} - {Document}";

        //[DisplayFormat(DataFormatString = "{0:N4}")]
        //public double Latitude { get; set; }

        //[DisplayFormat(DataFormatString = "{0:N4}")]
        //public double Logitude { get; set; }
    }
}
