using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JobFinder.Models;

namespace JobFinder.Controllers
{
    public class PosloprimacController : Controller
    {
        //
        // GET: /Posloprimac/

        public ActionResult Index()

        {
            BiografijeKategorijeModel dbm = new BiografijeKategorijeModel();
            bazaEntities dc = new bazaEntities();

            dbm.Kategorije = dc.kategorije.ToArray();
            return View("DodavanjeBiografije", dbm);
        }
        
        public ActionResult DodavanjeBiografije()
        {
            BiografijeKategorijeModel dbm = new BiografijeKategorijeModel();
            bazaEntities dc = new bazaEntities();

            dbm.Kategorije = dc.kategorije.ToArray();
            return View("DodavanjeBiografije", dbm);
        }

        [HttpPost]
        public ActionResult DodavanjeBiografije(BiografijeKategorijeModel bdm)
        {
            try
            {
                bazaEntities dc = new bazaEntities();

                string ime = System.Web.HttpContext.Current.User.Identity.Name;
                var posloprimac =
                    dc.posloprimci.Where(m => m.korisnici.username == System.Web.HttpContext.Current.User.Identity.Name)
                        .FirstOrDefault();
                dc.biografije.Add(new biografije()
                {
                    datum_biografije = DateTime.Now,
                    idealan_posao = bdm.Biografije.idealan_posao,
                    idkategorije = bdm.idkategorije,
                    idposloprimac = posloprimac.idposloprimci,
                    kompetencije = bdm.Biografije.kompetencije,
                    radno_iskustvo = bdm.Biografije.radno_iskustvo,
                    zanimanje = bdm.Biografije.zanimanje
                });
                dc.SaveChanges();
                bdm.Kategorije = dc.kategorije.ToArray();
            }
            catch (Exception exception)
            {
                return View();
            }
           
            return View(bdm);
        }
    }
}
