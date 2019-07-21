using EmpayeeApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmpayeeApp.Controllers
{
    [Authorize]
    public class TaxPlanController : Controller
    {
        public ActionResult BracketDetails()
        {
            using (PMSEntities3 emp = new PMSEntities3())
            {
                return View(emp.TaxBrackekts.ToList());
            }
        }
        public ActionResult SlabDetails()
        {
            using (PMSEntities3 emp = new PMSEntities3())
            {
                return View(emp.TaxSlabs.ToList());
            }
        }
        // GET: TaxPlan
        public ActionResult EditTaxBracket(int id)
        {
            using (PMSEntities3 emp = new PMSEntities3())
            {
                return View(emp.TaxBrackekts.Where(x => x.Bracket_Id == id).FirstOrDefault());
            }
        }

        // POST: EmployeeInfo/Edit/5
        [HttpPost]
        public ActionResult EditTaxBracket(TaxBrackekt bracket)
        {
            try
            {
                using (PMSEntities3 emp = new PMSEntities3())
                {
                    emp.Entry(bracket).State = EntityState.Modified;
                    emp.SaveChanges();
                }
                // TODO: Add update logic here

                return RedirectToAction("Calculate", "Calculations");
            }
            catch
            {
                return View();
            }
        }
        public ActionResult EditTaxSlab(int id)
        {
            using (PMSEntities3 emp = new PMSEntities3())
            {
                return View(emp.TaxSlabs.Where(x => x.Slab_Id == id).FirstOrDefault());
            }
        }

        // POST: EmployeeInfo/Edit/5
        [HttpPost]
        public ActionResult EditTaxSlab(TaxSlab slab)
        {
            try
            {
                using (PMSEntities3 emp = new PMSEntities3())
                {
                    emp.Entry(slab).State = EntityState.Modified;
                    emp.SaveChanges();
                }
                // TODO: Add update logic here

                return RedirectToAction("Calculate", "Calculations");
            }
            catch
            {
                return View();
            }
        }

    }
}