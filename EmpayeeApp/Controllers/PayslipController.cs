using EmpayeeApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmpayeeApp.Controllers
{
    [Authorize]
    public class PayslipController : Controller
    {
        // GET: Payslip
        public ActionResult Index()
        {
            using (PMSEntities3 emp = new PMSEntities3())

            {
                return View(emp.Calculations.ToList());
            }

        }
        public ActionResult EmpIndex(int id)
        {
            using (PMSEntities3 db = new PMSEntities3())

            {
                var CalculationDetail = db.Calculations.Where(x => x.StaffId == id).First();
                int cid = CalculationDetail.UserId;
                Calculation emp = db.Calculations.Find(cid);
                if (emp == null)
                {
                    return HttpNotFound();
                }
                return View(emp);

            }

        }
    }
}