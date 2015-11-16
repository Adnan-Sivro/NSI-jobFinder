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
            OglasiKategorijeModel okm = new OglasiKategorijeModel(); 
            bazaEntities dc = new bazaEntities();
            okm.Kategorije = dc.kategorije.ToArray();
            return View("Index",okm);
        }

    }
}
