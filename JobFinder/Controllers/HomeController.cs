using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JobFinder.Models;

namespace JobFinder.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                string ime = System.Web.HttpContext.Current.User.Identity.Name;
                bazaEntities dc = new bazaEntities();
                var u =
                    dc.korisnici.Where(m => m.username == System.Web.HttpContext.Current.User.Identity.Name)
                        .FirstOrDefault();

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
                return RedirectToAction("Login", "Account");

            }
        }
        
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
