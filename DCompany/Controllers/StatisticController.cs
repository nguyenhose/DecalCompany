using DCompany.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DCompany.Controllers
{
    public class StatisticController : Controller
    {
        private DecalCompanyEntities1 db = new DecalCompanyEntities1();
        //
        // GET: /Statistic/

        public ActionResult Index()
        {
            if (this.Session["Account"] == null)
            {
                return Redirect("/Login");
            }
            return View();
        }

        public ActionResult ChangYear(string YEAR)
        {
            string noOrder = "";
            string noOrderPrice = "";
            string noPayment = "";
            string noStore = "";
            int year;
            if (YEAR == null) { year = DateTime.Now.Year; }
            else { year = Convert.ToInt32(YEAR); }
            for (var i = 1; i <= 12; i++)
            {
                var n = db.Orders.Where(s => s.Invisible == false && s.DateInfo.Value.Month == i && s.DateInfo.Value.Year == year).Count();
                var m = db.Orders.Where(s => s.Invisible == false && s.DateInfo.Value.Month == i && s.DateInfo.Value.Year == year).Sum(s => s.TotalCash);
                if (m == null) { m = 0; }
                var z = db.PayMents.Where(s => s.Invisible == false && s.DatePay.Value.Month == i && s.DatePay.Value.Year == year).Sum(s => s.Cash);
                if (z == null) { z = 0; }
                var y = db.Stores.Where(s => s.Invisible == false && s.Time.Value.Month == i && s.Time.Value.Year == year).Sum(s => s.ProductPrice);
                if (y == null) { y = 0; }
                noOrder = n + "," + noOrder;
                noOrderPrice = m + "," + noOrderPrice;
                noPayment = z + "," + noPayment;
                noStore = y + "," + noStore;

            }
            ViewBag.OrderNO = noOrder.Substring(0, noOrder.Length - 1);
            ViewBag.PriceOrder = noOrderPrice.Substring(0, noOrderPrice.Length - 1);
            ViewBag.Payment = noPayment.Substring(0, noPayment.Length - 1);
            ViewBag.Store = noStore.Substring(0, noStore.Length - 1);
            return PartialView("ListSta");
        }

    }
}
