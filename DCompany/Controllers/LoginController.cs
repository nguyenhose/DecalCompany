using DCompany.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DCompany.Controllers
{
    public class LoginController : Controller
    {
        private DecalCompanyEntities1 db = new DecalCompanyEntities1();
        //
        // GET: /Login/

        public ActionResult Index()
        {

            return View();
        }
        [HttpPost]
        public ActionResult Index(Staff staffInfo)
        {
           
                if (db.Staffs.Any(s => s.StaffName == staffInfo.StaffName && s.Password == staffInfo.Password && s.Invisible==false))
                {
                   Staff staff = db.Staffs.Where(s=>s.StaffName==staffInfo.StaffName).FirstOrDefault();
                   var listPer1 = db.Permissions.Where(s => s.UserId == staff.Id).ToList();
                   var listPer2 = db.Permissions.Where(s => s.UserId == staff.Id && s.Edit==true).ToList();
                   this.Session["Account"] = staff.StaffName;
                   this.Session["ID"] = staff.Id;
                   this.Session["Per1"] = listPer1;
                   this.Session["Per2"] = listPer2;
                   return RedirectToAction("", "DashBoard") ;
            }
            return View();
        }

        public ActionResult Logout() {
            this.Session["Account"] = null;
            this.Session["ID"] = null;
            return RedirectToAction("", "Login");
        }
    }
}
