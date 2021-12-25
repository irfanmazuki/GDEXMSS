using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GDEXMSS.Models;
using HtmlAgilityPack;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using Syncfusion.HtmlConverter;
using Syncfusion.Pdf;

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
                if(objCartItem.quantity > objProduct.quantity)
                {
                    objCartItem.quantity = objCartItem.quantity - 1;
                    return Json(new { Success = false, Counter = listCartItem.Count() }, JsonRequestBehavior.AllowGet);
                }
                objCartItem.total = objCartItem.quantity * objCartItem.unitCost;
            }
            else
            {
                objCartItem.quantity = 1;
                if (objCartItem.quantity > objProduct.quantity)
                {
                    return Json(new { Success = false, Counter = listCartItem.Count() }, JsonRequestBehavior.AllowGet);
                }
                objCartItem.itemID = 0;
                objCartItem.productID = ItemId;
                objCartItem.imagePath = objProduct.imagePath;
                objCartItem.productName = objProduct.name;
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
                            item.total = item.quantity * item.unitCost;
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
                        item.total = item.quantity * item.unitCost;
                    }
                }
            }
            return RedirectToAction("Cart");
        }
        [UserSessionCheck]
        [HttpGet]
        public ActionResult Checkout()
        {
            combinedOrderModel objModel = new combinedOrderModel();
            objModel.mssSystem = (from mssSystem in dbModel.mssSystems where mssSystem.mss_Category == "shippingFee" select mssSystem).FirstOrDefault();
            return View(objModel);
        }
        [UserSessionCheck]
        [HttpPost]
        public ActionResult Checkout(combinedOrderModel objModel)
        {
            mssdbModel dbModel = new mssdbModel();
            decimal totalRM = 0;
            var orderID = Guid.NewGuid().ToString("N");
            //mssSystem
            objModel.mssSystem = (from mssSystem in dbModel.mssSystems where mssSystem.mss_Category == "shippingFee" select mssSystem).FirstOrDefault();
            decimal shippingFee = decimal.Parse(objModel.mssSystem.mss_Description.ToString());
            //cartItem
            objModel.listItems = Session["CartItem"] as List<cartItem>;
            foreach (var item in objModel.listItems)
            {
                cartItem objCartItem = new cartItem();
                objCartItem = item;
                objCartItem.orderID = orderID;
                totalRM += item.total.GetValueOrDefault(0.00M);
                //add sold quantity to the product model and remove from quantity
                product objProduct = new product();
                objProduct = (from product in dbModel.products where product.productID == item.productID select product).FirstOrDefault();
                objProduct.quantitySold = objProduct.quantitySold + item.quantity;
                objProduct.quantity -= item.quantity;
                dbModel.cartItems.Add(objCartItem);
                dbModel.SaveChanges();
            }
            //order
            order objOrder = new order();
            objOrder = objModel.order;
            objOrder.status = "new";
            objOrder.amountPaid = totalRM + shippingFee;
            objOrder.createdDT = DateTime.Now;
            objOrder.userID = Int32.Parse(Session["UserID"].ToString());
            objOrder.orderID = orderID;
            //orderShippingInfo
            objModel.orderShippingInfo.orderID = objModel.order.orderID;
            objModel.orderShippingInfo.cost = shippingFee;
            using (dbModel)
            {
                dbModel.orderShippingInfoes.Add(objModel.orderShippingInfo);
                dbModel.orders.Add(objOrder);
                dbModel.orders.Add(objModel.order);
                dbModel.SaveChanges();
            }
            Session["CartItem"] = 0;
            Session["CartCounter"] = 0;
            return RedirectToAction("Receipt", new { @orderID = orderID });
        }
        [UserSessionCheck]
        [HttpGet]
        public ActionResult Receipt(string orderID)
        {
            combinedOrderModel objOrder = new combinedOrderModel();
            objOrder.listItems = (from cartItem in dbModel.cartItems where cartItem.orderID == orderID select cartItem).ToList();
            objOrder.order = (from order in dbModel.orders where order.orderID == orderID select order).FirstOrDefault();
            objOrder.orderShippingInfo = (from orderShippingInfo in dbModel.orderShippingInfoes where orderShippingInfo.orderID == orderID select orderShippingInfo).FirstOrDefault();
            return View(objOrder);
        }
        [UserSessionCheck]
        [HttpPost]
        [ValidateInput(false)]
        public FileResult Export(string GridHtml)
        {
            //thanks to this https://dotnetgenetics.blogspot.com/2017/11/invalid-nested-tag-div-found-expected.html 
            //and this https://www.aspsnippets.com/Articles/MVC-iTextSharp-Example-Convert-HTML-to-PDF-using-iTextSharp-in-ASPNet-MVC.aspx
            HtmlNode.ElementsFlags["img"] = HtmlElementFlag.Closed;
            HtmlNode.ElementsFlags["input"] = HtmlElementFlag.Closed;
            HtmlDocument doc = new HtmlDocument();
            doc.OptionFixNestedTags = true;
            doc.LoadHtml(GridHtml);
            GridHtml = doc.DocumentNode.OuterHtml;

            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                StringReader sr = new StringReader(GridHtml);
                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 100f, 0f);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                pdfDoc.Close();
                return File(stream.ToArray(), "application/pdf", "Grid.pdf");
            }
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
                //set quantity sold to zero
                productModel.product.quantitySold = 0;
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
        [AdminSessionCheck]
        [HttpGet]
        public ActionResult ListTest()
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
        public ActionResult Details(int productID)
        {
            CombinedProductReview objProductReview = new CombinedProductReview();
            int userID = Int32.Parse(Session["UserID"].ToString());
            objProductReview.product = (from product in dbModel.products where product.productID == productID select product).FirstOrDefault();
            objProductReview.listReviews = (from reviewOrder in dbModel.reviewOrders where reviewOrder.ProductID == productID select reviewOrder).ToList();
            return View(objProductReview);
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
        [UserSessionCheck]
        [HttpGet]
        public ActionResult Received(string orderID)
        {
            order objOrder = new order();
            objOrder = (from order in dbModel.orders where order.orderID == orderID select order).FirstOrDefault();
            objOrder.status = "received";
            dbModel.SaveChanges();
            return RedirectToAction("History","Order");
        }
        //[UserSessionCheck]
        //[HttpGet]
        //public ActionResult Review(string orderID)
        //{

        //}
    }
}