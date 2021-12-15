using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GDEXMSS.Models;

namespace GDEXMSS.Controllers
{
    public class ProductsController : Controller
    {
        private mssdbModel dbModel = new mssdbModel();
        [UserSessionCheck]
        [HttpGet]
        public ActionResult Index()
        {
            mssdbModel dbModel = new mssdbModel();
            CombinedProductIndex objModel = new CombinedProductIndex();
            var categories = (from productCategory in dbModel.productCategories select productCategory).ToList();
            var products = (from product in dbModel.products select product).ToList();
            objModel.listCategories = categories;
            objModel.listProduct = products;
            return View(objModel);
        }
        [AdminSessionCheck]
        [HttpGet]
        public ActionResult Add()
        {
            mssdbModel db = new mssdbModel();
            CombinedProductCategories objModel = new CombinedProductCategories();
            objModel.product = new product();
            objModel.product.CategoriesList = new SelectList(db.productCategories, "categoryID", "name");

            return View(objModel);
        }
        [AdminSessionCheck]
        [HttpPost]
        public ActionResult Add(CombinedProductCategories productModel)
        {
            string fileName = Path.GetFileNameWithoutExtension(productModel.ImageFile.FileName);
            string extension = Path.GetExtension(productModel.ImageFile.FileName);
            fileName = productModel.product.name.ToString() + "-" + DateTime.Now.ToString("MMddyyyy") + "" + extension;
            productModel.product.imagePath = "~/Image/" + fileName;
            fileName = Path.Combine(Server.MapPath("../Image/"), fileName);
            productModel.ImageFile.SaveAs(fileName);
            using (mssdbModel dbModel = new mssdbModel())
            {
                dbModel.products.Add(productModel.product);
                dbModel.SaveChanges();
            }
            ModelState.Clear();
            ViewBag.SuccessMassage = "Add Successful";
            return RedirectToAction("List");
        }
        [AdminSessionCheck]
        [HttpGet]
        public ActionResult List()
        {
            mssdbModel db = new mssdbModel();
            var productList = (from product in db.products select product).ToList();
            return View(productList);
        }
        public ActionResult Suggested()
        {
            return View();
        }
        [AdminSessionCheck]
        [HttpGet]
        public ActionResult Details()
        {
            product dbModel = new product();
            return View(dbModel);
        }
        [AdminSessionCheck]
        public ActionResult Edit(int productID, string actions)
        {
            if (actions == "delete")
            {
                bool oldValidateOnSaveEnabled = dbModel.Configuration.ValidateOnSaveEnabled;
                try
                {
                    dbModel.Configuration.ValidateOnSaveEnabled = false;
                    var tryProductID = new product { productID = productID };
                    dbModel.products.Attach(tryProductID);
                    dbModel.Entry(tryProductID).State = System.Data.Entity.EntityState.Deleted;
                    dbModel.SaveChanges();
                }
                finally
                {
                    dbModel.Configuration.ValidateOnSaveEnabled = oldValidateOnSaveEnabled;
                }
                return RedirectToAction("List");
            }
            //if the actions is not delete then show all the records in edit form
            var editedProduct = (from product in dbModel.products where product.productID == productID select product).FirstOrDefault();
            CombinedProductCategories objModel = new CombinedProductCategories();
            //it works lol. attached the list to the combined object model. honestly i dont know how it works lol
            objModel.product = editedProduct;
            objModel.product.CategoriesList = new SelectList(dbModel.productCategories, "categoryID", "name");
            return View(objModel);
        }
        [AdminSessionCheck]
        [HttpPost]
        public ActionResult Edit(CombinedProductCategories productModel)
        {
            if (productModel.ImageFile == null)
            {
                using (mssdbModel dbModel = new mssdbModel())
                {
                    var EditedObj = dbModel.products.Where(x => x.productID == productModel.product.productID).FirstOrDefault();
                    productModel.product.productID = EditedObj.productID;
                    dbModel.Entry(EditedObj).CurrentValues.SetValues(productModel.product);
                    //dbModel.products.Add(productModel.product);
                    dbModel.SaveChanges();
                }
                ModelState.Clear();
                ViewBag.SuccessMassage = "Add Successful";
                return RedirectToAction("List");
            }
            else
            {
                string fileName = Path.GetFileNameWithoutExtension(productModel.ImageFile.FileName);
                string extension = Path.GetExtension(productModel.ImageFile.FileName);
                fileName = productModel.product.name.ToString() + "-" + DateTime.Now.ToString("MMddyyyy") + "" + extension;
                productModel.product.imagePath = "../Image/" + fileName;
                fileName = Path.Combine(Server.MapPath("../Image/"), fileName);
                using (mssdbModel dbModel = new mssdbModel())
                {
                    var EditedObj = dbModel.products.Where(x => x.productID == productModel.product.productID).FirstOrDefault();
                    productModel.product.productID = EditedObj.productID;
                    dbModel.Entry(EditedObj).CurrentValues.SetValues(productModel.product);
                    //dbModel.products.Add(productModel.product);
                    dbModel.SaveChanges();
                }
                ModelState.Clear();
                ViewBag.SuccessMassage = "Add Successful";
                return RedirectToAction("List");
            }
        }

    }
}