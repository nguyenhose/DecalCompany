using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DCompany.Models;
using System.Globalization;
using MvcPaging;
namespace DCompany.Controllers
{
    public class OrderController : Controller
    {
        private DecalCompanyEntities1 db = new DecalCompanyEntities1();
        private const int defaultPageSize = 3;
        private IList<Order> listOrder = new List<Order>();
        //
        // GET: /Order/

        public ActionResult Index(string employee_name, int? page)
        {
            if (this.Session["Account"] == null)
            {
                return Redirect("/Login");
            }
            var id = Convert.ToInt32(this.Session["ID"]);
            ViewBag.AllowEdit = db.Permissions.Any(s => s.UserId == id && s.ModuleId == 2 && s.Edit == true);
            ViewData["employee_name"] = employee_name;
            IList<Order> list = this.listOrder;
            int currentPageIndex = page.HasValue ? page.Value : 1;
             if (string.IsNullOrWhiteSpace(employee_name))
            {
                list = db.Orders.Include(o => o.Customer).Include(o => o.Staff).Where(s => s.Invisible == false).OrderByDescending(s => s.DateInfo).ToPagedList(currentPageIndex, defaultPageSize); 
            }
            else
            {
                list = db.Orders.Include(o => o.Customer).Include(o => o.Staff).Where(s => s.Invisible == false).OrderByDescending(s => s.DateInfo).ToPagedList(currentPageIndex, defaultPageSize); 
            }


             if (Request.IsAjaxRequest())
                 return PartialView("ListProcessOrder", list);
             else
             {
                 ViewBag.listOrder = list;
                 return View();
             }
        }

        //
        // GET: /Order/Details/5

