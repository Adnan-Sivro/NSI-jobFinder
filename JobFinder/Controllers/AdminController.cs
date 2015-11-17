using JobFinder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace JobFinder.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/

        public ActionResult Index()
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated) 
            {
                string ime = System.Web.HttpContext.Current.User.Identity.Name;
                bazaEntities dc = new bazaEntities();
                var u = dc.korisnici.Where(m => m.username == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault();
               
                if (u.tip_korisnika == "admin")
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (u.tip_korisnika == "poslodavac")
                {
                    return RedirectToAction("Index", "Poslodavac");
                }
                return RedirectToAction("Index", "Posloprimac");
                
               
            }
            else
            {
              return  RedirectToAction("Login", "Account");
            }
        }
        [HttpGet]
        public ActionResult PotvrdaRegistracije(int id, string guid)
        {

            bazaEntities db = new bazaEntities();
            korisnici u = db.korisnici.Find(id);
            u.aktivan = true;
            db.SaveChanges();

            var authticket = new
                            FormsAuthenticationTicket(1,
                                u.username,
                                DateTime.Now,
                                DateTime.Now.AddYears(1),
                                false,
                                "",
                                FormsAuthentication.FormsCookiePath);

            string hash = FormsAuthentication.Encrypt(authticket);

            var authcookie = new HttpCookie(FormsAuthentication.FormsCookieName, hash);

            if (authticket.IsPersistent) authcookie.Expires = authticket.Expiration;

            Response.Cookies.Add(authcookie);

            if (u.tip_korisnika == "admin")
            {
                return RedirectToAction("Index", "Admin");
            }
            if (u.tip_korisnika == "poslodavac")
            {
                return RedirectToAction("Index", "Poslodavac");
            }
            return RedirectToAction("Index", "Posloprimac");
        }

    }
}
