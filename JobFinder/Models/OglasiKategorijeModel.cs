using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JobFinder.Models
{
    public class OglasiKategorijeModel
    {
        public oglasi Oglasi { get; set; }
        public IEnumerable<kategorije> Kategorije { get; set; }
        public int[] kategorijeid { get; set; }
    }
}
