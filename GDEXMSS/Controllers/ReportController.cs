using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GDEXMSS.Controllers
{
    public class ReportController : Controller
    {
        // GET: Report
        public ActionResult Sales ()
        {
            return View();
        }
        public ActionResult SalesView()
        {
            return View();
        }
    }
}