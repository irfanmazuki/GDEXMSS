using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using GDEXMSS.Models;
using HtmlAgilityPack;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.css;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;
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
            if (Session["CartCounter"] == null)
            {
                Session["CartCounter"] = 0;
            }
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
            ViewBag.ErrorMassage = "";
            mssdbModel dbModel = new mssdbModel();
            int userID = Int32.Parse(Session["UserID"].ToString());
            CombinedCartUser objCartUser = new CombinedCartUser();
            objCartUser.listCartItems = Session["CartItem"] as List<cartItem>;
            objCartUser.eWallet = dbModel.eWallets.Where(x => x.userID == userID).FirstOrDefault();
            return View(objCartUser);
        }
        [UserSessionCheck]
        [HttpPost]
        public ActionResult Cart(CombinedCartUser objCartUser)
        {
            mssdbModel dbModel = new mssdbModel();
            int userID = Int32.Parse(Session["UserID"].ToString());
            double totalCart = Convert.ToDouble(Session["totalCart"].ToString());
            double pointInRM = objCartUser.order.pointRedeemed.GetValueOrDefault() / 100.0;
            objCartUser.eWallet = dbModel.eWallets.Where(x => x.userID == userID).FirstOrDefault();
            objCartUser.listCartItems = Session["CartItem"] as List<cartItem>;
            if(objCartUser.eWallet.availablePoints < objCartUser.order.pointRedeemed)
            {
                ViewBag.ErrorMassage = "Not enough point";
                return View(objCartUser);
            }
            else if (pointInRM > totalCart)
            {
                ViewBag.ErrorMassage = "Reached maximum points";
                return View(objCartUser);
            }
            else
            {
                combinedOrderModel objOrderModel = new combinedOrderModel();
                objOrderModel.listItems = objCartUser.listCartItems;
                objOrderModel.order = objCartUser.order;
                objOrderModel.mssSystem = (from mssSystem in dbModel.mssSystems where mssSystem.mss_Category == "shippingFee" select mssSystem).FirstOrDefault();
                objOrderModel.listStates = dbModel.mssSystems.Where(x => x.mss_Category == "states_name").ToList().Select(x => new SelectListItem
                {
                    Value = x.mss_Name,
                    Text = x.mss_Description
                });
                return View("Checkout", objOrderModel);
            }
        }
        public ActionResult EditCart(int productID, string actions)
        {
            listCartItem = Session["CartItem"] as List<cartItem>;
            var objProduct = (from product in dbModel.products where product.productID == productID select product).FirstOrDefault();
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
                        if (item.quantity > objProduct.quantity)
                        {
                            TempData["CartError"] = "maximumItems";
                            item.quantity--;
                        }
                        else
                        {
                            item.total = item.quantity * item.unitCost;
                        }
                    }
                }
            }
            return RedirectToAction("Cart");
        }
        [UserSessionCheck]
        [HttpGet]
        public ActionResult Checkout(combinedOrderModel objModel)
        {
            //objModel.mssSystem = (from mssSystem in dbModel.mssSystems where mssSystem.mss_Category == "shippingFee" select mssSystem).FirstOrDefault();
            return View(objModel);
        }
        [UserSessionCheck]
        [HttpGet]
        public ActionResult Receipt(string orderID)
        {
            Session["orderID"] = orderID;
            combinedOrderModel objOrder = new combinedOrderModel();
            objOrder.listItems = (from cartItem in dbModel.cartItems where cartItem.orderID == orderID select cartItem).ToList();
            objOrder.order = (from order in dbModel.orders where order.orderID == orderID select order).FirstOrDefault();
            objOrder.orderShippingInfo = (from orderShippingInfo in dbModel.orderShippingInfoes where orderShippingInfo.orderID == orderID select orderShippingInfo).FirstOrDefault();
            return View(objOrder);
        }
        //checkout will go here
        [UserSessionCheck]
        [HttpPost]
        public ActionResult Receipt(combinedOrderModel objModel)
        {
            objModel.listStates = dbModel.mssSystems.Where(x => x.mss_Category == "states_name").ToList().Select(x => new SelectListItem
            {
                Value = x.mss_Name,
                Text = x.mss_Description
            });
            if (ModelState.IsValid)
            {
                mssdbModel dbModel = new mssdbModel();
                decimal totalRM = 0;
                var orderID = Guid.NewGuid().ToString("N");
                //change length to orderID to 8
                orderID = orderID.Substring(0, 8);
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
                objOrder.totalCost = totalRM;
                objOrder.amountPaid = totalRM + shippingFee - (Convert.ToDecimal(objOrder.pointRedeemed.GetValueOrDefault() / 100.0));
                objOrder.createdDT = DateTime.Now;
                objOrder.userID = Int32.Parse(Session["UserID"].ToString());
                objOrder.orderID = orderID;
                Session["orderID"] = orderID;
                //minus point from wallet
                objModel.wallet = (from eWallet in dbModel.eWallets where eWallet.userID == objOrder.userID select eWallet).FirstOrDefault();
                objModel.wallet.availablePoints -= objOrder.pointRedeemed;
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
                Session["CartItem"] = null;
                Session["CartCounter"] = 0;
                return RedirectToAction("Receipt", new { @orderID = orderID });
            }
            objModel.mssSystem = (from mssSystem in dbModel.mssSystems where mssSystem.mss_Category == "shippingFee" select mssSystem).FirstOrDefault();
            return View("Checkout", objModel);
        }
        [UserSessionCheck]
        [HttpPost]
        [ValidateInput(false)]
        public FileResult Export(string GridHtml)
        {
            //thanks to this https://dotnetgenetics.blogspot.com/2017/11/invalid-nested-tag-div-found-expected.html 
            //and this https://www.aspsnippets.com/Articles/MVC-iTextSharp-Example-Convert-HTML-to-PDF-using-iTextSharp-in-ASPNet-MVC.aspx
            //and this https://sanushabalan.wordpress.com/2017/09/21/add-css-file-while-generating-pdf-using-itextsharp/comment-page-1/
            string extraElements = "<div class=\"text-center\"><img src=\"https://seeklogo.com/images/G/gdex-logo-6838448C0F-seeklogo.com.png\" class=\"rounded mx-auto d-block \" style=\"width:90%\"></div>";
            HtmlNode.ElementsFlags["img"] = HtmlElementFlag.Closed;
            HtmlNode.ElementsFlags["input"] = HtmlElementFlag.Closed;
            HtmlDocument doc = new HtmlDocument();
            doc.OptionFixNestedTags = true;
            doc.LoadHtml(extraElements+GridHtml);
            GridHtml = doc.DocumentNode.OuterHtml;
            //css
            List<string> cssFiles = new List<string>();
            cssFiles.Add(@"~/Content/css/style.css");
            cssFiles.Add(@"~/Content/css/adminltev2.css");
            cssFiles.Add(@"~/Content/css/bootstrap.css");
            cssFiles.Add(@"~/Content/css/reset.css");
            cssFiles.Add(@"~/Content/css/responsive.css");
            cssFiles.Add(@"~/Content/lib/font-awesome/css/all.css");
            cssFiles.Add(@"~/Content/css/owl-carousel.css");

            //end css

            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                StringReader sr = new StringReader(GridHtml);
                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 100f, 0f);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                //create CSS resolver to apply css
                HtmlPipelineContext htmlContext = new HtmlPipelineContext(null);
                htmlContext.SetTagFactory(Tags.GetHtmlTagProcessorFactory());
                ICSSResolver cssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(false);
                cssFiles.ForEach(i => cssResolver.AddCssFile(System.Web.HttpContext.Current.Server.MapPath(i), true));

                //create and attach pipelines
                IPipeline pipeline = new CssResolverPipeline(cssResolver,
                    new HtmlPipeline(htmlContext, new PdfWriterPipeline(pdfDoc, writer)));

                //XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                //create XMLWorker
                XMLWorker worker = new XMLWorker(pipeline, true);
                XMLParser xmlParser = new XMLParser(worker);
                xmlParser.Parse(new MemoryStream(Encoding.UTF8.GetBytes(GridHtml)));
                string fileName = "Receipt for order "+ Session["orderID"].ToString() + ".pdf";
                pdfDoc.Close();
                return File(stream.ToArray(), "application/pdf", fileName);
            }
        }
        [AdminSessionCheck]
        [HttpGet]
        public ActionResult Add(string productName)
        {
            mssdbModel db = new mssdbModel();
            CombinedProductCategories objModel = new CombinedProductCategories();
            objModel.product = new product();
            objModel.product.CategoriesList = new SelectList(db.productCategories, "categoryID", "name");
            if (productName != null)
            {
                objModel.product.name = productName;
            }

            return View(objModel);
        }
        [AdminSessionCheck]
        [HttpPost]
        public ActionResult Add(CombinedProductCategories productModel)
        {
            mssdbModel db = new mssdbModel();
            productModel.product.CategoriesList = new SelectList(db.productCategories, "categoryID", "name");
            if (ModelState.IsValid)
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
            return View(productModel);
        }
        [AdminSessionCheck]
        [HttpGet]
        public ActionResult List()
        {
            mssdbModel db = new mssdbModel();
            var productList = (from product in db.products select product).ToList();
            Session["query"] = "";
            return View(productList);
        }
        [AdminSessionCheck]
        [HttpPost]
        public ActionResult List(string Sortby, string query)
        {
            mssdbModel db = new mssdbModel();
            var productList = (from product in db.products select product).ToList();
            if (query == "")
            {
                if(Sortby == "default" || Sortby == "nameasc")
                {
                    productList = (from product in db.products select product).OrderBy(product => product.name).ToList();
                }
                else if(Sortby == "namedesc")
                {
                    productList = (from product in db.products select product).OrderByDescending(product => product.name).ToList();
                }
                else if (Sortby == "priceasc")
                {
                    productList = (from product in db.products select product).OrderBy(product => product.unitCost).ToList();
                }
                else if (Sortby == "pricedesc")
                {
                    productList = (from product in db.products select product).OrderByDescending(product => product.unitCost).ToList();
                }
                else if (Sortby == "quantityasc")
                {
                    productList = (from product in db.products select product).OrderBy(product => product.quantity).ToList();
                }
                else if (Sortby == "quantitydesc")
                {
                    productList = (from product in db.products select product).OrderByDescending(product => product.quantity).ToList();
                }
                else if (Sortby == "aoldasc")
                {
                    productList = (from product in db.products select product).OrderBy(product => product.quantitySold).ToList();
                }
                else if (Sortby == "solddesc")
                {
                    productList = (from product in db.products select product).OrderByDescending(product => product.quantitySold).ToList();
                }
                else if (Sortby == "available")
                {
                    productList = (from product in db.products where product.quantity > 0 select product).OrderBy(product => product.name).ToList();
                }
                else if (Sortby == "soldout")
                {
                    productList = (from product in db.products where product.quantity == 0 select product).OrderBy(product => product.name).ToList();
                }
                else if (Sortby == "discontinued")
                {
                    productList = (from product in db.products where product.isExist == false select product).OrderBy(product => product.name).ToList();
                }
            }
            else
            {
                productList = (from product in db.products where product.name.Contains(query) || product.description.Contains(query) select product).OrderBy(product => product.name).ToList();
            }
            Session["query"] = "";
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
                //delete the product
                //bool oldValidateOnSaveEnabled = dbModel.Configuration.ValidateOnSaveEnabled;
                //try
                //{
                //    dbModel.Configuration.ValidateOnSaveEnabled = false;
                //    var tryProductID = new product { productID = productID };
                //    dbModel.products.Attach(tryProductID);
                //    dbModel.Entry(tryProductID).State = System.Data.Entity.EntityState.Deleted;
                //    dbModel.SaveChanges();
                //}
                //finally
                //{
                //    dbModel.Configuration.ValidateOnSaveEnabled = oldValidateOnSaveEnabled;
                //}ch
                product deactivateProduct = dbModel.products.FirstOrDefault(x => x.productID == productID);
                deactivateProduct.isExist = false;
                dbModel.SaveChanges();
                return RedirectToAction("List");
            }
            if (actions == "activate")
            {
                //delete the product
                //bool oldValidateOnSaveEnabled = dbModel.Configuration.ValidateOnSaveEnabled;
                //try
                //{
                //    dbModel.Configuration.ValidateOnSaveEnabled = false;
                //    var tryProductID = new product { productID = productID };
                //    dbModel.products.Attach(tryProductID);
                //    dbModel.Entry(tryProductID).State = System.Data.Entity.EntityState.Deleted;
                //    dbModel.SaveChanges();
                //}
                //finally
                //{
                //    dbModel.Configuration.ValidateOnSaveEnabled = oldValidateOnSaveEnabled;
                //}ch
                product deactivateProduct = dbModel.products.FirstOrDefault(x => x.productID == productID);
                deactivateProduct.isExist = true;
                dbModel.SaveChanges();
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
                productModel.product.imagePath = "/Image/" + fileName;
                fileName = Path.Combine(Server.MapPath("../Image/"), fileName);
                productModel.ImageFile.SaveAs(fileName);
                using (mssdbModel dbModel = new mssdbModel())
                {
                    var EditedObj = dbModel.products.Where(x => x.productID == productModel.product.productID).FirstOrDefault();
                    EditedObj.imagePath = productModel.product.imagePath;
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
        [UserSessionCheck]
        [HttpGet]
        public ActionResult Search(int categoryID)
        {
            mssdbModel dbModel = new mssdbModel();
            CombinedProductIndex objModel = new CombinedProductIndex();
            var categories = (from productCategory in dbModel.productCategories select productCategory).ToList();
            var products = (from product in dbModel.products select product).ToList();
            if (categoryID > 0)
            {
                products = (from product in dbModel.products where product.categoryID == categoryID select product).ToList();
            }
            objModel.listCategories = categories;
            objModel.listProduct = products;
            return View(objModel);
        }
        [UserSessionCheck]
        [HttpPost]
        public ActionResult Search(string query, int categoryID)
        {
            mssdbModel dbModel = new mssdbModel();
            CombinedProductIndex objModel = new CombinedProductIndex();
            if (query != null)
            {
                var categories = (from productCategory in dbModel.productCategories select productCategory).ToList();
                var products = (from product in dbModel.products where product.name.Contains(query) select product).ToList();
                objModel.listCategories = categories;
                objModel.listProduct = products;
            }
            else
            {
                var categories = (from productCategory in dbModel.productCategories select productCategory).ToList();
                var products = (from product in dbModel.products where product.categoryID == categoryID select product).ToList();
                objModel.listCategories = categories;
                objModel.listProduct = products;
            }
            return View(objModel);
        }
    }
}