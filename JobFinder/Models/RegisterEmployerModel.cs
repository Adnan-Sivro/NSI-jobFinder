using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JobFinder.Models
{
    public class RegisterEmployerModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        [Required]
        [Display(Name = "Vrsta")]
        public string Vrsta { get; set; }
        [Required]
        [Display(Name = "Djelatnost")]
        public string Djelatnost { get; set; }
        [Required]
        [Display(Name = "Name")]
        public string Naziv { get; set; }
        [Required]
        [Display(Name = "OIB")]
        public string OIB { get; set; }
        [Required]
        [Display(Name = "Description")]
        public string Opis { get; set; }
        [Required]
        [Display(Name = "Webpage")]
        public string Webpage { get; set; }
        [Required]
        [Display(Name = "Number of Employees")]
        public int BrojZaposlenih { get; set; }
        [Required]
        [Display(Name = "Telephone Number")]
        public string Telefon { get; set; }
        [Required]
        [Display(Name = "City")]
        public string Grad { get; set; }
        [Required]
        [Display(Name = "State")]
        public string Drzava { get; set; }

        public RegisterEmployerModel() { }

        public RegisterEmployerModel(RegisterModel model)
        {
            this.UserName = model.UserName;
            this.Password = model.Password;
            this.Email = model.Email;
        }
    }
}