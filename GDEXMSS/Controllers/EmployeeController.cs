using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GDEXMSS.Controllers
{
    public class EmployeeController : Controller
    {
        // GET: Employee
        public ActionResult New()
        {
            return View();
        }
    }
}