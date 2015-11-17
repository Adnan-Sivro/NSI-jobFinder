using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JobFinder.Models;

namespace JobFinder.Controllers
{
    public class PoslodavacController : Controller
    {
        //
        // GET: /Poslodavac/

        public ActionResult Index()
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                       
            OglasiKategorijeModel okm = new OglasiKategorijeModel(); 
            bazaEntities dc = new bazaEntities();

            okm.Kategorije = dc.kategorije.ToArray();
            return View("Index",okm);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public ActionResult DodavanjeOglasa(OglasiKategorijeModel okm)
        {
            bazaEntities dc =new bazaEntities();
            string ime = System.Web.HttpContext.Current.User.Identity.Name;
            var poslodavac = dc.poslodavci.Where(m => m.korisnici.username == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault();
            dc.oglasi.Add(new oglasi
            {
                broj_pozicija = okm.Oglasi.broj_pozicija,
                datum_objave = DateTime.Now,
                datum_zavrsetka = okm.Oglasi.datum_zavrsetka,
                idkategorije = okm.kategorijeid[0],
                idposlodavci = poslodavac.idposlodavci,
                kontakt_email = okm.Oglasi.kontakt_email,
                spol = okm.Oglasi.spol,
                naziv_pozicije = okm.Oglasi.naziv_pozicije
                });
            dc.SaveChanges();
            return View();
        }



    }
}
