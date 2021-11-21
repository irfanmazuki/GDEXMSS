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
        private mssdbModel db = new mssdbModel();
        // GET: Order
        [HttpGet]
        public ActionResult Incoming()
        {
            var orderList = (from order in db.orders where order.status=="new" select order).ToList();
            return View(orderList);
        }
        [HttpPost]
        public ActionResult Incoming(order orderModel)
        {
            var data = db.orders.ToList();
            return View(data);
        }
        [HttpGet]
        public ActionResult List()
        {
            var orderList = (from order in db.orders select order).ToList();
            return View(orderList);
        }
        public ActionResult Process()
        {
            return View();
        }
        public ActionResult Details()
        {
            return View();
        }
        [HttpGet]
        public ActionResult History()
        {
            var orderList = (from order in db.orders select order).ToList();
            return View(orderList);
        }
    }
}