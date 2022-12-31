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
using System.ComponentModel.DataAnnotations;
using MySql.Data.Types;

namespace Developer4.Controllers
{
    public class UserSetupController : Controller
    {
        private MyDbContext db = new MyDbContext();

        // Reserved: Do not use Index controller for new code
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Users(string currentFilter, string searchString)
        {
            if (string.IsNullOrEmpty(searchString)) { searchString = currentFilter; }
            ViewData["CurrentFilter"] = searchString;

            var model = db.GetClients(Convert.ToInt32(Session["CustomerID"]), searchString).ToList();
            return View(model);
        }

        public JsonResult UserActiveSet(int id, bool isChecked)
        {
            int isActive = 0;
            if (isChecked) { isActive = 1; }

            db.ClientActiveUpdate(id, isActive);
            return Json(true);
        }

        public JsonResult RolesInUser(int id)
        {
            if (id == 0) { return Json(JsonRequestBehavior.DenyGet); }

            var model = db.GetAdminUserRoles(id, 1);
            return Json(new { success = true, model = model }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PermissionsInRole(int clientId, int roleId)
        {
            if (clientId == 0 || roleId == 0) { return Json(JsonRequestBehavior.DenyGet); }

            var model = db.GetUserPermissions(clientId, 1, roleId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        // GET: UserSetup/AddUser
        public ActionResult AddUser()
        {
            ClientViewModel model = new ClientViewModel();
            model.AllRoles = db.GetAdminRoles(Convert.ToInt32(Session["CustomerID"]), "").Select(r => new SelectListItem()
            {
                Value = r.ID.ToString(),
                Text = r.RoleName
            }).ToList();
            List<SelectListItem> list = (from r in db.GetStateList(null)
                                         select new SelectListItem
                                         {
                                             Value = r.KeyValue,
                                             Text = r.DisplayValue
                                         }).ToList();
            list.Insert(0, new SelectListItem { Text = "No selection", Value = "" });
            ViewData["State"] = new SelectList(list, "Value", "Text");

            ViewBag.Page = "Add User";
            return View("ActionUser", model);
        }

        // POST: UserSetup/AddUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUser([Bind(Exclude = "ID")] ClientViewModel model)
        {
            if (ModelState.IsValid)
            {
                MySqlDateTime dob = new MySqlDateTime();
                if (!string.IsNullOrEmpty(model.DOB)) { dob = MySqlCDateTimeUtil.DateStrToMySQLDateTime(model.DOB); }

                db.ClientAdd(
                    Convert.ToInt32(Session["CustomerID"]),
                    model.FirstName,
                    model.LastName,
                    model.PhoneNumber,
                    model.Email,
                    model.Active,
                    model.Admin,
                    model.HomePhone,
                    model.Address,
                    model.City,
                    model.State,
                    model.ZipCode,
                    model.Occupation,
                    model.EmergencyContact,
                    dob,
                    model.PreferredMethodOfComm);

                if (model.Roles != null)
                {
                    var clientId = db.GetClients(Convert.ToInt32(Session["CustomerID"]), "").OrderByDescending(r => r.ID).FirstOrDefault().ID;
                    db.ClientRolesAdd(clientId, String.Join(",", model.Roles));
                }
                return RedirectToAction(nameof(Users), new { smsg = "The changes have been made successfully." }); // Work with query string (_AlertBox)
            }
            List<SelectListItem> list = (from r in db.GetStateList(null)
                                         select new SelectListItem
                                         {
                                             Value = r.KeyValue,
                                             Text = r.DisplayValue
                                         }).ToList();
            list.Insert(0, new SelectListItem { Text = "No selection", Value = "" });
            ViewData["State"] = new SelectList(list, "Value", "Text", model.State);

            ViewBag.Page = "Add User";
            return View("ActionUser", model);
        }

        // GET: UserSetup/UpdateUser
        public ActionResult UpdateUser(int? id)
        {
            if (id == null) { return HttpNotFound(); }

            var client = db.GetClient(id).FirstOrDefault();
            if (client == null) { return HttpNotFound(); }

            var model = new ClientViewModel
            {
                ID = client.ID,
                FirstName = client.FirstName,
                LastName = client.LastName,
                PhoneNumber = client.PhoneNumber,
                Email = client.Email,
                Active = client.Active,
                Admin = client.Admin,
                HomePhone = client.HomePhone,
                Address = client.Address,
                City = client.City,
                State = client.State,
                ZipCode = client.ZipCode,
                Occupation = client.Occupation,
                EmergencyContact = client.EmergencyContact,
                DOB = Convert.ToDateTime(client.DOB).ToShortDateString(),
                PreferredMethodOfComm = client.PreferredMethodOfComm,
                Avatar = client.Avatar,
                LastLoginDate = client.LastLoginDate,
                LastLoginIP = client.LastLoginIP,
                AllRoles = db.GetAdminUserRoles(client.ID).Select(r => new SelectListItem()
                {
                    Value = r.ID.ToString(),
                    Text = r.RoleName,
                    Selected = r.Selected == "Selected" ? true : false
                }).ToList()
            };
            List<SelectListItem> list = (from r in db.GetStateList(null)
                                         select new SelectListItem
                                         {
                                             Value = r.KeyValue,
                                             Text = r.DisplayValue
                                         }).ToList();
            list.Insert(0, new SelectListItem { Text = "No selection", Value = "" });
            ViewData["State"] = new SelectList(list, "Value", "Text", client.State);

            ViewBag.Page = "Update User";
            return View("ActionUser", model);
        }

        // POST: UserSetup/UpdateUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateUser(int id, ClientViewModel model)
        {
            if (id != model.ID) { return HttpNotFound(); }

            if (ModelState.IsValid)
            {
                var client = db.GetClient(id).FirstOrDefault();
                if (client == null) { return HttpNotFound(); }

                MySqlDateTime dob = new MySqlDateTime();
                if (!string.IsNullOrEmpty(model.DOB)) { dob = MySqlCDateTimeUtil.DateStrToMySQLDateTime(model.DOB); }

                db.ClientUpdate(
                    model.ID,
                    model.FirstName,
                    model.LastName,
                    model.PhoneNumber,
                    model.Email,
                    model.Active,
                    model.Admin,
                    model.HomePhone,
                    model.Address,
                    model.City,
                    model.State,
                    model.ZipCode,
                    model.Occupation,
                    model.EmergencyContact,
                    dob,
                    model.PreferredMethodOfComm);

                if (model.Roles != null)
                {
                    db.ClientRolesAdd(client.ID, String.Join(",", model.Roles));
                }
                TempData["smsg"] = "The changes have been made successfully."; // Work with TempData (_AlertBox1)
                return RedirectToAction(nameof(Users));
            }
            List<SelectListItem> list = (from r in db.GetStateList(null)
                                         select new SelectListItem
                                         {
                                             Value = r.KeyValue,
                                             Text = r.DisplayValue
                                         }).ToList();
            list.Insert(0, new SelectListItem { Text = "No selection", Value = "" });
            ViewData["State"] = new SelectList(list, "Value", "Text", model.State);

            ViewBag.Page = "Update User";
            return View("ActionUser", model);
        }

        public JsonResult DeleteUser(int id)
        {
            var client = db.GetClient(id).FirstOrDefault();
            if (client == null) { return Json(false); }

            db.ClientDelete(id);
            return Json(true);
        }

        public JsonResult UploadAvatar(int id, string avatar)
        {
            string path = Server.MapPath("~/assets/images/users");
            if (!Directory.Exists(path)) { Directory.CreateDirectory(path); };
            string fileName = "avatar-" + id + ".jpg";

            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(avatar.Split(',')[1])))
            {
                ms.Position = 0;
                byte[] bytes = ms.ToArray();
                System.IO.File.WriteAllBytes(Path.Combine(path, fileName), bytes);
            }
            db.ClientAvatarUpdate(id, fileName);
            return Json(new { success = true, src = "/assets/images/users/" + fileName }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteAvatar(int id)
        {
            db.ClientAvatarDelete(id);
            return Json(new { success = true, src = "/assets/images/users/no-avatar.jpg" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RolesManager(string currentFilter, string searchString)
        {
            if (string.IsNullOrEmpty(searchString)) { searchString = currentFilter; }
            ViewData["CurrentFilter"] = searchString;

            var model = db.GetAdminRoles(Convert.ToInt32(Session["CustomerID"]), searchString).ToList();
            return View(model);
        }

        public JsonResult UsersInRole(int id)
        {
            if (id == 0) { return Json(JsonRequestBehavior.DenyGet); }

            var model = db.GetRoleUsers(id, Convert.ToInt32(Session["CustomerID"]));
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RoleManagerEdit(int id, string roleName, string msg1, string msg2)
        {
            RoleManagerViewModel model = new RoleManagerViewModel();
            model.AdminRoleUsersGet = db.GetAllRoleUsers(id, Convert.ToInt32(Session["CustomerID"])).ToList();
            model.AdminRolePermissionsGet = db.GetAllRolePermissions(id, Convert.ToInt32(Session["CustomerID"])).ToList();

            ViewData["RoleId"] = id;
            ViewData["RoleName"] = roleName;
            ViewData["Message1"] = msg1;
            ViewData["Message2"] = msg2;
            Session["UserChanges"] = null;
            Session["PermissionChanges"] = null;
            return View(model);
        }

        public JsonResult UserChanges(int id, int roleId, int isChecked)
        {
            List<AdminRoleUsersGet> list = (List<AdminRoleUsersGet>)Session["UserChanges"];
            if (list != null)
            {
                var exists = list.Where(r => r.ID == id).FirstOrDefault();
                if (exists != null) { list.Remove(exists); }
            }
            else { list = new List<AdminRoleUsersGet>(); }

            var item = new AdminRoleUsersGet()
            {
                ID = id,
                Checked = isChecked
            };

            var oldValue = db.GetAllRoleUsers(roleId, Convert.ToInt32(Session["CustomerID"])).Where(r => r.ID == id).Select(r => r.Checked).FirstOrDefault();
            if (oldValue != isChecked) { list.Add(item); }
            else { list.Remove(item); }

            Session["UserChanges"] = list;
            return Json(true);
        }

        public ActionResult UserSaveChanges(int roleId, string roleName)
        {
            List<AdminRoleUsersGet> list = (List<AdminRoleUsersGet>)Session["UserChanges"];

            if (list != null)
            {
                foreach (var item in list)
                {
                    db.RoleUserUpdate(item.ID, roleId, item.Checked);
                }
            }
            return RedirectToAction(nameof(RoleManagerEdit), new { id = roleId, roleName = roleName, msg1 = "The update was successful!" });
        }

        public JsonResult PermissionChanges(int roleId, int permissionId, int isChecked)
        {
            List<AdminRolePermissionsGet> list = (List<AdminRolePermissionsGet>)Session["PermissionChanges"];
            if (list != null)
            {
                var exists = list.Where(r => r.PermissionID == permissionId).FirstOrDefault();
                if (exists != null) { list.Remove(exists); }
            }
            else { list = new List<AdminRolePermissionsGet>(); }

            var item = new AdminRolePermissionsGet()
            {
                PermissionID = permissionId,
                Checked = isChecked
            };

            var oldValue = db.GetAllRolePermissions(roleId, Convert.ToInt32(Session["CustomerID"])).Where(r => r.PermissionID == permissionId).Select(r => r.Checked).FirstOrDefault();
            if (oldValue != isChecked) { list.Add(item); }
            else { list.Remove(item); }

            Session["PermissionChanges"] = list;
            return Json(true);
        }

        public ActionResult PermissionSaveChanges(int roleId, string roleName)
        {
            List<AdminRolePermissionsGet> list = (List<AdminRolePermissionsGet>)Session["PermissionChanges"];

            if (list != null)
            {
                foreach (var item in list)
                {
                    db.RolePermissionUpdate(roleId, item.PermissionID, item.Checked);
                }
            }
            return RedirectToAction(nameof(RoleManagerEdit), new { id = roleId, roleName = roleName, msg2 = "The update was successful!" });
        }

        public ActionResult UserUploadTool()
        {
            if (Request.QueryString["postBack"] != "1")
            {
                Session["FormQuestionsUpload_NoFileSelected"] = null;
                Session["FormQuestionsUpload_Status"] = null;
                Session["FormQuestionsUpload_Msg"] = null;
            }

            ViewBag.MenuItem = "User Administration";
            ViewBag.Page = "User Upload Tool";
            return View();
        }

        [HttpPost]
        public ActionResult UserUploadTool(FormCollection form, HttpPostedFileBase postedFile)
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
                Session["ClientsUpload_NoFileSelected"] = noFileSelected;
                return Json(new { status = status, Message = statusMsg });
            }

            List<TempClient> model = new List<TempClient>();
            int lines = 0;
            string filePath = string.Empty;

            if (postedFile != null)
            {
                if (postedFile.ContentLength > 0)
                {
                    noFileSelected = false;
                    string path = Server.MapPath("~/Content/Files/Uploads/");
                    if (!Directory.Exists(path)) { Directory.CreateDirectory(path); };
                    
                    filePath = Path.Combine(path, Path.GetFileName(postedFile.FileName));
                    //string extension = Path.GetExtension(postedFile.FileName);
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

                    while (!parser.EndOfData)
                    {
                        int i = 0;
                        TempClient client = new TempClient();
                        client.FirstName = String.Empty;
                        client.LastName = String.Empty;
                        client.PhoneNumber = String.Empty;
                        client.Email = String.Empty;
                        client.IsAdministrator = String.Empty;
                        client.ErrMsg = String.Empty;

                        string[] fields = parser.ReadFields();
                        foreach (string cell in fields)
                        {
                            string value = cell.Trim();
                            if (i == 0)
                            {
                                client.FirstName = value;
                                if (String.IsNullOrEmpty(value))
                                {
                                    client.ErrMsg = client.ErrMsg + (!String.IsNullOrEmpty(client.ErrMsg) ? "{br}" : "") + "FirstName is required.";
                                }
                                else if (value.Length > 50)
                                {
                                    client.ErrMsg = client.ErrMsg + (!String.IsNullOrEmpty(client.ErrMsg) ? "{br}" : "") + "FirstName too long.";
                                }
                            }
                            if (i == 1)
                            {
                                client.LastName = value;
                                if (String.IsNullOrEmpty(value))
                                {
                                    client.ErrMsg = client.ErrMsg + (!String.IsNullOrEmpty(client.ErrMsg) ? "{br}" : "") + "LastName is required.";
                                }
                                else if (value.Length > 50)
                                {
                                    client.ErrMsg = client.ErrMsg + (!String.IsNullOrEmpty(client.ErrMsg) ? "{br}" : "") + "LastName too long.";
                                }
                            }
                            if (i == 2)
                            {
                                client.PhoneNumber = value;
                                if (!String.IsNullOrEmpty(value) && value.Length > 20)
                                {
                                    client.ErrMsg = client.ErrMsg + (!String.IsNullOrEmpty(client.ErrMsg) ? "{br}" : "") + "PhoneNumber too long.";
                                }
                            }
                            if (i == 3)
                            {
                                client.Email = value;
                                var email = new EmailAddressAttribute();
                                if (String.IsNullOrEmpty(value))
                                {
                                    client.ErrMsg = client.ErrMsg + (!String.IsNullOrEmpty(client.ErrMsg) ? "{br}" : "") + "Email is required.";
                                }
                                else if (!email.IsValid(value))
                                {
                                    client.ErrMsg = client.ErrMsg + (!String.IsNullOrEmpty(client.ErrMsg) ? "{br}" : "") + "Email address is invalid.";
                                }
                                else if (value.Length > 75)
                                {
                                    client.ErrMsg = client.ErrMsg + (!String.IsNullOrEmpty(client.ErrMsg) ? "{br}" : "") + "Email too long.";
                                }
                                else if (!String.IsNullOrEmpty(value))
                                {
                                    var lookClient = db.Client.Where(f => f.Email == value).FirstOrDefault();
                                    if (lookClient != null && lookClient.Email.ToLower() == value.ToLower())
                                    {
                                        client.ErrMsg = client.ErrMsg + (!String.IsNullOrEmpty(client.ErrMsg) ? "{br}" : "") + "This Email is already exist.";
                                    }
                                }
                            }
                            if (i == 4)
                            {
                                client.IsAdministrator = value;
                                if (!String.IsNullOrEmpty(value) && value == "Y")
                                {
                                    client.IsAdministrator = "1";
                                }
                                else
                                {
                                    client.IsAdministrator = "0";
                                }
                            }
                            i++;
                        }
                        model.Add(client);
                        lines++;
                    }
                }
            }
            bool noFilesInUpload = lines == 0;
            var linesErrCount = model.Where(q => q.ErrMsg != "").Count();

            if (noFilesInUpload)
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
                if (status != "failed")
                {
                    var emails = model
                                    .GroupBy(q => q.Email)
                                    .Select(g => new { Count = g.Select(x => x.Email).Distinct().Count() })
                                    .ToList().Count;
                    if (emails != lines)
                    {
                        status = "failed";
                        statusMsg = "Duplicate Email values detected.";
                    }
                }
            }

            if (status != "failed")
            {
                try
                {
                    db.TempClientDelete(Convert.ToInt32(Session["userID"]));
                    foreach (var item in model)
                    {
                        db.TempUserAdd(Convert.ToInt32(Session["userID"]), item.FirstName, item.LastName, item.PhoneNumber, item.Email, int.Parse(item.IsAdministrator));
                    }
                    db.TempClientExecute(Convert.ToInt32(Session["userID"]));
                }
                catch (Exception e)
                {
                    status = "failed";
                    statusMsg = "The database didn't like the results insert: " + e.Message;
                }
            }
            Session["ClientsUpload_Clients"] = model;
            Session["ClientsUpload_NoFileSelected"] = noFileSelected;
            Session["ClientsUpload_Status"] = status;
            Session["ClientsUpload_Msg"] = statusMsg;

            return Json(new { status = status, Message = statusMsg });
        }

        // GET: UserSetup/MyAccount
        public ActionResult MyAccount(int? id)
        {
            if (id == null) { return HttpNotFound(); }

            var customer = db.GetCustomer(id).FirstOrDefault();
            if (customer == null) { return HttpNotFound(); }

            var model = new CustomerGet
            {
                ID = customer.ID,
                CustomerCode = customer.CustomerCode,
                BusinessName = customer.BusinessName,
                BusinessSubTitle = customer.BusinessSubTitle,
                ContactFirstName = customer.ContactFirstName,
                ContactLastName = customer.ContactLastName,
                ContactEmail = customer.ContactEmail,
                Address = customer.Address,
                City = customer.City,
                State = customer.State,
                Zip = customer.Zip,
                Phone = customer.Phone,
                Province = customer.Province,
                PostalCode = customer.PostalCode,
                Country = customer.Country,
                LogoFile = customer.LogoFile,
                AboutUsText = customer.AboutUsText
            };
            List<SelectListItem> list = (from r in db.GetStateList(id)
                                         select new SelectListItem
                                         {
                                             Value = r.KeyValue,
                                             Text = r.DisplayValue
                                         }).ToList();
            list.Insert(0, new SelectListItem { Text = "No selection", Value = "" });
            ViewData["State"] = new SelectList(list, "Value", "Text", customer.State);
            
            if (Request.UrlReferrer != null)
            {
                model.returnUrl = Request.UrlReferrer.ToString();
            }
            else
            {
                model.returnUrl = Url.Content("~/");
            }
            return View(model);
        }

        // POST: UserSetup/MyAccount
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MyAccount(int id, CustomerGet model)
        {
            if (id != model.ID) { return HttpNotFound(); }

            var customer = db.GetCustomer(id).FirstOrDefault();
            if (customer == null) { return HttpNotFound(); }

            if (ModelState.IsValid)
            {
                db.CustomerUpdate(
                    model.ID,
                    model.BusinessName,
                    model.BusinessSubTitle,
                    model.ContactFirstName,
                    model.ContactLastName,
                    model.ContactEmail,
                    model.Address,
                    model.City,
                    model.State,
                    model.Zip,
                    model.Phone,
                    model.Province,
                    model.PostalCode,
                    model.Country,
                    model.LogoFile,
                    model.AboutUsText);

                TempData["smsg"] = "The changes have been made successfully.";
                return Redirect(model.returnUrl);
            }
            List<SelectListItem> list = (from r in db.GetStateList(id)
                                         select new SelectListItem
                                         {
                                             Value = r.KeyValue,
                                             Text = r.DisplayValue
                                         }).ToList();
            list.Insert(0, new SelectListItem { Text = "No selection", Value = "" });
            ViewData["State"] = new SelectList(list, "Value", "Text", model.State);
            return View(model);
        }

        public JsonResult UploadLogo(int id, string logo, string extension)
        {
            string path = Server.MapPath("~/assets/images/logos");
            if (!Directory.Exists(path)) { Directory.CreateDirectory(path); };
            string fileName = "logo-" + id + "." + extension;

            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(logo.Split(',')[1])))
            {
                ms.Position = 0;
                byte[] bytes = ms.ToArray();
                System.IO.File.WriteAllBytes(Path.Combine(path, fileName), bytes);
            }
            return Json(new { success = true, logoName = fileName, src = "/assets/images/logos/" + fileName }, JsonRequestBehavior.AllowGet);
        }
    }
}