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
                ViewBag.errorMessage = "Wrong email or Password";
            }
            if(error == "deactivated")
            {
                ViewBag.errorMessage = "Your account has been deactivated. Please contact support for reactivation";
            }
            if (error == "login")
            {
                ViewBag.errorMessage = "Please login first to continue";
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
                        if (obj.isExist == false)
                        {
                            return RedirectToAction("Login", new { error = "deactivated" });
                        }
                        else
                        {
                            Session["Email"] = obj.email.ToString();
                            Session["Name"] = obj.fullname.ToString();
                            Session["Role"] = "User";
                            Session["Type"] = obj.user_type.ToString();
                            return RedirectToAction("Index", "Products");
                        }
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
            mssdbModel dbModel = new mssdbModel();
            var objUser = (from user in dbModel.users select user).ToList();
            return View(objUser);
        }
        [AdminSessionCheck]
        [HttpGet]
        public ActionResult Edit(int userID, string actions)
        {
            mssdbModel dbModel = new mssdbModel();
            user userModel = new user();
            if (actions == "edit")
            {
                var userRecord = dbModel.users.Where(x => x.userID == userID).FirstOrDefault();
                userModel = userRecord;
                return View(userModel);
            }
            if (actions == "deactivate")
            {
                user editedUser = dbModel.users.FirstOrDefault(x => x.userID == userID);
                editedUser.isExist = false;
                dbModel.SaveChanges();
                return RedirectToAction("List");
            }
            if (actions == "activate")
            {
                user editedUser = dbModel.users.FirstOrDefault(x => x.userID == userID);
                editedUser.isExist = true;
                dbModel.SaveChanges();
                return RedirectToAction("List");
            }
            return View(userModel);
        }
        [AdminSessionCheck]
        [HttpPost]
        public ActionResult Edit(user userModel)
        {
            mssdbModel dbModel = new mssdbModel();
            var EditedObj = dbModel.users.Where(x => x.userID == userModel.userID).FirstOrDefault();
            dbModel.Entry(EditedObj).CurrentValues.SetValues(userModel);
            dbModel.SaveChanges();
            return RedirectToAction("List");
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