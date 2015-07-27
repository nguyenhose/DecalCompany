using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DCompany.Models;
using MvcPaging;
using System.Globalization;
using System.Drawing;


namespace DCompany.Controllers
{
    public class PaymentController : Controller
    {
        private DecalCompanyEntities1 db = new DecalCompanyEntities1();
        private const int defaultPageSize = 3;
        private IList<PayMent> listPayment = new List<PayMent>();
        //
        // GET: /Payment/
        public static Image resizeImage(Image imgToResize, System.Drawing.Size size)
        {
            return (Image)(new Bitmap(imgToResize, size));
        }

        public ActionResult Index(string nameCus, string date,int? page)
        {
            if (this.Session["Account"] == null)
            {
                return Redirect("/Login");
            }
            var ids = Convert.ToInt32(this.Session["ID"]);
            ViewBag.AllowEdit = db.Permissions.Any(s => s.UserId == ids && s.ModuleId == 3 && s.Edit == true);
            var listOrder = db.Orders.Where(s => s.Invisible == false && s.State == 7).ToList();
            var listPayment = db.PayMents.Where(s => s.Invisible == false).ToList();
            ViewBag.payment = listPayment;
            ViewBag.order = listOrder;
            IList<PayMent> list = this.listPayment;
            int currentPageIndex = page.HasValue ? page.Value : 1;
         
            if (nameCus != null && date != null)
            {
                var thatdate = Convert.ToDateTime(date);
                var id = Convert.ToInt32(nameCus);
                ViewData["employee_name"] = nameCus;
                list = db.PayMents.Where(s=>s.Invisible==false
                    && s.DatePay > thatdate 
                    && s.CustomerId== id).OrderByDescending(s => s.DatePay).ToPagedList(currentPageIndex, defaultPageSize);
            }
            else { list = db.PayMents.Where(s=>s.Invisible==false).OrderByDescending(s => s.DatePay).ToPagedList(currentPageIndex, defaultPageSize); }

            if (Request.IsAjaxRequest())
                return PartialView("ListPayment", list);
            else
            {
                ViewBag.Payment = list;
                return View();
            }
        }

        //
        // GET: /Payment/Details/5

        public ActionResult Details(int id = 0)
        {
            PayMent payment = db.PayMents.Find(id);
            if (payment == null)
            {
                return HttpNotFound();
            }
            return View(payment);
        }

        //
        // GET: /Payment/Create

        public ActionResult Create()
        {
            if (this.Session["Account"] == null)
            {
                return Redirect("/Login");
            }
            ViewBag.Cus = db.Customers.Select(s => s.Name).ToArray();
            return View();
        }

        [HttpPost]
        public ActionResult Create(PayMent payment, HttpPostedFileBase ImageUrl, string nameCus)
        {
            if (this.Session["Account"] == null)
            {
                return Redirect("/Login");
            }
            if(nameCus==null){
                TempData["a"] = "Xin chọn khách hàng";
                return View();
            }
            if (ModelState.IsValid)
            {
                PayMent newP = new PayMent();
                newP.UserId = Convert.ToInt32(this.Session["ID"]);
                newP.DatePay = DateTime.Now;
                newP.Cash = payment.Cash;
                newP.Invisible = false;
                newP.CustomerId = Convert.ToInt32(nameCus);

                if (ImageUrl != null)
                {
                  
                    ImageUrl.SaveAs(HttpContext.Server.MapPath("~/img/") + ImageUrl.FileName);
                    newP.ImageUrl = ImageUrl.FileName;
                }
                db.PayMents.Add(newP);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
        //
        // GET: /Payment/Search

        public ActionResult Search(string ID, string Day)
        {
            if (this.Session["Account"] == null)
            {
                return Redirect("/Login");
            }
            var day = Convert.ToDateTime(Day);
            var listR = db.Customers.Where(s => (s.Email.Contains(ID) || s.PhoneNumber.Contains(ID) || s.Name.Contains(ID)||s.Code.Contains(ID)) && s.Invisible==false).ToList();
            return PartialView("ListCus",listR);
        }
        //
        // POST: /Payment/Create


        //
        // GET: /Payment/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (this.Session["Account"] == null)
            {
                return Redirect("/Login");
            }
            PayMent payment = db.PayMents.Find(id);
            if (payment == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerId = new SelectList(db.Customers, "Id", "Name", payment.CustomerId);
            ViewBag.UserId = new SelectList(db.Staffs, "Id", "StaffName", payment.UserId);
            return View(payment);
        }

        //
        // POST: /Payment/Edit/5

        [HttpPost]
        public ActionResult Edit(PayMent payment, HttpPostedFileBase fileImage)
        {
            if (this.Session["Account"] == null)
            {
                return Redirect("/Login");
            }
            if (ModelState.IsValid)
            {
                if (fileImage != null)
                {
                    fileImage.SaveAs(HttpContext.Server.MapPath("~/img/") + fileImage.FileName);
                    payment.ImageUrl = fileImage.FileName;
                }
                db.Entry(payment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerId = new SelectList(db.Customers, "Id", "Name", payment.CustomerId);
            ViewBag.UserId = new SelectList(db.Staffs, "Id", "StaffName", payment.UserId);
            return View(payment);
        }

        //
        // GET: /Payment/Delete/5

  

        //
        // POST: /Payment/Delete/5

     
        public void DeleteConfirmed(string Id)
        {
            if (this.Session["Account"] == null)
            {
                 Redirect("/Login");
            }

            PayMent payment = db.PayMents.Find(Convert.ToInt32(Id));
            payment.Invisible = true;
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