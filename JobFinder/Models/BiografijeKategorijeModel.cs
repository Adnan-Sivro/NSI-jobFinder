using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobFinder.Models
{
    public class BiografijeKategorijeModel
    {

        public biografije Biografije { get; set; }
        public IEnumerable<kategorije> Kategorije { get; set; }
        public Int32 idkategorije { get; set; }
      }
}