using GDEXMSS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GDEXMSS.Controllers
{
    public class CategoryController : Controller
    {
        mssdbModel dbModel = new mssdbModel();
        // GET: Category
        public ActionResult Index()
        {
            return View();
        }
        [AdminSessionCheck]
        [HttpGet]
        public ActionResult List()
        {
            var categoryList = (from productCategory in dbModel.productCategories select productCategory).ToList();
            return View(categoryList);
        }
        [AdminSessionCheck]
        [HttpGet]
        public ActionResult Edit(int categoryID, string actions)
        {
            productCategory categoryModel = new productCategory();
            if (actions == "edit")
            {
                var categoryRecord = dbModel.productCategories.Where(x => x.categoryID == categoryID).FirstOrDefault();
                categoryModel = categoryRecord;
                return View(categoryModel);
            }
            if (actions == "deactivate")
            {
                var categoryRecord = dbModel.productCategories.Where(x => x.categoryID == categoryID).FirstOrDefault();
                categoryRecord.isExist = false;
                dbModel.SaveChanges();
                return RedirectToAction("List");
            }
            if (actions == "activate")
            {
                var categoryRecord = dbModel.productCategories.Where(x => x.categoryID == categoryID).FirstOrDefault();
                categoryRecord.isExist = true;
                dbModel.SaveChanges();
                return RedirectToAction("List");
            }
            return View(categoryModel);
        }

        [AdminSessionCheck]
        [HttpPost]
        public ActionResult Edit(productCategory categoryModel)
        {
            var objCategory = dbModel.productCategories.Where(x => x.categoryID == categoryModel.categoryID).FirstOrDefault();
            dbModel.Entry(objCategory).CurrentValues.SetValues(categoryModel);
            dbModel.SaveChanges();
            return RedirectToAction("List");
        }
        [AdminSessionCheck]
        [HttpGet]
        public ActionResult Add()
        {
            productCategory categoryModel = new productCategory();
            return View(categoryModel);
        }
        [AdminSessionCheck]
        [HttpPost]
        public ActionResult Add(productCategory categoryModel)
        {
            categoryModel.isExist = true;
            using (mssdbModel dbModel = new mssdbModel())
            {
                dbModel.productCategories.Add(categoryModel);
                dbModel.SaveChanges();
            }
               
            return RedirectToAction("List");
        }

    }
}