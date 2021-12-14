using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using GDEXMSS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace GDEXMSS.Controllers
{
    public class UserController : Controller
    {
        [HttpGet]
        public ActionResult Registration()
        {
            user userModel = new user();
            return View(userModel);
        }

        [HttpPost]
        public ActionResult Registration(user userModel)
        {
            using(mssdbModel dbModel = new mssdbModel())
            {
                dbModel.users.Add(userModel);
                dbModel.SaveChanges();
            }
            ModelState.Clear();
            ViewBag.SuccessMassage = "Registration Successful";
            return View("Registration", new user()); 
        }
        [HttpGet]
        public ActionResult Login(string error)
        {
            user userModel = new user();
            if (error == "password")
            {
                ViewBag.errorMessage = "Wrong admin ID or Password";
            }
            if (Session["Email"] != null)
            {
                return RedirectToAction("Index", "Products");
            }
            return View(userModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(user objUser)
        {
            if (ModelState.IsValid)
            {
                using (mssdbModel db = new mssdbModel())
                {
                    var obj = db.users.Where(a => a.email.Equals(objUser.email) && a.password.Equals(objUser.password)).FirstOrDefault();
                    if (obj != null)
                    {
                        Session["Email"] = obj.email.ToString();
                        Session["Name"] = obj.fullname.ToString();
                        Session["Role"] = "User";
                        Session["Type"] = obj.user_type.ToString();
                        return RedirectToAction("Index", "Products");
                    }
                    return RedirectToAction("Login", new { error = "password" });
                }
            }
            return View(objUser);
        }
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
        [AdminSessionCheck]
        public ActionResult List()
        {
            using (mssdbModel dbModel = new mssdbModel())
            {
                return View(dbModel.users.ToList());
            }
        }
        [AdminSessionCheck]
        public ActionResult Edit()
        {
            user userModel = new user();
            return View(userModel);
        }
        [UserSessionCheck]
        public ActionResult Profile()
        {
            user userModel = new user();
            return View(userModel);
        }

        public ActionResult Test11()
        {
            using (mssdbModel dbModel = new mssdbModel()){
                return View(dbModel.users.ToList());
            }
            
        }
    }
}