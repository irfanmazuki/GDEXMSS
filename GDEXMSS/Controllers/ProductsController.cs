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
            product dbModel = new product();
            //to get the categories list from the model -> get from productCategory.cs
            categoryModel ProductCategories = new categoryModel();
            using(mssdbModel db = new mssdbModel())
            {
                ProductCategories.Categories = db.productCategories.ToList<productCategory>();
            }
            dynamic mymodel = new ExpandoObject();
            mymodel.productCategory = GetCategories();
            return View(dbModel);
        }
        [HttpPost]
        public ActionResult Add(product productModel, categoryModel categoryModel)
        {
            //store the checkboxes to string
            var selectedCategories = categoryModel.Categories.Where(x => x.IsChecked == true).ToList<productCategory>();
            var strBoxes = Content(String.Join(",", selectedCategories.Select(x => x.categoryID)));

            string fileName = Path.GetFileNameWithoutExtension(productModel.ImageFile.FileName);
            string extension = Path.GetExtension(productModel.ImageFile.FileName);
            fileName = productModel.name.ToString() + "-" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + extension;
            productModel.imagePath = "../Image/" + fileName;
            fileName = Path.Combine(Server.MapPath("../Image/"), fileName);
            productModel.ImageFile.SaveAs(fileName);
            using (productdbModel dbModel = new productdbModel())
            {
                dbModel.products.Add(productModel);
                dbModel.SaveChanges();
            }
            ModelState.Clear();
            ViewBag.SuccessMassage = "Add Successful";
            return View("Add", new product());
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