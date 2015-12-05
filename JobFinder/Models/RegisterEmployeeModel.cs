using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JobFinder.Models
{
    public class RegisterEmployeeModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string Ime { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string Prezime { get; set; }
        [Required]
        [Display(Name = "Date Of Birth")]
        public DateTime DatumRodjenja { get; set; }
        [Required]
        [Display(Name = "Gender")]
        public string Spol { get; set; }
        [Required]
        [Display(Name = "Qualification")]
        public string StrucnaSprema { get; set; }
        [Required]
        [Display(Name = "Telephone Number")]
        public string Telefon { get; set; }
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public RegisterEmployeeModel() { }

        public RegisterEmployeeModel(RegisterModel model) {
            this.UserName = model.UserName;
            this.Password = model.Password;
            this.Email = model.Email;
        }
    }
}