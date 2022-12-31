using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SharedAssemblies.Content.AppCode;
using SharedAssemblies.Models;
using SharedAssemblies.DAL;

namespace Developer4.Controllers
{
    public class CustomerSetupController : Controller
    {
        private MyDbContext db = new MyDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CustomerGlossary(string currentFilter, string searchString)
        {
            if (string.IsNullOrEmpty(searchString)) { searchString = currentFilter; }
            ViewData["CurrentFilter"] = searchString;

            var model = db.GetCustomerGlossaryItems(Convert.ToInt32(Session["CustomerID"]), searchString, null).ToList();

            Session["Glossaries"] = null;
            return View(model);
        }

        public JsonResult AddGlossaryText(int id, string lookupKey, string valueWithDefault)
        {
            List<CustomerGlossaryValueGet> list = (List<CustomerGlossaryValueGet>)Session["Glossaries"];
            if (list != null)
            {
                var exists = list.Where(r => r.ID == id).FirstOrDefault();
                if (exists != null) { list.Remove(exists); }
            }
            else { list = new List<CustomerGlossaryValueGet>(); }

            var item = new CustomerGlossaryValueGet()
            {
                ID = id,
                ValueWithDefault = valueWithDefault
            };

            var oldValueWithDefault = db.GetCustomerGlossaryValue(Convert.ToInt32(Session["CustomerID"]), lookupKey).Select(r => r.ValueWithDefault).FirstOrDefault();
            if (oldValueWithDefault != valueWithDefault) { list.Add(item); }
            else { list.Remove(item); }

            Session["Glossaries"] = list;
            return Json(true);
        }

        public ActionResult SaveForm(string filter)
        {
            List<CustomerGlossaryValueGet> list = (List<CustomerGlossaryValueGet>)Session["Glossaries"];

            if (list != null)
            {
                foreach (var item in list)
                {
                    db.CustomerGlossaryItemUpdate(Convert.ToInt32(Session["CustomerID"]), item.ID, item.ValueWithDefault);
                }
            }
            return RedirectToAction(nameof(CustomerGlossary), new { currentFilter = filter });
        }
    }
}
