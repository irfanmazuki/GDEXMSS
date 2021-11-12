using System;
using System.Collections.Generic;
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
        // GET: Products
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Add()
        {
            mssdbModel db = new mssdbModel();
            //product dbModel = new product();
            //test
            //productModel productModel = new productModel();
            //productCategoryModel productCategoryModel = new productCategoryModel();
            CombinedProductCategories objModel = new CombinedProductCategories();
            objModel.product = new product();
            objModel.product.CategoriesList = new SelectList(db.productCategories, "categoryID", "name");

            return View(objModel);
        }
        [HttpPost]
        //public ActionResult Add(product productModel)
        //{
        //    string fileName = Path.GetFileNameWithoutExtension(productModel.ImageFile.FileName);
        //    string extension = Path.GetExtension(productModel.ImageFile.FileName);
        //    fileName = productModel.name.ToString() + "-" + DateTime.Now.ToString("MMddyyyy") + "" + extension;
        //    productModel.imagePath = "../Image/" + fileName;
        //    fileName = Path.Combine(Server.MapPath("../Image/"), fileName);
        //    productModel.ImageFile.SaveAs(fileName);
        //    using (mssdbModel dbModel = new mssdbModel())
        //    {
        //        dbModel.products.Add(productModel);
        //        dbModel.SaveChanges();
        //    }
        //    ModelState.Clear();
        //    ViewBag.SuccessMassage = "Add Successful";
        //    return View("Add", new product());
        //}
        public ActionResult Add(CombinedProductCategories productModel)
        {
            string fileName = Path.GetFileNameWithoutExtension(productModel.ImageFile.FileName);
            string extension = Path.GetExtension(productModel.ImageFile.FileName);
            fileName = productModel.product.name.ToString() + "-" + DateTime.Now.ToString("MMddyyyy") + "" + extension;
            productModel.product.imagePath = "../Image/" + fileName;
            fileName = Path.Combine(Server.MapPath("../Image/"), fileName);
            productModel.ImageFile.SaveAs(fileName);
            using (mssdbModel dbModel = new mssdbModel())
            {
                dbModel.products.Add(productModel.product);
                dbModel.SaveChanges();
            }
            ModelState.Clear();
            ViewBag.SuccessMassage = "Add Successful";
            return View("List");
        }
        public ActionResult List()
        {
            return View();
        }
        public ActionResult Suggested()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Details()
        {
            product dbModel = new product();
            return View(dbModel);
        }

    }
}