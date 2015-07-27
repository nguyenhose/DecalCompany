using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DCompany.Models;
using MvcPaging;
namespace DCompany.Controllers
{
    public class CustomerController : Controller
    {
        private DecalCompanyEntities1 db = new DecalCompanyEntities1();
        private const int defaultPageSize = 3;
        private IList<Customer> listCus = new List<Customer>();
        //
        // GET: /Customer/

        private static string RandomString(int Size)
        {
            Random random = new Random((int)DateTime.Now.Ticks);
            string input = "abcdefghijklmnopqrstuvwxyz0123456789";
            var chars = Enumerable.Range(0, Size)
                                   .Select(x => input[random.Next(0, input.Length)]);
            return new string(chars.ToArray());
        }
        public bool IsExist(string code) {
            if (!db.Customers.Any(s => s.Code == code)) { return false; }
            return IsExist(code);
        }
        public ActionResult Index(string employee_name, int? page)
        {
            var id = Convert.ToInt32(this.Session["ID"]);
            if (this.Session["Account"] == null)
            {
                return Redirect("/Login");
            }
            else if (db.Permissions.Any(s => s.UserId == id && s.ModuleId == 1))
            {
                ViewBag.AllowEdit = db.Permissions.Any(s => s.UserId == id && s.ModuleId == 1 && s.Edit == true);
                ViewBag.AllowAddOrder = db.Permissions.Any(s => s.UserId == id  && s.ModuleId == 2 && s.Edit == true);
                var customers = db.Customers.Where(s => s.Invisible == false);
                var listOrder = db.Orders.Where(s => s.Invisible == false && s.State == 7).ToList();
                var listPayment = db.PayMents.Where(s => s.Invisible == false).ToList();

                ViewData["employee_name"] = employee_name;
                IList<Customer> list = this.listCus;
                int currentPageIndex = page.HasValue ? page.Value : 1;

                ViewBag.payment = listPayment;
                ViewBag.order = listOrder;
                ViewBag.customer = db.Customers;

                if (string.IsNullOrWhiteSpace(employee_name))
                {
                    list = db.Customers.Where(s => s.Invisible == false).OrderBy(s => s.Name).ToPagedList(currentPageIndex, defaultPageSize);
                }
                else
                {
                    list = db.Customers.Where(s => s.Invisible == false 
                        && (s.Name.Contains(employee_name) 
                        || s.PhoneNumber.Contains(employee_name) 
                        || s.Email.Contains(employee_name) 
                        || s.Code.Contains(employee_name))).OrderBy(s => s.Name).ToPagedList(currentPageIndex, defaultPageSize);
                    
                }
                if (Request.IsAjaxRequest())
                {
                   
                    return PartialView("ListCus", list);
                }
                else
                {
                    ViewBag.listCus = list;
                    return View();
                }

            }
            return Redirect("/Login");
        }
        
        //
        // GET: /Customer/Details/5

        public ActionResult Details(int id = 0)
        {
            if (this.Session["Account"] == null)
            {
                return Redirect("/Login");
            }
            Customer customer = db.Customers.Find(id);
            var Order = db.Orders.Where(s => s.Invisible == false && s.State == 7 && s.CustomerId == id);
            var listOrder = Order.OrderByDescending(s=>s.DateInfo).ToList();
            var CashOrder = Order.Sum(s => s.TotalCash);
            var listPayment = db.PayMents.Where(s => s.Invisible == false && s.CustomerId == id).ToList().Sum(s => s.Cash);
            if (CashOrder != null && listOrder != null) { ViewBag.loan = CashOrder - listPayment; }
            else { ViewBag.loan = 0; }
            ViewBag.Order = listOrder;
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        //
        // GET: /Customer/Create

        public ActionResult Create()
        {
            var id = Convert.ToInt32(this.Session["ID"]);
            if (this.Session["Account"] == null)
            {
                return Redirect("/Login");
            }
            else if (db.Permissions.Any(s => s.UserId == id && s.ModuleId == 1))
            {
                return View();
            }
            return Redirect("/Login");

        }

        //
        // POST: /Customer/Create

        [HttpPost]
        public ActionResult Create(Customer customer)
        {
            if (this.Session["Account"] == null)
            {
                return Redirect("/Login");

            }
            var id = Convert.ToInt32(this.Session["ID"]);
            if (ModelState.IsValid)
            {
                var code = RandomString(5);

                if (IsExist(code)==false) {
                    customer.CreatorId = id;
                    customer.Code = RandomString(5);
                    customer.DateCreater = DateTime.Now;
                    customer.Invisible = false;
                    db.Customers.Add(customer);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
               
               
            }

            return View(customer);
        }

        //
        // GET: /Customer/Edit/5

        public ActionResult Edit(string id)
        {
            var Sid = Convert.ToInt32(this.Session["ID"]);
            if (this.Session["Account"] == null || db.Permissions.Any(s => s.UserId == Sid && s.ModuleId == 1 && s.Edit ==false))
            {
                return Redirect("/Login");
            }
            Customer customer = db.Customers.Find(Convert.ToInt32(id));
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        //
        // POST: /Customer/Edit/5

        [HttpPost]
        public ActionResult Edit(Customer customer)
        {
            if (this.Session["Account"] == null)
            {
                return Redirect("/Login");
            }
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customer);
        }



        //
        // POST: /Customer/Delete/5

  
        public void DeleteConfirmed(string Id)
        {
            var Sid = Convert.ToInt32(this.Session["ID"]);
            if (this.Session["Account"] == null || db.Permissions.Any(s => s.UserId == Sid && s.ModuleId == 1 && s.Edit == false))
            {
              Redirect("/Login");
            }
                Customer customer = db.Customers.Find(Convert.ToInt32(Id));
                if (ModelState.IsValid) {
                    customer.Invisible = true;
                    db.SaveChanges();
            }
             RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}