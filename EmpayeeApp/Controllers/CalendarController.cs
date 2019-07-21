using EmpayeeApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmpayeeApp.Controllers
{
    [Authorize]
    public class CalendarController : Controller
    {
        // GET: Calendar
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetEvents()
        {
            using (PMSEntities3 dc = new PMSEntities3())
            {
                var events = dc.EventCalendars.ToList();
                return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        [HttpPost]
        public JsonResult SaveEvent(EventCalendar e)
        {
            var status = false;
            using (PMSEntities3 dc = new PMSEntities3())
            {
                if (e.EventId > 0)
                {
                    //Update the event
                    var v = dc.EventCalendars.Where(a => a.EventId == e.EventId).FirstOrDefault();
                    if (v != null)
                    {
                        v.Subject = e.Subject;
                        v.Start = e.Start;
                        v.End = e.End;
                        v.Description = e.Description;
                        v.IsFullDay = e.IsFullDay;
                        v.ThemeColor = e.ThemeColor;
                    }
                }
                else
                {
                    dc.EventCalendars.Add(e);
                }
                dc.SaveChanges();
                status = true;
            }
            return new JsonResult { Data = new { status = status } };
        }

        [HttpPost]
        public JsonResult DeleteEvent(int eventID)
        {
            var status = false;
            using (PMSEntities3 dc = new PMSEntities3())
            {
                var v = dc.EventCalendars.Where(a => a.EventId == eventID).FirstOrDefault();
                if (v != null)
                {
                    dc.EventCalendars.Remove(v);
                    dc.SaveChanges();
                    status = true;
                }
            }
            return new JsonResult { Data = new { status = status } };
        }
    }
}