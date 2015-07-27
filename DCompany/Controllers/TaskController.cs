using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DCompany.Models;
using System.Globalization;

namespace DCompany.Controllers
{
    public class TaskController : Controller
    {
        private DecalCompanyEntities1 db = new DecalCompanyEntities1();

        //
        // GET: /Task/

        public ActionResult Index()
        {
            if (this.Session["Account"] == null) { 
            return Redirect("/Login");
            }
            var StaffName = this.Session["Account"].ToString();
            var tasks = db.Tasks.Where(s=>s.Staff.StaffName == StaffName && s.Staff.Invisible==false && s.Invisible==false );
            if (tasks != null) { return View(tasks.ToList().OrderByDescending(s=>s.DateTime)); }
            return View();
            
        }

        //
        // GET: /Task/Details/5

        public ActionResult Details(int id = 0)
        {
            Task task = db.Tasks.Find(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            return View(task);
        }
        
        //
        // GET: /Task/Create

        public void Create(string Date,string Work, string TimeComplete)
        {
            if (this.Session["Account"] == null)
            {
                 Redirect("/Login");
            }
            IFormatProvider culture = new CultureInfo("vi-VN", true);
            Task task = new Task();
            DateTime date = Convert.ToDateTime(Date,culture);
            task.DateTime = date;
            task.Task1 = Work;
            task.Invisible = false;
            task.TimeComplete = Convert.ToInt32(TimeComplete);
            task.UserId = Convert.ToInt32(this.Session["ID"].ToString());
            task.State = 0;
            db.Tasks.Add(task);
            db.SaveChanges();
        }


        //
        // GET: /Task/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Task task = db.Tasks.Find(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.Staffs, "Id", "StaffName", task.UserId);
            return View(task);
        }

        //
        // POST: /Task/Edit/5
        public void EditNew(string Date, string Work, string TimeComplete, string id)
        {
            if (this.Session["Account"] == null)
            {
                 Redirect("/Login");
            }
            Task oldtask = db.Tasks.Find(Convert.ToInt32(id));
            oldtask.Invisible = true;
            IFormatProvider culture = new CultureInfo("vi-VN", true);
            Task task = new Task();
            DateTime date = Convert.ToDateTime(Date, culture);
            task.DateTime = DateTime.Now;
            task.Task1 = Work;
            task.TimeComplete = Convert.ToInt32(TimeComplete);
            task.UserId = Convert.ToInt32(this.Session["ID"].ToString());
            task.State = 0;
            task.Invisible = false;
            db.Tasks.Add(task);
            db.SaveChanges();
        }

        //
        // GET: /Task/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Task task = db.Tasks.Find(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            return View(task);
        }

        //
        // POST: /Task/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Task task = db.Tasks.Find(id);
            db.Tasks.Remove(task);
            db.SaveChanges();
            return RedirectToAction("/Index");
        }
        public int ConfirmTask(string list, string allList)
        {
            if (this.Session["Account"] == null)
            {
                 Redirect("/Login");
            }

            string[] all = allList.Split();
            string[] listId = list.Split();
            for (int j = 0; j < all.Length-1; j++)
            {

                int taskid = Convert.ToInt32(all[j].ToString());
                var model = db.Tasks.Find(taskid);
                model.State = 0;
            }
            for (int i = 0; i < listId.Length-1; i++)
            {
                int taskid = Convert.ToInt32(listId[i].ToString());
                var model = db.Tasks.Find(taskid);
                model.State = 1;
            }
            db.SaveChanges();
            return listId.Length-1;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}