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
        public ActionResult UserList(string message) 
        {
            if (message=="success")
            {
                ViewBag.Message = "e-Wallet successfully updated";
            }
            CombinedListWalletUser walletUserModel = new CombinedListWalletUser();
            walletUserModel.listWallet = (from eWallet in dbModel.eWallets select eWallet).ToList();
            walletUserModel.listUser = (from user in dbModel.users  select user).ToList();
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
            return View(walletModel);
        }
        [AdminSessionCheck]
        [HttpPost]
        public ActionResult Topup(eWallet walletModel)
        {
            eWallet walletRecord = dbModel.eWallets.Where(x => x.walletID == walletModel.walletID).FirstOrDefault();
            walletRecord.availablePoints = walletRecord.availablePoints + walletModel.availablePoints;
            walletRecord.amountRM = walletRecord.amountRM + walletModel.amountRM;
            dbModel.SaveChanges();
            Session["walletID"] = "";
            return RedirectToAction("UserList", new { message = "success" });
        }
        // GET: Wallet
        public ActionResult Balance()
        {
            return View();
        }
    }
}