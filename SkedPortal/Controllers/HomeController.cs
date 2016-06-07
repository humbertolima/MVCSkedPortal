using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace SkedPortal.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        
        [HttpGet]
        public ActionResult About()
        {
            ViewBag.Message = "With this Aplication you will be allow to manipulate and managment avey User or Flight Attendant, avery Flight, assign fligths, remove and add new flight attendants. Finally you can control the time of rest and the time working for avery flight attendant, pilot and mechanics";

            return View();
        }

        [HttpGet]
        public ActionResult Contact()
        {
            
            return View();
        }
    }

}