using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EmpayeeApp.Models;

namespace EmpayeeApp.Controllers
{
    [Authorize]
    public class CalculationsController : Controller
    {
        private PMSEntities3 db = new PMSEntities3();

        // GET: Calculations
        public ActionResult BeforeIndex()
        {
            var calculations = db.Calculations.Include(c => c.Staff);
            return View(calculations.ToList());
        }
        
        // GET: Calculations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Calculation calculation = db.Calculations.Find(id);
            if (calculation == null)
            {
                return HttpNotFound();
            }
            return View(calculation);
        }

        // GET: Calculations/Create
        public ActionResult Create()
        {
            ViewBag.StaffId = new SelectList(db.Staffs, "Id", "UserName");
            return View();
        }

        // POST: Calculations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserId,Employee_Name,Married,Monthly_Salary,CIT,PF,Taxable_Amount,Tax,Salary,Month,Days,Leave,StaffId,Allowance,Bonus")] Calculation calculation)
        {
            using (PMSEntities3 db = new PMSEntities3())
            {

                db.SaveChanges();
                if (ModelState.IsValid)
                {

                    db.Calculations.Add(calculation);
                    db.SaveChanges();
                    return RedirectToAction("Calculate");
                }

                ViewBag.StaffId = new SelectList(db.Staffs, "Id", "UserName", calculation.StaffId);
                return View(calculation);
            }
        }

