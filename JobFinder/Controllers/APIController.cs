using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web.Http;
using System.Linq;
using System.Linq.Expressions;
using JobFinder.Models;

namespace JobFinder.Controllers
{

    public class ApiKontroler : System.Web.Http.ApiController
    {
        bool POSALjIMAIL = false;

        [System.Web.Mvc.HttpPost]
        public System.Net.Http.HttpResponseMessage RegistracijaJson(korisnici korisnik)
        {
            Guid tmpGuid = Guid.NewGuid();
            var parametri = new Dictionary<string, object>{

                {"Username", korisnik.username},
                {"Password", korisnik.password},
                {"Email", korisnik.email},
                {"GUID", tmpGuid.ToString().ToLower()}
            };
            try
            {
                bazaEntities dc = new bazaEntities();
                dc.korisnici.Add(korisnik);
                korisnici u = dc.korisnici.Where(a => a.username == korisnik.username).FirstOrDefault();
                int ID = Convert.ToInt32(u.idkorisnici);
                korisnik.idkorisnici = ID;

                SendEmail("Potvrda Registracije", string.Format(@"
                Dobro došli na našu stranicu i čestitamo na uspješnoj registraciji.
                Da biste potvrdili registraciju, kliknite na link ispod:
                   http://www.nwt.somee.com/Account/PotvrdaRegistracijeJson/{0}?guid={1}", korisnik.idkorisnici, tmpGuid), korisnik.email);

                return new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.Created
                };
            }
            catch (Exception ex)
            {
                

                return new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                };
            }
        }
        public bool SendEmail(string subject, string body, string mailTo)
        {
           
            string fromMail = "enesmujic07@gmail.com";

            MailMessage mail = new MailMessage(fromMail, mailTo, subject, body);

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                UseDefaultCredentials = false,
                EnableSsl = true,
                Timeout = 20000,
                Credentials = new NetworkCredential("enesmujic07@gmail.com", "07071991enes")
            };
            try
            {
                client.Send(mail);
            }
            catch (Exception ex)
            {

                return false;
            }

            return true;
        }

    }
}
