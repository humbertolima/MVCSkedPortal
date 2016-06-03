using SkedPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SkedPortal.Controllers
{
    public class EmployeesController : Controller
    {
        SkedPortalEntities db = new SkedPortalEntities();

        [Authorize]
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(User model)
        {
            User user = db.Users.Where(x => x.email.Equals(model.email) && x.hash.Equals(model.hash)).FirstOrDefault();
            if (user != null)
            {
                FormsAuthentication.SetAuthCookie(user.username, false);
                return RedirectToAction("Dashboard");
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