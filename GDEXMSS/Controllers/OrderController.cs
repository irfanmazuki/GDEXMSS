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
            orderModel.listOrder = (from order in dbModel.orders where order.status == "new" select order).ToList();
            orderModel.listItems = (from cartItem in dbModel.cartItems select cartItem).ToList();
            return View(orderModel);
        }
        [HttpPost]
        public ActionResult Incoming(order orderModel)
        {
            var data = dbModel.orders.ToList();
            return View(data);
        }
        [HttpGet]
        public ActionResult List()
        {
            combinedOrderList orderModel = new combinedOrderList();
            orderModel.listOrder = (from order in dbModel.orders select order).ToList();
            orderModel.listItems = (from cartItem in dbModel.cartItems select cartItem).ToList();
            return View(orderModel);
        }
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
        [HttpPost]
        public ActionResult Process(combinedOrderModel orderModel)
        {
            using (mssdbModel dbModel = new mssdbModel())
            {
                var editedOrder = dbModel.orders.Where(x => x.orderID == orderModel.order.orderID).FirstOrDefault();
                var editedOrderShipping = dbModel.orderShippingInfoes.Where(x => x.orderID == orderModel.orderShippingInfo.orderID).FirstOrDefault();
                orderModel.order.orderID = editedOrder.orderID;
                orderModel.orderShippingInfo.orderID = editedOrder.orderID;
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
            orderModel.listOrder = (from order in dbModel.orders where order.userID == userID select order).ToList();
            orderModel.listItems = (from cartItem in dbModel.cartItems select cartItem).ToList();
            return View(orderModel);
        }
    }
}