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
        private List<cartItem> listCartItem;
        public ProductsController()
        {
            dbModel = new mssdbModel();
            listCartItem = new List<cartItem>();
        }
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
        public JsonResult Index(int ItemId)
        {
            cartItem objCartItem = new cartItem();
            //product objProduct = (from product in dbModel.products where product.productID == ItemId select product).FirstOrDefault();
            //listCartItem = (from cartItem in dbModel.cartItems where cartItem.productID == ItemId select cartItem).ToList();
            product objProduct = dbModel.products.Single(model => model.productID == ItemId);
            if (Session["CartItem"] != null)
            {
                listCartItem = Session["CartItem"] as List<cartItem>;
            }
            if (listCartItem.Any(Model => Model.productID == ItemId))
            {
                objCartItem = listCartItem.Single(model => model.productID == ItemId);
                objCartItem.quantity = objCartItem.quantity + 1;
                objCartItem.total = objCartItem.quantity * objCartItem.unitCost;
            }
            else
            {
                objCartItem.itemID = 0;
                objCartItem.productID = ItemId;
                objCartItem.imagePath = objProduct.imagePath;
                objCartItem.productName = objProduct.name;
                objCartItem.quantity = 1;
                objCartItem.total = objProduct.unitCost;
                objCartItem.unitCost = objProduct.unitCost;
                listCartItem.Add(objCartItem);
            }

            Session["CartCounter"] = listCartItem.Count();
            Session["CartItem"] = listCartItem;
            return Json(new { Success = true, Counter = listCartItem.Count() }, JsonRequestBehavior.AllowGet);
        }
        [UserSessionCheck]
        [HttpGet]
        public ActionResult Cart()
        {
            listCartItem = Session["CartItem"] as List<cartItem>;
            return View(listCartItem);
        }
        public ActionResult EditCart(int productID, string actions)
        {
            listCartItem = Session["CartItem"] as List<cartItem>;
            if (actions == "delete")
            {
                foreach(var item in listCartItem)
                {
                    if (item.productID == productID)
                    {
                        int index = listCartItem.FindIndex(a => a.productID == productID);
                        if (item.quantity > 1)
                        {
                            item.quantity--;
                            item.total -= item.quantity * item.unitCost;
                        }
                        else
                        {
                            listCartItem.RemoveAt(index);
                            return RedirectToAction("Cart");
                        }
                    }
                }
            }
            if (actions == "add")
            {
                foreach (var item in listCartItem)
                {
                    if (item.productID == productID)
                    {
                        item.quantity++;
                        item.total += item.quantity * item.unitCost;
                    }
                }
            }
            return RedirectToAction("Cart");
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
            productModel.product.imagePath = "/Image/" + fileName;
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