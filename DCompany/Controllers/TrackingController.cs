using DCompany.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcPaging;
namespace DCompany.Controllers
{
    public class TrackingController : Controller
    {
        //
        // GET: /Tracking/
        private DecalCompanyEntities1 db = new DecalCompanyEntities1();
        private const int defaultPageSize = 3;
        private IList<Task> listTask = new List<Task>();
        private IList<Order> listProcess = new List<Order>();
        private IList<Store> listStore = new List<Store>();
        private IList<PayMent> listPayment = new List<PayMent>();
        public ActionResult Index()
        {
            if (this.Session["Account"] == null)
            {
                return Redirect("/Login");
            }
            return View();
        }
        public ActionResult  Tracking(string FROM, string TO, string module) 
        
        {
            if (this.Session["Account"] == null)
            {
                return Redirect("/Login");
            }
            var from = DateTime.Parse(FROM);
            var to = DateTime.Parse(TO);
          
            if (module == "Task")
            {
               
                var list = db.Tasks.Where(s => s.DateTime >= from && s.DateTime <= to).OrderByDescending(s => s.DateTime).ToList();
                return PartialView("ListTask_Tr", list);
            }
            if (module == "Process") {

             var  list = db.Orders.Where(s => s.DateInfo >= from && s.DateInfo <= to).OrderByDescending(s => s.DateInfo).ToList();
                return PartialView("Process_Tr", list);
            }
            if (module == "Store") {

                var list = db.Stores.Where(s => s.Time >= from && s.Time <= to).OrderByDescending(s => s.Time).ToList();
                return PartialView("Store_Tr", list);
            }
            if (module == "Payment")
            {

                var list = db.PayMents.Where(s => s.DatePay >= from && s.DatePay <=to).OrderByDescending(s => s.DatePay).ToList();
                return PartialView("Payment_Tr", list);
            }
            return Redirect("Index");
        }
    }
}

