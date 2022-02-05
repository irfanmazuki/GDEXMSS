using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GDEXMSS.Models;

namespace GDEXMSS.Controllers
{
    public class OrderController : Controller
    {
        private mssdbModel dbModel = new mssdbModel();
        // GET: Order
        [HttpGet]
        public ActionResult Incoming()
        {
            combinedOrderList orderModel = new combinedOrderList();
            orderModel.listOrder = (from order in dbModel.orders where order.status == "new" select order).OrderByDescending(order => order.createdDT).ToList();
            orderModel.listItems = (from cartItem in dbModel.cartItems select cartItem).ToList();
            return View(orderModel);
        }
        [HttpPost]
        public ActionResult Incoming(order orderModel)
        {
            var data = dbModel.orders.ToList();
            return View(data);
        }
        [AdminSessionCheck]
        [HttpGet]
        public ActionResult List()
        {
            combinedOrderList orderModel = new combinedOrderList();
            //orderModel.order.orderByListItems = new List<SelectListItem>()
            //{
            //  new SelectListItem(){ Value = "paid", Text = "Amount Paid" },
            //  new SelectListItem(){ Value = "userID", Text = "user ID" },
            //};
            orderModel.listOrder = (from order in dbModel.orders where order.status != "new" select order).OrderByDescending(order => order.shippedDT).ToList();
            orderModel.listItems = (from cartItem in dbModel.cartItems select cartItem).ToList();
            Session["query"] = "";
            return View(orderModel);
        }
        [AdminSessionCheck]
        [HttpPost]
        public ActionResult List(string Sortby, string query)
        {
            combinedOrderList orderModel = new combinedOrderList();
            if (query == "")
            {
                if (Sortby == "paid")
                {
                    orderModel.listOrder = (from order in dbModel.orders where order.status != "new" select order).OrderByDescending(order => order.amountPaid).ToList();
                }
                else if (Sortby == "userID")
                {
                    orderModel.listOrder = (from order in dbModel.orders where order.status != "new" select order).OrderByDescending(order => order.userID).ToList();
                }
                else if (Sortby == "orderID")
                {
                    orderModel.listOrder = (from order in dbModel.orders where order.status != "new" select order).OrderByDescending(order => order.orderID).ToList();
                }
                else if (Sortby == "sent")
                {
                    orderModel.listOrder = (from order in dbModel.orders where order.status == "sent" select order).OrderByDescending(order => order.orderID).ToList();
                }
                else if (Sortby == "received")
                {
                    orderModel.listOrder = (from order in dbModel.orders where order.status == "received" select order).OrderByDescending(order => order.orderID).ToList();
                }
                else if (Sortby == "reviewed")
                {
                    orderModel.listOrder = (from order in dbModel.orders where order.status == "reviewed" select order).OrderByDescending(order => order.orderID).ToList();
                }
                else if (Sortby == "default")
                {
                    orderModel.listOrder = (from order in dbModel.orders where order.status != "new" select order).OrderByDescending(order => order.shippedDT).ToList();
                }
            }
            else
            {
                orderModel.listOrder = (from order in dbModel.orders where order.orderID == query || order.consignment == query || order.status == query || order.userID.ToString() == query select order).OrderByDescending(order => order.shippedDT).ToList();
            }
            orderModel.listItems = (from cartItem in dbModel.cartItems select cartItem).ToList();
            Session["query"] = "";
            return View(orderModel);
        }
        [AdminSessionCheck]
        [HttpGet]
        public ActionResult Process(string orderID)
        {
            var orderShippingInfoRecord = (from orderShippingInfo in dbModel.orderShippingInfoes where orderShippingInfo.orderID == orderID select orderShippingInfo).FirstOrDefault();
            var orderRecord = (from order in dbModel.orders where order.orderID == orderID select order).FirstOrDefault();
            var cartItems = (from cartItem in dbModel.cartItems where cartItem.orderID == orderID select cartItem).ToList();
            combinedOrderModel objModel = new combinedOrderModel();
            objModel.orderShippingInfo = orderShippingInfoRecord;
            objModel.order = orderRecord;
            if (cartItems != null)
            {
                objModel.listItems = cartItems;
            }
            return View(objModel);
        }
        [AdminSessionCheck]
        [HttpPost]
        public ActionResult Process(combinedOrderModel orderModel)
        {
            using (mssdbModel dbModel = new mssdbModel())
            {
                var editedOrder = dbModel.orders.Where(x => x.orderID == orderModel.order.orderID).FirstOrDefault();
                var editedOrderShipping = dbModel.orderShippingInfoes.Where(x => x.orderID == orderModel.orderShippingInfo.orderID).FirstOrDefault();
                orderModel.order.orderID = editedOrder.orderID;
                orderModel.orderShippingInfo.orderID = editedOrder.orderID;
                orderModel.order.status = "sent";
                orderModel.order.shippedDT = DateTime.Now;
                dbModel.Entry(editedOrder).CurrentValues.SetValues(orderModel.order);
                dbModel.SaveChanges();
                dbModel.Entry(editedOrderShipping).CurrentValues.SetValues(orderModel.orderShippingInfo);
                dbModel.SaveChanges();
            }
            ModelState.Clear();
            ViewBag.SuccessMassage = "Add Successful";
            return RedirectToAction("List");
        }
        public ActionResult Details()
        {
            return View();
        }
        [UserSessionCheck]
        [HttpGet]
        public ActionResult History()
        {
            mssdbModel dbModel = new mssdbModel();
            int userID = Int32.Parse(Session["UserID"].ToString());
            combinedOrderList orderModel = new combinedOrderList();
            orderModel.listOrder = (from order in dbModel.orders where order.userID == userID select order).OrderByDescending(order => order.createdDT).ToList();
            orderModel.listItems = (from cartItem in dbModel.cartItems select cartItem).ToList();
            Session["query"] = "";
            return View(orderModel);
        }
        [UserSessionCheck]
        [HttpPost]
        public ActionResult History(string Sortby, string query)
        {
            mssdbModel dbModel = new mssdbModel();
            int userID = Int32.Parse(Session["UserID"].ToString());
            combinedOrderList orderModel = new combinedOrderList();
            orderModel.listOrder = (from order in dbModel.orders where order.userID == userID select order).OrderByDescending(order => order.createdDT).ToList();
            if(query == "")
            {
                if(Sortby == "dateasc")
                {
                    orderModel.listOrder = (from order in dbModel.orders where order.userID == userID select order).OrderBy(order => order.createdDT).ToList();
                }
                else if(Sortby == "priceasc")
                {
                    orderModel.listOrder = (from order in dbModel.orders where order.userID == userID select order).OrderBy(order => order.amountPaid).ToList();
                }
                else if (Sortby == "pricedesc")
                {
                    orderModel.listOrder = (from order in dbModel.orders where order.userID == userID select order).OrderByDescending(order => order.amountPaid).ToList();
                }
                else if (Sortby == "processing")
                {
                    orderModel.listOrder = (from order in dbModel.orders where order.userID == userID && order.status == "new" select order).OrderByDescending(order => order.amountPaid).ToList();
                }
                else if (Sortby == "sent")
                {
                    orderModel.listOrder = (from order in dbModel.orders where order.userID == userID && order.status == "sent" select order).OrderByDescending(order => order.amountPaid).ToList();
                }
                else if (Sortby == "received")
                {
                    orderModel.listOrder = (from order in dbModel.orders where order.userID == userID && order.status == "received" select order).OrderByDescending(order => order.amountPaid).ToList();
                }
                else if (Sortby == "reviewed")
                {
                    orderModel.listOrder = (from order in dbModel.orders where order.userID == userID && order.status == "reviewed" select order).OrderByDescending(order => order.amountPaid).ToList();
                }
            }
            else
            {
                orderModel.listOrder = (from order in dbModel.orders where order.userID == userID && order.orderID == query select order).OrderByDescending(order => order.createdDT).ToList();
            }
            orderModel.listItems = (from cartItem in dbModel.cartItems select cartItem).ToList();
            Session["query"] = "";
            return View(orderModel);
        }
        [UserSessionCheck]
        [HttpGet]
        public ActionResult Review(string orderID)
        {
            combinedOrderReview objOrderReview = new combinedOrderReview();
            int userID = Int32.Parse(Session["UserID"].ToString());
            objOrderReview.listItem = (from cartItem in dbModel.cartItems where cartItem.orderID == orderID select cartItem).ToList();
            objOrderReview.order = (from order in dbModel.orders where order.orderID == orderID select order).FirstOrDefault();
            objOrderReview.orderShippingInfo = (from orderShippingInfo in dbModel.orderShippingInfoes where orderShippingInfo.orderID == orderID select orderShippingInfo).FirstOrDefault();
            return View(objOrderReview);
        }
        [UserSessionCheck]
        [HttpPost]
        public ActionResult Review(combinedOrderReview objOrderReview)
        {
            using (dbModel)
            {
                var orderID = objOrderReview.order.orderID;
                int userID = Int32.Parse(Session["UserID"].ToString());
                objOrderReview.listItem = (from cartItem in dbModel.cartItems where cartItem.orderID == orderID select cartItem).ToList();
                foreach(var item in objOrderReview.listItem)
                {
                    reviewOrder reviewOrder = new reviewOrder();
                    reviewOrder.reviewComment = objOrderReview.reviewOrder.reviewComment;
                    reviewOrder.ProductID = item.productID;
                    reviewOrder.reviewDT = DateTime.Now;
                    reviewOrder.userID = userID;
                    reviewOrder.orderID = orderID;
                    reviewOrder.userName = Session["Name"].ToString();
                    dbModel.reviewOrders.Add(reviewOrder);
                }
                order objOrder = new order();
                objOrder = (from order in dbModel.orders where order.orderID == orderID select order).FirstOrDefault();
                objOrder.status = "reviewed";
                dbModel.SaveChanges();
                return RedirectToAction("History", "Order");
            }
        }
    }
}