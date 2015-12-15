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
            bazaEntities db = new bazaEntities();
            var poslodavac = db.poslodavci.Where(m => m.korisnici.username == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault();
            var oglasi = db.oglasi.Where(m => m.idposlodavci == poslodavac.idposlodavci).ToArray();
            return View("MyOffers", oglasi);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }


        public ActionResult AddJobOffer()
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                       
            OglasiKategorijeModel okm = new OglasiKategorijeModel(); 
            bazaEntities dc = new bazaEntities();

            okm.Kategorije = dc.kategorije.ToArray();
            return View("AddJobOffer", okm);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult DodavanjeOglasa(OglasiKategorijeModel okm)
        {
            if (ModelState.IsValid) 
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
                text_oglasa = okm.Oglasi.text_oglasa,
                naziv_pozicije = okm.Oglasi.naziv_pozicije
                });
            dc.SaveChanges();
            return RedirectToAction("Index");
            }
            bazaEntities db = new bazaEntities();
            okm.Kategorije = db.kategorije.ToArray();
            return View("AddJobOffer", okm);
        }
   
        [HttpGet]
        public ActionResult DeleteOffer(int id)
        {
            bazaEntities db = new bazaEntities();          
            db.oglasi.Remove(db.oglasi.Find(id));
            db.SaveChanges();
            var poslodavac = db.poslodavci.Where(m => m.korisnici.username == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault();
            var oglasi = db.oglasi.Where(m => m.idposlodavci == poslodavac.idposlodavci).ToArray();           
            return View("MyOffers", oglasi);            
        }
        [HttpGet]
        public ActionResult EditOffer(int id)
        {
            OglasiKategorijeModel okm = new OglasiKategorijeModel();
            bazaEntities dc = new bazaEntities();
            okm.Oglasi = dc.oglasi.Find(id); 
            okm.Kategorije = dc.kategorije.ToArray();
            return View("EditOffer",okm);
        }

        [HttpPost]
        public ActionResult SaveEditetOffer(OglasiKategorijeModel okm, int id)
        {
            bazaEntities dc = new bazaEntities();
            var poslodavac = dc.poslodavci.Where(m => m.korisnici.username == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault();
            var c = dc.oglasi.Find(id);
            c.broj_pozicija = okm.Oglasi.broj_pozicija;
            c.datum_objave = DateTime.Now;
            c.datum_zavrsetka = okm.Oglasi.datum_zavrsetka;
            c.idkategorije = okm.kategorijeid[0];
            c.idposlodavci = poslodavac.idposlodavci;
            c.kontakt_email = okm.Oglasi.kontakt_email;
            c.spol = okm.Oglasi.spol;
            c.text_oglasa = okm.Oglasi.text_oglasa;
            c.naziv_pozicije = okm.Oglasi.naziv_pozicije;
            dc.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
