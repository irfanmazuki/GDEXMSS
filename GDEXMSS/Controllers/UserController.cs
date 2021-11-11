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
        public ActionResult Login()
        {
            user userModel = new user();
            return View(userModel);
        }
        public ActionResult List()
        {
            using (mssdbModel dbModel = new mssdbModel())
            {
                return View(dbModel.users.ToList());
            }
        }
        public ActionResult Edit()
        {
            user userModel = new user();
            return View(userModel);
        }
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