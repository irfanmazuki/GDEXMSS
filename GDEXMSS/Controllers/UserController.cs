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
            eWallet objWallet = new eWallet();
            using(mssdbModel dbModel = new mssdbModel())
            {
                dbModel.users.Add(userModel);
                dbModel.SaveChanges();
            }
            ModelState.Clear();
            ViewBag.SuccessMassage = "Registration Successful";
            return RedirectToAction("Login", new { error = "success" });
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
            if (error == "success")
            {
                ViewBag.errorMessage = "Registration successful. Please log in to continue";
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
                    var obj = db.users.Where(a => a.username.Equals(objUser.email) && a.password.Equals(objUser.password)).FirstOrDefault();
                    if (obj != null)
                    {
                        if (obj.isExist == false)
                        {
                            return RedirectToAction("Login", new { error = "deactivated" });
                        }
                        else
                        {
                            Session["Email"] = obj.username.ToString();
                            Session["UserID"] = obj.userID.ToString();
                            Session["Name"] = obj.fullname.ToString();
                            Session["Role"] = "User";
                            Session["Type"] = obj.user_type.ToString();
                            //create the wallet account if the wallet is not existed yet
                            eWallet objWallet = new eWallet();
                            objWallet = (from eWallet in db.eWallets where eWallet.userID == obj.userID select eWallet).FirstOrDefault();
                            if(objWallet == null)
                            {
                                objWallet = new eWallet();
                                objWallet.amountRM = 0;
                                objWallet.availablePoints = 0;
                                objWallet.userID = obj.userID;
                                db.eWallets.Add(objWallet);
                                db.SaveChanges();

                            }
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
        [HttpGet]
        public ActionResult List()
        {
            mssdbModel dbModel = new mssdbModel();
            Session["query"] = "";
            var objUser = (from user in dbModel.users select user).ToList();
            return View(objUser);
        }
        [AdminSessionCheck]
        [HttpPost]
        public ActionResult List(string Sortby, string query)
        {
            mssdbModel dbModel = new mssdbModel();
            var objUser = (from user in dbModel.users select user).OrderByDescending(user => user.userID).ToList();
            if (query == "")
            {
                if (Sortby == "default")
                {
                    objUser = (from user in dbModel.users select user).OrderByDescending(user => user.userID).ToList();
                }
                else if(Sortby == "employee")
                {
                    objUser = (from user in dbModel.users where user.user_type == "Employee" select user).OrderByDescending(user => user.userID).ToList();
                }
                else if (Sortby == "public")
                {
                    objUser = (from user in dbModel.users where user.user_type == "Public" select user).OrderByDescending(user => user.userID).ToList();
                }
                else if (Sortby == "fullname")
                {
                    objUser = (from user in dbModel.users select user).OrderByDescending(user => user.fullname).ToList();
                }
                else if (Sortby == "race")
                {
                    objUser = (from user in dbModel.users select user).OrderByDescending(user => user.race).ToList();
                }
            }
            else
            {
                objUser = (from user in dbModel.users where user.fullname.Contains(query) || user.race == query || user.icnumber.ToString() == query || user.userID.ToString() == query || user.contact_number.ToString() == query select user).OrderByDescending(user => user.userID).ToList();
            }
            Session["query"] = "";
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
        [HttpGet]
        public ActionResult Profile()
        {
            mssdbModel dbModel = new mssdbModel();
            int userID = Int32.Parse(Session["UserID"].ToString()) ;
            var userRecord = dbModel.users.Where(x => x.userID == userID).FirstOrDefault();
            userRecord.listRaces = dbModel.mssSystems.Where(x => x.mss_Category == "user_race").ToList().Select(x => new SelectListItem
            {
                Value = x.mss_Name,
                Text = x.mss_Description
            });
            userRecord.listBankType = dbModel.mssSystems.Where(x => x.mss_Category == "bank_name").ToList().Select(x => new SelectListItem
            {
                Value = x.mss_Name,
                Text = x.mss_Description
            });
            userRecord.listStates = dbModel.mssSystems.Where(x => x.mss_Category == "states_name").ToList().Select(x => new SelectListItem
            {
                Value = x.mss_Name,
                Text = x.mss_Description
            });
            return View(userRecord);
        }
        [UserSessionCheck]
        [HttpPost]
        public ActionResult Profile(user objUser)
        {
            mssdbModel dbModel = new mssdbModel();
            //You need to populate it again, probably with the same code used before
            objUser.listRaces = dbModel.mssSystems.Where(x => x.mss_Category == "user_race").ToList().Select(x => new SelectListItem
            {
                Value = x.mss_Name,
                Text = x.mss_Description
            });
            objUser.listBankType = dbModel.mssSystems.Where(x => x.mss_Category == "bank_name").ToList().Select(x => new SelectListItem
            {
                Value = x.mss_Name,
                Text = x.mss_Description
            });
            objUser.listStates = dbModel.mssSystems.Where(x => x.mss_Category == "states_name").ToList().Select(x => new SelectListItem
            {
                Value = x.mss_Name,
                Text = x.mss_Description
            });
            var EditedObj = dbModel.users.Where(x => x.userID == objUser.userID).FirstOrDefault();
            bool anyExist = false;
            bool checkUsernameExist = dbModel.users.Where(x => x.username == objUser.username).Any();
            bool checkEmailExist = dbModel.users.Where(x => x.email == objUser.email).Any();
            if (checkUsernameExist && objUser.username != EditedObj.username)
            {
                ModelState.AddModelError("username", "Username is taken");
                anyExist = true;
            }
            if (checkEmailExist && objUser.email != EditedObj.email)
            {
                ModelState.AddModelError("email", "Email is taken");
                anyExist = true;
            }
            if (anyExist)
            {
                return View(objUser);
            }
            dbModel.Entry(EditedObj).CurrentValues.SetValues(objUser);
            dbModel.SaveChanges();
            return RedirectToAction("Profile");
        }
        public ActionResult Test11()
        {
            using (mssdbModel dbModel = new mssdbModel()){
                return View(dbModel.users.ToList());
            }
            
        }
    }
}