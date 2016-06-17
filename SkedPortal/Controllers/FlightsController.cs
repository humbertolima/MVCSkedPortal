using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SkedPortal.Models;

namespace SkedPortal.Controllers
{
    [Authorize(Roles = "Admin, Pilot")]
    public class FlightsController : Controller
    {

        private SkedPortalEntities db = new SkedPortalEntities();



        //Get
        public ActionResult Assign(int id)
        {
            return RedirectToAction("Create", "AssignedFlights", db.Flights.Find(id));
        }
        // GET: Flights
        public ActionResult Index()
        {
            MvcApplication.Restart();
            return View(db.Flights.ToList());
        }

        // GET: Flights/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Flight flight = db.Flights.Find(id);
            if (flight == null)
            {
                return HttpNotFound();
            }
            return View(flight);
        }

        // GET: Flights/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Flights/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,flight_number,flight_date,flight_origin,flight_start,flight_destination,flight_end,assigned,completed")] Flight flight)
        {
            if (ModelState.IsValid && DateTime.Parse(flight.flight_date).CompareTo(DateTime.Now.Date) >= 0)
            {
                if (!Validate_flight(flight.flight_number, flight.id))
                {
                    flight.assigned = false; flight.completed = false;
                    db.Flights.Add(flight);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(flight);
        }
        private bool Validate_flight(int fn, int id)
        {
            Flight temp = db.Flights.Where(x => x.flight_number == fn && x.id != id).FirstOrDefault();
            if (temp == null)
            {
                return false;
            }
            else
            {
                ViewBag.Flight_Error = "This Flight already exist";
                return true;
            }
        }
        // GET: Flights/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Flight flight = db.Flights.Find(id);
            if (flight == null)
            {
                return HttpNotFound();
            }

            return View(flight);
        }

        // POST: Flights/Edit/5
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,flight_number,flight_date,flight_origin,flight_start,flight_destination,flight_end,assigned,completed")] Flight flight)
        {
            if (ModelState.IsValid && DateTime.Parse(flight.flight_date).CompareTo(DateTime.Now.Date) >= 0)
            {
                if (!Validate_flight(flight.flight_number, flight.id))
                {
                    AssignedFlight af = db.AssignedFlights.Where(x => x.flight_id == flight.id).FirstOrDefault();
                    if (flight.assigned == false)
                    {
                        
                        if (af != null)
                        {
                            db.AssignedFlights.Remove(af);
                        }
                    }
                    else
                    {
                        if (af != null)
                        {
                            af.flight_date = flight.flight_date;
                            af.flight_number = flight.flight_number;
                            db.Entry(af).State = EntityState.Modified;
                        }
                        else
                        {
                            flight.assigned = false;
                        }
                        
                    }
                    db.Entry(flight).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                 }
            }
            Flight f = db.Flights.Where(x => x.id == flight.id).FirstOrDefault();
            return View(f);
        }

        // GET: Flights/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Flight flight = db.Flights.Find(id);
            if (flight == null)
            {
                return HttpNotFound();
            }
            return View(flight);
        }

        // POST: Flights/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            Flight flight = db.Flights.Find(id);
            AssignedFlight af = db.AssignedFlights.Where(x => x.flight_number == flight.flight_number).FirstOrDefault();
            if (af != null)
            {
                db.AssignedFlights.Remove(af);
            }
            db.Flights.Remove(flight);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Current_Flights()
        {
            List<Flight> current = new List<Flight>();
            if (User.IsInRole("Admin"))
            {
                List<Flight> temp = db.Flights.Where(x => x.assigned == true && x.completed == false).ToList();
                foreach (Flight f in temp)
                {
                    if (DateTime.Parse(f.flight_date).CompareTo(DateTime.Now) <= 0 && f.flight_start.CompareTo(DateTime.Now.TimeOfDay) <= 0)
                    {
                        current.Add(f);
                    }
                }
            }
            else if(User.IsInRole("Pilot"))
            {
                User user = db.Users.Where(x => x.username == User.Identity.Name).FirstOrDefault();
                List<AssignedFlight> af = db.AssignedFlights.Where(x => x.captain == user.id || x.first_officer == user.id).ToList();
                List<Flight> temp = new List<Flight>();
                foreach (AssignedFlight a in af)
                {
                     Flight f = db.Flights.Where(x =>x.flight_number == a.flight_number).FirstOrDefault();
                    if (f.completed == false)
                        temp.Add(f);
                }
                foreach (Flight f in temp)
                {
                    if (DateTime.Parse(f.flight_date).CompareTo(DateTime.Now) <= 0 && f.flight_start.CompareTo(DateTime.Now.TimeOfDay) <= 0)
                    {
                        current.Add(f);
                    }
                }
            }
            return View(current);
        }
        public ActionResult End(int flight_number)
        {
            Flight f = db.Flights.Where(x => x.flight_number == flight_number).FirstOrDefault();
            AssignedFlight af = db.AssignedFlights.Where(x => x.flight_number == flight_number).FirstOrDefault();
            foreach(User u in db.Users.Where(x => x.id == af.captain || x.id == af.first_officer || x.id == af.fal || x.id == af.fa1 || x.id == af.fa2 || x.id == af.fa3 || x.id == af.fa4 || x.id == af.fa5).ToList())
            {
                u.current_hours += f.flight_end.Subtract(f.flight_start).Hours;
                u.total_hours += f.flight_end.Subtract(f.flight_start).Hours;
                if (u.current_hours >= 9)
                {
                    u.rest_start = DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString();
                    u.availability = false;
                    u.current_hours = 0;
                }
            }
            db.Flights.Where(x => x.flight_number == flight_number).FirstOrDefault().completed = true;
            //db.AssignedFlights.Remove(db.AssignedFlights.Where(x => x.flight_number == flight_number).FirstOrDefault());
            db.SaveChanges();
            ViewBag.End = "Flight #: " + flight_number + " ended";
            return RedirectToAction("Current_Flights");
        }
        public ActionResult Completed_Flights()
        {
            return View(db.Flights.Where(x => x.completed == true).ToList());
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