        public ActionResult Details(int id = 0)
        {
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        //
        // GET: /Order/Create

        public ActionResult Create(int id ,string employee_name, int? page)
        {
            if (this.Session["Account"] == null)
            {
                return Redirect("/Login");
            }
            ViewData["employee_name"] = employee_name;
            IList<Order> list = this.listOrder;
            int currentPageIndex = page.HasValue ? page.Value : 1;

            if (string.IsNullOrWhiteSpace(employee_name))
            {
                list = db.Orders.Where(s => s.CustomerId == id && s.Invisible == false).OrderBy(s => s.Deadline).ToPagedList(currentPageIndex, defaultPageSize); 
            }
            else
            {
                list = db.Orders.Where(s => s.CustomerId == id && s.Invisible == false && s.Name.Contains(employee_name)).OrderBy(s => s.Deadline).ToPagedList(currentPageIndex, defaultPageSize); 
            }
            Customer cus = db.Customers.Find(id);
            ViewBag.customer = cus;

            if (Request.IsAjaxRequest())
                return PartialView("ListOrder", list);
            else {
                ViewBag.listOrder = list;
                return View();
            }
              
        }

        //
        // POST: /Order/Create

        [HttpPost]
        public ActionResult Create(Order order, string cusId)
        {
            if (this.Session["Account"] == null)
            {
                return Redirect("/Login");
            }
            var id = Convert.ToInt32(cusId);
            IFormatProvider culture = new CultureInfo("vi-VN", true);
            IList<Order> list = this.listOrder;
            if (ModelState.IsValid) {
                order.CustomerId = id;
                order.DateInfo = DateTime.Now;
                order.CreaterId = Convert.ToInt32(this.Session["ID"]);
                order.State = 1;
                order.Invisible = false;
                if (order.DateInfo < order.Deadline)
                {
                    db.Orders.Add(order);
                    if (order.DateStart != null) {
                        Process schedule = new Process();
                        schedule.OrderId = order.Id;
                        db.Processes.Add(schedule);
                    }
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else { ViewBag.Error = "Hạn giao phải lớn hơn hôm nay!"; }
            }
            list = db.Orders.Where(s => s.CustomerId == id && s.Invisible == false).OrderBy(s => s.Deadline).ToPagedList(1, defaultPageSize);
            ViewBag.listOrder = list;
            Customer cus = db.Customers.Find(id);
            ViewBag.customer = cus;
            return View();
        }
        public void ChangeState(string Id, string OrderId)
        {
            if (this.Session["Account"] == null)
            {
                 Redirect("/Login");
            }
            int Sid = Convert.ToInt32(Id);
            int Oid = Convert.ToInt32(OrderId);
            Order changeOrder = new Order();
            Order newOrder = new Order();
            changeOrder = db.Orders.Find(Oid);
            newOrder.Name = changeOrder.Name;
            newOrder.Note = changeOrder.Note;
            newOrder.TotalCash = changeOrder.TotalCash;
            newOrder.DateInfo = DateTime.Now;
            newOrder.Deadline = changeOrder.Deadline;
            newOrder.State = Sid;
            newOrder.CreaterId = changeOrder.CreaterId;
            newOrder.CustomerId = changeOrder.CustomerId;
            newOrder.Invisible = false;
            changeOrder.Invisible = true;
            db.Orders.Add(newOrder);
         db.SaveChanges();
        }
        public ActionResult Process(string employee_name, int? page)
        {
            if (this.Session["Account"] == null)
            {
                return Redirect("/Login");
            }
            var id = Convert.ToInt32(this.Session["ID"]);
            ViewBag.AllowEdit = db.Permissions.Any(s => s.UserId == id && s.ModuleId == 6 && s.Edit == true);
            ViewData["employee_name"] = employee_name;
            IList<Order> list = this.listOrder;
            int currentPageIndex = page.HasValue ? page.Value : 1;
            if (string.IsNullOrWhiteSpace(employee_name))
            {
                list = db.Orders.Include(o => o.Customer).Include(o => o.Staff).Where(s => s.Invisible == false).OrderByDescending(s => s.DateInfo).ToPagedList(currentPageIndex, defaultPageSize);
            }
            else
            {
                list = db.Orders.Include(o => o.Customer).Include(o => o.Staff).Where(s => s.Invisible == false ).OrderByDescending(s => s.DateInfo).ToPagedList(currentPageIndex, defaultPageSize);
            }


            if (Request.IsAjaxRequest())
                return PartialView("ListProcess", list);
            else
            {
                ViewBag.listOrder = list;
                return View();
            }
         
        }
        public ActionResult Payment() {
            if (this.Session["Account"] == null)
            {
                return Redirect("/Login");
            }
            var order = db.PayMents.ToList();
            return View(order);
        }
        public ActionResult CreatePayment() {
            if (this.Session["Account"] == null)
            {
                return Redirect("/Login");
            }
            return View();
        }
        [HttpPost]
        public ActionResult CreatePayment( PayMent payment)
        {
            if (this.Session["Account"] == null)
            {
                return Redirect("/Login");
            }
            db.PayMents.Add(payment);
            db.SaveChanges();
            return View();
        }
        //
        // GET: /Order/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (this.Session["Account"] == null)
            {
                return Redirect("/Login");
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerId = new SelectList(db.Customers, "Id", "Name", order.CustomerId);
            ViewBag.CreaterId = new SelectList(db.Staffs, "Id", "StaffName", order.CreaterId);
            return View(order);
        }

        //
        // POST: /Order/Edit/5

        [HttpPost]
        public ActionResult Edit(Order order)
        {
            if (this.Session["Account"] == null)
            {

                return Redirect("/Login");
            }
            if (ModelState.IsValid)
            {
                if (order.DateInfo < order.Deadline)
                {
                    db.Entry(order).State = EntityState.Modified;
                    if (order.DateStart != null)
                    {
                        var schedule = db.Processes.Where(s => s.OrderId == order.Id).FirstOrDefault();
                        if (schedule == null)
                        {
                            schedule.OrderId = order.Id;
                            db.Processes.Add(schedule);
                        }
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
                    else { ViewBag.Error = "Hạn giao phải lớn hơn hôm nay!"; }
                }
                ViewBag.CustomerId = new SelectList(db.Customers, "Id", "Name", order.CustomerId);
                ViewBag.CreaterId = new SelectList(db.Staffs, "Id", "StaffName", order.CreaterId);
                return View(order);
            }
        

        //
        // GET: /Order/Delete/5

        //
        // POST: /Order/Delete/5

    
        public void DeleteConfirmed(string Id)
        {
            if (this.Session["Account"] == null)
            {
                 Redirect("/Login");
            }
            Order order = db.Orders.Find(Convert.ToInt32(Id));
            order.Invisible = true;
            db.SaveChanges();
             RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}