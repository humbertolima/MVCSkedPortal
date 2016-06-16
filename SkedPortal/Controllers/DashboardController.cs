using SkedPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Principal;
namespace SkedPortal.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private SkedPortalEntities db = new SkedPortalEntities();
        [HttpGet]
        public ActionResult Index()
        {
            MvcApplication.Restart();
            User user = db.Users.Where(x => x.username == User.Identity.Name).FirstOrDefault();
            if (User.IsInRole("Admin"))
            {
                if (db.Flights.Where(x => x.assigned == true && x.completed == false).OrderBy(x => x.flight_date).ToList() != null)
                    return View(db.Flights.Where(x => x.assigned == true && x.completed == false).OrderBy(x => x.flight_date).ToList());
                else
                    return View();
            }
            else if (User.IsInRole("Pilot"))
            {
                
                List<Flight> f = new List<Flight>();
                List<AssignedFlight> af = db.AssignedFlights.Where(x => x.captain == user.id || x.first_officer == user.id).ToList();
                if (af.Count() > 0)
                {
                    foreach (AssignedFlight a in af)
                    {
                        f.Add(db.Flights.Where(x => x.flight_number == a.flight_number && x.completed==false).FirstOrDefault());
                    }
                }
                
                return View(f.ToList());
            }
            else
            {
                List<Flight> fl = new List<Flight>();
                List<AssignedFlight> af = db.AssignedFlights.Where(x =>x.fal == user.id || x.fa1 == user.id || x.fa2 == user.id || x.fa3 == user.id || x.fa4 == user.id || x.fa5 == user.id).ToList();
                if (af.Count() > 0)
                {
                    foreach (AssignedFlight f in af)
                    {
                        fl.Add(db.Flights.Where(x => x.flight_number == f.flight_number && x.completed == false).First());
                    }
                
                }
                
                return View(fl.ToList());
            }
        }
        [HttpGet]
        public ActionResult Crew(int flight_number)
        {
            ViewBag.Index = flight_number.ToString();
            List<User> crew = new List<Models.User>();
            AssignedFlight af = db.AssignedFlights.Where(x => x.flight_number == flight_number).FirstOrDefault();
            if (af != null) {
                crew.Add(db.Users.Where(x => x.id == af.captain).FirstOrDefault());
                crew.Add(db.Users.Where(x => x.id == af.first_officer).FirstOrDefault());
                crew.Add(db.Users.Where(x => x.id == af.fal).FirstOrDefault());
                crew.Add(db.Users.Where(x => x.id == af.fa1).FirstOrDefault());
                crew.Add(db.Users.Where(x => x.id == af.fa2).FirstOrDefault());
                crew.Add(db.Users.Where(x => x.id == af.fa3).FirstOrDefault());
                crew.Add(db.Users.Where(x => x.id == af.fa4).FirstOrDefault());
                crew.Add(db.Users.Where(x => x.id == af.fa5).FirstOrDefault());
            }
            string[] roles = { "Captain", "First Officer", "FAL", "FA1", "FA2", "FA3", "FA4", "FA5" };
            ViewBag.Roles = roles;
            return PartialView(crew);
        }
        public ActionResult Close()
        {
            return PartialView();
        }
    }

   
}
 