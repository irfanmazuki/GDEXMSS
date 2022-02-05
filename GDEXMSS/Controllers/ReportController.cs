using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GDEXMSS.Models;
using Newtonsoft.Json;

namespace GDEXMSS.Controllers
{
    public class ReportController : Controller
    {
        private mssdbModel dbModel = new mssdbModel();
        // GET: Report
        [AdminSessionCheck]
        [HttpGet]
        public ActionResult Sales()
        {
            combinedSalesReport objSales = new combinedSalesReport();
            //get total sales
            var listTotalCost = (from order in dbModel.orders select order.totalCost).ToList();
            double totalSales = listTotalCost.Sum(x => Convert.ToDouble(x));
            TempData["TotalSale"] = totalSales;
            //get the number of orders
            TempData["TotalOrder"] = (from order in dbModel.orders select order).ToList().Count();
            //get the number of product sold
            TempData["TotalProductsSold"] = (from cartItem in dbModel.cartItems select cartItem).ToList().Count();
            //get total points redeemed
            var listPoints = (from order in dbModel.orders select order.pointRedeemed).ToList();
            TempData["TotalPoints"] = listPoints.Sum(x => Convert.ToDouble(x));

            objSales.listProducts = (from product in dbModel.products select product).ToList();
            List<DataPoint> dataPoints = new List<DataPoint>();
            foreach (var item in objSales.listProducts)
            {
                dataPoints.Add(new DataPoint(item.name, Convert.ToInt32(item.quantitySold)));
            }
            ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);
            return View(objSales);
        }
        public ActionResult SalesView()
        {
            return View();
        }
    }
}