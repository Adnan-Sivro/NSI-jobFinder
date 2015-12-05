using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using JobFinder.Filters;
using JobFinder.Models;
using System.Net.Mail;
using System.Net;

namespace JobFinder.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class AccountController : Controller
    {
        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {

            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginData m, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                bazaEntities dc = new bazaEntities();
                var user = dc.korisnici.Where(a => a.username.Equals(m.Username) && a.password.Equals(m.Password)).FirstOrDefault();

                if (user != null)
                {
                    var model = new LoginData();
                    if (user.aktivan == true)
                    {

                        model.Username = user.username;
                        model.Password = user.password;
                        model.RememberMe = m.RememberMe;

                        var authticket = new
                            FormsAuthenticationTicket(1,
                                user.username,
                                DateTime.Now,
                                DateTime.Now.AddYears(1),
                                model.RememberMe,
                                "",
                                FormsAuthentication.FormsCookiePath);

                        string hash = FormsAuthentication.Encrypt(authticket);

                        var authcookie = new HttpCookie(FormsAuthentication.FormsCookieName, hash);

                        if (authticket.IsPersistent) authcookie.Expires = authticket.Expiration;

                        Response.Cookies.Add(authcookie);
                        if (user.tip_korisnika == "admin") 
                        {
                            return RedirectToAction("Index", "Admin");
                        }
                        if (user.tip_korisnika == "poslodavac")
                        {
                            return RedirectToAction("Index", "Poslodavac");
                        }
                        return RedirectToAction("Index", "Posloprimac");
                    }
                    ModelState.AddModelError("", "User account is not activated.");
                    return View(m);
                }
                ModelState.AddModelError("", "The user name or password provided is incorrect.");
                return View(m);
            }
                // If we got this far, something failed, redisplay form
                ModelState.AddModelError("", "The user name or password provided is incorrect.");
                return View(m);
            
        }
        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();

            // clear authentication cookie
            HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie1.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie1);


            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ContinueRegistration(RegisterModel model)
        {
            if (model.tipkorisnika){
                RegisterEmployeeModel employeeModel = new RegisterEmployeeModel(model);

                return View("ContinueRegistrationEmployee", employeeModel);
            }
            else
            {
                RegisterEmployerModel employerModel = new RegisterEmployerModel(model);
                return View("ContinueRegistrationEmployer", employerModel);
            }
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterEmployee(RegisterEmployeeModel model)
        {
            if (ModelState.IsValid) 
            { 
                Guid tmpGuid = Guid.NewGuid();
                try
                {
                    bazaEntities dc = new bazaEntities();
                    korisnici novikorisnik = new korisnici
                    {
                        aktivan = false,
                        email = model.Email,
                        username = model.UserName,
                        password = model.Password,
                        GUID = tmpGuid.ToString(),
                        tip_korisnika = "posloprimac"
                    };
                    dc.korisnici.Add(novikorisnik);
                    dc.SaveChanges();
                    var idKorisnika = dc.korisnici.Where(x => x.username == model.UserName).Select(x => x.idkorisnici).FirstOrDefault();
                    posloprimci noviPosloprimac = new posloprimci
                    {
                        datum_rodjenja = model.DatumRodjenja,
                        idkorisnici = idKorisnika,
                        ime = model.Ime,
                        prezime = model.Prezime,
                        spol = model.Spol,
                        strucna_sprema = model.StrucnaSprema,
                        telefon = model.Telefon
                    };
                    dc.posloprimci.Add(noviPosloprimac);
                    dc.SaveChanges();
                    
                    ApiKontroler k = new ApiKontroler();
                    if (k.SendEmail("Potvrda Registracije", string.Format(@"
                    Dobro došli na našu stranicu i čestitamo na uspješnoj registraciji.
                    Da biste potvrdili registraciju, kliknite na link ispod:
                       http://localhost:50164/Admin/PotvrdaRegistracije/{0}?guid={1}", idKorisnika, tmpGuid), model.Email))
                    {
                        ViewBag.poslanaPotvrda = "Confirmation mail has been sent.";
                    }

                    return RedirectToAction("Login");
                }
                catch (Exception ex)
                {                
                    //vratiti ponovo s greškom
                    ViewBag.errorOccured = "An error occured. Please try again";
                    return View("ContinueRegistrationEmployee",model);
                }
            }
            return View("ContinueRegistrationEmployee", model);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterEmployer(RegisterEmployerModel model)
        {
            if (ModelState.IsValid)
            {
                Guid tmpGuid = Guid.NewGuid();
                try
                {
                    bazaEntities dc = new bazaEntities();
                    korisnici novikorisnik = new korisnici
                    {
                        aktivan = false,
                        email = model.Email,
                        username = model.UserName,
                        password = model.Password,
                        GUID = tmpGuid.ToString(),
                        tip_korisnika = "poslodavac"
                    };
                    dc.korisnici.Add(novikorisnik);
                    dc.SaveChanges();

                    lokacije lokacija = new lokacije()
                    {
                        drzava = model.Drzava,
                        grad = model.Grad
                    };
                    var idLokacije = dc.lokacije.Where(x => x.drzava == lokacija.drzava && x.grad == lokacija.grad).Select(x => x.idlokacije).FirstOrDefault();
                    if(idLokacije == 0)
                    {
                        dc.lokacije.Add(lokacija);
                        idLokacije = dc.lokacije.Where(x => x.drzava == lokacija.drzava && x.grad == lokacija.grad).Select(x => x.idlokacije).FirstOrDefault();
                    }

                    var idKorisnika = dc.korisnici.Where(x => x.username == model.UserName).Select(x => x.idkorisnici).FirstOrDefault();
                    poslodavci noviPoslodavac = new poslodavci
                    {
                        telefon = model.Telefon,
                        broj_zaposlenih = model.BrojZaposlenih,
                        djelatnost = model.Djelatnost,
                        idkorisnici = idKorisnika,
                        naziv = model.Naziv,
                        OIB = model.OIB,
                        opis = model.Opis,
                        vrsta = model.Vrsta,
                        webpage = model.Webpage,
                        idlokacije = idLokacije
                    };
                    dc.poslodavci.Add(noviPoslodavac);
                    dc.SaveChanges();

                    ApiKontroler k = new ApiKontroler();
                    if (k.SendEmail("Potvrda Registracije", string.Format(@"
                    Dobro došli na našu stranicu i čestitamo na uspješnoj registraciji.
                    Da biste potvrdili registraciju, kliknite na link ispod:
                       http://localhost:50164/Admin/PotvrdaRegistracije/{0}?guid={1}", idKorisnika, tmpGuid), model.Email))
                    {
                        ViewBag.poslanaPotvrda = "Confirmation mail has been sent.";
                    }
                    return RedirectToAction("Login");
                }
                catch (Exception ex)
                {
                    //vratiti ponovo s greškom
                    ViewBag.errorOccured = "An error occured. Please try again";
                    return View("ContinueRegistrationEmployer", model);
                }
            }
            return View("ContinueRegistrationEmployer", model);
        }


        [HttpGet]
        public ActionResult PotvrdaRegistracije(int id, string guid)
        {
            bazaEntities db = new bazaEntities();
            korisnici u = db.korisnici.Find(id);
            u.aktivan = true;
            db.SaveChanges();
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
       
        // POST: /Account/Disassociate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            // Only disassociate the account if the currently logged in user is the owner
            if (ownerAccount == User.Identity.Name)
            {
                // Use a transaction to prevent the user from deleting their last login credential
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
                {
                    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        message = ManageMessageId.RemoveLoginSuccess;
                    }
                }
            }

            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage

        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    }
                }
            }
            else
            {
                // User does not have a local password so remove any validation errors caused by a missing
                // OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", String.Format("Unable to create local account. An account with the name \"{0}\" may already exist.", User.Identity.Name));
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
            {
                return RedirectToLocal(returnUrl);
            }

            if (User.Identity.IsAuthenticated)
            {
                // If the current user is logged in add the new account
                OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // User is new, ask for their desired membership name
                string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
                ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
                ViewBag.ReturnUrl = returnUrl;
                return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        {
            string provider = null;
            string providerUserId = null;

            if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Insert a new user into the database
                using (UsersContext db = new UsersContext())
                {
                    UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
                    // Check if user already exists
                    if (user == null)
                    {
                        // Insert name into the profile table
                        db.UserProfiles.Add(new UserProfile { UserName = model.UserName });
                        db.SaveChanges();

                        OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
                        OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("UserName", "User name already exists. Please enter a different user name.");
                    }
                }
            }

            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // GET: /Account/ExternalLoginFailure

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        [ChildActionOnly]
        public ActionResult RemoveExternalLogins()
        {
            ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
            List<ExternalLogin> externalLogins = new List<ExternalLogin>();
            foreach (OAuthAccount account in accounts)
            {
                AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

                externalLogins.Add(new ExternalLogin
                {
                    Provider = account.Provider,
                    ProviderDisplayName = clientData.DisplayName,
                    ProviderUserId = account.ProviderUserId,
                });
            }

            ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        }

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
