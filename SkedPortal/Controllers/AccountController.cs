using SkedPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SkedPortal.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private SkedPortalEntities db = new SkedPortalEntities();
    
        [HttpGet]
        public ActionResult Login()
        {
            if(User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(User model)
        {
            User user = db.Users.Where(x => x.username.Equals(model.username) && x.hash.Equals(model.hash)).FirstOrDefault();
            if (user != null)
            {
                FormsAuthentication.SetAuthCookie(user.username, false);
                return RedirectToAction("Index", "Dashboard", user);
            }
            else
            {
                ViewBag.Error = "Invalid User";
                return View();
            }

        }

        [HttpGet]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

    }
}