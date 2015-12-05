using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JobFinder.Models
{
        public class RegisterModel
        {
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
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
            [Required]
            [Display(Name="User type")]
            public bool tipkorisnika { get; set; }

            public RegisterModel() { }

            ////poslodavac (employer)
            //[Required]
            //[Display(Name = "Vrsta")]
            //public string Vrsta { get; set; }
            //[Required]
            //[Display(Name = "Djelatnost")]
            //public string Djelatnost { get; set; }
            //[Required]
            //[Display(Name = "Name")]
            //public string Naziv { get; set; }
            //[Required]
            //[Display(Name = "OIB")]
            //public string OIB { get; set; }
            //[Required]
            //[Display(Name = "Description")]
            //public string Opis { get; set; }
            //[Required]
            //[Display(Name = "Webpage")]
            //public string Webpage { get; set; }
            //[Required]
            //[Display(Name = "Number of Employees")]
            //public int BrojZaposlenih { get; set; }
            //[Required]
            //[Display(Name = "Telephone Number")]
            //public string Telefon { get; set; }
        }

    
}