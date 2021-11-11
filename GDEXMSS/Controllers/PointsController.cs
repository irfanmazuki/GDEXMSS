using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GDEXMSS.Controllers
{
    public class PointsController : Controller
    {
        // GET: Points
        public ActionResult Assign()
        {
            return View();
        }
        public ActionResult Users()
        {
            return View();
        }
        public ActionResult History()
        {
            return View();
        }
    }
}