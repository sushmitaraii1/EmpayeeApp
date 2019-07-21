using EmpayeeApp.Models;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace EmpayeeApp.Controllers
{
    [Authorize]
    public class StaffsController : Controller
    {
        private PMSEntities3 db = new PMSEntities3();

        // GET: Staffs
        public ActionResult AdminIndex()
        {
            var staffs = db.Staffs.Include(s => s.Role);
            return View(staffs.ToList());
        }
        public ActionResult UserIndex()
        {
            using (PMSEntities3 dbmodel = new PMSEntities3())
            {
                int id = (int)Session["UserId"];
                return View(dbmodel.Staffs.Where(x => x.Id == id).FirstOrDefault());
            }

        }
        // GET: Staffs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Staff staff = db.Staffs.Find(id);
            if (staff == null)
            {
                return HttpNotFound();
            }
            return View(staff);
        }

        // GET: Staffs/Create
        public ActionResult Create()
        {
            ViewBag.role_id = new SelectList(db.Roles, "RoleId", "RoleType");
            return View();
        }

        // POST: Staffs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserName,Password,Full_Name,Designation,Email,Joined_date,role_id")] Staff staff)
        {
            if (ModelState.IsValid)
            {
                db.Staffs.Add(staff);
                db.SaveChanges();
                return RedirectToAction("AdminIndex");
            }

            ViewBag.role_id = new SelectList(db.Roles, "RoleId", "RoleType", staff.role_id);
            return View(staff);
        }

        // GET: Staffs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Staff staff = db.Staffs.Find(id);
            if (staff == null)
            {
                return HttpNotFound();
            }
            ViewBag.role_id = new SelectList(db.Roles, "RoleId", "RoleType", staff.role_id);
            return View(staff);
        }

        // POST: Staffs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserName,Password,Full_Name,Designation,Email,Joined_date,role_id")] Staff staff)
        {
            if (ModelState.IsValid)
            {
                db.Entry(staff).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("AdminIndex");
            }
            ViewBag.role_id = new SelectList(db.Roles, "RoleId", "RoleType", staff.role_id);
            return View(staff);
        }

        // GET: Staffs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Staff staff = db.Staffs.Find(id);
            if (staff == null)
            {
                return HttpNotFound();
            }
            return View(staff);
        }

        // POST: Staffs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Staff staff = db.Staffs.Find(id);
            db.Staffs.Remove(staff);
            db.SaveChanges();
            return RedirectToAction("AdminIndex");
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
