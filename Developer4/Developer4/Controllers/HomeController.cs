using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SharedAssemblies.Content.AppCode;
using SharedAssemblies.Models;
using SharedAssemblies.DAL;
using MySql.Data.Types;
using SharedAssemblies.ViewModels;

namespace Developer4.Controllers
{
    public class HomeController : Controller
    {
        private MyDbContext db = new MyDbContext();

        public ActionResult Index()
        {
            Session["CustomerID"] = 3;
            Session["sessionKey"] = RandomHelper.GenerateGUID();
            Session["userID"] = 123;  // Use this for wherver the current user logged in clientID is needed
            return View();
        }

        public ActionResult ExampleSignature()
        {
            ViewData["TermsText"] = "Terms <br /><br />" +
                "Service provider <br /><br />" +
                "Google services are provided by, and you’re contracting with: <br />" +
                "Google LLC" +
                "organized under the laws of the State of Delaware, USA, and operating under the laws of the USA <br />" +
                "1600 Amphitheatre Parkway <br />" +
                "Mountain View, California 94043 <br />" +
                "USA";
            return View();
        }

        public ActionResult ExampleCallToTable()
        {
            var lookSettings = db.Setting.Where(s => s.Description.Contains("forms")).ToList();
            string results = String.Empty;
            foreach (var item in lookSettings)
            {
                results += item.SettingName + "<br />";
            }
            return Content(results);
        }

        public ActionResult ExampleCallToStoredProcedure()
        {
            string results = String.Empty;
            var lookSettings = db.GetCustomerSettings(3, 0, "", "");
            if (lookSettings != null)
            {
                var settings = lookSettings.ToList();
                if (settings.Count > 0)
                {
                    foreach (var item in settings)
                    {
                        results += item.SettingName + "<br />";
                    }

                }
            }
            return Content(results);
        }

        public ActionResult ExampleUpdateUsingStoredProcedure()
        {
            int customerId = Convert.ToInt32(Session["CustomerID"]);
            int glossaryId = 9999;
            string glossaryText = "Testing";

            db.CustomerGlossaryItemUpdate(customerId, glossaryId, glossaryText);

            return Content("The record was updated.");
        }

        // For Business Requirement Screenshots/Samples
        public ActionResult temp()
        {
            return View();
        }

        public ActionResult Support()
        {
            IList<SupportTicketsGet> model = null;
            using (var db1 = new MyDbContext())
            {
                model = db1.GetSupportTickets(Convert.ToInt32(Session["userID"])).ToList();
            }

            if (model == null)
            {
                return RedirectToAction("Notfound", "Home");
            }
            return View(model);
        }

