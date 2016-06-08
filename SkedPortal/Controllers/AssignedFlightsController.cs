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
    public class AssignedFlightsController : Controller
    {
        private SkedPortalEntities db = new SkedPortalEntities();

        public bool Validate(AssignedFlight af)
        {
            List<int?> ids = new List<int?>() { af.fal, af.fa1, af.fa2, af.fa3, af.fa4, af.fa5 };
            if (ids.Count() > 0)
            {
                for (int i = 0; i < ids.Count() - 1; i++)
                    for (int j = i + 1; j < ids.Count(); j++)
                    {
                        if (ids[i] != null && ids[j] != null)
                        {
                            if (ids[i] == ids[j])
                            {
                                ViewBag.Error = "FAs must not be iquals";
                                return false;
                            }
                        }
                    }
            }

            if ((af.captain != null && af.first_officer != null) && (af.captain == af.first_officer))
            {
                ViewBag.Error = "Captain must not be the same First Officer";
                    return false; }
            else
                return true;
            
        }
        // GET: AssignedFlights
        public ActionResult Index()
        {
            return View(db.AssignedFlights.ToList());
        }

        // GET: AssignedFlights/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AssignedFlight assignedFlight = db.AssignedFlights.Find(id);
            if (assignedFlight == null)
            {
                return HttpNotFound();
            }
            return View(assignedFlight);
        }

        // GET: AssignedFlights/Create
        public ActionResult Create(Flight f)
        {
            List<User> pilots = db.Users.Where(x => x.permissions == "Pilot" && x.availability == true).ToList();
            List<User> fas = db.Users.Where(x => x.permissions == "FA" && x.availability == true).ToList();

            if (pilots.Count() > 0)
            {
                ViewBag.Captains = pilots;
            }
            else
            { ViewBag.Captains = ""; }

            if (fas.Count() > 0)
            {
                ViewBag.FAs = fas;
            }
            else
            { ViewBag.FAs = ""; }
      
            ViewBag.Number = f.flight_number;
            ViewBag.Date = f.flight_date;
            return View();
        }

        // POST: AssignedFlights/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,flight_number,flight_date,captain,first_officer,fal,fa1,fa2,fa3,fa4,fa5")] AssignedFlight assignedFlight)
        {
            if (ModelState.IsValid && Validate(assignedFlight))
            {
                db.Flights.Where(x => x.flight_number == assignedFlight.flight_number).FirstOrDefault().assigned = true;
                db.Flights.Where(x => x.flight_number == assignedFlight.flight_number).FirstOrDefault().completed = false; ;
                db.SaveChanges();
                db.AssignedFlights.Add(assignedFlight);
                db.SaveChanges();
                return RedirectToAction("Index", "Dashboard");
            }
           
            return RedirectToAction("Create", assignedFlight);
        }

        // GET: AssignedFlights/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AssignedFlight assignedFlight = db.AssignedFlights.Find(id);
            if (assignedFlight == null)
            {
                return HttpNotFound();
            }
            List<User> pilots = db.Users.Where(x => x.permissions == "Pilot" && x.availability == true).ToList();
            List<User> fas = db.Users.Where(x => x.permissions == "FA" && x.availability == true).ToList();

            if (pilots.Count() > 0)
            {
                ViewBag.Captains = pilots;
            }
            else
            { ViewBag.Captains = ""; }

            if (fas.Count() > 0)
            {
                ViewBag.FAs = fas;
            }
            else
            { ViewBag.FAs = ""; }

            return View(assignedFlight);
        }

        // POST: AssignedFlights/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,flight_number,flight_date,captain,first_officer,fal,fa1,fa2,fa3,fa4,fa5")] AssignedFlight assignedFlight)
        {
            if (ModelState.IsValid && Validate(assignedFlight))
            {
                db.Entry(assignedFlight).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(assignedFlight);
        }

        // GET: AssignedFlights/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AssignedFlight assignedFlight = db.AssignedFlights.Find(id);
            if (assignedFlight == null)
            {
                return HttpNotFound();
            }
            return View(assignedFlight);
        }

        // POST: AssignedFlights/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AssignedFlight assignedFlight = db.AssignedFlights.Find(id);
            db.AssignedFlights.Remove(assignedFlight);
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
