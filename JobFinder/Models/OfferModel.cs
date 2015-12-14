using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JobFinder.Models
{
    public class OfferModel
    {
        public string NazivPozicije { get; set; }
        [Display(Name = "Added on")]
        public string DatumObjave { get; set; }
        [Display(Name = "Expires on")]
        public string DatumZavrsetka { get; set; }
        public string TextOglasa { get; set; }
        [Display(Name = "Number of open slots")]
        public int BrojPozicija { get; set; }
        [Display(Name = "Gender preferences")]
        public string Spol { get; set; }
        [Display(Name = "Contact email")]
        public string KontaktEmail { get; set; }
        [Display(Name="Offer type")]
        public string TipOglasa { get; set; }
        [Display(Name = "Category")]
        public string NazivKategorije { get; set; }
        public int IdKategorije { get; set; }
    }
}