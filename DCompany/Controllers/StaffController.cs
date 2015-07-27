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
    public class StaffController : Controller
    {
        private DecalCompanyEntities1 db = new DecalCompanyEntities1();
        private const int defaultPageSize = 3;
        private IList<Staff> listStaff = new List<Staff>();
        //
        // GET: /Staff/

        public ActionResult Index(string employee_name, int? page)
        {
            var id = Convert.ToInt32(this.Session["ID"]);
          
            if (this.Session["Account"] == null)
            {
                return Redirect("/Login");
            }
            else if (db.Permissions.Any(s => s.UserId == id && s.ModuleId == 4 && s.Staff.Invisible == false))
            {
                ViewBag.AllowEdit = db.Permissions.Any(s => s.UserId == id && s.ModuleId == 4 && s.Edit == true);
                ViewData["employee_name"] = employee_name;
                IList<Staff> list = this.listStaff;
                int currentPageIndex = page.HasValue ? page.Value : 1;
                if (string.IsNullOrWhiteSpace(employee_name))
                {
                    list = db.Staffs.Where(s => s.Invisible == false && s.StaffName!= "admin").OrderByDescending(s => s.StaffName).ToPagedList(currentPageIndex, defaultPageSize);
                }
                else
                {
                    list = db.Staffs.Where(s => s.Invisible == false && s.StaffName != "admin").OrderByDescending(s => s.StaffName).ToPagedList(currentPageIndex, defaultPageSize);
                }
                if (Request.IsAjaxRequest())
                {
                    var currentTime = DateTime.Now;
                    ViewBag.listStaff = db.Tasks.Where(s => s.Staff.Invisible == false && s.Invisible == false 
                                                                                      && s.DateTime.Value.Day == currentTime.Day
                                                                                      && s.DateTime.Value.Month == currentTime.Month
                                                                                      && s.DateTime.Value.Year == currentTime.Year).ToList();
                    return PartialView("ListStaff", list);
                }
                  
                else
                {
                    var currentTime = DateTime.Now;
                    ViewBag.oldStaff = list;
                    ViewBag.listStaff = db.Tasks.Where(s => s.Staff.Invisible == false && s.Invisible==false
                                                                                       &&  s.DateTime.Value.Day == currentTime.Day
                                                                                       && s.DateTime.Value.Month == currentTime.Month
                                                                                       && s.DateTime.Value.Year == currentTime.Year ).ToList();
                    return View();
                }
            }
            return Redirect("/Login");
        }

        //
        // GET: /Staff/Details/5

        public ActionResult Details(int id = 0)
        {
            Staff staff = db.Staffs.Find(id);
            if (staff == null)
            {
                return HttpNotFound();
            }
            return View(staff);
        }

        //
        // GET: /Staff/Create

        public ActionResult Create()
        {
            if (this.Session["Account"] == null)
            {
                return Redirect("/Login");
            }
           
            var id = Convert.ToInt32(this.Session["ID"]);
             if (this.Session["Account"] == null)
            {
                return Redirect("/Login");
            }
             else if (db.Permissions.Any(s => s.UserId == id && s.ModuleId == 4))
             {
                 return View();
             }
             return Redirect("/Login");
        }

        //
        // POST: /Staff/Create

        [HttpPost]
        public ActionResult Create(Staff staff, string customer, string staffs, string process, string order, string store, string finance
            , string customer_edit, string staffs_edit, string process_edit, string order_edit, string store_edit, string finance_edit, HttpPostedFileBase fileImage){
        
            if (this.Session["Account"] == null)
            {
                return Redirect("/Login");
            }
            if (ModelState.IsValid)
            {
                 Permission per = new Permission();
                 var time = DateTime.Now;
                 staff.CreaterId=Convert.ToInt32(this.Session["ID"]);
                 staff.Invisible = false;
                 staff.Time = time;
                 if (fileImage != null)
                 {
                     fileImage.SaveAs(HttpContext.Server.MapPath("~/img/") + fileImage.FileName);
                     staff.ImageURL = fileImage.FileName;
                 }
                db.Staffs.Add(staff);
                db.SaveChanges();
                Staff newstaff = db.Staffs.Where(s => s.StaffName == staff.StaffName).FirstOrDefault();
              
                if (customer == "on")
                {
                    per.ModuleId = 1;
                    per.UserId = newstaff.Id;
                    per.Edit = false;
                    if (customer_edit == "on") { per.Edit = true; }
                    db.Permissions.Add(per);
                    db.SaveChanges();
                }
                if (staffs == "on")
                {
                    per.ModuleId = 4;
                    per.UserId = newstaff.Id;
                    per.Edit = false;
                    if (staffs_edit == "on") { per.Edit = true; }
                    db.Permissions.Add(per);
                    db.SaveChanges();
                }
                if (process == "on")
                {
                    per.ModuleId = 6;
                    per.Edit = false;
                    per.UserId = newstaff.Id;
                    if (process_edit == "on") { per.Edit = true; }
                    db.Permissions.Add(per);
                    db.SaveChanges();
                }
                if (order == "on")
                {
                    per.ModuleId = 2;
                    per.UserId = newstaff.Id;
                    per.Edit = false;
                    if (store_edit == "on") { per.Edit = true; }
                    db.Permissions.Add(per);
                    db.SaveChanges();
                }
                if (store == "on")
                {
                    per.ModuleId = 5;
                    per.UserId = newstaff.Id;
                    per.Edit = false;
                    if (store_edit == "on") { per.Edit = true; }
                    db.Permissions.Add(per);
                    db.SaveChanges();
                }
                if (finance == "on")
                {
                    per.ModuleId = 3;
                    per.UserId = newstaff.Id;
                    per.Edit = false;
                    if (finance_edit == "on") { per.Edit = true; }
                    db.Permissions.Add(per);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            return View(staff);
        }

        //
        // GET: /Staff/Edit/5

        public ActionResult Edit(int id = 0)
        {
            var cur_id = Convert.ToInt32(this.Session["ID"]);
            if (this.Session["Account"] == null)
            {
                return Redirect("/Login");

            }
            else if (db.Permissions.Any(s => s.UserId ==cur_id  && s.ModuleId == 4 && s.Edit==true)||(cur_id==id)){
                ViewBag.AllowEdit = db.Permissions.Any(s => s.UserId == cur_id && s.ModuleId == 4 && s.Edit == true);
                Staff staff = db.Staffs.Find(id);
                if (staff.StaffName == "admin" && Convert.ToInt32(this.Session["ID"])!=1) 
                {
                    return Redirect("/Login");
                }
                if (staff == null)
                {
                    return HttpNotFound();
                }
                ViewBag.ListRole = db.Permissions.Where(s => s.UserId == staff.Id).ToList();
                return View(staff);
            }
            return Redirect("/Login");
          
        }

        //
        // POST: /Staff/Edit/5

        [HttpPost]
        public ActionResult Edit(Staff staff, string customer, string staffs, string process, string order, string store,string finance,
            string customer_edit, string staffs_edit, string process_edit, string order_edit, string store_edit, string finance_edit, HttpPostedFileBase fileImage)
        {
            if (this.Session["Account"] == null)
            {
                return Redirect("/Login");
            }
            Permission per = new Permission();
            
            ViewBag.ListRole = db.Permissions.Where(s => s.UserId == staff.Id).ToList();
            if (ModelState.IsValid)
            {
                staff.Invisible = false;
                if (fileImage != null)
                {
                    fileImage.SaveAs(HttpContext.Server.MapPath("~/img/") + fileImage.FileName);
                    staff.ImageURL = fileImage.FileName;
                }
                db.Entry(staff).State = EntityState.Modified;
                db.SaveChanges();
                var listPer = db.Permissions.Where(s => s.UserId == staff.Id).ToList();
                for (int i = 0; i < listPer.Count; i++) {
                    db.Permissions.Remove(listPer[i]);
                }
             
              if (customer == "on")
                {
                    per.Edit = false;
                  per.ModuleId = 1;
                    per.UserId = staff.Id;
                    if (customer_edit == "on") { per.Edit = true; }
                    db.Permissions.Add(per);
                    db.SaveChanges();
                }
                if (staffs == "on")
                {
                    per.Edit = false;
                    per.ModuleId = 4;
                    per.UserId = staff.Id;
                    if (staffs_edit == "on") { per.Edit = true; }
                    db.Permissions.Add(per);
                    db.SaveChanges();
                }
                if (process == "on")
                {
                    per.Edit = false;
                    per.ModuleId = 6;
                    per.UserId = staff.Id;
                    if (process_edit == "on") { per.Edit = true; }
                    db.Permissions.Add(per);
                    db.SaveChanges();
                }
                if (order == "on")
                {

                    per.Edit = false;
                    per.ModuleId = 2;
                    per.UserId = staff.Id;
                    if (order_edit == "on") { per.Edit = true; }
                    db.Permissions.Add(per);
                    db.SaveChanges();
                }
                if (store == "on")
                {
                    per.Edit = false;
                    per.ModuleId = 5;
                    per.UserId = staff.Id;
                    if (store_edit == "on") { per.Edit = true; }
                    db.Permissions.Add(per);
                    db.SaveChanges();
                }
                if (finance == "on")
                {
                    per.Edit = false;
                    per.ModuleId = 3;
                    per.UserId = staff.Id;
                   
                    if (finance_edit == "on") { per.Edit = true; }
                    db.Permissions.Add(per);
                    db.SaveChanges();
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(staff);
        }

        //
        // GET: /Customer/Delete/5


        //
        // POST: /Staff/Delete/5

     
        public ActionResult DeleteConfirmed(string Id)
        {
            if (this.Session["Account"] == null)
            {
                return Redirect("/Login");
            }
            Staff staff = db.Staffs.Find(Convert.ToInt32(Id));
            staff.Invisible = true;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}