using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SkedPortal.Models;
using System.Web.Security;

namespace SkedPortal.Controllers
{
    
    [Authorize(Roles ="Admin")]
    public class UsersController : Controller
    {

        private SkedPortalEntities db = new SkedPortalEntities();

        public bool Validate(User user)
        {
            if (DateTime.Parse(user.rest_start).CompareTo(DateTime.Now) < 0)
            {
                ViewBag.Error = "Rest Start Incorrect";
                return false;
            }
            else
                return true;
        }
        // GET: Users
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

       
        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,photo,first_name,last_name,email,username,hash,permissions,total_hours,current_hours,rest_start,availability")] User user)
        {
            if (ModelState.IsValid)
            {
                
                if (!Validate_Username(user))
                {
                    user.current_hours = 0; user.availability = true; user.total_hours = 0;
                    user.rest_start = "";
                    db.Users.Add(user);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            return View(user);
        }

        private bool Validate_Username(User user)
        {
            User temp = db.Users.Where(x => x.username == user.username).FirstOrDefault();
            if (temp == null)
            {
                return false;
            }
            else
            {
                ViewBag.Username_Error = "This username already exist";
                return true;
            }
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,photo,first_name,last_name,email,username,hash,permissions,total_hours,current_hours,rest_start,availability")] User user)
        {
            if (ModelState.IsValid && Validate(user))
            {
                if (!Validate_Username(user))
                {
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
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
