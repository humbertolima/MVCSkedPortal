using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SkedPortal.Models;

namespace SkedPortal.Controllers
{
    public class FlightsController : Controller
    {
        SkedPortalEntities db = new SkedPortalEntities();

        // GET: Flights
        public ActionResult Index()
        {
            return View();
        }
    }
}