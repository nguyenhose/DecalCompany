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
    public class StoreController : Controller
    {
        private DecalCompanyEntities1 db = new DecalCompanyEntities1();
        private const int defaultPageSize = 3;
        private IList<Store> listStore = new List<Store>();
        //
        // GET: /Store/

        public ActionResult Index(string employee_name, int? page)
        {
            if (this.Session["Account"] == null)
            {
                return Redirect("/Login");
            }
            var id = Convert.ToInt32(this.Session["ID"]);
            ViewBag.AllowEdit = db.Permissions.Any(s => s.UserId == id && s.ModuleId == 5 && s.Edit == true);
            ViewData["employee_name"] = employee_name;
            IList<Store> list = this.listStore;

            int currentPageIndex = page.HasValue ? page.Value : 1;
            if (string.IsNullOrWhiteSpace(employee_name))
            {
                list = db.Stores.Where(s => s.Invisible == false).OrderByDescending(s => s.Time).ToPagedList(currentPageIndex, defaultPageSize);
            }
            else
            {
                list = db.Stores.Where(s => s.Invisible == false).OrderByDescending(s => s.Time).ToPagedList(currentPageIndex, defaultPageSize);
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("ListStore", list);
            }
            else
            {
                ViewBag.store = list;
                return View();
            }
        }

        //
        // GET: /Store/Details/5

        public ActionResult Details(int id = 0)
        {
            Store store = db.Stores.Find(id);
            if (store == null)
            {
                return HttpNotFound();
            }
            return View(store);
        }
        public void Export(string Export, string ID,  string VL) {
            if (this.Session["Account"] == null || this.Session["ID"] == null)
            {
                 Redirect("/Login");
            }
            var store = db.Stores.Find(Convert.ToInt32(ID));
            var rest = store.CurrentQ - Convert.ToInt32(Export);
            if (rest >= 0) {
                var newStore = new Store();
                newStore.ProductName = store.ProductName;
                newStore.ProductPrice = store.ProductPrice;
                newStore.SupplierName = store.SupplierName;
                newStore.CurrentQ = rest;
                newStore.InputQ = store.CurrentQ;
                newStore.OutputQ = Convert.ToInt32(Export);
                newStore.Invisible = false;
                newStore.Note = store.Note;
                newStore.Time = DateTime.Now;
                newStore.CreatorId = Convert.ToInt32(this.Session["ID"]);
                newStore.Width = store.Width;
                newStore.Height = store.Height;
                newStore.Square = store.Square;
                store.Invisible = true;
                store.OutputQ = 0;
                db.Stores.Add(newStore);
                db.SaveChanges();
            }
        }
        public void ExportS(string Export, string ID, string ProcessId)
        {
            if (this.Session["Account"] == null)
            {
                Redirect("/Login");
            }
            var store = db.Stores.Find(Convert.ToInt32(ID));
            var rest = store.CurrentQ - Convert.ToInt32(Export);
            if (rest >= 0)
            {
                var newStore = new Store();
                newStore.ProductName = store.ProductName;
                newStore.ProductPrice = store.ProductPrice;
                newStore.SupplierName = store.SupplierName;
                newStore.CurrentQ = rest;
                newStore.InputQ = store.CurrentQ;
                newStore.OutputQ = Convert.ToInt32(Export);
                newStore.Invisible = false;
                newStore.Note = store.Note;
                newStore.Time = DateTime.Now;
                newStore.CreatorId = Convert.ToInt32(this.Session["ID"]);
                newStore.Width = store.Width;
                newStore.Height = store.Height;
                newStore.Square = store.Square;
                store.Invisible = true;
                store.OutputQ = 0;
                db.Stores.Add(newStore);
                ///them moi
                var PID = Convert.ToInt32(ProcessId);
                var newProcess = db.Processes.Where(s => s.Id == PID).FirstOrDefault();
                newProcess.OutPutQ = Convert.ToInt32(Export);
                newProcess.Height = store.Height;
                newProcess.Width = store.Width;
                newProcess.Square = store.Square;
                ///
                db.SaveChanges();

            }
        }
        //
        // GET: /Store/Create

        public ActionResult Create()
        {
            if (this.Session["Account"] == null)
            {
                return Redirect("/Login");
            }
            var stores = db.Stores.Where(s => s.Invisible == false);
            ViewBag.list = stores.ToList();
            return View();
        }

        //
        // POST: /Store/Create

        [HttpPost]
        public ActionResult Create(Store store)
        {
            if (this.Session["Account"] == null || this.Session["ID"]==null)
            {
                Redirect("/Login");
            }
            var stores = db.Stores.Where(s => s.Invisible == false);
            ViewBag.list = stores.ToList();
            var b = store.ProductPrice;
            var c = Convert.ToDouble(b);
            store.ProductPrice = c;
            if (ModelState.IsValid)
            {
                if (store.Note.Contains("Tờ")) {
                    if (store.Width == null || store.Height == null || store.Square == null) { return View(); }
                }
                store.State = 0;
                store.Invisible = false;
                store.CreatorId = Convert.ToInt32(this.Session["ID"]);
            
                db.Stores.Add(store);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

          
            return View(store);
        }
        public void Import(string Name, string Quantity, string Note, string Date,string Supplier, string Price ) {
            if (this.Session["Account"] == null || this.Session["ID"] == null)
            {
                 Redirect("/Login");
            }
            Store store = new Store();
            store.ProductName = Name;
            store.CurrentQ = Convert.ToInt32(Quantity);
            store.Note = Note;
            store.ProductPrice = Convert.ToDouble(Price);
            store.Time = DateTime.Now;
            store.SupplierName = Supplier;
            store.State = 0;
            store.Invisible = false;
            store.CreatorId = Convert.ToInt32(this.Session["ID"]);
            
            db.Stores.Add(store);
            db.SaveChanges();
        }
        //
        // GET: /Store/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (this.Session["Account"] == null)
            {
                return Redirect("/Login");
            }
            Store store = db.Stores.Find(id);
            if (store == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerId = new SelectList(db.Customers, "Id", "Name", store.CustomerId);
            ViewBag.CreatorId = new SelectList(db.Staffs, "Id", "StaffName", store.CreatorId);
            return View(store);
        }

        //
        // POST: /Store/Edit/5

        [HttpPost]
        public ActionResult Edit(Store store)
        {
            if (this.Session["Account"] == null)
            {
                return Redirect("/Login");
            }
            if (ModelState.IsValid)
            {
                db.Entry(store).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerId = new SelectList(db.Customers, "Id", "Name", store.CustomerId);
            ViewBag.CreatorId = new SelectList(db.Staffs, "Id", "StaffName", store.CreatorId);
            return View(store);
        }

        //
        // GET: /Store/Delete/5

        //
        // POST: /Store/Delete/5


    
        public ActionResult DeleteConfirmed(string Id)
        {
            if (this.Session["Account"] == null)
            {
                return Redirect("/Login");
            }
            Store store = db.Stores.Find(Convert.ToInt32(Id));
            store.Invisible = true;
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