        public JsonResult AddSupport(string subject, string message)
        {
            if (string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(message))
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            using (var db1 = new MyDbContext())
            {
                db1.AddSupportMessage(Convert.ToInt32(Session["userID"]), subject, message);
            }
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        #region SamplePages
        public ActionResult FormElements()
        {
            return View();
        }

        public ActionResult FormAdvanced()
        {
            return View();
        }

        public ActionResult BasicTables()
        {
            return View();
        }

        public ActionResult SummerNote()
        {
            return View();
        }

        public ActionResult DataTables()
        {
            return View();
        }

        public ActionResult TablesWithPaging()
        {
            return View();
        }

        public ActionResult Icons1()
        {
            return View();
        }

        public ActionResult Icons2()
        {
            return View();
        }

        public ActionResult Buttons()
        {
            return View();
        }

        public ActionResult Modals()
        {
            return View();
        }

        public ActionResult DropDowns()
        {
            return View();
        }

        public ActionResult ToolTipsPopOvers()
        {
            return View();
        }

        public ActionResult CheckboxesRadio()
        {
            return View();
        }

        public ActionResult Notifications()
        {
            return View();
        }

        public ActionResult DropZone()
        {
            return View();
        }

        public ActionResult TabsAccordions()
        {
            return View();
        }

        public ActionResult AppsCalendar()
        {
            return View();
        }

        public ActionResult SweetAlerts()
        {
            return View();
        }
        #endregion

        public ActionResult AddBookingEventExample()
        {
            MySqlDateTime eventDte = new MySqlDateTime();
            eventDte = MySqlCDateTimeUtil.DateStrToMySQLDateTime("11/9/2022");

            MySqlDateTime startTime = new MySqlDateTime();
            startTime = MySqlCDateTimeUtil.Time12To24HourDateTime("11:00 AM");  // To add afternoon time, use for example "05:00 PM"

            db.AddBookingEvent(1, 123, eventDte, startTime, "Dante", "Perkins", "glenn.cherie@aol.com", "+18632769011", -4, Session["sessionKey"].ToString());

            return Content("success");
        }

        public ActionResult Test()
        {
            MySqlDateTime eventDte = new MySqlDateTime();
            eventDte = MySqlCDateTimeUtil.DateStrToMySQLDateTime("02/11/2022", "dd/mm/yyyy");

            MySqlDateTime startTime = new MySqlDateTime();
            startTime = MySqlCDateTimeUtil.Time12To24HourDateTime("10:00 AM");


            db.AddBookingEvent(1, 123, eventDte, startTime, "Anna", "Donalds", "glenn.cherie@aol.com", "+18632769011", -4, Session["sessionKey"].ToString());

            return Content("success");
        }

        public ActionResult Test2()
        {
            var str = "";
            List<AdminFormsGet> lookForms = new List<AdminFormsGet>() { };
            using (var db1 = new MyDbContext())
            {
                lookForms = db1.GetForms(115).ToList();
                foreach (var item in lookForms)
                {
                    str += item.FormName + "<br />";
                }
            }

            return Content(str);
        }

        public ActionResult Test3()
        {
            var str = "";
            AdminFormGet lookForm = new AdminFormGet();
            using (var db1 = new MyDbContext())
            {
                lookForm = db1.GetForm(20).FirstOrDefault();
                str = lookForm.FormName;
            }

            return Content(str);
        }

        public ActionResult Test4()
        {
            int formId = 0;
            using (var db1 = new MyDbContext())
            {
                formId = db1.AddForm(1, "formName", "formCode123", "idfierMsg",
                    "coverletterBulletItemText",
                    "approvalInstructions", 1, 2,
                    "supplementPageMsg", "supplementPageFieldMsg", 3,
                    "ptrn", 4, 1, 1,
                    "header", "hiddenFormCompleted", 1,
                    1, "N/A");
            }

            return Content("success.. formID: " + formId.ToString());
        }

        public ActionResult Test5()
        {
            using (var db1 = new MyDbContext())
            {
                db1.UpdateForm(48, "formname", "FRMTEST111", 0, "unlckmsg",
                    "coverletterBulletItemText",
                    "approvalInstructions", 0, 0,
                    "supplementPageMsg", "supplementPageFieldMsg", 0,
                    "ptrn", 0, 0, 0,
                    "header", "hiddenFormCompleted", 0,
                    0, "na");
            }

            return Content("success");
        }

        public ActionResult Test6()
        {
            using (var db1 = new MyDbContext())
            {
                db1.FormDelete(42);
            }

            return Content("success");
        }

        public ActionResult TimezoneDataGet()
        {
            var data = MySqlCDateTimeUtil.GetTimezoneData();
            var str = "";
            foreach (var item in data)
            {
                str += item.OffSet + " - " + item.Text + "<br />";
            }

            return Content(str);
        }

        public JsonResult GetCalendarData(bool isAdministrator, DateTime start, DateTime end)
        {
            if (isAdministrator != true)
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            using (var db1 = new MyDbContext())
            {
                MySqlDateTime startDte = new MySqlDateTime(start.ToString("yyyy-MM-dd"));
                MySqlDateTime endDte = new MySqlDateTime(end.ToString("yyyy-MM-dd"));

                var customerId = Convert.ToInt32(Session["CustomerID"]);
                var model = db1.GetBookedEvents(customerId, startDte, endDte).ToList();
                var list = new List<EventViewModel>();

                foreach (var item in model)
                {
                    list.Add(new EventViewModel
                    {
                        id = item.ID.ToString(),
                        title = DateTime.Parse(item.StartTime).ToString("hh:mm tt") + " - " + item.EventName,
                        start = item.StartDateTime,
                        end = item.EndDateTime
                    });
                }
                return Json(new { success = true, data = list }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetBookedEvent(int id)
        {
            using (var db1 = new MyDbContext())
            {
                var data = db1.GetBookedEvent(id).FirstOrDefault();
                var model = new EventViewModel()
                {
                    duration = data.Duration.ToString(),
                    location = data.Location,
                    description = data.Description,
                    name = data.FirstName + " " + data.LastName,
                    email = data.Email,
                    phone = data.Cell,
                    organizer = data.OrganizerFirstName + " " + data.OrganizerLastName,
                    date = DateTime.Parse(data.StartDateTime).ToString("MMMM d, yyyy h:mm tt"),
                    eventName = data.EventName
                };
                return Json(new { success = true, data = model }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetEventOrganizers(int eventId)
        {
            using (var db1 = new MyDbContext())
            {
                var data = db1.GetEventOrganizers(eventId, 1);
                var model = new List<EventOrganizersGet>();
                foreach (var item in data)
                {
                    model.Add(new EventOrganizersGet()
                    {
                        ClientID = item.ClientID,
                        FirstName = item.FirstName,
                        LastName = item.LastName
                    });
                }

                return Json(new { success = true, data = model }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetAvailableClientEvents(int clientId, int eventId, DateTime bookingDate, int userTimezoneOffset)
        {
            using (var db1 = new MyDbContext())
            {
                MySqlDateTime startDte = new MySqlDateTime(bookingDate.ToString("yyyy-MM-dd"));
                var data = db1.GetAvailableClientEvents(clientId, eventId, startDte, userTimezoneOffset).ToList();
                var model = new List<AvailableClientEventsGet>();
                foreach (var item in data)
                {
                    model.Add(new AvailableClientEventsGet()
                    {
                        StartTime = item.StartTime,
                        StartDateTimeDisplay = item.DateDifference == 0 ? DateTime.Parse(item.StartTimeDisplay).ToString("h:mm tt") : DateTime.Parse(item.StartDateTimeDisplay).ToString("MMMM d, yyyy h:mm tt"),
                        DateDifference = item.DateDifference
                    });
                }

                if (model.Count > 0)
                {
                    return Json(new { success = true, data = model }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public ActionResult calendar(int eventID = 1)
        {
            EventGet model = new EventGet();
            using (var db1 = new MyDbContext())
            {
                model = db1.GetEvent(eventID, 0).FirstOrDefault();
            }

            // TODO: Take the default value of 1 off in the param list and add a notfound if the event isn't found.  --testcode--
            //       This is LOW PRIORITY FOR THE ADMIN SECTION.. TOP PRIORITY IS IN THE FORMILAE HOME SITE

            var customerCurrentDateStr = "";
            using (var db2 = new MyDbContext())
            {
                customerCurrentDateStr = db2.GetCustomerCurrentDate(Convert.ToInt32(Session["CustomerID"])).FirstOrDefault().CurrentDate;
            }

            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var item in MySqlCDateTimeUtil.GetTimezoneData())
            {
                list.Add(new SelectListItem { Value = item.OffSet.ToString(), Text = item.Text });
            }
            var currentMonthDateRange = MySqlCDateTimeUtil.GetMySQLDateRange(customerCurrentDateStr);

            ViewData["TimeZones"] = list;
            ViewBag.ShowBookedEvents = true;
            ViewBag.DefaultTimeZone = -4;
            ViewBag.CustomerCurrentDateStr = MySqlCDateTimeUtil.DateTimeToMySQLDateTimeStampString(Convert.ToDateTime(customerCurrentDateStr), false);
            ViewBag.CustomerCurrentMonthStart = !String.IsNullOrEmpty(Request.QueryString["sd"]) ? Request.QueryString["sd"] : currentMonthDateRange[0];
            ViewBag.CustomerCurrentMonthEnd = !String.IsNullOrEmpty(Request.QueryString["ed"]) ? Request.QueryString["ed"] : currentMonthDateRange[1];
            var bookingEventId = "";
            if (!String.IsNullOrEmpty(Request.QueryString["bId"]) && VisitorTracking.FormilaeSiteReferer)
            {
                bookingEventId = Request.QueryString["bId"];
            }
            ViewBag.BookingEventID = bookingEventId;

            return View(model);
        }

        public JsonResult AddBookingEvent(
            int eventId,
            int organizerClientId,
            DateTime eventDate,
            string startTime,
            string firstName,
            string lastName,
            string email,
            string cellPhoneNumber,
            int timezoneOffset)
        {
            var bookingEventId = "";
            var eventStartDte = eventDate.ToString("yyyy-MM-dd");
            var dte = MySqlCDateTimeUtil.GetMySQLDateRange(eventDate);

            using (var db1 = new MyDbContext())
            {
                MySqlDateTime _eventDate = new MySqlDateTime(eventStartDte);
                MySqlDateTime _startTime = new MySqlDateTime();
                _startTime = MySqlCDateTimeUtil.Time12To24HourDateTime(startTime);

                db1.AddBookingEvent(
                        eventId,
                        organizerClientId,
                        _eventDate,
                        _startTime,
                        firstName,
                        lastName,
                        email,
                        cellPhoneNumber,
                        timezoneOffset,
                        Session["sessionKey"].ToString()
                    );

                bookingEventId = ClientHelper.GetSessionInsertOutput();
            }
            return Json(new { success = true, bookingEID = bookingEventId, dteStart = dte[0], dteEnd = dte[1] }, JsonRequestBehavior.AllowGet);
        }
    }
}