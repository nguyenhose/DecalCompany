using DCompany.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DCompany.Controllers
{
    public class DashBoardController : Controller
    {
        private DecalCompanyEntities1 db = new DecalCompanyEntities1();
        //
        // GET: /DashBoard/

        public ActionResult Index()
        {
            if (this.Session["Account"] == null)
            {
                return Redirect("/Login");
            }
            var Id = Convert.ToInt32(this.Session["ID"]);
            var currentTime = DateTime.Now;
            ViewBag.myTask = db.Tasks.Where(s => s.UserId == Id && s.Invisible==false && s.DateTime.Value.Day == currentTime.Day
                                                                && s.DateTime.Value.Month == currentTime.Month
                                                                && s.DateTime.Value.Year == currentTime.Year ).ToList();
            ViewBag.otherTask = db.Tasks.Where(s => s.UserId != Id && s.Invisible == false && s.DateTime.Value.Day == currentTime.Day
                                                                && s.DateTime.Value.Month == currentTime.Month
                                                                && s.DateTime.Value.Year == currentTime.Year).ToList();
            return View(db.Staffs.Where(s=>s.Invisible==false &&  s.Id != Id && s.StaffName != "admin").ToList());
        }

    }
}
