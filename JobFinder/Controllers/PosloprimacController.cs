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

        private void PopulateOffersAndCategories(ViewOffersModel dbm)
        {
            try
            {
                bazaEntities be = new bazaEntities();
                var offersFromDb = be.oglasi.ToList();
                var offersVModel = new List<OfferModel>();
                if (offersFromDb != null && offersFromDb.Count != 0)
                {
                    foreach (var offer in offersFromDb)
                    {
                        OfferModel offerModel = new OfferModel
                        {
                            BrojPozicija = offer.broj_pozicija != null ? (int)offer.broj_pozicija : 0,
                            DatumObjave = offer.datum_objave != null ? offer.datum_objave.Value.ToShortDateString() : string.Empty,
                            DatumZavrsetka = offer.datum_zavrsetka != null ? offer.datum_zavrsetka.Value.ToShortDateString() : string.Empty,
                            IdKategorije = offer.idkategorije != null ? (int)offer.idkategorije : 0,
                            KontaktEmail = offer.kontakt_email,
                            NazivPozicije = offer.naziv_pozicije,
                            Spol = offer.spol,
                            TextOglasa = offer.text_oglasa,
                            TipOglasa = offer.tip_oglasa
                        };
                        if (offer.kategorije != null && !string.IsNullOrEmpty(offer.kategorije.naziv))
                            offerModel.NazivKategorije = offer.kategorije.naziv;

                        offersVModel.Add(offerModel);
                    }
                    dbm.offers = offersVModel;
                }
                var categoriesFromDb = be.kategorije.ToList();
                dbm.Categories = new List<string>();
                foreach (var cat in categoriesFromDb)
                    dbm.Categories.Add(cat.naziv);
            }
            catch (Exception e)
            {

            } 
        }
        public ActionResult DodavanjeBiografije()
        {
            BiografijeKategorijeModel dbm = new BiografijeKategorijeModel();
            bazaEntities dc = new bazaEntities();

            dbm.Kategorije = dc.kategorije.ToArray();
            return View("DodavanjeBiografije", dbm);
        }

        public ActionResult PregledPoslova()
        {
            ViewOffersModel dbm = new ViewOffersModel();
            PopulateOffersAndCategories(dbm);
            return View("PregledOglasa", dbm);
        }

        public ActionResult FilterOffersByCategory(string CategoryName)
        {
            ViewOffersModel model = new ViewOffersModel();
            PopulateOffersAndCategories(model);

            var matching = model.offers.Where(x => x.NazivKategorije == CategoryName).ToList();
            model.offers = matching;

            return View("PregledOglasa", model);
        }

        [HttpPost]
        public ActionResult DodavanjeBiografije(BiografijeKategorijeModel bdm)
        {
            try
            {
                bazaEntities dc = new bazaEntities();

                string ime = System.Web.HttpContext.Current.User.Identity.Name;
                var idKorisnika = dc.korisnici.Where(m => m.username == System.Web.HttpContext.Current.User.Identity.Name).Select(x => x.idkorisnici).FirstOrDefault();
                var posloprimac = dc.posloprimci.Where(x => x.idkorisnici == idKorisnika).FirstOrDefault();

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
                bdm = new BiografijeKategorijeModel();
                bdm.Kategorije = dc.kategorije.ToArray();
            }
            catch (Exception exception)
            {
                return View();
            }

            return View("DodavanjeBiografije", bdm);
        }

        [HttpPost]
        public void ApplyForJob(string emailTxt, string contactEmail)
        {
            if (string.IsNullOrEmpty(emailTxt))
                emailTxt = "Hello!" + Environment.NewLine 
                    + "I would like to get in touch and learn more about the job you are offering." 
                    + Environment.NewLine
                    + "Best regards, " + Environment.NewLine + User.Identity.Name;
            ApiKontroler k = new ApiKontroler();
            k.SendEmail("Potvrda Registracije", emailTxt , contactEmail);
            return;
        }
    }
}
