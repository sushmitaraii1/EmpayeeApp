using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using EmpayeeApp.Controllers;
using EmpayeeApp.Models;

namespace EmpayeeApp.Controllers
{

    [AllowAnonymous]
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Staff model)
        {
            PMSEntities3 db = new PMSEntities3();
            using (var context = new PMSEntities3())
            {

                bool isValid = context.Staffs.Any(x => x.UserName == model.UserName && x.Password == model.Password);
                var userDetaills = db.Staffs.Where(x => x.UserName == model.UserName && x.Password == model.Password).FirstOrDefault();
                if (isValid)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, false);
                    Session["UserId"] = userDetaills.Id;
                    if (Convert.ToInt32(Session["UserId"]) == 1)
                        return RedirectToAction("AdminIndex", "Staffs");
                    else
                        return RedirectToAction("UserIndex", "Staffs");



                    //    return RedirectToAction("Index", "workers");

                }
                ModelState.AddModelError("", "Invalid username and password");
                return View();
            }

        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}