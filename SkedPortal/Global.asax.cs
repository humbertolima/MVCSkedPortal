﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using SkedPortal.Models;
namespace SkedPortal
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalFilters.Filters.Add(new AuthorizeAttribute());
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Restart();
            
            
        }
        public static void Restart()
        {
            SkedPortalEntities db = new SkedPortalEntities();
            foreach (User u in db.Users.ToList())
            {
                if (u.availability == false && DateTime.Now.TimeOfDay.Subtract(TimeSpan.Parse(u.rest_start)).Hours >= 8)
                {
                    u.current_hours = 0;
                    u.rest_start = "";
                    u.availability = true;
                }
                db.SaveChanges();
            }
        }
    }
}
