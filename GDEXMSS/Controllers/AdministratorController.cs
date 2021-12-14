using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GDEXMSS.Models;

namespace GDEXMSS.Controllers
{
    public class AdministratorController : Controller
    {
        public ActionResult Login(string error)
        {
            administrator userModel = new administrator();
            if (error == "password")
            {
                ViewBag.errorMessage = "Wrong admin ID or Password";
            }
            if (Session["userID"] != null)
            {
                return RedirectToAction("List", "Products");
            }
            return View(userModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(administrator objAdmin)
        {
            if (ModelState.IsValid)
            {
                using (mssdbModel db = new mssdbModel())
                {
                    var obj = db.administrators.Where(a => a.adminID.Equals(objAdmin.adminID) && a.password.Equals(objAdmin.password)).FirstOrDefault();
                    if (obj != null)
                    {
                        Session["UserID"] = obj.adminID.ToString();
                        Session["Role"] = "Admin";
                        Session["Name"] = obj.name.ToString();
                        return RedirectToAction("List", "Products");
                    }
                    return RedirectToAction("Login", new { error = "password" });
                }
            }
            return View(objAdmin);
        }
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}