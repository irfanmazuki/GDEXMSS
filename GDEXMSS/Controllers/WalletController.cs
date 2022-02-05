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
    public class WalletController : Controller
    {
        private mssdbModel dbModel = new mssdbModel();
        [AdminSessionCheck]
        [HttpGet]
        public ActionResult UserList() 
        {
            CombinedListWalletUser walletUserModel = new CombinedListWalletUser();
            walletUserModel.listWallet = (from eWallet in dbModel.eWallets select eWallet).ToList();
            walletUserModel.listUser = (from user in dbModel.users  select user).ToList();
            //walletUserModel.eWallet = (from eWallet in dbModel.eWallets where eWallet.userID == Int32.Parse(Session["UserID"].ToString()) select eWallet).FirstOrDefault();
            //walletUserModel.userObj = (from user in dbModel.users where user.userID == Int32.Parse(Session["UserID"].ToString()) select user).FirstOrDefault();
            return View(walletUserModel);
        }
        [AdminSessionCheck]
        [HttpPost]
        public ActionResult UserList(string query)
        {
            CombinedListWalletUser walletUserModel = new CombinedListWalletUser();
            walletUserModel.listWallet = (from eWallet in dbModel.eWallets select eWallet).ToList();
            walletUserModel.listUser = (from user in dbModel.users where user.fullname.Contains(query) select user).ToList();
            //walletUserModel.eWallet = (from eWallet in dbModel.eWallets where eWallet.userID == Int32.Parse(Session["UserID"].ToString()) select eWallet).FirstOrDefault();
            //walletUserModel.userObj = (from user in dbModel.users where user.userID == Int32.Parse(Session["UserID"].ToString()) select user).FirstOrDefault();
            return View(walletUserModel);
        }
        [AdminSessionCheck]
        [HttpGet]
        public ActionResult Topup(int walletID)
        {
            Session["walletID"] = walletID;
            eWallet walletModel = new eWallet();
            walletModel.amountRM = 0;
            walletModel.availablePoints = 0;
            return View(walletModel);
        }
        [AdminSessionCheck]
        [HttpPost]
        public ActionResult Topup(eWallet walletModel)
        {
            eWallet walletRecord = dbModel.eWallets.Where(x => x.walletID == walletModel.walletID).FirstOrDefault();
            if (walletModel.availablePoints != null)
            {
                walletRecord.availablePoints = walletRecord.availablePoints + walletModel.availablePoints;
            }
            //if(walletModel.amountRM != null)
            //{
            //    walletRecord.amountRM = walletRecord.amountRM + walletModel.amountRM;
            //}
            dbModel.SaveChanges();
            TempData["point_success"] = "success";
            return RedirectToAction("UserList");
        }
        [UserSessionCheck]
        [HttpGet]
        public ActionResult Balance()
        {
            int userID = Int32.Parse(Session["userID"].ToString());
            eWallet objWallet = new eWallet();
            objWallet = (from eWallet in dbModel.eWallets where eWallet.userID == userID select eWallet).FirstOrDefault();
            return View(objWallet);
        }
    }
}