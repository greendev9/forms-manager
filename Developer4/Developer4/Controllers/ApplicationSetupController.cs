using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SharedAssemblies.Content.AppCode;
using SharedAssemblies.Models;
using SharedAssemblies.DAL;
using Admin.ViewModels;
using System.IO;
using Microsoft.VisualBasic.FileIO;

namespace Developer4.Controllers
{
    public class ApplicationSetupController : Controller
    {
        private MyDbContext db = new MyDbContext();

        public ActionResult Index()
        {
            return Content("Coming soon.");
        }

        public ActionResult Test()
        {
            var str = "";
            //var look = db.GetLookupItems(54);
            //var look = db.GetLookupListForms(54);
            var look = db.GetLookups(1, "");
            var k = look.ToList();
            foreach (var item in k)
            {
                str += item.ListName + "<br />";
            }
            return Content(str);
        }

        public ActionResult ChoiceListManager(string currentFilter, string searchString)
        {
            if (string.IsNullOrEmpty(searchString)) { searchString = currentFilter; }
            ViewData["CurrentFilter"] = searchString;

            var model = db.GetLookups(Convert.ToInt32(Session["CustomerID"]), searchString).ToList();
            return View(model);
        }

        public JsonResult DeactivateLookup(int lookupID)
        {
            if (lookupID == 0) { return Json(JsonRequestBehavior.DenyGet); }

            db.DeactivateLookup(lookupID);
            return Json(JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLookupListItems(int lookupID)
        {
            if (lookupID == 0) { return Json(JsonRequestBehavior.DenyGet); }

            var model = db.GetLookupItems(lookupID).Where(r => r.Active == 1);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLookupListForms(int lookupID)
        {
            if (lookupID == 0) { return Json(JsonRequestBehavior.DenyGet); }

            var model = db.GetLookupListForms(Convert.ToInt32(Session["CustomerID"]), lookupID);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddLookup(string description)
        {
            if (!string.IsNullOrEmpty(description))
            {
                db.CustomerLookupAdd(Convert.ToInt32(Session["CustomerID"]), description);
                return Json(JsonRequestBehavior.AllowGet);
            }
            return Json(JsonRequestBehavior.DenyGet);
        }

        public ActionResult ChoiceListEdit(int? id, string msg)
        {
            if (id == null) { return HttpNotFound(); }

            var lookups = db.GetLookups(Convert.ToInt32(Session["CustomerID"]), null).Where(r => r.LookupID == id).FirstOrDefault();
            if (lookups == null) { return HttpNotFound(); }

            var lookupItems = db.GetLookupItems(lookups.LookupID).Select(r => new LookupItemsViewModel()
            {
                ID = r.ID,
                KeyValue = r.KeyValue,
                DisplayValue = r.DisplayValue,
                Active = r.Active
            }).ToList();

            var model = new LookupViewModel()
            {
                LookupID = lookups.LookupID,
                ListName = lookups.ListName,
                LookupItems = lookupItems
            };
            ViewData["Message"] = msg;
            Session["NewLookupListItems"] = null;
            Session["OldLookupListItems"] = null;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChoiceListEdit(int id, LookupViewModel model)
        {
            if (id != model.LookupID) { return HttpNotFound(); }

            if (ModelState.IsValid)
            {
                // Update Lookup List Name
                var lookups = db.GetLookups(Convert.ToInt32(Session["CustomerID"]), null).Where(r => r.LookupID == model.LookupID).FirstOrDefault();
                if (lookups != null && lookups.ListName != model.ListName)
                {
                    db.LookupUpdate(model.LookupID, model.ListName);
                }

                // Update Lookup List Item
                List<LookupItemsViewModel> list = (List<LookupItemsViewModel>)Session["OldLookupListItems"];
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        db.LookupListItemUpdate(item.ID, item.KeyValue, item.DisplayValue, item.Active);
                    }
                }
                // Add Lookup List Item
                List<LookupItemsViewModel> list2 = (List<LookupItemsViewModel>)Session["NewLookupListItems"];
                if (list2 != null)
                {
                    foreach (var item in list2)
                    {
                        db.LookupListItemAdd(model.LookupID, item.KeyValue, item.DisplayValue);
                    }
                }
                return RedirectToAction(nameof(ChoiceListEdit), new { lookupID = model.LookupID, msg = "The update was successful!" });
            }
            return View(model);
        }

        public JsonResult NewLookupListItems(int id, string keyValue, string displayValue, int active)
        {
            List<LookupItemsViewModel> list = (List<LookupItemsViewModel>)Session["NewLookupListItems"];
            if (list != null)
            {
                var exists = list.Where(r => r.ID == id).FirstOrDefault();
                if (exists != null) { list.Remove(exists); }
            }
            else { list = new List<LookupItemsViewModel>(); }

            var item = new LookupItemsViewModel()
            {
                ID = id,
                KeyValue = keyValue,
                DisplayValue = displayValue,
                Active = active
            };
            list.Add(item);

            Session["NewLookupListItems"] = list;
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult OldLookupListItems(int id, string keyValue, string displayValue, int active, int lookupId)
        {
            List<LookupItemsViewModel> list = (List<LookupItemsViewModel>)Session["OldLookupListItems"];
            if (list != null)
            {
                var exists = list.Where(r => r.ID == id).FirstOrDefault();
                if (exists != null) { list.Remove(exists); }
            }
            else { list = new List<LookupItemsViewModel>(); }

            var item = new LookupItemsViewModel()
            {
                ID = id,
                KeyValue = keyValue,
                DisplayValue = displayValue,
                Active = active
            };

            var oldValue = db.GetLookupItems(lookupId).Where(r => r.ID == id).FirstOrDefault();
            if (oldValue.KeyValue != keyValue || oldValue.DisplayValue != displayValue || oldValue.Active != active) { list.Add(item); }
            else { list.Remove(item); }

            Session["OldLookupListItems"] = list;
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FormQuestionsUpload()
        {
            if (Request.QueryString["postBack"] != "1")
            {
                Session["FormQuestionsUpload_NoFileSelected"] = null;
                Session["FormQuestionsUpload_Status"] = null;
                Session["FormQuestionsUpload_Msg"] = null;
            }

            ViewBag.Menu = "Service Administration";
            ViewBag.MenuItem = "Application Setup";
            ViewBag.Page = "Question Upload Utility";
            return View();
        }

        [HttpPost]
        public ActionResult FormQuestionsUpload(FormCollection form, HttpPostedFileBase postedFile)
        {
            bool noFileSelected = true;
            int customerId = Convert.ToInt32(form["customerId"]);
            string clientId = form["clientId"];
            if (Request.Files.AllKeys.Any())
            {
                // Get the uploaded image from the Files collection
                postedFile = HttpContext.Request.Files[0];
            }
            var status = "success";
            var statusMsg = "File uploaded.";

            if (!postedFile.FileName.Contains(".csv"))
            {
                status = "failed";
                statusMsg = "File must be a csv file.";
                Session["FormQuestionsUpload_NoFileSelected"] = noFileSelected;
                return Json(new { status = status, Message = statusMsg });
            }

            List<TempFormQuestion> formQuestions = new List<TempFormQuestion>();
            int lines = 0;
            string filePath = string.Empty;
            var formAlreadyExists = false;
            if (postedFile != null)
            {
                if (postedFile.ContentLength > 0)
                {
                    noFileSelected = false;
                    string path = Server.MapPath("~/Content/Files/Uploads/");  // You will need to create this folder/path on your local to test on localhost

                    filePath = path + Path.GetFileName(postedFile.FileName);
                    string extension = Path.GetExtension(postedFile.FileName);
                    postedFile.SaveAs(filePath);

                    //Read the contents of CSV file.
                    string csvData = System.IO.File.ReadAllText(filePath);
                    csvData = csvData.Replace("\r\n", "\n");

                    // Add to references Microsoft.VisualBasic availble in assemblies framework
                    TextFieldParser parser = new TextFieldParser(new StringReader(csvData));
                    parser.HasFieldsEnclosedInQuotes = true;
                    parser.SetDelimiters(",");

                    if (!parser.EndOfData)
                    {
                        parser.ReadLine();
                    }
                    int? formId = 0;
                    while (!parser.EndOfData)
                    {
                        int i = 0;
                        TempFormQuestion question = new TempFormQuestion();
                        question.FormCode = string.Empty;
                        question.FormLabel = string.Empty;
                        question.QuestionNo = string.Empty;
                        question.QuestionOfficial = string.Empty;
                        question.SectionName = string.Empty;
                        question.SubSectionName = string.Empty;
                        question.ChoiceText = string.Empty;
                        question.MaxChars = string.Empty;
                        question.IsSingleCheckbox = string.Empty;
                        question.SortOrder = string.Empty;
                        question.PageNo = string.Empty;
                        question.MultiSectionName = string.Empty;
                        question.MultiSectionNo = string.Empty;
                        question.MultiSectionQuestion = string.Empty;
                        question.ErrMsg = String.Empty;

                        string[] fields = parser.ReadFields();
                        foreach (string cell in fields)
                        {
                            string value = cell.Trim();
                            if (i == 0)
                            {
                                question.FormCode = value;
                                if (String.IsNullOrEmpty(value))
                                {
                                    question.ErrMsg = question.ErrMsg + (!String.IsNullOrEmpty(question.ErrMsg) ? "{br}" : "") + "FormCode is required.";
                                }
                                else if (value.Length > 50)
                                {
                                    question.ErrMsg = question.ErrMsg + (!String.IsNullOrEmpty(question.ErrMsg) ? "{br}" : "") + "FormCode too long.";
                                }
                                else if (formId == 0)
                                {
                                    // This is an example of an exists in database check. Although it is disabled with the 1==0 comparisons, it was added as an example call to the database to check for dupliate
                                    // Uset his method to find out if the email already exists in the client table (db.Client)
                                    var lookForm = db.Form.Where(f => (f.CustomerID == customerId || customerId == 1 && f.CustomerID.HasValue == false) && f.FormCode == value.Trim() && f.DateInactive == null).FirstOrDefault();
                                    if (lookForm == null && 1==0)
                                    {
                                        question.ErrMsg = question.ErrMsg + (!String.IsNullOrEmpty(question.ErrMsg) ? "{br}" : "") + "Form Code not found.";
                                    }
                                    else
                                    {
                                        //formId = lookForm.ID;
                                        formId = 1;  // 
                                        var lookFormQuestion = db.FormQuestion.Where(q => q.FormID == formId).FirstOrDefault();  // This is disabled, but an example of an "exists in database check"
                                        if (lookFormQuestion != null && 1==0)
                                        {
                                            formAlreadyExists = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (i == 1)
                            {
                                question.QuestionNo = value;
                                if (value.Length > 6)
                                {
                                    question.ErrMsg = question.ErrMsg + (!String.IsNullOrEmpty(question.ErrMsg) ? "{br}" : "") + "Length must be less than 6 characters.";
                                }
                            }
                            if (i == 2)
                            {
                                question.MultiSectionQuestion = value;
                                if (value.Length > 500)
                                {
                                    question.ErrMsg = question.ErrMsg + (!String.IsNullOrEmpty(question.ErrMsg) ? "{br}" : "") + "Text is too long.";
                                }

                            }
                            if (i == 3)
                            {
                                question.QuestionOfficial = value;
                                if (String.IsNullOrEmpty(value))
                                {
                                    question.ErrMsg = question.ErrMsg + (!String.IsNullOrEmpty(question.ErrMsg) ? "{br}" : "") + "QuestionOfficial is required.";
                                }
                                else if (value.Length > 500)
                                {
                                    question.ErrMsg = question.ErrMsg + (!String.IsNullOrEmpty(question.ErrMsg) ? "{br}" : "") + "Text is too long.";
                                }
                            }
                            if (i == 4)
                            {
                                question.SectionName = value;
                                if (value.Length > 255)
                                {
                                    question.ErrMsg = question.ErrMsg + (!String.IsNullOrEmpty(question.ErrMsg) ? "{br}" : "") + "Text is too long.";
                                }
                            }
                            if (i == 5)
                            {
                                question.SubSectionName = value;
                                if (value.Length > 255)
                                {
                                    question.ErrMsg = question.ErrMsg + (!String.IsNullOrEmpty(question.ErrMsg) ? "{br}" : "") + "Text is too long.";
                                }
                            }
                            if (i == 6)
                            {
                                question.ChoiceText = value;   // NameOfLookupList:Value,DisplayText|Value,DisplayText|Value,DisplayText, etc.
                                if (!String.IsNullOrEmpty(value))
                                {
                                    var arrChoiceText = value.Split(':');
                                    if (value.Contains(":") && (arrChoiceText.Length != 2 || String.IsNullOrEmpty(arrChoiceText[1].Trim())))
                                    {
                                        question.ErrMsg = question.ErrMsg + (!String.IsNullOrEmpty(question.ErrMsg) ? "{br}" : "") + "Improper formatting: Err Code 1";
                                    }
                                    else
                                    {
                                        if (arrChoiceText[0].Trim().Length > 200)
                                        {
                                            question.ErrMsg = question.ErrMsg + (!String.IsNullOrEmpty(question.ErrMsg) ? "{br}" : "") + "ChoiceText element identifier/name is too long.";
                                        }
                                        else if (String.IsNullOrEmpty(arrChoiceText[0].Trim()))
                                        {
                                            question.ErrMsg = question.ErrMsg + (!String.IsNullOrEmpty(question.ErrMsg) ? "{br}" : "") + "ChoiceText identifier/name can't be identified.";
                                        }
                                        if (value.Contains(":"))
                                        {
                                            var arrChoices = arrChoiceText[1].Split('|');
                                            if (arrChoices.Length > 1)
                                            {
                                                foreach (var item in arrChoices)
                                                {
                                                    var arrText = item.Split(',');
                                                    if (arrText.Length != 2)
                                                    {
                                                        question.ErrMsg = question.ErrMsg + (!String.IsNullOrEmpty(question.ErrMsg) ? "{br}" : "") + "Improper formatting: Err Code 2";
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        var text1 = arrText[0].Trim();
                                                        var text2 = arrText[1].Trim();
                                                        if (text1.Length == 0 || text2.Length == 0)
                                                        {
                                                            question.ErrMsg = question.ErrMsg + (!String.IsNullOrEmpty(question.ErrMsg) ? "{br}" : "") + "Improper formatting: Err Code 3";
                                                            break;
                                                        }
                                                        else if (text1.Length > 255 || text2.Length > 255)
                                                        {
                                                            question.ErrMsg = question.ErrMsg + (!String.IsNullOrEmpty(question.ErrMsg) ? "{br}" : "") + "Value/Display text too long.";
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                question.ErrMsg = question.ErrMsg + (!String.IsNullOrEmpty(question.ErrMsg) ? "{br}" : "") + "Improper formatting: Err Code 4";
                                            }

                                        }
                                    }
                                }
                            }
                            // Purposely placing this after "choice text"
                            if (i == 7)
                            {
                                question.FormLabel = value;
                                if (String.IsNullOrEmpty(question.FormLabelProtected) && !(question.ChoiceText.Contains("MultiSelect:") && question.ChoiceText.Contains("RadioList:")))
                                {
                                    question.ErrMsg = question.ErrMsg + (!String.IsNullOrEmpty(question.ErrMsg) ? "{br}" : "") + "FormLabel is required.";
                                }
                                else if (value.Length > 255)
                                {
                                    question.ErrMsg = question.ErrMsg + (!String.IsNullOrEmpty(question.ErrMsg) ? "{br}" : "") + "Text is too long.";
                                }
                            }
                            if (i == 8)
                            {
                                question.MaxChars = value;
                                int maxChars = 0;
                                if (!String.IsNullOrEmpty(value))
                                {
                                    var isNumeric = int.TryParse(value, out maxChars);
                                    if (!isNumeric)
                                    {
                                        question.ErrMsg = question.ErrMsg + (!String.IsNullOrEmpty(question.ErrMsg) ? "{br}" : "") + "MaxChars must be an integer.";
                                    }
                                }
                            }
                            if (i == 9)
                            {
                                question.IsSingleCheckbox = value;
                            }
                            if (i == 10)
                            {
                                question.SortOrder = value;
                                if (String.IsNullOrEmpty(value))
                                {
                                    question.ErrMsg = question.ErrMsg + (!String.IsNullOrEmpty(question.ErrMsg) ? "{br}" : "") + "SortOrder is required.";
                                }
                                int sortOrder = 0;
                                var isNumeric = int.TryParse(value, out sortOrder);
                                if (!isNumeric)
                                {
                                    question.ErrMsg = question.ErrMsg + (!String.IsNullOrEmpty(question.ErrMsg) ? "{br}" : "") + "SortOrder must be an integer.";
                                }
                            }
                            if (i == 11)
                            {
                                question.PageNo = value;
                            }
                            if (i == 12)
                            {
                                question.MultiSectionName = value;
                                if (value.Length > 255)
                                {
                                    question.ErrMsg = question.ErrMsg + (!String.IsNullOrEmpty(question.ErrMsg) ? "{br}" : "") + "Text is too long.";
                                }
                            }
                            if (i == 13)
                            {
                                question.MultiSectionNo = value;
                                if (!String.IsNullOrEmpty(value))
                                {
                                    int multiSectionNo = 0;
                                    var isNumeric = int.TryParse(value, out multiSectionNo);
                                    if (!isNumeric)
                                    {
                                        question.ErrMsg = question.ErrMsg + (!String.IsNullOrEmpty(question.ErrMsg) ? "{br}" : "") + "MultiSectionNo must be an integer.";
                                    }
                                }
                            }
                            i++;
                        }

                        formQuestions.Add(question);
                        lines++;
                    }
                }
            }

            bool noFilesInUpload = lines == 0;
            var linesErrCount = formQuestions.Where(q => q.ErrMsg != "").Count();
            if (formAlreadyExists)
            {
                status = "failed";
                statusMsg = "Questions or sections have already been created for this form.";
            }
            else if (noFilesInUpload)
            {
                status = "failed";
                statusMsg = "No files found in the upload.";
            }
            else if (noFileSelected)
            {
                status = "failed";
                statusMsg = "No file selected.";
            }
            else if (linesErrCount > 0)
            {
                status = "failed";
                statusMsg = "Line item errors detected. (See highlighted errors below.)";
            }

            if (!noFilesInUpload)
            {
                var formCodesCount = formQuestions
                                    .GroupBy(q => q.FormCode)
                                    .Select(g => new
                                    {
                                        Count = g.Select(x => x.FormCode).Distinct().Count()
                                    }).ToList().Count;

                if (formCodesCount > 1)
                {
                    status = "failed";
                    statusMsg = "There should only be one Form Code per upload.";
                }

                if (status != "failed")
                {
                    var lookMultiInvalid = formQuestions.Where(q => q.MultiSectionInvalid == true).FirstOrDefault();
                    if (lookMultiInvalid != null)
                    {
                        status = "failed";
                        statusMsg = "Fields with MultiSectionName values must have a MultiSectionNo.";
                    }
                }

                if (status != "failed")
                {
                    var lookMultiQuestion = formQuestions.Where(q => q.MultiSectionNo.Length > 0).ToList();
                    if (lookMultiQuestion.Count > 0)
                    {
                        foreach (var question in lookMultiQuestion)
                        {
                            var headerQuestionCount = formQuestions.Where(q => q.MultiSectionName == question.MultiSectionName && q.MultiSectionNo == "1" && q.MultiSectionQuestion.Length > 0).Count();
                            if (headerQuestionCount == 0)
                            {
                                status = "failed";
                                statusMsg = "One or more questions missing MultiSectionQuestion.";
                                break;
                            }
                        }
                    }
                }

                // Example of a distinct check
                if (status != "failed")
                {
                    var formLabels = formQuestions
                                    .GroupBy(q => q.FormLabelProtected)
                                    .Select(g => new
                                    {
                                        Count = g.Select(x => x.FormLabelProtected).Distinct().Count()
                                    }).ToList().Count;
                    if (formLabels != lines)
                    {
                        status = "failed";
                        statusMsg = "Duplicate Form Label values detected.";
                    }
                }


            }

            if (status != "failed")
            {
                try
                {
                    var sql = "DELETE FROM temp_form_question WHERE AddUserId = " + clientId;  // Use stored procedure instead of this manual SQL
                    db.Database.ExecuteSqlCommand(sql);

                    foreach (var item in formQuestions)
                    {
                        // **** Use stored procedure instead of this manual SQL
                        sql = @"CALL admin_tempformquestion(" +
                                clientId + "," +
                                "'" + item.FormCode + "', " +
                                "'" + item.FormLabel + "', " +
                                "'" + item.QuestionNo + "', " +
                                "'" + item.MultiSectionQuestion + "', " +
                                "'" + item.QuestionOfficial + "', " +
                                "'" + item.SectionName + "', " +
                                "'" + item.SubSectionName + "', " +
                                "'" + item.ChoiceText + "', " +
                                "'" + item.MaxCharsDB + "', " +
                                "'" + item.IsSingleCheckboxDB + "', " +
                                "'" + item.SortOrder + "', " +
                                "'" + item.PageNoDB + "', " +
                                "'" + item.MultiSectionName + "', " +
                                "'" + item.MultiSectionNo + "')";
                        db.Database.ExecuteSqlCommand(sql);
                    }

                    sql = "call admin_tempformquestionexecute(" + clientId + ")";  // Use stored procedure instead of this manual SQL
                    db.Database.ExecuteSqlCommand(sql);
                }
                catch (Exception e)
                {
                    status = "failed";
                    statusMsg = "The database didn't like the results insert: " + e.Message;
                }
            }

            Session["FormQuestionsUpload_FormQuestions"] = formAlreadyExists ? new List<TempFormQuestion>() : formQuestions;
            Session["FormQuestionsUpload_NoFileSelected"] = noFileSelected;
            Session["FormQuestionsUpload_Status"] = status;
            Session["FormQuestionsUpload_Msg"] = statusMsg;

            return Json(new { status = status, Message = statusMsg });
        }

        public ActionResult ProcessManager(string currentFilter, string searchString)
        {
            ViewBag.AllowedRoles = new string[] { "Administrator", "UI Developer" };
            if (string.IsNullOrEmpty(searchString)) { searchString = currentFilter; }
            ViewData["CurrentFilter"] = searchString;

            var model = db.GetAdminProcesses(Convert.ToInt32(Session["CustomerID"]), searchString).ToList();
            return View(model);
        }

        public JsonResult GetProcessForms(int processId)
        {
            if (processId == 0) { return Json(JsonRequestBehavior.DenyGet); }
            var model = db.AdminGetProcessForms(processId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ProcessActiveSet(int id, bool isChecked)
        {
            int isActive = 0;
            if (isChecked) { isActive = 1; }

            db.ProcessActiveUpdate(id, isActive);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddProcess()
        {
            ViewBag.AllowedRoles = new string[] { "Administrator", "UI Developer" };
            ViewBag.Page = "Add Process";
            var model = ProcessViewModel.Instance;
            Session["NewProcessCosts"] = null;
            return View("ActionProcess", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddProcess(ProcessViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Add Process
                db.AddNonSurveyProcess(
                    Convert.ToInt32(Session["CustomerID"]),
                    model.ProcessName,
                    model.Summary,
                    model.GovernmentMailingAddress1,
                    model.PacketTitle,
                    model.RequiresAdminApproval,
                    model.GeneratesEmail,
                    model.ExternalRedirect,
                    model.FormListHeaderMessage,
                    model.WelcomeEmail);

                model.ProcessId = db.GetAdminProcesses(Convert.ToInt32(Session["CustomerID"]), null).OrderByDescending(r => r.ID).Select(r => r.ID).FirstOrDefault();
                // Add Process Costs
                List<ProcessCostsViewModel> list = (List<ProcessCostsViewModel>)Session["NewProcessCosts"];
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        db.AdminAddProcessCost(
                            Convert.ToInt32(Session["CustomerID"]),
                            model.ProcessId,
                            item.ItemName,
                            item.ItemDescription,
                            item.Price,
                            item.SortOrder);
                    }
                }
                TempData["smsg"] = "The changes have been made successfully.";
                return RedirectToAction(nameof(ProcessManager));
            }
            ViewBag.Page = "Add Process";
            return View("ActionProcess", model);
        }

        public ActionResult UpdateProcess(int id)
        {
            ViewBag.AllowedRoles = new string[] { "Administrator", "UI Developer" };
            var process = db.GetCustomerProcess(Convert.ToInt32(Session["CustomerID"]), id).FirstOrDefault();
            if (process == null) { return HttpNotFound(); }

            var processCosts = db.GetProcessCosts(id).Select(r => new ProcessCostsViewModel()
            {
                ID = r.ID,
                ItemName = r.ItemName,
                ItemDescription = r.ItemDescription,
                Price = r.Price,
                SortOrder = r.SortOrder,
                Active = r.Active
            }).ToList();

            var model = new ProcessViewModel
            {
                ProcessId = id,
                FormilaeProcess = process.FormilaeProcess,
                ProcessName = process.ProcessName,
                Summary = process.Summary,
                GovernmentMailingAddress1 = process.GovernmentMailingAddress1,
                PacketTitle = process.PacketTitle,
                RequiresAdminApproval = process.RequiresAdminApproval,
                GeneratesEmail = process.GeneratesEmail,
                ExternalRedirect = process.ExternalRedirect,
                HideFromServicesPurchasedList = process.HideFromServicesPurchasedList,
                FormListHeaderMessage = process.FormListHeaderMessage,
                WelcomeEmail = process.WelcomeEmail,
                ProcessCosts = processCosts,
                ProcessForms = db.GetProcessForms(id, 1, 1).ToList()
            };
            ViewBag.Page = "Modify Process";
            Session["NewProcessCosts"] = null;
            Session["OldProcessCosts"] = null;
            return View("ActionProcess", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProcess(int id, ProcessViewModel model)
        {
            if (id != model.ProcessId) { return HttpNotFound(); }

            if (ModelState.IsValid)
            {
                var process = db.GetCustomerProcess(Convert.ToInt32(Session["CustomerID"]), id).FirstOrDefault();
                if (process == null) { return HttpNotFound(); }
                var value = String.Empty;

                if (!String.IsNullOrEmpty(model.FormListHeaderMessage))
                {
                    value = model.FormListHeaderMessage;
                    value = Server.UrlDecode(value);
                    value = value.Replace("<b>", "<span style='font-weight: bold;'>");
                    value = value.Replace("</b>", "</span>");
                    value = value.Replace("<strong>", "<span style='font-weight: bold;'>");
                    value = value.Replace("</strong>", "</span>");
                    value = value.Replace("<span style=\"font-weight: bold;\"> </span>", "");
                    if (value == "<p><br></p>")
                    {
                        value = "";
                    }

                    model.FormListHeaderMessage = value;
                }

                if (!String.IsNullOrEmpty(model.WelcomeEmail))
                {
                    value = model.WelcomeEmail;
                    value = Server.UrlDecode(value);
                    value = value.Replace("<b>", "<span style='font-weight: bold;'>");
                    value = value.Replace("</b>", "</span>");
                    value = value.Replace("<strong>", "<span style='font-weight: bold;'>");
                    value = value.Replace("</strong>", "</span>");
                    value = value.Replace("<span style=\"font-weight: bold;\"> </span>", "");
                    if (value == "<p><br></p>")
                    {
                        value = "";
                    }

                    model.WelcomeEmail = value;
                }

                // Update Process
                db.UpdateNonSurveyProcess(
                    Convert.ToInt32(Session["CustomerID"]),
                    model.ProcessId,
                    model.ProcessName,
                    model.Summary,
                    model.GovernmentMailingAddress1,
                    model.PacketTitle,
                    model.RequiresAdminApproval,
                    model.GeneratesEmail,
                    model.ExternalRedirect,
                    model.FormListHeaderMessage,
                    model.WelcomeEmail);

                // Update Process Costs
                List<ProcessCostsViewModel> list = (List<ProcessCostsViewModel>)Session["OldProcessCosts"];
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        db.AdminUpdateProcessCost(
                            item.ID,
                            item.ItemName,
                            item.ItemDescription,
                            item.Price,
                            item.SortOrder,
                            item.Active);
                    }
                }
                // Add Process Costs
                List<ProcessCostsViewModel> list2 = (List<ProcessCostsViewModel>)Session["NewProcessCosts"];
                if (list2 != null)
                {
                    foreach (var item in list2)
                    {
                        db.AdminAddProcessCost(
                            Convert.ToInt32(Session["CustomerID"]),
                            model.ProcessId,
                            item.ItemName,
                            item.ItemDescription,
                            item.Price,
                            item.SortOrder);
                    }
                }
                TempData["smsg"] = "The changes have been made successfully.";
                return RedirectToAction(nameof(ProcessManager));
            }
            ViewBag.Page = "Modify Process";
            return View("ActionProcess", model);
        }

        public JsonResult ProcessFormActiveSet(int id, bool isChecked)
        {
            int isActive = 0;
            if (isChecked) { isActive = 1; }

            db.ProcessFormActiveUpdate(id, isActive);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult NewProcessCosts(int id, string itemName, string itemDescription, decimal price, int sortOrder, int active)
        {
            List<ProcessCostsViewModel> list = (List<ProcessCostsViewModel>)Session["NewProcessCosts"];
            if (list != null)
            {
                var exists = list.Where(r => r.ID == id).FirstOrDefault();
                if (exists != null) { list.Remove(exists); }
            }
            else { list = new List<ProcessCostsViewModel>(); }

            var item = new ProcessCostsViewModel()
            {
                ID = id,
                ItemName = itemName,
                ItemDescription = itemDescription,
                Price = price,
                SortOrder = sortOrder,
                Active = active
            };
            list.Add(item);

            Session["NewProcessCosts"] = list;
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult OldProcessCosts(int id, string itemName, string itemDescription, decimal price, int sortOrder, int active, int processId)
        {
            List<ProcessCostsViewModel> list = (List<ProcessCostsViewModel>)Session["OldProcessCosts"];
            if (list != null)
            {
                var exists = list.Where(r => r.ID == id).FirstOrDefault();
                if (exists != null) { list.Remove(exists); }
            }
            else { list = new List<ProcessCostsViewModel>(); }

            var item = new ProcessCostsViewModel()
            {
                ID = id,
                ItemName = itemName,
                ItemDescription = itemDescription,
                Price = price,
                SortOrder = sortOrder,
                Active = active
            };

            var oldValue = db.GetProcessCosts(processId).Where(r => r.ID == id).FirstOrDefault();
            if (oldValue.ItemName != itemName
                || oldValue.ItemDescription != itemDescription
                || oldValue.Price != price
                || oldValue.SortOrder != sortOrder
                || oldValue.Active != active)
            { list.Add(item); }
            else { list.Remove(item); }

            Session["OldProcessCosts"] = list;
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Surveys(string currentFilter, string searchString)
        {
            ViewBag.AllowedRoles = new string[] { "Administrator", "UI Developer" };
            if (string.IsNullOrEmpty(searchString)) { searchString = currentFilter; }
            ViewData["CurrentFilter"] = searchString;

            var model = db.GetAdminSurveys(Convert.ToInt32(Session["CustomerID"]), searchString).ToList();
            return View(model);
        }

        public ActionResult AddSurvey()
        {
            ViewBag.AllowedRoles = new string[] { "Administrator", "UI Developer" };
            ViewBag.Page = "Add Survey/Questionnaire";
            var model = ProcessViewModel.Instance;
            model.NoEmailAuth = 1;

            List<SelectListItem> list = (from r in db.GetSurveyDirectories(null)
                                         select new SelectListItem
                                         {
                                             Value = r.KeyValue,
                                             Text = r.DisplayValue
                                         }).ToList();
            list.Insert(0, new SelectListItem { Text = "No selection", Value = "" });
            ViewData["HyperlinkDirectory"] = new SelectList(list, "Value", "Text");

            return View("ActionProcess", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSurvey(ProcessViewModel model)
        {
            if (ModelState.IsValid)
            {
                db.AddSurveyProcess(
                    Convert.ToInt32(Session["CustomerID"]),
                    model.ProcessName,
                    model.Summary,
                    model.GovernmentMailingAddress1,
                    model.PacketTitle,
                    model.GeneratesEmail,
                    model.ExternalRedirect,
                    model.FormListHeaderMessage,
                    model.WelcomeEmail,
                    model.UserSuppliesEmail,
                    model.NoEmailAuth,
                    model.HumanVerifyNeeded,
                    model.EmailSurveyImage,
                    model.EmailSurveyPhoneUse,
                    model.EmailSurveyNameUse,
                    model.EmailSurveyNameRequired,
                    model.EmailSurveyPhoneRequired,
                    model.EmailSurveyEmailRequired,
                    model.EmailSurveyStartHeaderText,
                    model.BeginWithoutRegistration,
                    model.HyperlinkDirectory,
                    model.QuestionnaireGeneratePDF);

                TempData["smsg"] = "The changes have been made successfully.";
                return RedirectToAction(nameof(Surveys));
            }
            List<SelectListItem> list = (from r in db.GetSurveyDirectories(null)
                                         select new SelectListItem
                                         {
                                             Value = r.KeyValue,
                                             Text = r.DisplayValue
                                         }).ToList();
            list.Insert(0, new SelectListItem { Text = "No selection", Value = "" });
            ViewData["HyperlinkDirectory"] = new SelectList(list, "Value", "Text", model.HyperlinkDirectory);

            ViewBag.Page = "Add Survey/Questionnaire";
            return View("ActionProcess", model);
        }

        public ActionResult UpdateSurvey(int id)
        {
            ViewBag.AllowedRoles = new string[] { "Administrator", "UI Developer" };
            var process = db.GetCustomerProcess(Convert.ToInt32(Session["CustomerID"]), id).FirstOrDefault();
            if (process == null) { return HttpNotFound(); }

            var model = new ProcessViewModel
            {
                ProcessId = id,
                FormilaeProcess = process.FormilaeProcess,
                ProcessName = process.ProcessName,
                Summary = process.Summary,
                GovernmentMailingAddress1 = process.GovernmentMailingAddress1,
                PacketTitle = process.PacketTitle,
                RequiresAdminApproval = process.RequiresAdminApproval,
                GeneratesEmail = process.GeneratesEmail,
                ExternalRedirect = process.ExternalRedirect,
                HideFromServicesPurchasedList = process.HideFromServicesPurchasedList,
                FormListHeaderMessage = process.FormListHeaderMessage,
                WelcomeEmail = process.WelcomeEmail,
                NoEmailAuth = process.NoEmailAuth,
                UserSuppliesEmail = process.UserSuppliesEmail,
                EmailSurveyEmailRequired = process.EmailSurveyEmailRequired,
                HumanVerifyNeeded = process.HumanVerifyNeeded,
                EmailSurveyNameUse = process.EmailSurveyNameUse,
                EmailSurveyNameRequired = process.EmailSurveyNameRequired,
                EmailSurveyPhoneUse = process.EmailSurveyPhoneUse,
                EmailSurveyPhoneRequired = process.EmailSurveyPhoneRequired,
                EmailSurveyStartHeaderText = process.EmailSurveyStartHeaderText,
                EmailSurveyImage = process.EmailSurveyImage,
                BeginWithoutRegistration = process.BeginWithoutRegistration,
                HyperlinkDirectory = process.HyperlinkDirectory,
                QuestionnaireGeneratePDF = process.QuestionnaireGeneratePDF,
                ProcessForms = db.GetSurveyProcessForms(id, 1, 1).ToList()
            };
            List<SelectListItem> list = (from r in db.GetSurveyDirectories(null)
                                         select new SelectListItem
                                         {
                                             Value = r.KeyValue,
                                             Text = r.DisplayValue
                                         }).ToList();
            list.Insert(0, new SelectListItem { Text = "No selection", Value = "" });
            ViewData["HyperlinkDirectory"] = new SelectList(list, "Value", "Text", process.HyperlinkDirectory);

            ViewBag.Page = "Modify Survey/Questionnaire";
            return View("ActionProcess", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateSurvey(int id, ProcessViewModel model)
        {
            if (id != model.ProcessId) { return HttpNotFound(); }

            if (ModelState.IsValid)
            {
                var process = db.GetCustomerProcess(Convert.ToInt32(Session["CustomerID"]), id).FirstOrDefault();
                if (process == null) { return HttpNotFound(); }
                var value = String.Empty;

                if (!String.IsNullOrEmpty(model.FormListHeaderMessage))
                {
                    value = model.FormListHeaderMessage;
                    value = Server.UrlDecode(value);
                    value = value.Replace("<b>", "<span style='font-weight: bold;'>");
                    value = value.Replace("</b>", "</span>");
                    value = value.Replace("<strong>", "<span style='font-weight: bold;'>");
                    value = value.Replace("</strong>", "</span>");
                    value = value.Replace("<span style=\"font-weight: bold;\"> </span>", "");
                    if (value == "<p><br></p>")
                    {
                        value = "";
                    }

                    model.FormListHeaderMessage = value;
                }

                if (!String.IsNullOrEmpty(model.WelcomeEmail))
                {
                    value = model.WelcomeEmail;
                    value = Server.UrlDecode(value);
                    value = value.Replace("<b>", "<span style='font-weight: bold;'>");
                    value = value.Replace("</b>", "</span>");
                    value = value.Replace("<strong>", "<span style='font-weight: bold;'>");
                    value = value.Replace("</strong>", "</span>");
                    value = value.Replace("<span style=\"font-weight: bold;\"> </span>", "");
                    if (value == "<p><br></p>")
                    {
                        value = "";
                    }

                    model.WelcomeEmail = value;
                }

                if (!String.IsNullOrEmpty(model.EmailSurveyStartHeaderText))
                {
                    value = model.EmailSurveyStartHeaderText;
                    value = Server.UrlDecode(value);
                    value = value.Replace("<b>", "<span style='font-weight: bold;'>");
                    value = value.Replace("</b>", "</span>");
                    value = value.Replace("<strong>", "<span style='font-weight: bold;'>");
                    value = value.Replace("</strong>", "</span>");
                    value = value.Replace("<span style=\"font-weight: bold;\"> </span>", "");
                    if (value == "<p><br></p>")
                    {
                        value = "";
                    }

                    model.EmailSurveyStartHeaderText = value;
                }

                db.UpdateSurveyProcess(
                    Convert.ToInt32(Session["CustomerID"]),
                    model.ProcessId,
                    model.ProcessName,
                    model.Summary,
                    model.GovernmentMailingAddress1,
                    model.PacketTitle,
                    model.RequiresAdminApproval,
                    model.GeneratesEmail,
                    model.ExternalRedirect,
                    model.FormListHeaderMessage,
                    model.WelcomeEmail,
                    model.UserSuppliesEmail,
                    model.NoEmailAuth,
                    model.HumanVerifyNeeded,
                    model.EmailSurveyImage,
                    model.EmailSurveyPhoneUse,
                    model.EmailSurveyNameUse,
                    model.EmailSurveyNameRequired,
                    model.EmailSurveyPhoneRequired,
                    model.EmailSurveyEmailRequired,
                    model.EmailSurveyStartHeaderText,
                    model.BeginWithoutRegistration,
                    model.HyperlinkDirectory,
                    model.QuestionnaireGeneratePDF);

                TempData["smsg"] = "The changes have been made successfully.";
                return RedirectToAction(nameof(Surveys));
            }
            List<SelectListItem> list = (from r in db.GetSurveyDirectories(null)
                                         select new SelectListItem
                                         {
                                             Value = r.KeyValue,
                                             Text = r.DisplayValue
                                         }).ToList();
            list.Insert(0, new SelectListItem { Text = "No selection", Value = "" });
            ViewData["HyperlinkDirectory"] = new SelectList(list, "Value", "Text", model.HyperlinkDirectory);

            ViewBag.Page = "Modify Survey/Questionnaire";
            return View("ActionProcess", model);
        }

        public ActionResult UpdateProcessForm(int id)
        {
            ProcessFormGet model = new ProcessFormGet();
            using (var db1 = new MyDbContext())
            {
                model = db1.GetProcessForm(id).FirstOrDefault();
            }

            if (model == null)
            {
                return RedirectToAction("Notfound", "Home");
            }
            return View("ActionProcessForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProcessForm(int id, ProcessFormGet model)
        {
            if (id != model.ID) { return HttpNotFound(); }

            if (ModelState.IsValid)
            {
                ReplaceHtml(model.FormMessageTop);
                ReplaceHtml(model.ResultsMessageTop);
                ReplaceHtml(model.FormApprovedOpeningEmailBodyMsg);
                ReplaceHtml(model.ApprovedEmailIfYouHaveQuestionsFooter);

                if (model.IsSurveyOrQuestionnaire != 1)
                {
                    using (var db1 = new MyDbContext())
                    {
                        db1.ProcessFormUpdate(
                            model.ID,
                            model.FormMessageTop,
                            model.FormMessageTopPlain,
                            model.DisplayResults,
                            model.ResultsMessageTop,
                            model.AutoAddToService,
                            model.RequiresAdminApproval,
                            model.AllowAdditionalAttachment,
                            model.Required,
                            model.ServiceListDisplay,
                            model.FormApprovedOpeningEmailBodyMsg,
                            model.ApprovedEmailIfYouHaveQuestionsFooter);
                    }
                    return RedirectToAction("UpdateProcess", "ApplicationSetup", new { id = model.ProcessID });
                }
                else
                {
                    using (var db1 = new MyDbContext())
                    {
                        db1.ProcessFormUpdate(model.ID, model.FormMessageTop, model.FormMessageTopPlain, model.DisplayResults, model.ResultsMessageTop);
                    }
                    return RedirectToAction("UpdateSurvey", "ApplicationSetup", new { id = model.ProcessID });
                }
            }
            return View("ActionProcessForm", model);
        }

        private string ReplaceHtml(string text)
        {
            var value = String.Empty;

            if (!String.IsNullOrEmpty(text))
            {
                value = text;
                value = Server.UrlDecode(value);
                value = value.Replace("<b>", "<span style='font-weight: bold;'>");
                value = value.Replace("</b>", "</span>");
                value = value.Replace("<strong>", "<span style='font-weight: bold;'>");
                value = value.Replace("</strong>", "</span>");
                value = value.Replace("<span style=\"font-weight: bold;\"> </span>", "");
                if (value == "<p><br></p>")
                {
                    value = "";
                }

                text = value;
            }
            return text;
        }

        public JsonResult GetEmailSurveyImage(int noEmailAuth, int userSuppliesEmail)
        {
            if (noEmailAuth == 0 && userSuppliesEmail == 0)
            {
                var list = db.GetSocialMediaThumbImages(Convert.ToInt32(Session["CustomerID"])).ToList();
                return Json(new { success = true, text = "Facebook Post Image", data = list }, JsonRequestBehavior.AllowGet);
            }
            else if (userSuppliesEmail == 1)
            {
                var list = db.GetSurveyStartPageImages(Convert.ToInt32(Session["CustomerID"])).ToList();
                return Json(new { success = true, text = "Start Page Image", data = list }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Events()
        {
            using (var db1 = new MyDbContext())
            {
                var model = db1.GetEvents(Convert.ToInt32(Session["CustomerID"])).ToList();
                if (model == null)
                {
                    return RedirectToAction("Notfound", "Home");
                }
                return View(model);
            }
        }

        public ActionResult AddEvent()
        {
            var model = new EventGet()
            {
                Duration = 60,
                Active = 1,
            };
            using (var db1 = new MyDbContext())
            {
                List<SelectListItem> list = (from r in db1.EventBlockedDaysForwardGet(1)
                                             select new SelectListItem
                                             {
                                                 Value = r.ID.ToString(),
                                                 Text = r.Label
                                             }).ToList();
                ViewData["BlockedDaysForwardID"] = new SelectList(list, "Value", "Text", 11).ToList();
            }
            ViewBag.Page = "Add Event";
            return View("ActionEvent", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddEvent(EventGet model)
        {
            if (ModelState.IsValid)
            {
                using (var db1 = new MyDbContext())
                {
                    db1.EventAdd(
                        Convert.ToInt32(Session["userID"]),
                        model.EventName,
                        model.Description,
                        model.Location,
                        model.LocationDetails,
                        model.Duration,
                        model.Price,
                        -1,
                        model.ConfirmationMsgFooter,
                        model.CancellationMsgFooter,
                        model.ForwardingWebAddress,
                        model.BlockedDaysForwardID,
                        model.Active);

                        return RedirectToAction(nameof(Events), new { smsg = "The changes have been made successfully." });
                }
            }
            using (var db1 = new MyDbContext())
            {
                List<SelectListItem> list = (from r in db1.EventBlockedDaysForwardGet(1)
                                             select new SelectListItem
                                             {
                                                 Value = r.ID.ToString(),
                                                 Text = r.Label
                                             }).ToList();
                ViewData["BlockedDaysForwardID"] = new SelectList(list, "Value", "Text", model.BlockedDaysForwardID).ToList();
            }
            ViewBag.Page = "Add Event";
            return View("ActionEvent", model);
        }

        public ActionResult UpdateEvent(int? id)
        {
            if (id == null) { return HttpNotFound(); }

            var events = db.GetEvent(id, Convert.ToInt32(Session["CustomerID"]), 0).FirstOrDefault();
            if (events == null) { return HttpNotFound(); }

            var model = new EventGet()
            {
                ID = events.ID,
                EventName = events.EventName,
                Description = events.Description,
                Location = events.Location,
                LocationDetails = events.LocationDetails,
                Duration = events.Duration,
                Price = events.Price,
                ConfirmationMsgFooter= events.ConfirmationMsgFooter,
                CancellationMsgFooter= events.CancellationMsgFooter,
                ForwardingWebAddress= events.ForwardingWebAddress,
                BlockedDaysForwardID= events.BlockedDaysForwardID,
                Active = events.Active
            };
            using (var db1 = new MyDbContext())
            {
                List<SelectListItem> list = (from r in db1.EventBlockedDaysForwardGet(1)
                                             select new SelectListItem
                                             {
                                                 Value = r.ID.ToString(),
                                                 Text = r.Label
                                             }).ToList();
                ViewData["BlockedDaysForwardID"] = new SelectList(list, "Value", "Text", events.BlockedDaysForwardID).ToList();
            }
            ViewBag.Page = "Update Event";
            return View("ActionEvent", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateEvent(int id, EventGet model)
        {
            if (id != model.ID) { return HttpNotFound(); }

            if (ModelState.IsValid)
            {
                using (var db1 = new MyDbContext())
                {
                    var events = db1.GetEvent(id, Convert.ToInt32(Session["CustomerID"]), 0).FirstOrDefault();
                    if (events == null) { return HttpNotFound(); }

                    db1.EventUpdate(
                        model.ID,
                        Convert.ToInt32(Session["userID"]),
                        model.EventName,
                        model.Description,
                        model.Location,
                        model.LocationDetails,
                        model.Duration,
                        model.Price,
                        -1,
                        model.ConfirmationMsgFooter,
                        model.CancellationMsgFooter,
                        model.ForwardingWebAddress,
                        model.BlockedDaysForwardID,
                        model.Active);

                    return RedirectToAction(nameof(Events), new { smsg = "The changes have been made successfully." });
                }
            }
            using (var db1 = new MyDbContext())
            {
                List<SelectListItem> list = (from r in db1.EventBlockedDaysForwardGet(1)
                                             select new SelectListItem
                                             {
                                                 Value = r.ID.ToString(),
                                                 Text = r.Label
                                             }).ToList();
                ViewData["BlockedDaysForwardID"] = new SelectList(list, "Value", "Text", model.BlockedDaysForwardID).ToList();
            }
            ViewBag.Page = "Update Event";
            return View("ActionEvent", model);
        }

        public JsonResult DeleteEvent(int id)
        {
            using (var db1 = new MyDbContext())
            {
                var events = db1.GetEvent(id, Convert.ToInt32(Session["CustomerID"]), 0).FirstOrDefault();
                if (events == null) { return Json(false); }

                db1.EventDelete(id);
            }
            TempData["smsg"] = "The changes have been made successfully.";
            return Json(true);
        }
    }
}