        // GET: Calculations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Calculation calculation = db.Calculations.Find(id);
            if (calculation == null)
            {
                return HttpNotFound();
            }
            ViewBag.StaffId = new SelectList(db.Staffs, "Id", "UserName", calculation.StaffId);
            return View(calculation);
        }

        // POST: Calculations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,Employee_Name,Married,Monthly_Salary,CIT,PF,Taxable_Amount,Tax,Salary,Month,Days,Leave,StaffId,Allowance,Bonus")] Calculation calculation)
        {
            PMSEntities3 newstate = new PMSEntities3();
            if (ModelState.IsValid)
            {
                
                var sname = newstate.Staffs.ToList();
                var calc = newstate.Calculations.ToList();
                foreach (var item in calc)
                {
                    foreach (var sitem in sname)
                    {
                        if (item.UserId == sitem.Id)
                        { item.Employee_Name = sitem.Full_Name; }
                    }
                }
                db.Entry(calculation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Calculate");
            }
            ViewBag.StaffId = new SelectList(newstate.Staffs, "Id", "UserName", calculation.StaffId);
            return View(calculation);
        }

        // GET: Calculations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Calculation calculation = db.Calculations.Find(id);
            if (calculation == null)
            {
                return HttpNotFound();
            }
            return View(calculation);
        }

        // POST: Calculations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Calculation calculation = db.Calculations.Find(id);
            db.Calculations.Remove(calculation);
            db.SaveChanges();
            return RedirectToAction("Calculate");
        }

        // GET: Calculation
        public ActionResult Calculate([Bind(Include = "UserId,Employee_Name,Married,Monthly_Salary,CIT,PF,Taxable_Amount,Tax,Salary,Month,Days,Leave,StaffId,Allowance,Bonus")]Calculation calculation)
        {
            decimal GrossSalary;
            decimal CIT;

            /*using (PayrollEntities entities = new PayrollEntities())
            {*/
            var sname = db.Staffs.ToList();
            var calc = db.Calculations.ToList();
            var slab1 = db.TaxSlabs.Where(x => x.Married == true).ToList();
            var slab2 = db.TaxSlabs.Where(x => x.Married == false).ToList();
            var bracket = db.TaxBrackekts.ToList();
            foreach (var item in calc)
            {
                item.Days = 31-item.Leave;
                
                foreach (var bracketvalue in bracket)
                {


                    GrossSalary = (decimal)(item.Monthly_Salary * 12) + item.Allowance + item.Bonus + (decimal)0.1 * (item.Monthly_Salary * 12);
                    if (item.CIT == true)
                    {
                        CIT = (decimal)0.23 * (item.Monthly_Salary * 12);
                        if (CIT > 300000)
                        { CIT = 300000; }
                        item.Taxable_Amount = GrossSalary - (decimal)0.2 * (item.Monthly_Salary * 12) - CIT;
                    }
                    else
                    {
                        item.Taxable_Amount = GrossSalary - (decimal)0.2 * (item.Monthly_Salary * 12);
                    }
                    foreach (var slabvalue in slab2)
                    {

                        if (item.Married == false && ((item.Taxable_Amount) <= slabvalue.First_Slab))
                        {
                            item.Tax = ((decimal)bracketvalue.First_Bracket * (item.Taxable_Amount));
                        }
                        else if (item.Married == false && ((item.Taxable_Amount) > slabvalue.First_Slab) && ((item.Taxable_Amount) <= slabvalue.Second_Slab))
                        {
                            item.Tax = ((decimal)bracketvalue.First_Bracket * slabvalue.First_Slab + (decimal)bracketvalue.Second_Bracket * ((item.Taxable_Amount) - slabvalue.First_Slab));
                        }
                        else if (item.Married == false && ((item.Taxable_Amount) > slabvalue.Second_Slab) && ((item.Taxable_Amount) <= slabvalue.Third_Slab))
                        {
                            item.Tax = ((decimal)bracketvalue.First_Bracket * slabvalue.First_Slab + (decimal)bracketvalue.Second_Bracket * slabvalue.Second_Slab + (decimal)bracketvalue.Third_Bracket * ((item.Taxable_Amount) - slabvalue.Second_Slab));
                        }
                        else if (item.Married == false && ((item.Taxable_Amount) > slabvalue.Third_Slab) && ((item.Taxable_Amount) <= slabvalue.Fourth_Slab))
                        {
                            item.Tax = ((decimal)bracketvalue.First_Bracket * slabvalue.First_Slab + (decimal)bracketvalue.Second_Bracket * slabvalue.Second_Slab + (decimal)bracketvalue.Third_Bracket * slabvalue.Third_Slab + (decimal)bracketvalue.Fourth_Bracket * ((item.Taxable_Amount) - slabvalue.Third_Slab));
                        }
                        else if (item.Married == false )
                        {
                            item.Tax = ((decimal)bracketvalue.First_Bracket * slabvalue.First_Slab + (decimal)bracketvalue.Second_Bracket * slabvalue.Second_Slab + (decimal)bracketvalue.Third_Bracket * slabvalue.Third_Slab + (decimal)bracketvalue.Fourth_Bracket *( slabvalue.Fourth_Slab-slabvalue.Third_Slab) + (decimal)bracketvalue.Fifth_Bracket * ((item.Taxable_Amount) - slabvalue.Fourth_Slab));
                        }
                        
                    }
                    foreach (var slabvalue1 in slab1)
                    {
                        if (item.Married == true && ((item.Taxable_Amount) <= slabvalue1.First_Slab))
                        {
                            item.Tax = ((decimal)bracketvalue.First_Bracket * (item.Taxable_Amount));
                        }
                        else if (item.Married == true && ((item.Taxable_Amount) > slabvalue1.First_Slab) && ((item.Taxable_Amount) <= slabvalue1.Second_Slab))
                        {
                            item.Tax = ((decimal)bracketvalue.First_Bracket * slabvalue1.First_Slab + (decimal)bracketvalue.Second_Bracket * ((item.Taxable_Amount) - slabvalue1.First_Slab));
                        }
                        else if (item.Married == true && ((item.Taxable_Amount) > slabvalue1.Second_Slab) && ((item.Taxable_Amount) <= slabvalue1.Third_Slab))
                        {
                            item.Tax = ((decimal)bracketvalue.First_Bracket * slabvalue1.First_Slab + (decimal)bracketvalue.Second_Bracket * slabvalue1.Second_Slab + (decimal)bracketvalue.Third_Bracket * ((item.Taxable_Amount) - slabvalue1.Second_Slab));
                        }
                        else if (item.Married == true && ((item.Taxable_Amount) > slabvalue1.Third_Slab) && ((item.Taxable_Amount) <= slabvalue1.Fourth_Slab))
                        {
                            item.Tax = ((decimal)bracketvalue.First_Bracket * slabvalue1.First_Slab + (decimal)bracketvalue.Second_Bracket * slabvalue1.Second_Slab + (decimal)bracketvalue.Third_Bracket * slabvalue1.Third_Slab + (decimal)bracketvalue.Fourth_Bracket * ((item.Taxable_Amount) - slabvalue1.Third_Slab));
                        }
                        else if (item.Married == true && ((item.Taxable_Amount) > slabvalue1.Fourth_Slab))
                        {
                            item.Tax = ((decimal)bracketvalue.First_Bracket * slabvalue1.First_Slab + (decimal)bracketvalue.Second_Bracket * slabvalue1.Second_Slab + (decimal)bracketvalue.Third_Bracket * slabvalue1.Third_Slab + (decimal)bracketvalue.Fourth_Bracket * (slabvalue1.Fourth_Slab - slabvalue1.Third_Slab) + (decimal)bracketvalue.Fifth_Bracket * ((item.Taxable_Amount) - slabvalue1.Fourth_Slab));
                        }
                        
                    }
                    item.Salary = (item.Taxable_Amount - item.Tax)/12;
                    foreach (var sitem in sname)
                    {
                        if (item.StaffId == sitem.Id)
                        { item.Employee_Name = sitem.Full_Name; }
                    }

                }

            }
            if (ModelState.IsValid)
            {
                if (calculation != null)
                {// db.Entry(calculation).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return View(db.Calculations.Include(c => c.Staff).ToList());
        }
        public ActionResult EmpCalculate([Bind(Include = "UserId,Employee_Name,Married,Monthly_Salary,CIT,PF,Taxable_Amount,Tax,Salary,Month,Days,Leave,StaffId,Allowance,Bonus")]Calculation calculation, int id)
        {
            decimal GrossSalary;
            decimal CIT;
            /*using (PayrollEntities entities = new PayrollEntities())
            {*/

            var CalculationDetail = db.Calculations.Where(x => x.StaffId == id).First();
            int cid = CalculationDetail.UserId;
            var sname = db.Staffs.Where(x => x.Id == id).FirstOrDefault();
            var item = db.Calculations.Find(cid);
            var slab1 = db.TaxSlabs.Where(x => x.Married == true).ToList();
            var slab2 = db.TaxSlabs.Where(x => x.Married == false).ToList();
            var bracket = db.TaxBrackekts.ToList();
            foreach (var bracketvalue in bracket)
            {
                item.Days = 31-item.Leave;
              
                    GrossSalary = (decimal)(item.Monthly_Salary * 12) + item.Allowance + item.Bonus + (decimal)0.1 * (item.Monthly_Salary * 12);
                    if (item.CIT == true)
                    {
                        CIT = (decimal)0.23 * (item.Monthly_Salary * 12);
                        if (CIT > 300000)
                        { CIT = 300000; }
                        item.Taxable_Amount = GrossSalary - (decimal)0.2 * (item.Monthly_Salary * 12) - CIT;
                    }
                    else
                    {
                        item.Taxable_Amount = GrossSalary - (decimal)0.2 * (item.Monthly_Salary * 12);
                    }
                    foreach (var slabvalue in slab2)
                    {

                        if (item.Married == false && ((item.Taxable_Amount) <= slabvalue.First_Slab))
                        {
                            item.Tax = ((decimal)bracketvalue.First_Bracket * (item.Taxable_Amount));
                        }
                        else if (item.Married == false && ((item.Taxable_Amount) > slabvalue.First_Slab) && ((item.Taxable_Amount) <= slabvalue.Second_Slab))
                        {
                            item.Tax = ((decimal)bracketvalue.First_Bracket * slabvalue.First_Slab + (decimal)bracketvalue.Second_Bracket * ((item.Taxable_Amount) - slabvalue.First_Slab));
                        }
                        else if (item.Married == false && ((item.Taxable_Amount) > slabvalue.Second_Slab) && ((item.Taxable_Amount) <= slabvalue.Third_Slab))
                        {
                            item.Tax = ((decimal)bracketvalue.First_Bracket * slabvalue.First_Slab + (decimal)bracketvalue.Second_Bracket * slabvalue.Second_Slab + (decimal)bracketvalue.Third_Bracket * ((item.Taxable_Amount) - slabvalue.Second_Slab));
                        }
                        else if (item.Married == false && ((item.Taxable_Amount) > slabvalue.Third_Slab) && ((item.Taxable_Amount) <= slabvalue.Fourth_Slab))
                        {
                            item.Tax = ((decimal)bracketvalue.First_Bracket * slabvalue.First_Slab + (decimal)bracketvalue.Second_Bracket * slabvalue.Second_Slab + (decimal)bracketvalue.Third_Bracket * slabvalue.Third_Slab + (decimal)bracketvalue.Fourth_Bracket * ((item.Taxable_Amount) - slabvalue.Third_Slab));
                        }
                        else if (item.Married == false && ((item.Taxable_Amount) > slabvalue.Fourth_Slab))
                        {
                            item.Tax = ((decimal)bracketvalue.First_Bracket * slabvalue.First_Slab + (decimal)bracketvalue.Second_Bracket * slabvalue.Second_Slab + (decimal)bracketvalue.Third_Bracket * slabvalue.Third_Slab + (decimal)bracketvalue.Fourth_Bracket * (slabvalue.Fourth_Slab - slabvalue.Third_Slab) + (decimal)bracketvalue.Fifth_Bracket * ((item.Taxable_Amount) - slabvalue.Fourth_Slab));
                        }


                    }
                    foreach (var slabvalue1 in slab1)
                    {
                        if (item.Married == true && ((item.Taxable_Amount) <= slabvalue1.First_Slab))
                        {
                            item.Tax = ((decimal)bracketvalue.First_Bracket * (item.Taxable_Amount));
                        }
                        else if (item.Married == true && ((item.Taxable_Amount) > slabvalue1.First_Slab) && ((item.Taxable_Amount) <= slabvalue1.Second_Slab))
                        {
                            item.Tax = ((decimal)bracketvalue.First_Bracket * slabvalue1.First_Slab + (decimal)bracketvalue.Second_Bracket * ((item.Taxable_Amount) - slabvalue1.First_Slab));
                        }
                        else if (item.Married == true && ((item.Taxable_Amount) > slabvalue1.Second_Slab) && ((item.Taxable_Amount) <= slabvalue1.Third_Slab))
                        {
                            item.Tax = ((decimal)bracketvalue.First_Bracket * slabvalue1.First_Slab + (decimal)bracketvalue.Second_Bracket * slabvalue1.Second_Slab + (decimal)bracketvalue.Third_Bracket * ((item.Taxable_Amount) - slabvalue1.Second_Slab));
                        }
                        else if (item.Married == true && ((item.Taxable_Amount) > slabvalue1.Third_Slab) && ((item.Taxable_Amount) <= slabvalue1.Fourth_Slab))
                        {
                            item.Tax = ((decimal)bracketvalue.First_Bracket * slabvalue1.First_Slab + (decimal)bracketvalue.Second_Bracket * slabvalue1.Second_Slab + (decimal)bracketvalue.Third_Bracket * slabvalue1.Third_Slab + (decimal)bracketvalue.Fourth_Bracket * ((item.Taxable_Amount) - slabvalue1.Third_Slab));
                        }
                        else if (item.Married == true && ((item.Taxable_Amount) > slabvalue1.Fourth_Slab))
                        {
                            item.Tax = ((decimal)bracketvalue.First_Bracket * slabvalue1.First_Slab + (decimal)bracketvalue.Second_Bracket * slabvalue1.Second_Slab + (decimal)bracketvalue.Third_Bracket * slabvalue1.Third_Slab + (decimal)bracketvalue.Fourth_Bracket * (slabvalue1.Fourth_Slab - slabvalue1.Third_Slab) + (decimal)bracketvalue.Fifth_Bracket * ((item.Taxable_Amount) - slabvalue1.Fourth_Slab));
                        }

                    }
                    item.Salary = (item.Taxable_Amount - item.Tax) / 12;
                    if (item.StaffId == sname.Id)
                    { item.Employee_Name = sname.Full_Name; }
                }
                if (ModelState.IsValid)
                {
                    if (calculation != null)
                    {// db.Entry(calculation).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                Calculation emp = db.Calculations.Find(cid);
                if (emp == null)
                {
                    return HttpNotFound();
                }
                return View(emp);
            }
        

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}

