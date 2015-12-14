using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobFinder.Models
{
    public class ViewOffersModel
    {
        public List<OfferModel> offers { get; set; }
        public List<string> Categories { get; set; }
    }
}