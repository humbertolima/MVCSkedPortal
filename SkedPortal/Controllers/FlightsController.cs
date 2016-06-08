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
    [Authorize(Roles = "Admin")]
    public class FlightsController : Controller
    {
        private SkedPortalEntities db = new SkedPortalEntities();

        public bool Validate(Flight flight)
        {
            if (flight.flight_date.CompareTo(DateTime.Now)==-1)
            {
                ViewBag.Error = "Date is incorrect";
                return false;
            }
            else if (flight.flight_end.CompareTo(flight.flight_start)==-1)
            {
                ViewBag.Error = "Flight End or Flight Start is incorrect";
                return false;
            }
            else
            {
                return true;
            }
        }

        //Get
        public ActionResult Assign(int id)
        {
            return RedirectToAction("Create", "AssignedFlights", db.Flights.Find(id));
        }
        // GET: Flights
        public ActionResult Index()
        {
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
            if (ModelState.IsValid && Validate(flight))
            {
                if (!Validate_flight(flight.flight_number))
                {
                    flight.assigned = false; flight.completed = false;
                    db.Flights.Add(flight);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(flight);
        }
        private bool Validate_flight(int fn)
        {
            Flight temp = db.Flights.Where(x => x.flight_number == fn).FirstOrDefault();
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
            ViewBag.Model = flight;
            return View(flight);
        }

        // POST: Flights/Edit/5
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,flight_number,flight_date,flight_origin,flight_start,flight_destination,flight_end,assigned,completed")] Flight flight)
        {
            if (ModelState.IsValid && Validate(flight))
            {
                if (!Validate_flight(flight.flight_number))
                {
                    if (flight.assigned == false)
                    {
                        AssignedFlight af = db.AssignedFlights.Where(x => x.flight_number == flight.flight_number).First();
                        if (af != null)
                        {
                            db.AssignedFlights.Remove(af);
                        }
                    }
                    else
                    {
                        db.AssignedFlights.Where(x => x.flight_number == flight.flight_number).FirstOrDefault().flight_date = flight.flight_date;
                        db.AssignedFlights.Where(x => x.flight_number == flight.flight_number).FirstOrDefault().flight_number = flight.flight_number;
                    }
                    db.Entry(flight).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                 }
            }
            return View(flight);
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
            db.AssignedFlights.Remove(af);
            db.Flights.Remove(flight);
            db.SaveChanges();
            return RedirectToAction("Index");
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
