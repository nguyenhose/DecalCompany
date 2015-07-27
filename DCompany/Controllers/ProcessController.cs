using DCompany.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcPaging;
namespace DCompany.Controllers
{
    public class ProcessController : Controller
    {
        private DecalCompanyEntities1 db = new DecalCompanyEntities1();
        private const int defaultPageSize = 3;
        private IList<Process> listPr = new List<Process>();
        //
        // GET: /Process/

        public ActionResult Index(string employee_name, int? page)
        {
            if (this.Session["Account"] == null)
            {
                return Redirect("/Login");
            }
            var id = Convert.ToInt32(this.Session["ID"]);
            var thatdate = Convert.ToDateTime(employee_name);
            var today = DateTime.Today;
            ViewData["employee_name"] = employee_name;
            IList<Process> list = this.listPr;
            int currentPageIndex = page.HasValue ? page.Value : 1;
            if (string.IsNullOrWhiteSpace(employee_name))
            {

                list = db.Processes.OrderByDescending(s => s.Order.DateStart).ToPagedList(currentPageIndex, defaultPageSize);
            }
            else {
                list = db.Processes.Where(s => s.Order.DateStart == thatdate).OrderByDescending(s => s.Order.DateStart).ToPagedList(currentPageIndex, defaultPageSize);
            }
            ViewBag.AllowEditEx = db.Permissions.Any(s => s.UserId == id && s.ModuleId == 5 && s.Edit == true);
            ViewBag.AllowEditPr = db.Permissions.Any(s => s.UserId == id && s.ModuleId == 6 && s.Edit == true);
            if (Request.IsAjaxRequest())
                return PartialView("ListPro", list);
            else
            {
                return View(list);
            }
          
        }
        public ActionResult ExportQuick(string Id) {
            var nameId = Convert.ToInt32(Id);
            ViewBag.OrderName = db.Processes.Where(s => s.Id == nameId).Select(s => s.Order.Name).First();
            ViewBag.OrderId = Id;
            var id = Convert.ToInt32(this.Session["ID"]);
            ViewBag.AllowEdit = db.Permissions.Any(s => s.UserId == id && s.ModuleId == 5 && s.Edit == true);
            var listPaper = db.Stores.Where(s => s.Note.Contains("Tờ") && s.Invisible==false);
            return PartialView("ListExport", listPaper.ToList());
        }
        public void CheckDone(string Id, string SL, string USL) {
            var processId = Convert.ToInt32(Id);
            var quantity = Convert.ToInt32(SL);
            var uquantity = Convert.ToInt32(USL);
            var process = db.Processes.Where(s => s.Id == processId).First();
            process.Done = quantity;
            process.Rest = uquantity;
            db.SaveChanges();
        }
        public void CheckUnDone(string Id, string SL)
        {
            var processId = Convert.ToInt32(Id);
            var quantity = Convert.ToInt32(SL);
            var process = db.Processes.Where(s => s.Id == processId).First();
            process.Rest = quantity;
            db.SaveChanges();
        }
        public void CheckComplete(string Id, string allChecked)
        {
            if (allChecked == "true")
            {
                var processId = Convert.ToInt32(Id);
                var process = db.Processes.Where(s => s.Id == processId).First();
                var order = db.Orders.Where(s => s.Id == process.OrderId).First();
                order.State = 7;
                process.IsDeleted = true;
                db.SaveChanges();
            }
            else {
                var processId = Convert.ToInt32(Id);
                var process = db.Processes.Where(s => s.Id == processId).First();
                var order = db.Orders.Where(s => s.Id == process.OrderId).First();
                order.State = 3;
                process.IsDeleted = false;
                db.SaveChanges();
            }
           
        }

    }

}
