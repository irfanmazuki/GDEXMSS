using GDEXMSS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GDEXMSS.Controllers
{
    public class SettingController : Controller
    {
        mssdbModel dbModel = new mssdbModel();
        [AdminSessionCheck]
        [HttpGet]
        public ActionResult Shipping()
        {
            var objShipping = dbModel.mssSystems.Where(x => x.mss_Name == "semenanjung").FirstOrDefault();
            return View(objShipping);
        }
        [AdminSessionCheck]
        [HttpPost]
        public ActionResult Shipping(mssSystem objSystem)
        {
            var objShipping = dbModel.mssSystems.Where(x => x.mss_Name == "semenanjung").FirstOrDefault();
            objShipping.mss_Description = objSystem.mss_Description;
            dbModel.SaveChanges();
            ViewBag.SuccessMessage = "Shipping fee is successfully updated";
            return View(objShipping);
        }
        public ActionResult PromoPage()
        {
            return View();
        }
    }
}