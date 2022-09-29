using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FoodOrderSystem.Models
{
    public class SignupLogin
    {
        [Key]
        public int userid { get; set; }
        [StringLength(50)]
        [Required(ErrorMessage = "Name Required")]
        public string Name { get; set; }

        [StringLength(50)]
        [EmailAddress]
        [Required(ErrorMessage = "Email Required")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password Required")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long", MinimumLength = 4)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [StringLength(10)]
        [Required(ErrorMessage = "Not Valid Mobile No!")]
        [DataType(DataType.PhoneNumber)]
        public string MobileNo { get; set; }
    }
}