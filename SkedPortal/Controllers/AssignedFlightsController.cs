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
            MvcApplication.Restart();
            List<List<string>> crew = new List<List<string>>();
            List<AssignedFlight> af = db.AssignedFlights.ToList();
            foreach(var i in af)
            {
                List<string> names = new List<string>();
                User captain = db.Users.Where(x => x.id == i.captain).FirstOrDefault();
                User fo = db.Users.Where(x => x.id == i.first_officer).FirstOrDefault();
                User fal = db.Users.Where(x => x.id == i.fal).FirstOrDefault();
                User fa1 = db.Users.Where(x => x.id == i.fa1).FirstOrDefault();
                User fa2 = db.Users.Where(x => x.id == i.fa2).FirstOrDefault();
                User fa3 = db.Users.Where(x => x.id == i.fa3).FirstOrDefault();
                User fa4 = db.Users.Where(x => x.id == i.fa4).FirstOrDefault();
                User fa5 = db.Users.Where(x => x.id == i.fa5).FirstOrDefault();
                if (captain != null)
                {
                    names.Add(captain.first_name + " " + captain.last_name);
                }
                else
                    names.Add("");
                if (fo != null)
                {
                    names.Add(fo.first_name + " " + fo.last_name);
                }
                else
                    names.Add("");
                if (fal != null)
                {
                    names.Add(fal.first_name + " " + fal.last_name);
                }
                else
                    names.Add("");
                if (fa1 != null)
                {
                    names.Add(fa1.first_name + " " + fa1.last_name);
                }
                else
                    names.Add("");
                if (fa2 != null)
                {
                    names.Add(fa2.first_name + " " + fa2.last_name);
                }
                else
                    names.Add("");
                if (fa3 != null)
                {
                    names.Add(fa3.first_name + " " + fa3.last_name);
                }
                else
                    names.Add("");
                if (fa4 != null)
                {
                    names.Add(fa4.first_name + " " + fa4.last_name);
                }
                else
                    names.Add("");
                if (fa5 != null)
                {
                    names.Add(fa5.first_name + " " + fa5.last_name);
                }
                else
                    names.Add("");
                crew.Add(names);
            }
            ViewBag.Names = crew;
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
            ViewBag.FId = f.id;
            ViewBag.Number = f.flight_number;
            ViewBag.Date = f.flight_date;
            return View();
        }

        // POST: AssignedFlights/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id, flight_id,flight_number,flight_date,captain,first_officer,fal,fa1,fa2,fa3,fa4,fa5")] AssignedFlight assignedFlight)
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
            AssignedFlight af = db.AssignedFlights.Find(id);
            if (af == null)
            {
                return HttpNotFound();
            }
            List<int> ids = new List<int>();
            List<string> names = new List<string>();
            User captain = db.Users.Where(x => x.id == af.captain).FirstOrDefault();
            User fo = db.Users.Where(x => x.id == af.first_officer).FirstOrDefault();
            User fal = db.Users.Where(x => x.id == af.fal).FirstOrDefault();
            User fa1 = db.Users.Where(x => x.id == af.fa1).FirstOrDefault();
            User fa2 = db.Users.Where(x => x.id == af.fa2).FirstOrDefault();
            User fa3 = db.Users.Where(x => x.id == af.fa3).FirstOrDefault();
            User fa4 = db.Users.Where(x => x.id == af.fa4).FirstOrDefault();
            User fa5 = db.Users.Where(x => x.id == af.fa5).FirstOrDefault();
            if (captain != null)
            {
                names.Add(captain.first_name + " " + captain.last_name);
                ids.Add(captain.id);
            }
            else
            {
                names.Add("");
                ids.Add(-1);
            }
            if (fo != null)
            {
                names.Add(fo.first_name + " " + fo.last_name);
                ids.Add(fo.id);
            }
            else
            {
                names.Add("");
                ids.Add(-1);
            }
            if (fal != null)
            {
                names.Add(fal.first_name + " " + fal.last_name);
                ids.Add(fal.id);
            }
            else
            {
                names.Add("");
                ids.Add(-1);
            }
            if (fa1 != null)
            {
                names.Add(fa1.first_name + " " + fa1.last_name);
                ids.Add(fa1.id);
            }
            else
            {
                names.Add("");
                ids.Add(-1);
            }
            if (fa2 != null)
            {
                names.Add(fa2.first_name + " " + fa2.last_name);
                ids.Add(fa2.id);
            }
            else
            {
                names.Add("");
                ids.Add(-1);
            }
            if (fa3 != null)
            {
                names.Add(fa3.first_name + " " + fa3.last_name);
                ids.Add(fa3.id);
            }
            else
            {
                names.Add("");
                ids.Add(-1);
            }
            if (fa4 != null)
            {
                names.Add(fa4.first_name + " " + fa4.last_name);
                ids.Add(fa4.id);
            }
            else
            {
                names.Add("");
                ids.Add(-1);
            }
            if (fa5 != null)
            {
                names.Add(fa5.first_name + " " + fa5.last_name);
                ids.Add(fa5.id);
            }
            else
            {
                names.Add("");
                ids.Add(-1);
            }
            ViewBag.IDs = ids;
            ViewBag.Names = names;
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

            return View(af);
        }

        // POST: AssignedFlights/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,flight_id,flight_number,flight_date,captain,first_officer,fal,fa1,fa2,fa3,fa4,fa5")] AssignedFlight assignedFlight)
        {
            if (ModelState.IsValid && Validate(assignedFlight))
            {
                db.Entry(assignedFlight).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Edit", assignedFlight.id);
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
            db.Flights.Where(x => x.flight_number == assignedFlight.flight_number).First().assigned = false;
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
