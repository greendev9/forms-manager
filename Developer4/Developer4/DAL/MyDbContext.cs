using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using SharedAssemblies.Models;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using MySql.Data.MySqlClient;
using System.Data;
using System;
using System.Text;
using System.Collections.Generic;
using SharedAssemblies.Content.AppCode;

namespace SharedAssemblies.DAL
{
    public class MyDbContext : DbContext
    {
        public MyDbContext()
          : base("MyDbContextConnectionString")
        {
            Database.SetInitializer<MyDbContext>(new MyDbInitializer());
        }

        #region declarations
        public DbSet<Setting> Setting { get; set; }
        public DbSet<Setting_Category> SettingCategory { get; set; }
        public DbSet<Customer_Setting> CustomerSetting { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<Client> Client { get; set; }
        public DbSet<Glossary> Glossary { get; set; }
        public DbSet<Customer_Glossary> CustomerGlossary { get; set; }
        public DbSet<Form> Form { get; set; }
        public DbSet<Form_Question> FormQuestion { get; set; }

        // Stored procedures
        public DbSet<CustomerSettingsGet> sp_CustomerSettingsGet { get; set; }
        public DbSet<CustomerGlossaryItemsGet> sp_CustomerGlossaryItemsGet { get; set; }
        public DbSet<CustomerGlossaryValueGet> sp_CustomerGlossaryValueGet { get; set; }
        public DbSet<AdminClientsGet> sp_AdminClientsGet { get; set; }
        public DbSet<AdminRoleUsersGet> sp_AdminRoleUsersGet { get; set; }
        public DbSet<AdminRolePermissionsGet> sp_AdminRolePermissionsGet { get; set; }
        public DbSet<AdminRolesGet> sp_AdminRolesGet { get; set; }
        public DbSet<AdminLookupsGet> sp_AdminLookupsGet { get; set; }
        public DbSet<AdminLookupListFormsGet> sp_AdminLookupListFormsGet { get; set; }
        public DbSet<AdminLookupItemsGet> sp_AdminLookupItemsGet { get; set; }
        public DbSet<AdminUserRolesGet> sp_AdminUserRolesGet { get; set; }
        #endregion

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Types().Configure(entity => entity.ToTable(entity.ClrType.Name.ToLower()));
        }

        // Stored procedure calls
        public virtual ObjectResult<CustomerSettingsGet> GetCustomerSettings(Nullable<int> customerId, Nullable<int> forClientSession, string categoryName, string settingName)
        {
            return GetCustomerSettings(customerId, forClientSession, categoryName, settingName, String.Empty);
        }

        public virtual ObjectResult<CustomerSettingsGet> GetCustomerSettings(Nullable<int> customerId, Nullable<int> forClientSession, string categoryName, string settingName, string settingNames)
        {
            MySqlParameter customerIDParam = new MySqlParameter("vCustomerID", MySqlDbType.Int32);
            customerIDParam.Direction = ParameterDirection.Input;
            customerIDParam.Value = customerId;

            MySqlParameter forClientSessionParam = new MySqlParameter("vForClientSession", MySqlDbType.Int32);
            forClientSessionParam.Direction = ParameterDirection.Input;
            forClientSessionParam.Value = forClientSession;

            MySqlParameter categoryNameParam = new MySqlParameter("vCategoryName", MySqlDbType.String);
            categoryNameParam.Direction = ParameterDirection.Input;
            categoryNameParam.Value = categoryName;

            MySqlParameter settingNameParam = new MySqlParameter("vSettingName", MySqlDbType.String);
            settingNameParam.Direction = ParameterDirection.Input;
            settingNameParam.Value = settingName;

            MySqlParameter settingNamesParam = new MySqlParameter("vSettingNames", MySqlDbType.String);
            settingNamesParam.Direction = ParameterDirection.Input;
            settingNamesParam.Value = settingNames;

            MySqlParameter dataTypeParam = new MySqlParameter("vDataType", MySqlDbType.String);
            dataTypeParam.Direction = ParameterDirection.Input;
            dataTypeParam.Value = "";   // Not in use right now (I only using for when I'm debugging in Workbench for now)

            MySqlParameter[] spParams = new MySqlParameter[] {
                customerIDParam,
                forClientSessionParam,
                categoryNameParam,
                settingNameParam,
                settingNamesParam,
                dataTypeParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL customersettingsget(@vCustomerID, @vForClientSession, @vCategoryName, @vSettingName, @vSettingNames, @vDataType)");

            string commandText = sb.ToString();

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<CustomerSettingsGet>(commandText, spParams);
        }

        public virtual ObjectResult<CustomerGlossaryValueGet> GetCustomerGlossaryValue(Nullable<int> customerId, string lookupKey)
        {
            MySqlParameter customerIDParam = new MySqlParameter("vCustomerID", MySqlDbType.Int32);
            customerIDParam.Direction = ParameterDirection.Input;
            customerIDParam.Value = customerId;

            MySqlParameter lookupKeyParam = new MySqlParameter("vLookupKey", MySqlDbType.String);
            lookupKeyParam.Direction = ParameterDirection.Input;
            lookupKeyParam.Value = lookupKey;

            MySqlParameter[] spParams = new MySqlParameter[] {
                customerIDParam,
                lookupKeyParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL customerglossaryvalueget(@vCustomerID, @vLookupKey)");

            string commandText = sb.ToString();

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<CustomerGlossaryValueGet>(commandText, spParams);
        }

        public virtual ObjectResult<CustomerGlossaryItemsGet> GetCustomerGlossaryItems(Nullable<int> customerId, string searchText, Nullable<int> includeInSessionObject)
        {
            MySqlParameter customerIDParam = new MySqlParameter("vCustomerID", MySqlDbType.Int32);
            customerIDParam.Direction = ParameterDirection.Input;
            customerIDParam.Value = customerId;

            MySqlParameter searchTextParam = new MySqlParameter("vSearchText", MySqlDbType.String);
            searchTextParam.Direction = ParameterDirection.Input;
            searchTextParam.Value = searchText;

            MySqlParameter incInSessionObjParam = new MySqlParameter("vIncludeInSessionObjectOnly", MySqlDbType.Int32);
            incInSessionObjParam.Direction = ParameterDirection.Input;
            incInSessionObjParam.Value = includeInSessionObject;

            MySqlParameter[] spParams = new MySqlParameter[] {
                customerIDParam,
                searchTextParam,
                incInSessionObjParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL customerglossaryitemsget(@vCustomerID, @vSearchText, @vIncludeInSessionObjectOnly)");

            string commandText = sb.ToString();

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<CustomerGlossaryItemsGet>(commandText, spParams);
        }

        public void CustomerGlossaryItemUpdate(int customerId, int glossaryId, string text)
        {
            MySqlParameter customerIDParam = new MySqlParameter("vCustomerID", MySqlDbType.Int32);
            customerIDParam.Direction = ParameterDirection.Input;
            customerIDParam.Value = customerId;

            MySqlParameter glossaryIDParam = new MySqlParameter("vGlossaryID", MySqlDbType.Int32);
            glossaryIDParam.Direction = ParameterDirection.Input;
            glossaryIDParam.Value = glossaryId;

            MySqlParameter textParam = new MySqlParameter("vText", MySqlDbType.String);
            textParam.Direction = ParameterDirection.Input;
            textParam.Value = text;

            MySqlParameter[] spParams = new MySqlParameter[] {
                customerIDParam,
                glossaryIDParam,
                textParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL customerglossaryitemupdate(@vCustomerID, @vGlossaryID, @vText)");

            string commandText = sb.ToString();
            this.Database.ExecuteSqlCommand(commandText, spParams);
        }

        public virtual ObjectResult<AdminClientsGet> GetClient(Nullable<int> clientId)
        {
            var id = Convert.ToInt32(clientId);
            var customerId = CustomerHelper.GetCustomerIDFromClientID(id);

            return GetClients(customerId, String.Empty, clientId);
        }

        public virtual ObjectResult<AdminClientsGet> GetClients(Nullable<int> customerId, string searchText)
        {
            return GetClients(customerId, searchText, -1);
        }

        public virtual ObjectResult<AdminClientsGet> GetClients(Nullable<int> customerId, string searchText, Nullable<int> clientId)
        {
            MySqlParameter customerIDParam = new MySqlParameter("vCustomerID", MySqlDbType.Int32);
            customerIDParam.Direction = ParameterDirection.Input;
            customerIDParam.Value = customerId;

            if (clientId.HasValue == false)
            {
                clientId = -1;
            }

            searchText = !String.IsNullOrEmpty(searchText) ? searchText : String.Empty;

            MySqlParameter clientIDParam = new MySqlParameter("vClientID", MySqlDbType.Int32);
            clientIDParam.Direction = ParameterDirection.Input;
            clientIDParam.Value = clientId;

            MySqlParameter searchTextParam = new MySqlParameter("vSearchText", MySqlDbType.String);
            searchTextParam.Direction = ParameterDirection.Input;
            searchTextParam.Value = searchText;

            MySqlParameter[] spParams = new MySqlParameter[] {
                customerIDParam,
                clientIDParam,
                searchTextParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL admin_clientsget(@vCustomerID, @vClientID, @vSearchText)");

            string commandText = sb.ToString();

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<AdminClientsGet>(commandText, spParams);
        }

        public void ClientActiveUpdate(int clientID, int active)
        {
            MySqlParameter userIDParam = new MySqlParameter("vClientID", MySqlDbType.Int32);
            userIDParam.Direction = ParameterDirection.Input;
            userIDParam.Value = clientID;

            MySqlParameter activeParam = new MySqlParameter("vActive", MySqlDbType.Int32);
            activeParam.Direction = ParameterDirection.Input;
            activeParam.Value = active;

            MySqlParameter[] spParams = new MySqlParameter[] {
                userIDParam,
                activeParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL admin_clientactiveset(@vClientID, @vActive)");

            string commandText = sb.ToString();
            this.Database.ExecuteSqlCommand(commandText, spParams);
        }

        public void ClientUpdate(int clientID, string firstName, string lastName, string phoneNumber, string email, int active, int admin,
            string homePhone, string address, string city, string state, string zipCode, string occupation, string emergencyContact, MySql.Data.Types.MySqlDateTime dob,
            string preferredMethodOfComm)
        {
            MySqlParameter userIDParam = new MySqlParameter("vClientID", MySqlDbType.Int32);
            userIDParam.Direction = ParameterDirection.Input;
            userIDParam.Value = clientID;

            MySqlParameter firstNameParam = new MySqlParameter("vFirstName", MySqlDbType.String);
            firstNameParam.Direction = ParameterDirection.Input;
            firstNameParam.Value = firstName;

            MySqlParameter lastNameParam = new MySqlParameter("vLastName", MySqlDbType.String);
            lastNameParam.Direction = ParameterDirection.Input;
            lastNameParam.Value = lastName;

            MySqlParameter phoneNumberParam = new MySqlParameter("vPhoneNumber", MySqlDbType.String);
            phoneNumberParam.Direction = ParameterDirection.Input;
            phoneNumberParam.Value = phoneNumber;

            MySqlParameter emailParam = new MySqlParameter("vEmail", MySqlDbType.String);
            emailParam.Direction = ParameterDirection.Input;
            emailParam.Value = email;

            MySqlParameter activeParam = new MySqlParameter("vActive", MySqlDbType.Int32);
            activeParam.Direction = ParameterDirection.Input;
            activeParam.Value = active;

            MySqlParameter adminParam = new MySqlParameter("vAdmin", MySqlDbType.Int32);
            adminParam.Direction = ParameterDirection.Input;
            adminParam.Value = admin;

            MySqlParameter homePhoneParam = new MySqlParameter("vHomePhone", MySqlDbType.String);
            homePhoneParam.Direction = ParameterDirection.Input;
            homePhoneParam.Value = homePhone;

            MySqlParameter addressParam = new MySqlParameter("vAddress", MySqlDbType.String);
            addressParam.Direction = ParameterDirection.Input;
            addressParam.Value = address;

            MySqlParameter cityParam = new MySqlParameter("vCity", MySqlDbType.String);
            cityParam.Direction = ParameterDirection.Input;
            cityParam.Value = city;

            MySqlParameter stateParam = new MySqlParameter("vState", MySqlDbType.String);
            stateParam.Direction = ParameterDirection.Input;
            stateParam.Value = state;

            MySqlParameter zipParam = new MySqlParameter("vZipCode", MySqlDbType.String);
            zipParam.Direction = ParameterDirection.Input;
            zipParam.Value = zipCode;

            MySqlParameter occupationParam = new MySqlParameter("vOccupation", MySqlDbType.String);
            occupationParam.Direction = ParameterDirection.Input;
            occupationParam.Value = occupation;

            MySqlParameter emergencyContactParam = new MySqlParameter("vEmergencyContact", MySqlDbType.String);
            emergencyContactParam.Direction = ParameterDirection.Input;
            emergencyContactParam.Value = emergencyContact;

            MySqlParameter dobParam = new MySqlParameter("vDOB", MySqlDbType.Date);
            dobParam.Direction = ParameterDirection.Input;
            dobParam.Value = dob;

            MySqlParameter preferredParam = new MySqlParameter("vPreferredMethodOfComm", MySqlDbType.String);
            preferredParam.Direction = ParameterDirection.Input;
            preferredParam.Value = preferredMethodOfComm;

            MySqlParameter[] spParams = new MySqlParameter[] {
                userIDParam,
                firstNameParam,
                lastNameParam,
                phoneNumberParam,
                emailParam,
                activeParam,
                adminParam,
                homePhoneParam,
                addressParam,
                cityParam,
                stateParam,
                zipParam,
                occupationParam,
                emergencyContactParam,
                dobParam,
                preferredParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL admin_clientupdate(@vClientID, @vFirstName, @vLastName, @vPhoneNumber, @vEmail, @vActive, @vAdmin, @vHomePhone, @vAddress, @vCity, @vState, @vZipCode, @vOccupation, @vEmergencyContact, @vDOB, @vPreferredMethodOfComm)");

            string commandText = sb.ToString();
            this.Database.ExecuteSqlCommand(commandText, spParams);
        }


        public void ClientAdd(int customerID, string firstName, string lastName, string phoneNumber, string email, int active, int admin,
            string homePhone, string address, string city, string state, string zipCode, string occupation, string emergencyContact, MySql.Data.Types.MySqlDateTime dob,
            string preferredMethodOfComm)
        {
            MySqlParameter customerIDParam = new MySqlParameter("vCustomerID", MySqlDbType.Int32);
            customerIDParam.Direction = ParameterDirection.Input;
            customerIDParam.Value = customerID;

            MySqlParameter firstNameParam = new MySqlParameter("vFirstName", MySqlDbType.String);
            firstNameParam.Direction = ParameterDirection.Input;
            firstNameParam.Value = firstName;

            MySqlParameter lastNameParam = new MySqlParameter("vLastName", MySqlDbType.String);
            lastNameParam.Direction = ParameterDirection.Input;
            lastNameParam.Value = lastName;

            MySqlParameter phoneNumberParam = new MySqlParameter("vPhoneNumber", MySqlDbType.String);
            phoneNumberParam.Direction = ParameterDirection.Input;
            phoneNumberParam.Value = phoneNumber;

            MySqlParameter emailParam = new MySqlParameter("vEmail", MySqlDbType.String);
            emailParam.Direction = ParameterDirection.Input;
            emailParam.Value = email;

            MySqlParameter activeParam = new MySqlParameter("vActive", MySqlDbType.Int32);
            activeParam.Direction = ParameterDirection.Input;
            activeParam.Value = active;

            MySqlParameter adminParam = new MySqlParameter("vAdmin", MySqlDbType.Int32);
            adminParam.Direction = ParameterDirection.Input;
            adminParam.Value = admin;

            MySqlParameter homePhoneParam = new MySqlParameter("vHomePhone", MySqlDbType.String);
            homePhoneParam.Direction = ParameterDirection.Input;
            homePhoneParam.Value = homePhone;

            MySqlParameter addressParam = new MySqlParameter("vAddress", MySqlDbType.String);
            addressParam.Direction = ParameterDirection.Input;
            addressParam.Value = address;

            MySqlParameter cityParam = new MySqlParameter("vCity", MySqlDbType.String);
            cityParam.Direction = ParameterDirection.Input;
            cityParam.Value = city;

            MySqlParameter stateParam = new MySqlParameter("vState", MySqlDbType.String);
            stateParam.Direction = ParameterDirection.Input;
            stateParam.Value = state;

            MySqlParameter zipParam = new MySqlParameter("vZipCode", MySqlDbType.String);
            zipParam.Direction = ParameterDirection.Input;
            zipParam.Value = zipCode;

            MySqlParameter occupationParam = new MySqlParameter("vOccupation", MySqlDbType.String);
            occupationParam.Direction = ParameterDirection.Input;
            occupationParam.Value = occupation;

            MySqlParameter emergencyContactParam = new MySqlParameter("vEmergencyContact", MySqlDbType.String);
            emergencyContactParam.Direction = ParameterDirection.Input;
            emergencyContactParam.Value = emergencyContact;

            MySqlParameter dobParam = new MySqlParameter("vDOB", MySqlDbType.Date);
            dobParam.Direction = ParameterDirection.Input;
            dobParam.Value = dob;

            MySqlParameter preferredParam = new MySqlParameter("vPreferredMethodOfComm", MySqlDbType.String);
            preferredParam.Direction = ParameterDirection.Input;
            preferredParam.Value = preferredMethodOfComm;

            MySqlParameter[] spParams = new MySqlParameter[] {
                customerIDParam,
                firstNameParam,
                lastNameParam,
                phoneNumberParam,
                emailParam,
                activeParam,
                adminParam,
                homePhoneParam,
                addressParam,
                cityParam,
                stateParam,
                zipParam,
                occupationParam,
                emergencyContactParam,
                dobParam,
                preferredParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL admin_clientadd(@vCustomerID, @vFirstName, @vLastName, @vPhoneNumber, @vEmail, @vActive, @vAdmin, @vHomePhone, @vAddress, @vCity, @vState, @vZipCode, @vOccupation, @vEmergencyContact, @vDOB, @vPreferredMethodOfComm)");

            string commandText = sb.ToString();
            this.Database.ExecuteSqlCommand(commandText, spParams);
        }

        public void ClientDelete(int clientID)
        {
            MySqlParameter userIDParam = new MySqlParameter("vClientID", MySqlDbType.Int32);
            userIDParam.Direction = ParameterDirection.Input;
            userIDParam.Value = clientID;

            MySqlParameter[] spParams = new MySqlParameter[] {
                userIDParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL DeleteClient(@vClientID)");

            string commandText = sb.ToString();
            this.Database.ExecuteSqlCommand(commandText, spParams);
        }

        public virtual ObjectResult<AdminRolesGet> GetAdminRoles(Nullable<int> customerId, string searchText)
        {
            MySqlParameter customerIDParam = new MySqlParameter("vCustomerID", MySqlDbType.Int32);
            customerIDParam.Direction = ParameterDirection.Input;
            customerIDParam.Value = customerId;

            searchText = !String.IsNullOrEmpty(searchText) ? searchText : String.Empty;

            MySqlParameter searchTextParam = new MySqlParameter("vSearchText", MySqlDbType.String);
            searchTextParam.Direction = ParameterDirection.Input;
            searchTextParam.Value = searchText;

            MySqlParameter[] spParams = new MySqlParameter[] {
                customerIDParam,
                searchTextParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL admin_rolesget(@vCustomerID, @vSearchText)");

            string commandText = sb.ToString();

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<AdminRolesGet>(commandText, spParams);
        }

        public virtual ObjectResult<AdminRolePermissionsGet> GetAllRolePermissions(Nullable<int> roleId, Nullable<int> customerID)
        {
            return GetRolePermissions(roleId, customerID, 1);
        }

        public virtual ObjectResult<AdminRolePermissionsGet> GetRolePermissions(Nullable<int> roleId, Nullable<int> customerID)
        {
            return GetRolePermissions(roleId, customerID, 0);
        }

        public virtual ObjectResult<AdminRolePermissionsGet> GetRolePermissions(Nullable<int> roleId, Nullable<int> customerID, Nullable<int> showAll)
        {
            MySqlParameter roleIDParam = new MySqlParameter("vRoleID", MySqlDbType.Int32);
            roleIDParam.Direction = ParameterDirection.Input;
            roleIDParam.Value = roleId;

            MySqlParameter customerIDParam = new MySqlParameter("vCustomerID", MySqlDbType.Int32);
            customerIDParam.Direction = ParameterDirection.Input;
            customerIDParam.Value = customerID;

            MySqlParameter showAllParam = new MySqlParameter("vShowAll", MySqlDbType.Int32);
            showAllParam.Direction = ParameterDirection.Input;
            showAllParam.Value = showAll;

            MySqlParameter[] spParams = new MySqlParameter[] {
                roleIDParam,
                customerIDParam,
                showAllParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL admin_rolepermissionsget(@vRoleID, @vCustomerID, @vShowAll)");

            string commandText = sb.ToString();

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<AdminRolePermissionsGet>(commandText, spParams);
        }

        public virtual ObjectResult<AdminRoleUsersGet> GetAllRoleUsers(Nullable<int> roleId, Nullable<int> customerID)
        {
            return GetRoleUsers(roleId, customerID, 1);
        }

        public virtual ObjectResult<AdminRoleUsersGet> GetRoleUsers(Nullable<int> roleId, Nullable<int> customerID)
        {
            return GetRoleUsers(roleId, customerID, 0);
        }

        public virtual ObjectResult<AdminRoleUsersGet> GetRoleUsers(Nullable<int> roleId, Nullable<int> customerID, Nullable<int> showAll)
        {
            MySqlParameter roleIDParam = new MySqlParameter("vRoleID", MySqlDbType.Int32);
            roleIDParam.Direction = ParameterDirection.Input;
            roleIDParam.Value = roleId;

            MySqlParameter customerIDParam = new MySqlParameter("vCustomerID", MySqlDbType.Int32);
            customerIDParam.Direction = ParameterDirection.Input;
            customerIDParam.Value = customerID;

            MySqlParameter showAllParam = new MySqlParameter("vShowAll", MySqlDbType.Int16);
            showAllParam.Direction = ParameterDirection.Input;
            showAllParam.Value = showAll;

            MySqlParameter[] spParams = new MySqlParameter[] {
                roleIDParam,
                customerIDParam,
                showAllParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL admin_roleusersget(@vRoleID, @vCustomerID, @vShowAll)");

            string commandText = sb.ToString();

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<AdminRoleUsersGet>(commandText, spParams);
        }

        public void RolePermissionUpdate(int roleId, int permissionId, int isChecked)
        {
            MySqlParameter roleIdParam = new MySqlParameter("vRoleID", MySqlDbType.Int32);
            roleIdParam.Direction = ParameterDirection.Input;
            roleIdParam.Value = roleId;

            MySqlParameter permissionIdParam = new MySqlParameter("vPermissionID", MySqlDbType.Int32);
            permissionIdParam.Direction = ParameterDirection.Input;
            permissionIdParam.Value = permissionId;

            MySqlParameter checkedParam = new MySqlParameter("vChecked", MySqlDbType.Int16);
            checkedParam.Direction = ParameterDirection.Input;
            checkedParam.Value = isChecked;

            MySqlParameter[] spParams = new MySqlParameter[] {
                roleIdParam,
                permissionIdParam,
                checkedParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL admin_rolepermissionupdate(@vRoleID, @vPermissionID, @vChecked)");

            string commandText = sb.ToString();
            this.Database.ExecuteSqlCommand(commandText, spParams);
        }

        public void RoleUserUpdate(int userId, int roleId, int isChecked)
        {
            MySqlParameter userIDParam = new MySqlParameter("vUserID", MySqlDbType.Int32);
            userIDParam.Direction = ParameterDirection.Input;
            userIDParam.Value = userId;

            MySqlParameter roleIdParam = new MySqlParameter("vRoleID", MySqlDbType.Int32);
            roleIdParam.Direction = ParameterDirection.Input;
            roleIdParam.Value = roleId;

            MySqlParameter checkedParam = new MySqlParameter("vChecked", MySqlDbType.Int16);
            checkedParam.Direction = ParameterDirection.Input;
            checkedParam.Value = isChecked;

            MySqlParameter[] spParams = new MySqlParameter[] {
                userIDParam,
                roleIdParam,
                checkedParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL admin_roleuserupdate(@vUserID, @vRoleID, @vChecked)");

            string commandText = sb.ToString();
            this.Database.ExecuteSqlCommand(commandText, spParams);
        }

        public virtual ObjectResult<AdminLookupsGet> GetLookups(Nullable<int> customerID, string filter)
        {
            MySqlParameter customerIDParam = new MySqlParameter("vCustomerID", MySqlDbType.Int32);
            customerIDParam.Direction = ParameterDirection.Input;
            customerIDParam.Value = customerID;

            filter = !String.IsNullOrEmpty(filter) ? filter : String.Empty;

            MySqlParameter filterParam = new MySqlParameter("vFilter", MySqlDbType.String);
            filterParam.Direction = ParameterDirection.Input;
            filterParam.Value = filter;

            MySqlParameter[] spParams = new MySqlParameter[] {
                customerIDParam,
                filterParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL admin_lookupsget(@vCustomerID, @vFilter)");

            string commandText = sb.ToString();

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<AdminLookupsGet>(commandText, spParams);
        }

        public void DeactivateLookup(int lookupID)
        {
            LookupActiveUpdate(lookupID, 0);
        }

        public void LookupActiveUpdate(int lookupID, int active)
        {
            MySqlParameter lookupIDParam = new MySqlParameter("vLookupID", MySqlDbType.Int32);
            lookupIDParam.Direction = ParameterDirection.Input;
            lookupIDParam.Value = lookupID;

            MySqlParameter activeParam = new MySqlParameter("vActive", MySqlDbType.Int16);
            activeParam.Direction = ParameterDirection.Input;
            activeParam.Value = active;

            MySqlParameter[] spParams = new MySqlParameter[] {
                lookupIDParam,
                activeParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL admin_lookupactiveupdate(@vLookupID, @vActive)");

            string commandText = sb.ToString();
            this.Database.ExecuteSqlCommand(commandText, spParams);
        }

        public void CustomerLookupAdd(int customerID, string description)
        {
            LookupAdd(customerID, description);
        }

        public void LookupAdd(int? customerID, string description)
        {
            MySqlParameter customerIDParam = new MySqlParameter("vCustomerID", MySqlDbType.Int32);
            customerIDParam.Direction = ParameterDirection.Input;
            customerIDParam.Value = customerID;

            description = !String.IsNullOrEmpty(description) ? description : String.Empty;

            MySqlParameter descriptionParam = new MySqlParameter("vDescription", MySqlDbType.String);
            descriptionParam.Direction = ParameterDirection.Input;
            descriptionParam.Value = description;

            MySqlParameter[] spParams = new MySqlParameter[] {
                customerIDParam,
                descriptionParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL admin_lookupadd(@vCustomerID, @vDescription)");

            string commandText = sb.ToString();
            this.Database.ExecuteSqlCommand(commandText, spParams);
        }

        public virtual ObjectResult<AdminLookupListFormsGet> GetLookupListForms(Nullable<int> customerId, Nullable<int> lookupId)
        {
            MySqlParameter customerIDParam = new MySqlParameter("vCustomerID", MySqlDbType.Int32);
            customerIDParam.Direction = ParameterDirection.Input;
            customerIDParam.Value = lookupId;

            MySqlParameter lookupIDParam = new MySqlParameter("vLookupID", MySqlDbType.Int32);
            lookupIDParam.Direction = ParameterDirection.Input;
            lookupIDParam.Value = lookupId;

            MySqlParameter[] spParams = new MySqlParameter[] {
                customerIDParam,
                lookupIDParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL admin_lookuplistformsget(@vCustomerID, @vLookupID)");

            string commandText = sb.ToString();

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<AdminLookupListFormsGet>(commandText, spParams);
        }

        public void LookupListItemUpdate(int lookupListItemID, string keyValue, string displayValue, int active)
        {
            MySqlParameter lookupIDParam = new MySqlParameter("vLookupListItemID", MySqlDbType.Int32);
            lookupIDParam.Direction = ParameterDirection.Input;
            lookupIDParam.Value = lookupListItemID;

            keyValue = !String.IsNullOrEmpty(keyValue) ? keyValue : String.Empty;
            displayValue = !String.IsNullOrEmpty(displayValue) ? displayValue : String.Empty;

            MySqlParameter valueParam = new MySqlParameter("vValue", MySqlDbType.String);
            valueParam.Direction = ParameterDirection.Input;
            valueParam.Value = keyValue;

            MySqlParameter displayParam = new MySqlParameter("vDisplay", MySqlDbType.String);
            displayParam.Direction = ParameterDirection.Input;
            displayParam.Value = displayValue;

            MySqlParameter activeParam = new MySqlParameter("vActive", MySqlDbType.Int16);
            activeParam.Direction = ParameterDirection.Input;
            activeParam.Value = active;

            MySqlParameter[] spParams = new MySqlParameter[] {
                lookupIDParam,
                valueParam,
                displayParam,
                activeParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL admin_lookuplistitemupdate(@vLookupListItemID, @vValue, @vDisplay, @vActive)");

            string commandText = sb.ToString();
            this.Database.ExecuteSqlCommand(commandText, spParams);
        }

        public virtual ObjectResult<AdminLookupItemsGet> GetLookupItems(Nullable<int> lookupId)
        {
            MySqlParameter lookupIDParam = new MySqlParameter("vLookupID", MySqlDbType.Int32);
            lookupIDParam.Direction = ParameterDirection.Input;
            lookupIDParam.Value = lookupId;

            MySqlParameter[] spParams = new MySqlParameter[] {
                lookupIDParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL admin_lookupitemsget(@vLookupID)");

            string commandText = sb.ToString();

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<AdminLookupItemsGet>(commandText, spParams);
        }

        public void LookupListItemAdd(int lookupId, string keyValue, string displayValue)
        {
            MySqlParameter lookupIDParam = new MySqlParameter("vLookupID", MySqlDbType.Int32);
            lookupIDParam.Direction = ParameterDirection.Input;
            lookupIDParam.Value = lookupId;

            keyValue = !String.IsNullOrEmpty(keyValue) ? keyValue : String.Empty;
            displayValue = !String.IsNullOrEmpty(displayValue) ? displayValue : String.Empty;

            MySqlParameter valueParam = new MySqlParameter("vValue", MySqlDbType.String);
            valueParam.Direction = ParameterDirection.Input;
            valueParam.Value = keyValue;

            MySqlParameter displayParam = new MySqlParameter("vDisplay", MySqlDbType.String);
            displayParam.Direction = ParameterDirection.Input;
            displayParam.Value = displayValue;

            MySqlParameter[] spParams = new MySqlParameter[] {
                lookupIDParam,
                valueParam,
                displayParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL admin_lookuplistitemadd(@vLookupID, @vValue, @vDisplay)");

            string commandText = sb.ToString();
            this.Database.ExecuteSqlCommand(commandText, spParams);
        }

        public void LookupUpdate(int lookupId, string listName)
        {
            MySqlParameter lookupIDParam = new MySqlParameter("vLookupID", MySqlDbType.Int32);
            lookupIDParam.Direction = ParameterDirection.Input;
            lookupIDParam.Value = lookupId;

            listName = !String.IsNullOrEmpty(listName) ? listName : String.Empty;

            MySqlParameter descriptionParam = new MySqlParameter("vDescription", MySqlDbType.String);
            descriptionParam.Direction = ParameterDirection.Input;
            descriptionParam.Value = listName;

            MySqlParameter[] spParams = new MySqlParameter[] {
                lookupIDParam,
                descriptionParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL admin_lookupupdate(@vLookupID, @vDescription)");

            string commandText = sb.ToString();
            this.Database.ExecuteSqlCommand(commandText, spParams);
        }

        public void TempClientDelete(int clientId)
        {
            MySqlParameter clientIDParam = new MySqlParameter("vAddUser", MySqlDbType.Int32);
            clientIDParam.Direction = ParameterDirection.Input;
            clientIDParam.Value = clientId;

            MySqlParameter[] spParams = new MySqlParameter[] {
                clientIDParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL temp_clientdelete(@vAddUser)");

            string commandText = sb.ToString();
            this.Database.ExecuteSqlCommand(commandText, spParams);
        }

        // Use this one for regular users - no survey
        public void TempUserAdd(int clientId, string firstName, string lastName, string phoneNumber, string email, int isAdministrator)
        {
            TempClientAdd(clientId, firstName, lastName, phoneNumber, email, isAdministrator, 0);
        }

        public void TempClientAdd(int clientId, string firstName, string lastName, string phoneNumber, string email, int isAdministrator, int forSurvey)
        {
            MySqlParameter clientIDParam = new MySqlParameter("vAddUser", MySqlDbType.Int32);
            clientIDParam.Direction = ParameterDirection.Input;
            clientIDParam.Value = clientId;

            firstName = !String.IsNullOrEmpty(firstName) ? firstName : String.Empty;
            lastName = !String.IsNullOrEmpty(lastName) ? lastName : String.Empty;
            email = !String.IsNullOrEmpty(email) ? email : String.Empty;
            phoneNumber = !String.IsNullOrEmpty(phoneNumber) ? phoneNumber : String.Empty;

            MySqlParameter firstNameParam = new MySqlParameter("vFirstName", MySqlDbType.String);
            firstNameParam.Direction = ParameterDirection.Input;
            firstNameParam.Value = firstName;

            MySqlParameter lastNameParam = new MySqlParameter("vLastName", MySqlDbType.String);
            lastNameParam.Direction = ParameterDirection.Input;
            lastNameParam.Value = lastName;

            MySqlParameter phoneParam = new MySqlParameter("vPhoneNumber", MySqlDbType.String);
            phoneParam.Direction = ParameterDirection.Input;
            phoneParam.Value = phoneNumber;

            MySqlParameter emailParam = new MySqlParameter("vEmail", MySqlDbType.String);
            emailParam.Direction = ParameterDirection.Input;
            emailParam.Value = email;

            MySqlParameter isAdminParam = new MySqlParameter("vIsAdministrator", MySqlDbType.Int16);
            isAdminParam.Direction = ParameterDirection.Input;
            isAdminParam.Value = isAdministrator;

            MySqlParameter forSurveyParam = new MySqlParameter("vForSurvey", MySqlDbType.Int16);
            forSurveyParam.Direction = ParameterDirection.Input;
            forSurveyParam.Value = forSurvey;

            MySqlParameter[] spParams = new MySqlParameter[] {
                clientIDParam,
                firstNameParam,
                lastNameParam,
                phoneParam,
                emailParam,
                isAdminParam,
                forSurveyParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL temp_clientadd(@vAddUser, @vFirstName, @vLastName, @vPhoneNumber, @vEmail, @vIsAdministrator, @vForSurvey)");

            string commandText = sb.ToString();
            this.Database.ExecuteSqlCommand(commandText, spParams);
        }

        public void TempClientExecute(int clientId)
        {
            MySqlParameter clientIDParam = new MySqlParameter("vClientID", MySqlDbType.Int32);
            clientIDParam.Direction = ParameterDirection.Input;
            clientIDParam.Value = clientId;

            MySqlParameter[] spParams = new MySqlParameter[] {
                clientIDParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL temp_clientexecute(@vClientID)");

            string commandText = sb.ToString();
            this.Database.ExecuteSqlCommand(commandText, spParams);
        }

        public virtual ObjectResult<ProcessCostsGet> GetProcessCosts(Nullable<int> processId)
        {
            MySqlParameter processIdParam = new MySqlParameter("vProcessID", MySqlDbType.Int32);
            processIdParam.Direction = ParameterDirection.Input;
            processIdParam.Value = processId;

            MySqlParameter[] spParams = new MySqlParameter[] {
                processIdParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL processcostsget(@vProcessID)");

            string commandText = sb.ToString();

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<ProcessCostsGet>(commandText, spParams);
        }

        public void AddSurveyProcess(int customerId, string processName, string summary, string governmentMailingAddress1,
            string packetTitle, int generatesEmail, string externalRedirect, string formListHeaderMessage,
            string welcomeEmail, int userSuppliesEmail, int noEmailAuth, int humanVerifyNeeded, string emailSurveyImage,
            int emailSurveyPhoneUse, int emailSurveyNameUse, int emailSurveyNameRequired, int emailSurveyPhoneRequired,
            int emailSurveyEmailRequired, string emailSurveyStartHeaderText, int beginWithoutRegistration, string hyperlinkDirectory, int questionnaireGeneratePDF)
        {
            AddProcess(customerId, 0, 1, processName, summary, governmentMailingAddress1, packetTitle, 0, generatesEmail,
                externalRedirect, formListHeaderMessage, 0, welcomeEmail,
                userSuppliesEmail, noEmailAuth, humanVerifyNeeded, emailSurveyImage, emailSurveyPhoneUse, emailSurveyNameUse,
                emailSurveyNameRequired, emailSurveyPhoneRequired, emailSurveyEmailRequired, emailSurveyStartHeaderText,
                beginWithoutRegistration, hyperlinkDirectory, questionnaireGeneratePDF);
        }

        // Non-survey, Non-formilae admin
        public void AddNonSurveyProcess(int customerId, string processName, string summary, string governmentMailingAddress1,
            string packetTitle, int requiresAdminApproval, int generatesEmail, string externalRedirect, string formListHeaderMessage,
            string welcomeEmail)
        {
            AddProcess(customerId, 0, 0, processName, summary, governmentMailingAddress1, packetTitle, requiresAdminApproval, generatesEmail,
                externalRedirect, formListHeaderMessage, 0, welcomeEmail,
                0, 0, 0, "", 0, 0, 0, 0, 0, "", 0, "", 0);
        }

        public void UpdateSurveyProcess(int customerId, int processId, string processName, string summary, string governmentMailingAddress1,
            string packetTitle, int requiresAdminApproval, int generatesEmail, string externalRedirect, string formListHeaderMessage,
            string welcomeEmail, int userSuppliesEmail, int noEmailAuth, int humanVerifyNeeded, string emailSurveyImage,
            int emailSurveyPhoneUse, int emailSurveyNameUse, int emailSurveyNameRequired, int emailSurveyPhoneRequired,
            int emailSurveyEmailRequired, string emailSurveyStartHeaderText, int beginWithoutRegistration, string hyperlinkDirectory, int questionnaireGeneratePDF)
        {
            UpdateProcess(customerId, processId, 0, processName, summary, governmentMailingAddress1, packetTitle, 0, generatesEmail,
                externalRedirect, formListHeaderMessage, welcomeEmail,
                userSuppliesEmail, noEmailAuth, humanVerifyNeeded, emailSurveyImage, emailSurveyPhoneUse, emailSurveyNameUse,
                emailSurveyNameRequired, emailSurveyPhoneRequired, emailSurveyEmailRequired, emailSurveyStartHeaderText,
                beginWithoutRegistration, hyperlinkDirectory, questionnaireGeneratePDF);
        }

        // Non-survey, Non-formilae admin
        public void UpdateNonSurveyProcess(int customerId, int processId, string processName, string summary, string governmentMailingAddress1,
            string packetTitle, int requiresAdminApproval, int generatesEmail, string externalRedirect, string formListHeaderMessage,
            string welcomeEmail)
        {
            UpdateProcess(customerId, processId, 0, processName, summary, governmentMailingAddress1, packetTitle, requiresAdminApproval, generatesEmail,
                externalRedirect, formListHeaderMessage, welcomeEmail,
                0, 0, 0, "", 0, 0, 0, 0, 0, "", 0, "", 0);
        }

        public void AddProcess(int customerId, int formilaeAdmin, int isSurvey, string processName, string summary, string governmentMailingAddress1,
            string packetTitle, int requiresAdminApproval, int generatesEmail, string externalRedirect, string formListHeaderMessage,
            int governmentFormProcess, string welcomeEmail,
            int userSuppliesEmail, int noEmailAuth, int humanVerifyNeeded, string emailSurveyImage, int emailSurveyPhoneUse, int emailSurveyNameUse,
            int emailSurveyNameRequired, int emailSurveyPhoneRequired, int emailSurveyEmailRequired, string emailSurveyStartHeaderText,
            int beginWithoutRegistration, string hyperlinkDirectory, int questionnaireGeneratePDF)
        {
            packetTitle = String.IsNullOrEmpty(packetTitle) ? "" : packetTitle;

            MySqlParameter customerIDParam = new MySqlParameter("vCustomerID", MySqlDbType.Int32);
            customerIDParam.Direction = ParameterDirection.Input;
            customerIDParam.Value = customerId;

            MySqlParameter formilaeAdminParam = new MySqlParameter("vFormilaeAdmin", MySqlDbType.Int32);
            formilaeAdminParam.Direction = ParameterDirection.Input;
            formilaeAdminParam.Value = formilaeAdmin;

            MySqlParameter isSurveyParam = new MySqlParameter("vIsSurvey", MySqlDbType.Int32);
            isSurveyParam.Direction = ParameterDirection.Input;
            isSurveyParam.Value = isSurvey;

            MySqlParameter processNameParam = new MySqlParameter("vProcessName", MySqlDbType.String);
            processNameParam.Direction = ParameterDirection.Input;
            processNameParam.Value = processName;

            MySqlParameter summaryParam = new MySqlParameter("vSummary", MySqlDbType.String);
            summaryParam.Direction = ParameterDirection.Input;
            summaryParam.Value = summary;

            MySqlParameter govMailAddr1Param = new MySqlParameter("vGovernmentMailingAddress1", MySqlDbType.String);
            govMailAddr1Param.Direction = ParameterDirection.Input;
            govMailAddr1Param.Value = governmentMailingAddress1;

            MySqlParameter packetTitleParam = new MySqlParameter("vPacketTitle", MySqlDbType.String);
            packetTitleParam.Direction = ParameterDirection.Input;
            packetTitleParam.Value = packetTitle;

            MySqlParameter reqAdminApprParam = new MySqlParameter("vRequiresAdminApproval", MySqlDbType.Int32);
            reqAdminApprParam.Direction = ParameterDirection.Input;
            reqAdminApprParam.Value = requiresAdminApproval;

            MySqlParameter genEmailParam = new MySqlParameter("vGeneratesEmail", MySqlDbType.Int32);
            genEmailParam.Direction = ParameterDirection.Input;
            genEmailParam.Value = generatesEmail;

            MySqlParameter extRedirectParam = new MySqlParameter("vExternalRedirect", MySqlDbType.String);
            extRedirectParam.Direction = ParameterDirection.Input;
            extRedirectParam.Value = externalRedirect;

            MySqlParameter flHeaderMsgParam = new MySqlParameter("vFormListHeaderMessage", MySqlDbType.String);
            flHeaderMsgParam.Direction = ParameterDirection.Input;
            flHeaderMsgParam.Value = formListHeaderMessage;

            MySqlParameter govFormProcessParam = new MySqlParameter("vGovernmentFormProcess", MySqlDbType.Int32);
            govFormProcessParam.Direction = ParameterDirection.Input;
            govFormProcessParam.Value = governmentFormProcess;

            MySqlParameter welcomeEmailParam = new MySqlParameter("vWelcomeEmail", MySqlDbType.String);
            welcomeEmailParam.Direction = ParameterDirection.Input;
            welcomeEmailParam.Value = welcomeEmail;

            MySqlParameter userSuppliesEmailParam = new MySqlParameter("vUserSuppliesEmail", MySqlDbType.Int16);
            userSuppliesEmailParam.Direction = ParameterDirection.Input;
            userSuppliesEmailParam.Value = userSuppliesEmail;

            MySqlParameter noEmailAuthParam = new MySqlParameter("vNoEmailAuth", MySqlDbType.Int16);
            noEmailAuthParam.Direction = ParameterDirection.Input;
            noEmailAuthParam.Value = noEmailAuth;

            MySqlParameter humanVerifyNeededParam = new MySqlParameter("vHumanVerifyNeeded", MySqlDbType.Int16);
            humanVerifyNeededParam.Direction = ParameterDirection.Input;
            humanVerifyNeededParam.Value = humanVerifyNeeded;

            MySqlParameter emailSurveyImageParam = new MySqlParameter("vEmailSurveyImage", MySqlDbType.String);
            emailSurveyImageParam.Direction = ParameterDirection.Input;
            emailSurveyImageParam.Value = emailSurveyImage;

            MySqlParameter emailSurveyPhoneUseParam = new MySqlParameter("vEmailSurveyPhoneUse", MySqlDbType.Int16);
            emailSurveyPhoneUseParam.Direction = ParameterDirection.Input;
            emailSurveyPhoneUseParam.Value = emailSurveyPhoneUse;

            MySqlParameter emailSurveyNameUseparam = new MySqlParameter("vEmailSurveyNameUse", MySqlDbType.Int16);
            emailSurveyNameUseparam.Direction = ParameterDirection.Input;
            emailSurveyNameUseparam.Value = emailSurveyNameUse;

            MySqlParameter emailSurveyNameReqParam = new MySqlParameter("vEmailSurveyNameRequired", MySqlDbType.Int16);
            emailSurveyNameReqParam.Direction = ParameterDirection.Input;
            emailSurveyNameReqParam.Value = emailSurveyNameRequired;

            MySqlParameter emailSurveyPhoneReqParam = new MySqlParameter("vEmailSurveyPhoneRequired", MySqlDbType.Int16);
            emailSurveyPhoneReqParam.Direction = ParameterDirection.Input;
            emailSurveyPhoneReqParam.Value = emailSurveyPhoneRequired;

            MySqlParameter emailSurveyEmailReqParam = new MySqlParameter("vEmailSurveyEmailRequired", MySqlDbType.Int16);
            emailSurveyEmailReqParam.Direction = ParameterDirection.Input;
            emailSurveyEmailReqParam.Value = emailSurveyEmailRequired;

            MySqlParameter emailSurveyStartHeaderTxtParam = new MySqlParameter("vEmailSurveyStartHeaderText", MySqlDbType.String);
            emailSurveyStartHeaderTxtParam.Direction = ParameterDirection.Input;
            emailSurveyStartHeaderTxtParam.Value = emailSurveyStartHeaderText;

            MySqlParameter beginWithoutRegistrationParam = new MySqlParameter("vBeginWithoutRegistration", MySqlDbType.Int16);
            beginWithoutRegistrationParam.Direction = ParameterDirection.Input;
            beginWithoutRegistrationParam.Value = beginWithoutRegistration;

            MySqlParameter hyperlinkDirectoryParam = new MySqlParameter("vHyperlinkDirectory", MySqlDbType.String);
            hyperlinkDirectoryParam.Direction = ParameterDirection.Input;
            hyperlinkDirectoryParam.Value = hyperlinkDirectory;

            MySqlParameter questionnaireGeneratePDFParam = new MySqlParameter("vQuestionnaireGeneratePDF", MySqlDbType.Int16);
            questionnaireGeneratePDFParam.Direction = ParameterDirection.Input;
            questionnaireGeneratePDFParam.Value = questionnaireGeneratePDF;

            MySqlParameter[] spParams = new MySqlParameter[] {
                customerIDParam,
                formilaeAdminParam,
                isSurveyParam,
                processNameParam,
                summaryParam,
                govMailAddr1Param,
                packetTitleParam,
                reqAdminApprParam,
                genEmailParam,
                extRedirectParam,
                flHeaderMsgParam,
                govFormProcessParam,
                welcomeEmailParam,
                userSuppliesEmailParam,
                noEmailAuthParam,
                humanVerifyNeededParam,
                emailSurveyImageParam,
                emailSurveyPhoneUseParam,
                emailSurveyNameUseparam,
                emailSurveyNameReqParam,
                emailSurveyPhoneReqParam,
                emailSurveyEmailReqParam,
                emailSurveyStartHeaderTxtParam,
                beginWithoutRegistrationParam,
                hyperlinkDirectoryParam,
                questionnaireGeneratePDFParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL processadd(@vCustomerID, @vFormilaeAdmin, @vIsSurvey, @vProcessName, @vSummary, @vGovernmentMailingAddress1, @vPacketTitle, @vRequiresAdminApproval, @vGeneratesEmail, @vExternalRedirect, @vFormListHeaderMessage, @vGovernmentFormProcess, @vWelcomeEmail, @vUserSuppliesEmail, @vNoEmailAuth, @vHumanVerifyNeeded, @vEmailSurveyImage, @vEmailSurveyPhoneUse, @vEmailSurveyNameUse, @vEmailSurveyNameRequired, @vEmailSurveyPhoneRequired, @vEmailSurveyEmailRequired, @vEmailSurveyStartHeaderText, @vBeginWithoutRegistration, @vHyperlinkDirectory, @vQuestionnaireGeneratePDF)");

            string commandText = sb.ToString();
            this.Database.ExecuteSqlCommand(commandText, spParams);
        }

        public void UpdateProcess(int customerId, int processId, int formilaeAdmin, string processName, string summary, string governmentMailingAddress1,
            string packetTitle, int requiresAdminApproval, int generatesEmail, string externalRedirect, string formListHeaderMessage, string welcomeEmail,
            int userSuppliesEmail, int noEmailAuth, int humanVerifyNeeded, string emailSurveyImage, int emailSurveyPhoneUse, int emailSurveyNameUse,
            int emailSurveyNameRequired, int emailSurveyPhoneRequired, int emailSurveyEmailRequired, string emailSurveyStartHeaderText,
            int beginWithoutRegistration, string hyperlinkDirectory, int questionnaireGeneratePDF)
        {
            packetTitle = String.IsNullOrEmpty(packetTitle) ? "" : packetTitle;

            MySqlParameter customerIDParam = new MySqlParameter("vCustomerID", MySqlDbType.Int32);
            customerIDParam.Direction = ParameterDirection.Input;
            customerIDParam.Value = customerId;

            MySqlParameter processIdParam = new MySqlParameter("vProcessID", MySqlDbType.Int32);
            processIdParam.Direction = ParameterDirection.Input;
            processIdParam.Value = processId;

            MySqlParameter formilaeAdminParam = new MySqlParameter("vFormilaeAdmin", MySqlDbType.Int32);
            formilaeAdminParam.Direction = ParameterDirection.Input;
            formilaeAdminParam.Value = formilaeAdmin;

            MySqlParameter processNameParam = new MySqlParameter("vProcessName", MySqlDbType.String);
            processNameParam.Direction = ParameterDirection.Input;
            processNameParam.Value = processName;

            MySqlParameter summaryParam = new MySqlParameter("vSummary", MySqlDbType.String);
            summaryParam.Direction = ParameterDirection.Input;
            summaryParam.Value = summary;

            MySqlParameter govMailAddr1Param = new MySqlParameter("vGovernmentMailingAddress1", MySqlDbType.String);
            govMailAddr1Param.Direction = ParameterDirection.Input;
            govMailAddr1Param.Value = governmentMailingAddress1;

            MySqlParameter packetTitleParam = new MySqlParameter("vPacketTitle", MySqlDbType.String);
            packetTitleParam.Direction = ParameterDirection.Input;
            packetTitleParam.Value = packetTitle;

            MySqlParameter reqAdminApprParam = new MySqlParameter("vRequiresAdminApproval", MySqlDbType.Int32);
            reqAdminApprParam.Direction = ParameterDirection.Input;
            reqAdminApprParam.Value = requiresAdminApproval;

            MySqlParameter genEmailParam = new MySqlParameter("vGeneratesEmail", MySqlDbType.Int32);
            genEmailParam.Direction = ParameterDirection.Input;
            genEmailParam.Value = generatesEmail;

            MySqlParameter extRedirectParam = new MySqlParameter("vExternalRedirect", MySqlDbType.String);
            extRedirectParam.Direction = ParameterDirection.Input;
            extRedirectParam.Value = externalRedirect;

            MySqlParameter flHeaderMsgParam = new MySqlParameter("vFormListHeaderMessage", MySqlDbType.String);
            flHeaderMsgParam.Direction = ParameterDirection.Input;
            flHeaderMsgParam.Value = formListHeaderMessage;

            MySqlParameter welcomeEmailParam = new MySqlParameter("vWelcomeEmail", MySqlDbType.String);
            welcomeEmailParam.Direction = ParameterDirection.Input;
            welcomeEmailParam.Value = welcomeEmail;

            MySqlParameter userSuppliesEmailParam = new MySqlParameter("vUserSuppliesEmail", MySqlDbType.Int16);
            userSuppliesEmailParam.Direction = ParameterDirection.Input;
            userSuppliesEmailParam.Value = userSuppliesEmail;

            MySqlParameter noEmailAuthParam = new MySqlParameter("vNoEmailAuth", MySqlDbType.Int16);
            noEmailAuthParam.Direction = ParameterDirection.Input;
            noEmailAuthParam.Value = noEmailAuth;

            MySqlParameter humanVerifyNeededParam = new MySqlParameter("vHumanVerifyNeeded", MySqlDbType.Int16);
            humanVerifyNeededParam.Direction = ParameterDirection.Input;
            humanVerifyNeededParam.Value = humanVerifyNeeded;

            MySqlParameter emailSurveyImageParam = new MySqlParameter("vEmailSurveyImage", MySqlDbType.String);
            emailSurveyImageParam.Direction = ParameterDirection.Input;
            emailSurveyImageParam.Value = emailSurveyImage;

            MySqlParameter emailSurveyPhoneUseParam = new MySqlParameter("vEmailSurveyPhoneUse", MySqlDbType.Int16);
            emailSurveyPhoneUseParam.Direction = ParameterDirection.Input;
            emailSurveyPhoneUseParam.Value = emailSurveyPhoneUse;

            MySqlParameter emailSurveyNameUseparam = new MySqlParameter("vEmailSurveyNameUse", MySqlDbType.Int16);
            emailSurveyNameUseparam.Direction = ParameterDirection.Input;
            emailSurveyNameUseparam.Value = emailSurveyNameUse;

            MySqlParameter emailSurveyNameReqParam = new MySqlParameter("vEmailSurveyNameRequired", MySqlDbType.Int16);
            emailSurveyNameReqParam.Direction = ParameterDirection.Input;
            emailSurveyNameReqParam.Value = emailSurveyNameRequired;

            MySqlParameter emailSurveyPhoneReqParam = new MySqlParameter("vEmailSurveyPhoneRequired", MySqlDbType.Int16);
            emailSurveyPhoneReqParam.Direction = ParameterDirection.Input;
            emailSurveyPhoneReqParam.Value = emailSurveyPhoneRequired;

            MySqlParameter emailSurveyEmailReqParam = new MySqlParameter("vEmailSurveyEmailRequired", MySqlDbType.Int16);
            emailSurveyEmailReqParam.Direction = ParameterDirection.Input;
            emailSurveyEmailReqParam.Value = emailSurveyEmailRequired;

            MySqlParameter emailSurveyStartHeaderTxtParam = new MySqlParameter("vEmailSurveyStartHeaderText", MySqlDbType.String);
            emailSurveyStartHeaderTxtParam.Direction = ParameterDirection.Input;
            emailSurveyStartHeaderTxtParam.Value = emailSurveyStartHeaderText;

            MySqlParameter beginWithoutRegistrationParam = new MySqlParameter("vBeginWithoutRegistration", MySqlDbType.Int16);
            beginWithoutRegistrationParam.Direction = ParameterDirection.Input;
            beginWithoutRegistrationParam.Value = beginWithoutRegistration;

            MySqlParameter hyperlinkDirectoryParam = new MySqlParameter("vHyperlinkDirectory", MySqlDbType.String);
            hyperlinkDirectoryParam.Direction = ParameterDirection.Input;
            hyperlinkDirectoryParam.Value = hyperlinkDirectory;

            MySqlParameter questionnaireGeneratePDFParam = new MySqlParameter("vQuestionnaireGeneratePDF", MySqlDbType.Int16);
            questionnaireGeneratePDFParam.Direction = ParameterDirection.Input;
            questionnaireGeneratePDFParam.Value = questionnaireGeneratePDF;

            MySqlParameter[] spParams = new MySqlParameter[] {
                customerIDParam,
                processIdParam,
                formilaeAdminParam,
                processNameParam,
                summaryParam,
                govMailAddr1Param,
                packetTitleParam,
                reqAdminApprParam,
                genEmailParam,
                extRedirectParam,
                flHeaderMsgParam,
                welcomeEmailParam,
                userSuppliesEmailParam,
                noEmailAuthParam,
                humanVerifyNeededParam,
                emailSurveyImageParam,
                emailSurveyPhoneUseParam,
                emailSurveyNameUseparam,
                emailSurveyNameReqParam,
                emailSurveyPhoneReqParam,
                emailSurveyEmailReqParam,
                emailSurveyStartHeaderTxtParam,
                beginWithoutRegistrationParam,
                hyperlinkDirectoryParam,
                questionnaireGeneratePDFParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL processupdate(@vCustomerID, @vProcessID, @vFormilaeAdmin, @vProcessName, @vSummary, @vGovernmentMailingAddress1, @vPacketTitle, @vRequiresAdminApproval, @vGeneratesEmail, @vExternalRedirect, @vFormListHeaderMessage, @vWelcomeEmail, @vUserSuppliesEmail, @vNoEmailAuth, @vHumanVerifyNeeded, @vEmailSurveyImage, @vEmailSurveyPhoneUse, @vEmailSurveyNameUse, @vEmailSurveyNameRequired, @vEmailSurveyPhoneRequired, @vEmailSurveyEmailRequired, @vEmailSurveyStartHeaderText, @vBeginWithoutRegistration, @vHyperlinkDirectory, @vQuestionnaireGeneratePDF)");

            string commandText = sb.ToString();
            this.Database.ExecuteSqlCommand(commandText, spParams);
        }

        public void ProcessActiveUpdate(int processId, int active)
        {
            MySqlParameter processIDParam = new MySqlParameter("vProcessID", MySqlDbType.Int32);
            processIDParam.Direction = ParameterDirection.Input;
            processIDParam.Value = processId;

            MySqlParameter activeParam = new MySqlParameter("vActive", MySqlDbType.Int32);
            activeParam.Direction = ParameterDirection.Input;
            activeParam.Value = active;

            MySqlParameter[] spParams = new MySqlParameter[] {
                processIDParam,
                activeParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL processactiveupdate(@vProcessID, @vActive)");

            string commandText = sb.ToString();
            this.Database.ExecuteSqlCommand(commandText, spParams);
        }

        public void AdminAddProcessCost(int customerId, int processId, string itemName, string itemDescription, decimal price, int sortOrder)
        {
            MySqlParameter customerIDParam = new MySqlParameter("vCustomerID", MySqlDbType.Int32);
            customerIDParam.Direction = ParameterDirection.Input;
            customerIDParam.Value = customerId;

            MySqlParameter processIdParam = new MySqlParameter("vProcessID", MySqlDbType.Int32);
            processIdParam.Direction = ParameterDirection.Input;
            processIdParam.Value = processId;

            MySqlParameter itemNameParam = new MySqlParameter("vItemName", MySqlDbType.String);
            itemNameParam.Direction = ParameterDirection.Input;
            itemNameParam.Value = itemName;

            MySqlParameter itemDescParam = new MySqlParameter("vItemDescription", MySqlDbType.String);
            itemDescParam.Direction = ParameterDirection.Input;
            itemDescParam.Value = itemDescription;

            MySqlParameter priceParam = new MySqlParameter("vPrice", MySqlDbType.Decimal);
            priceParam.Direction = ParameterDirection.Input;
            priceParam.Value = price;

            MySqlParameter sortOrderParam = new MySqlParameter("vSortOrder", MySqlDbType.Int32);
            sortOrderParam.Direction = ParameterDirection.Input;
            sortOrderParam.Value = sortOrder;

            MySqlParameter[] spParams = new MySqlParameter[] {
                customerIDParam,
                processIdParam,
                itemNameParam,
                itemDescParam,
                priceParam,
                sortOrderParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL admin_processcostadd(@vCustomerID, @vProcessID, @vItemName, @vItemDescription, @vPrice, @vSortOrder)");

            string commandText = sb.ToString();
            this.Database.ExecuteSqlCommand(commandText, spParams);
        }

        public void AdminUpdateProcessCost(int processCostId, string itemName, string itemDescription, decimal price, int sortOrder, int active)
        {
            MySqlParameter processCostIDParam = new MySqlParameter("vProcessCostID", MySqlDbType.Int32);
            processCostIDParam.Direction = ParameterDirection.Input;
            processCostIDParam.Value = processCostId;

            MySqlParameter itemNameParam = new MySqlParameter("vItemName", MySqlDbType.String);
            itemNameParam.Direction = ParameterDirection.Input;
            itemNameParam.Value = itemName;

            MySqlParameter itemDescParam = new MySqlParameter("vItemDescription", MySqlDbType.String);
            itemDescParam.Direction = ParameterDirection.Input;
            itemDescParam.Value = itemDescription;

            MySqlParameter priceParam = new MySqlParameter("vPrice", MySqlDbType.Decimal);
            priceParam.Direction = ParameterDirection.Input;
            priceParam.Value = price;

            MySqlParameter sortOrderParam = new MySqlParameter("vSortOrder", MySqlDbType.Int32);
            sortOrderParam.Direction = ParameterDirection.Input;
            sortOrderParam.Value = sortOrder;

            MySqlParameter activeParam = new MySqlParameter("vActive", MySqlDbType.Int16);
            activeParam.Direction = ParameterDirection.Input;
            activeParam.Value = active;

            MySqlParameter[] spParams = new MySqlParameter[] {
                processCostIDParam,
                itemNameParam,
                itemDescParam,
                priceParam,
                sortOrderParam,
                activeParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL admin_processcostupdate(@vProcessCostID, @vItemName, @vItemDescription, @vPrice, @vSortOrder, @vActive)");

            string commandText = sb.ToString();
            this.Database.ExecuteSqlCommand(commandText, spParams);
        }

        // For the services grid in the admin section
        public virtual ObjectResult<ProcessesGet> GetAdminProcesses(Nullable<int> customerId, string filter)
        {
            return GetProcesses(customerId, 0, filter);
        }

        // For the surveys grid in the admin section
        public virtual ObjectResult<ProcessesGet> GetAdminSurveys(Nullable<int> customerId, string filter)
        {
            return GetProcesses(customerId, 1, filter);
        }

        public virtual ObjectResult<ProcessesGet> GetProcesses(Nullable<int> customerId, Nullable<int> surveysOnly, string filter)
        {
            MySqlParameter customerIDParam = new MySqlParameter("vCustomerID", MySqlDbType.Int32);
            customerIDParam.Direction = ParameterDirection.Input;
            customerIDParam.Value = customerId;

            MySqlParameter surveysOnlyParam = new MySqlParameter("vSurveysOnly", MySqlDbType.Int16);
            surveysOnlyParam.Direction = ParameterDirection.Input;
            surveysOnlyParam.Value = surveysOnly;

            filter = String.IsNullOrEmpty(filter) ? String.Empty : filter;

            MySqlParameter filterParam = new MySqlParameter("vFilter", MySqlDbType.String);
            filterParam.Direction = ParameterDirection.Input;
            filterParam.Value = filter;

            MySqlParameter[] spParams = new MySqlParameter[] {
                customerIDParam,
                surveysOnlyParam,
                filterParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL processesget(@vCustomerID, @vSurveysOnly, @vFilter)");

            string commandText = sb.ToString();

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<ProcessesGet>(commandText, spParams);
        }

        public virtual ObjectResult<ProcessGet> GetCustomerProcess(Nullable<int> customerId, Nullable<int> processId)
        {
            MySqlParameter customerIDParam = new MySqlParameter("vCustomerID", MySqlDbType.Int32);
            customerIDParam.Direction = ParameterDirection.Input;
            customerIDParam.Value = customerId;

            MySqlParameter processIDParam = new MySqlParameter("vProcessID", MySqlDbType.Int32);
            processIDParam.Direction = ParameterDirection.Input;
            processIDParam.Value = processId;

            MySqlParameter[] spParams = new MySqlParameter[] {
                customerIDParam,
                processIDParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL processget(@vCustomerID, @vProcessID)");

            string commandText = sb.ToString();


            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<ProcessGet>(commandText, spParams);
        }

        // User for admin process manager grid
        public virtual ObjectResult<ProcessFormsGet> AdminGetProcessForms(int processId)
        {
            return GetProcessForms(processId, 1);
        }

        public virtual ObjectResult<ProcessFormsGet> GetProcessForms(int processId, int includeAsNeededForms)
        {
            return GetProcessForms(processId, includeAsNeededForms, 0);
        }

        public virtual ObjectResult<ProcessFormsGet> GetProcessForms(int processId, int includeAsNeededForms, int includeInactive)
        {
            MySqlParameter processIdParam = new MySqlParameter("vProcessID", MySqlDbType.Int32);
            processIdParam.Direction = ParameterDirection.Input;
            processIdParam.Value = processId;

            MySqlParameter inclAsNeededForms = new MySqlParameter("vIncludeAsNeededForms", MySqlDbType.Int32);
            inclAsNeededForms.Direction = ParameterDirection.Input;
            inclAsNeededForms.Value = includeAsNeededForms;

            MySqlParameter inclInactive = new MySqlParameter("vIncludeInactive", MySqlDbType.Int16);
            inclInactive.Direction = ParameterDirection.Input;
            inclInactive.Value = includeInactive;

            MySqlParameter[] spParams = new MySqlParameter[] {
                processIdParam,
                inclAsNeededForms,
                inclInactive
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL processformsget(@vProcessID, @vIncludeAsNeededForms, @vIncludeInactive)");

            string commandText = sb.ToString();

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<ProcessFormsGet>(commandText, spParams);
        }

        public void ClientAvatarDelete(int clientId)
        {
            ClientAvatarUpdate(clientId, "");
        }

        public void ClientAvatarUpdate(int clientId, string avatar)
        {
            MySqlParameter clientIDParam = new MySqlParameter("vClientID", MySqlDbType.Int32);
            clientIDParam.Direction = ParameterDirection.Input;
            clientIDParam.Value = clientId;

            MySqlParameter avatarParam = new MySqlParameter("vAvatar", MySqlDbType.String);
            avatarParam.Direction = ParameterDirection.Input;
            avatarParam.Value = avatar;

            MySqlParameter[] spParams = new MySqlParameter[] {
                clientIDParam,
                avatarParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL admin_clientavatarupdate(@vClientID, @vAvatar)");

            string commandText = sb.ToString();
            this.Database.ExecuteSqlCommand(commandText, spParams);
        }

        public void ClientRolesAdd(int clientId, string roleIds)
        {
            MySqlParameter clientIDParam = new MySqlParameter("vClientID", MySqlDbType.Int32);
            clientIDParam.Direction = ParameterDirection.Input;
            clientIDParam.Value = clientId;

            MySqlParameter roleIdsParam = new MySqlParameter("vRoleIDs", MySqlDbType.String);
            roleIdsParam.Direction = ParameterDirection.Input;
            roleIdsParam.Value = roleIds;

            MySqlParameter[] spParams = new MySqlParameter[] {
                clientIDParam,
                roleIdsParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL admin_userrolesadd(@vClientID, @vRoleIDs)");

            string commandText = sb.ToString();
            this.Database.ExecuteSqlCommand(commandText, spParams);
        }

        public virtual ObjectResult<AdminUserRolesGet> GetAdminUserRoles(Nullable<int> clientId)
        {
            return GetAdminUserRoles(clientId, 0);
        }

        public virtual ObjectResult<AdminUserRolesGet> GetAdminUserRoles(Nullable<int> clientId, Nullable<int> selectedOnly)
        {
            MySqlParameter clientIDParam = new MySqlParameter("vClientID", MySqlDbType.Int32);
            clientIDParam.Direction = ParameterDirection.Input;
            clientIDParam.Value = clientId;

            MySqlParameter selectedOnlyParam = new MySqlParameter("vSelectedOnly", MySqlDbType.Int16);
            selectedOnlyParam.Direction = ParameterDirection.Input;
            selectedOnlyParam.Value = selectedOnly;

            MySqlParameter[] spParams = new MySqlParameter[] {
                clientIDParam,
                selectedOnlyParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL admin_userrolesget(@vClientID, @vSelectedOnly)");

            string commandText = sb.ToString();

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<AdminUserRolesGet>(commandText, spParams);
        }

        public virtual ObjectResult<UserPermissionsGet> GetUserPermissions(Nullable<int> userId, Nullable<int> activeOnly)
        {
            return GetUserPermissions(userId, activeOnly, -1);
        }

        // Returns permissions whether they are active/inactive
        public virtual ObjectResult<UserPermissionsGet> GetUserPermissions(Nullable<int> userId, Nullable<int> activeOnly, Nullable<int> roleId)
        {
            MySqlParameter userIDParam = new MySqlParameter("vUserID", MySqlDbType.Int32);
            userIDParam.Direction = ParameterDirection.Input;
            userIDParam.Value = userId;

            MySqlParameter activeOnlyParam = new MySqlParameter("vActiveOnly", MySqlDbType.Int16);
            activeOnlyParam.Direction = ParameterDirection.Input;
            activeOnlyParam.Value = activeOnly;

            MySqlParameter roleIdParam = new MySqlParameter("vRoleID", MySqlDbType.Int32);
            roleIdParam.Direction = ParameterDirection.Input;
            roleIdParam.Value = roleId;

            MySqlParameter[] spParams = new MySqlParameter[] {
                userIDParam,
                activeOnlyParam,
                roleIdParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL userpermissionsget(@vUserID, @vActiveOnly, @vRoleID)");

            string commandText = sb.ToString();

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<UserPermissionsGet>(commandText, spParams);
        }

        public virtual ObjectResult<SurveySocialMediaThumbImagesGet> GetSocialMediaThumbImages(Nullable<int> customerId)
        {
            MySqlParameter customerIDParam = new MySqlParameter("vCustomerID", MySqlDbType.Int32);
            customerIDParam.Direction = ParameterDirection.Input;
            customerIDParam.Value = customerId;

            MySqlParameter[] spParams = new MySqlParameter[] {
                customerIDParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL surveysocialmediathumbimagesget(@vCustomerID)");

            string commandText = sb.ToString();

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<SurveySocialMediaThumbImagesGet>(commandText, spParams);
        }

        public virtual ObjectResult<SurveyStartPageImagesGet> GetSurveyStartPageImages(Nullable<int> customerId)
        {
            MySqlParameter customerIDParam = new MySqlParameter("vCustomerID", MySqlDbType.Int32);
            customerIDParam.Direction = ParameterDirection.Input;
            customerIDParam.Value = customerId;

            MySqlParameter[] spParams = new MySqlParameter[] {
                customerIDParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL surveystartpageimagesget(@vCustomerID)");

            string commandText = sb.ToString();

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<SurveyStartPageImagesGet>(commandText, spParams);
        }

        public virtual ObjectResult<CustomerGet> GetCustomer(Nullable<int> customerId)
        {
            MySqlParameter customerIDParam = new MySqlParameter("vCustomerID", MySqlDbType.Int32);
            customerIDParam.Direction = ParameterDirection.Input;
            customerIDParam.Value = customerId;

            MySqlParameter[] spParams = new MySqlParameter[] {
                customerIDParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL customerget(@vCustomerID)");

            string commandText = sb.ToString();

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<CustomerGet>(commandText, spParams);
        }

        public virtual ObjectResult<StateListGet> GetStateList(Nullable<int> customerId)
        {
            MySqlParameter customerIDParam = new MySqlParameter("vCustomerID", MySqlDbType.Int32);
            customerIDParam.Direction = ParameterDirection.Input;
            customerIDParam.Value = customerId;

            MySqlParameter[] spParams = new MySqlParameter[] {
                customerIDParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL statelistget(@vCustomerID)");

            string commandText = sb.ToString();

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<StateListGet>(commandText, spParams);
        }

        public void CustomerUpdate(int customerId, string businessName, string businessSubTitle, string contactFirstName,
            string contactLastName, string contactEmail, string address, string city, string state, string zip,
            string phone, string province, string postalCode, string country, string logoFile, string aboutUsText)
        {
            businessSubTitle = String.IsNullOrEmpty(businessSubTitle) ? "" : businessSubTitle;
            contactFirstName = String.IsNullOrEmpty(contactFirstName) ? "" : contactFirstName;
            contactLastName = String.IsNullOrEmpty(contactLastName) ? "" : contactLastName;
            contactEmail = String.IsNullOrEmpty(contactEmail) ? "" : contactEmail;
            address = String.IsNullOrEmpty(address) ? "" : address;
            city = String.IsNullOrEmpty(city) ? "" : city;
            state = String.IsNullOrEmpty(state) ? "" : state;
            zip = String.IsNullOrEmpty(zip) ? "" : zip;
            phone = String.IsNullOrEmpty(phone) ? "" : phone;
            province = String.IsNullOrEmpty(province) ? "" : province;
            postalCode = String.IsNullOrEmpty(postalCode) ? "" : postalCode;
            country = String.IsNullOrEmpty(country) ? "" : country;
            logoFile = String.IsNullOrEmpty(logoFile) ? "" : logoFile;
            aboutUsText = String.IsNullOrEmpty(aboutUsText) ? "" : aboutUsText;

            MySqlParameter customerIDParam = new MySqlParameter("vCustomerID", MySqlDbType.Int32);
            customerIDParam.Direction = ParameterDirection.Input;
            customerIDParam.Value = customerId;

            MySqlParameter businessNameParam = new MySqlParameter("vBusinessName", MySqlDbType.String);
            businessNameParam.Direction = ParameterDirection.Input;
            businessNameParam.Value = businessName;

            MySqlParameter businessSubTitleParam = new MySqlParameter("vBusinessSubTitle", MySqlDbType.String);
            businessSubTitleParam.Direction = ParameterDirection.Input;
            businessSubTitleParam.Value = businessSubTitle;

            MySqlParameter contactFirstNameParam = new MySqlParameter("vContactFirstName", MySqlDbType.String);
            contactFirstNameParam.Direction = ParameterDirection.Input;
            contactFirstNameParam.Value = contactFirstName;

            MySqlParameter contactLastNameParam = new MySqlParameter("vContactLastName", MySqlDbType.String);
            contactLastNameParam.Direction = ParameterDirection.Input;
            contactLastNameParam.Value = contactLastName;

            MySqlParameter contactEmailParam = new MySqlParameter("vContactEmail", MySqlDbType.String);
            contactEmailParam.Direction = ParameterDirection.Input;
            contactEmailParam.Value = contactEmail;

            MySqlParameter addressParam = new MySqlParameter("vAddress", MySqlDbType.String);
            addressParam.Direction = ParameterDirection.Input;
            addressParam.Value = address;

            MySqlParameter cityParam = new MySqlParameter("vCity", MySqlDbType.String);
            cityParam.Direction = ParameterDirection.Input;
            cityParam.Value = city;

            MySqlParameter stateParam = new MySqlParameter("vState", MySqlDbType.String);
            stateParam.Direction = ParameterDirection.Input;
            stateParam.Value = state;

            MySqlParameter zipCodeParam = new MySqlParameter("vZip", MySqlDbType.String);
            zipCodeParam.Direction = ParameterDirection.Input;
            zipCodeParam.Value = zip;

            MySqlParameter phoneParam = new MySqlParameter("vPhone", MySqlDbType.String);
            phoneParam.Direction = ParameterDirection.Input;
            phoneParam.Value = phone;

            MySqlParameter provinceParam = new MySqlParameter("vProvince", MySqlDbType.String);
            provinceParam.Direction = ParameterDirection.Input;
            provinceParam.Value = province;

            MySqlParameter postalCodeParam = new MySqlParameter("vPostalCode", MySqlDbType.String);
            postalCodeParam.Direction = ParameterDirection.Input;
            postalCodeParam.Value = postalCode;

            MySqlParameter countryParam = new MySqlParameter("vCountry", MySqlDbType.String);
            countryParam.Direction = ParameterDirection.Input;
            countryParam.Value = country;

            MySqlParameter logoFileParam = new MySqlParameter("vLogoFile", MySqlDbType.String);
            logoFileParam.Direction = ParameterDirection.Input;
            logoFileParam.Value = logoFile;

            MySqlParameter aboutUsTextParam = new MySqlParameter("vAboutUsText", MySqlDbType.String);
            aboutUsTextParam.Direction = ParameterDirection.Input;
            aboutUsTextParam.Value = aboutUsText;

            MySqlParameter[] spParams = new MySqlParameter[] {
                customerIDParam,
                businessNameParam,
                businessSubTitleParam,
                contactFirstNameParam,
                contactLastNameParam,
                contactEmailParam,
                addressParam,
                cityParam,
                stateParam,
                zipCodeParam,
                phoneParam,
                provinceParam,
                postalCodeParam,
                countryParam,
                logoFileParam,
                aboutUsTextParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL customerupdate(@vCustomerID, @vBusinessName, @vBusinessSubTitle, @vContactFirstName, @vContactLastName, @vContactEmail, @vAddress, " +
                "@vCity, @vState, @vZip, @vPhone, @vProvince, @vPostalCode, @vCountry, @vLogoFile, @vAboutUsText)");

            string commandText = sb.ToString();
            this.Database.ExecuteSqlCommand(commandText, spParams);
        }

        public virtual ObjectResult<SurveyDirectoriesGet> GetSurveyDirectories(Nullable<int> customerId)
        {
            MySqlParameter customerIDParam = new MySqlParameter("vCustomerID", MySqlDbType.Int32);
            customerIDParam.Direction = ParameterDirection.Input;
            customerIDParam.Value = customerId;

            MySqlParameter[] spParams = new MySqlParameter[] {
                customerIDParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL surveydirectoriesget(@vCustomerID)");

            string commandText = sb.ToString();

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<SurveyDirectoriesGet>(commandText, spParams);
        }

        public void ProcessFormActiveUpdate(int processFormId, int active)
        {
            MySqlParameter processFormIDParam = new MySqlParameter("vProcessFormID", MySqlDbType.Int32);
            processFormIDParam.Direction = ParameterDirection.Input;
            processFormIDParam.Value = processFormId;

            MySqlParameter activeParam = new MySqlParameter("vActive", MySqlDbType.Int32);
            activeParam.Direction = ParameterDirection.Input;
            activeParam.Value = active;

            MySqlParameter[] spParams = new MySqlParameter[] {
                processFormIDParam,
                activeParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL processformactiveupdate(@vProcessFormID, @vActive)");

            string commandText = sb.ToString();
            this.Database.ExecuteSqlCommand(commandText, spParams);
        }

        public virtual ObjectResult<ProcessFormsGet> GetSurveyProcessForms(int processId, int includeAsNeededForms, int includeInactive)
        {
            MySqlParameter processIdParam = new MySqlParameter("vProcessID", MySqlDbType.Int32);
            processIdParam.Direction = ParameterDirection.Input;
            processIdParam.Value = processId;

            MySqlParameter inclAsNeededForms = new MySqlParameter("vIncludeAsNeededForms", MySqlDbType.Int32);
            inclAsNeededForms.Direction = ParameterDirection.Input;
            inclAsNeededForms.Value = includeAsNeededForms;

            MySqlParameter inclInactive = new MySqlParameter("vIncludeInactive", MySqlDbType.Int16);
            inclInactive.Direction = ParameterDirection.Input;
            inclInactive.Value = includeInactive;

            MySqlParameter[] spParams = new MySqlParameter[] {
                processIdParam,
                inclAsNeededForms,
                inclInactive
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL surveyprocessformsget(@vProcessID, @vIncludeAsNeededForms, @vIncludeInactive)");

            string commandText = sb.ToString();

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<ProcessFormsGet>(commandText, spParams);
        }

        public virtual ObjectResult<ProcessFormGet> GetProcessForm(int processFormId)
        {
            MySqlParameter processFormIdParam = new MySqlParameter("vProcessFormID", MySqlDbType.Int32);
            processFormIdParam.Direction = ParameterDirection.Input;
            processFormIdParam.Value = processFormId;

            MySqlParameter[] spParams = new MySqlParameter[] {
                processFormIdParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL processformget(@vProcessFormID)");

            string commandText = sb.ToString();

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<ProcessFormGet>(commandText, spParams);
        }

        // Use this method for "surveys" ONLY
        public void ProcessFormUpdate(int processFormId, string formMessageTop, int formMessageTopPlain,
            int displayResults, string resultsMessageTop)
        {
            ProcessFormUpdate(processFormId, formMessageTop, formMessageTopPlain, displayResults, resultsMessageTop,
                1, 0, 0, 1, 0, "", "");
        }

        public void ProcessFormUpdate(int processFormId, string formMessageTop, int formMessageTopPlain,
            int displayResults, string resultsMessageTop,
            int autoAddToService, int requiresAdminApproval, int allowAdditionalAttachment, int required,
            int serviceListDisplay, string formApprovedOpeningEmailBodyMsg, string approvedEmailIfYouHaveQuestionsFooter)
        {
            formMessageTop = String.IsNullOrEmpty(formMessageTop) ? "" : formMessageTop;
            resultsMessageTop = String.IsNullOrEmpty(resultsMessageTop) ? "" : resultsMessageTop;
            formApprovedOpeningEmailBodyMsg = String.IsNullOrEmpty(formApprovedOpeningEmailBodyMsg) ? "" : formApprovedOpeningEmailBodyMsg;
            approvedEmailIfYouHaveQuestionsFooter = String.IsNullOrEmpty(approvedEmailIfYouHaveQuestionsFooter) ? "" : approvedEmailIfYouHaveQuestionsFooter;

            MySqlParameter processFormIdParam = new MySqlParameter("vProcessFormID", MySqlDbType.Int32);
            processFormIdParam.Direction = ParameterDirection.Input;
            processFormIdParam.Value = processFormId;

            MySqlParameter formMsgTopParam = new MySqlParameter("vFormMessageTop", MySqlDbType.String);
            formMsgTopParam.Direction = ParameterDirection.Input;
            formMsgTopParam.Value = formMessageTop;

            MySqlParameter formMsgTopPlainParam = new MySqlParameter("vFormMessageTopPlain", MySqlDbType.Int16);
            formMsgTopPlainParam.Direction = ParameterDirection.Input;
            formMsgTopPlainParam.Value = formMessageTopPlain;

            MySqlParameter displayResultsParam = new MySqlParameter("vDisplayResults", MySqlDbType.Int16);
            displayResultsParam.Direction = ParameterDirection.Input;
            displayResultsParam.Value = displayResults;

            MySqlParameter resultsMsgTopParam = new MySqlParameter("vResultsMessageTop", MySqlDbType.String);
            resultsMsgTopParam.Direction = ParameterDirection.Input;
            resultsMsgTopParam.Value = resultsMessageTop;

            MySqlParameter autoAddToServParam = new MySqlParameter("vAutoAddToService", MySqlDbType.Int16);
            autoAddToServParam.Direction = ParameterDirection.Input;
            autoAddToServParam.Value = autoAddToService;

            MySqlParameter reqAdminApprovalParam = new MySqlParameter("vRequiresAdminApproval", MySqlDbType.Int16);
            reqAdminApprovalParam.Direction = ParameterDirection.Input;
            reqAdminApprovalParam.Value = requiresAdminApproval;

            MySqlParameter allowAdditnlAttmntParam = new MySqlParameter("vAllowAdditionalAttachment", MySqlDbType.Int16);
            allowAdditnlAttmntParam.Direction = ParameterDirection.Input;
            allowAdditnlAttmntParam.Value = allowAdditionalAttachment;

            MySqlParameter requiredParam = new MySqlParameter("vRequired", MySqlDbType.Int16);
            requiredParam.Direction = ParameterDirection.Input;
            requiredParam.Value = required;

            MySqlParameter serviceListDisplayParam = new MySqlParameter("vServiceListDisplay", MySqlDbType.Int16);
            serviceListDisplayParam.Direction = ParameterDirection.Input;
            serviceListDisplayParam.Value = serviceListDisplay;

            MySqlParameter formApprovedOpEmBodyMsgParam = new MySqlParameter("vFormApprovedOpeningEmailBodyMsg", MySqlDbType.String);
            formApprovedOpEmBodyMsgParam.Direction = ParameterDirection.Input;
            formApprovedOpEmBodyMsgParam.Value = formApprovedOpeningEmailBodyMsg;

            MySqlParameter apprvdEmailQstnFooterParam = new MySqlParameter("vApprovedEmailIfYouHaveQuestionsFooter", MySqlDbType.String);
            apprvdEmailQstnFooterParam.Direction = ParameterDirection.Input;
            apprvdEmailQstnFooterParam.Value = approvedEmailIfYouHaveQuestionsFooter;

            MySqlParameter[] spParams = new MySqlParameter[] {
                processFormIdParam,
                formMsgTopParam,
                formMsgTopPlainParam,
                displayResultsParam,
                resultsMsgTopParam,
                autoAddToServParam,
                reqAdminApprovalParam,
                allowAdditnlAttmntParam,
                requiredParam,
                serviceListDisplayParam,
                formApprovedOpEmBodyMsgParam,
                apprvdEmailQstnFooterParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL processformupdate(@vProcessFormID, @vFormMessageTop, @vFormMessageTopPlain, @vDisplayResults, @vResultsMessageTop, @vAutoAddToService, @vRequiresAdminApproval, @vAllowAdditionalAttachment, @vRequired, @vServiceListDisplay, @vFormApprovedOpeningEmailBodyMsg, @vApprovedEmailIfYouHaveQuestionsFooter)");

            string commandText = sb.ToString();
            this.Database.ExecuteSqlCommand(commandText, spParams);
        }

        public virtual ObjectResult<SessionInsertOutputGet> GetSessionInsertOutput(string sessionKey)
        {
            sessionKey = String.IsNullOrEmpty(sessionKey) ? String.Empty : sessionKey;

            MySqlParameter sessionKeyParam = new MySqlParameter("vSessionKey", MySqlDbType.String);
            sessionKeyParam.Direction = ParameterDirection.Input;
            sessionKeyParam.Value = sessionKey;

            MySqlParameter[] spParams = new MySqlParameter[] {
                sessionKeyParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL sessioninsertoutputget(@vSessionKey)");

            string commandText = sb.ToString();

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<SessionInsertOutputGet>(commandText, spParams);
        }

        // Add support ticket w/first message
        public void AddSupportMessage(int clientId, string subject, string message)
        {
            var sessionKey = RandomHelper.GenerateGUID();
            AddSupportMessage(clientId, -1, subject, message, 0, sessionKey);  // When no output SupportID needed
        }
        public void AddSupportMessage(int clientId, string subject, string message, string sessionKey)
        {
            AddSupportMessage(clientId, -1, subject, message, 0, sessionKey);  // Output SupportID needed
        }

        // Add additional message to already existing support ticket
        public void AddSupportMessage(int clientId, int supportId, string message, int internalMessage)
        {
            AddSupportMessage(clientId, supportId, "", message, internalMessage, "");
        }

        public void AddSupportMessage(int clientId, int supportId, string subject, string message, 
            int internalMessage, string sessionKey)
        {
            subject = String.IsNullOrEmpty(subject) ? String.Empty : subject;
            message = String.IsNullOrEmpty(message) ? String.Empty : message;
            sessionKey = String.IsNullOrEmpty(sessionKey) ? String.Empty : sessionKey;

            MySqlParameter clientIdParam = new MySqlParameter("vClientID", MySqlDbType.Int32);
            clientIdParam.Direction = ParameterDirection.Input;
            clientIdParam.Value = clientId;

            MySqlParameter supportIdParam = new MySqlParameter("vSupportID", MySqlDbType.Int32);
            supportIdParam.Direction = ParameterDirection.Input;
            supportIdParam.Value = supportId;

            MySqlParameter subjectParam = new MySqlParameter("vSubject", MySqlDbType.String);
            subjectParam.Direction = ParameterDirection.Input;
            subjectParam.Value = subject;

            MySqlParameter messageParam = new MySqlParameter("vMessage", MySqlDbType.String);
            messageParam.Direction = ParameterDirection.Input;
            messageParam.Value = message;

            MySqlParameter internalMsgParam = new MySqlParameter("vInternal", MySqlDbType.Int16);
            internalMsgParam.Direction = ParameterDirection.Input;
            internalMsgParam.Value = internalMessage;

            MySqlParameter sessionKeyParam = new MySqlParameter("vSessionKey", MySqlDbType.String);
            sessionKeyParam.Direction = ParameterDirection.Input;
            sessionKeyParam.Value = sessionKey;

            MySqlParameter[] spParams = new MySqlParameter[] {
                clientIdParam,
                supportIdParam,
                subjectParam,
                messageParam,
                internalMsgParam,
                sessionKeyParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL supportmessageadd(@vClientID, @vSupportID, @vSubject, @vMessage, @vInternal, @vSessionKey)");

            string commandText = sb.ToString();
            this.Database.ExecuteSqlCommand(commandText, spParams);
        }

        public virtual ObjectResult<SupportTicketsGet> GetSupportTickets(Nullable<int> clientId)
        {
            MySqlParameter clientIDParam = new MySqlParameter("vClientID", MySqlDbType.Int32);
            clientIDParam.Direction = ParameterDirection.Input;
            clientIDParam.Value = clientId;

            MySqlParameter[] spParams = new MySqlParameter[] {
                clientIDParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL supportticketsget(@vClientID)");

            string commandText = sb.ToString();

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<SupportTicketsGet>(commandText, spParams);
        }

        public virtual ObjectResult<AvailableClientEventsGet> GetAvailableClientEvents(Nullable<int> clientId, Nullable<int> eventId,
            MySql.Data.Types.MySqlDateTime bookingDate, Nullable<int> userTimezoneOffset)
        {
            return GetAvailableClientEvents(clientId, eventId, bookingDate, -1, userTimezoneOffset);
        }

        public virtual ObjectResult<AvailableClientEventsGet> GetAvailableClientEvents(Nullable<int> clientId, Nullable<int> eventId, 
            MySql.Data.Types.MySqlDateTime bookingDate, Nullable<int> loggedInClientId, Nullable<int> userTimezoneOffset)
        {
            MySqlParameter clientIDParam = new MySqlParameter("vClientID", MySqlDbType.Int32);
            clientIDParam.Direction = ParameterDirection.Input;
            clientIDParam.Value = clientId;

            MySqlParameter eventIdParam = new MySqlParameter("vEventID", MySqlDbType.Int32);
            eventIdParam.Direction = ParameterDirection.Input;
            eventIdParam.Value = eventId;

            MySqlParameter bookingDateParam = new MySqlParameter("vBookingDate", MySqlDbType.Date);
            bookingDateParam.Direction = ParameterDirection.Input;
            bookingDateParam.Value = bookingDate;

            MySqlParameter loggedInClientIdParam = new MySqlParameter("vLoggedInClientID", MySqlDbType.Int32);
            loggedInClientIdParam.Direction = ParameterDirection.Input;
            loggedInClientIdParam.Value = loggedInClientId;

            MySqlParameter userTimeZnOffSetParam = new MySqlParameter("vUserTimezoneOffset", MySqlDbType.Int32);
            userTimeZnOffSetParam.Direction = ParameterDirection.Input;
            userTimeZnOffSetParam.Value = userTimezoneOffset;

            MySqlParameter[] spParams = new MySqlParameter[] {
                clientIDParam,
                eventIdParam,
                bookingDateParam,
                loggedInClientIdParam,
                userTimeZnOffSetParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL availableclienteventsget(@vClientID, @vEventID, @vBookingDate, @vLoggedInClientID, @vUserTimezoneOffset)");

            string commandText = sb.ToString();

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<AvailableClientEventsGet>(commandText, spParams);
        }

        public virtual ObjectResult<BookedEventGet> GetBookedEvent(Nullable<int> bookedEventId)
        {
            MySqlParameter bookedEventIdParam = new MySqlParameter("vBookedEventID", MySqlDbType.Int32);
            bookedEventIdParam.Direction = ParameterDirection.Input;
            bookedEventIdParam.Value = bookedEventId;

            MySqlParameter[] spParams = new MySqlParameter[] {
                bookedEventIdParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL bookedeventget(@vBookedEventID)");

            string commandText = sb.ToString();

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<BookedEventGet>(commandText, spParams);
        }

        public virtual ObjectResult<BookedEventsGet> GetBookedEvents(Nullable<int> customerId, MySql.Data.Types.MySqlDateTime startDate,
              MySql.Data.Types.MySqlDateTime endDate)
        {
            return GetBookedEvents(customerId, startDate, endDate, 0, 0, 0);
        }

        public virtual ObjectResult<BookedEventsGet> GetBookedEvents(Nullable<int> customerId, MySql.Data.Types.MySqlDateTime startDate,
              MySql.Data.Types.MySqlDateTime endDate, Nullable<int> includeCanceled, Nullable<int> pendingOnly, Nullable<int> excludePending)
        {
            MySqlParameter customerIdParam = new MySqlParameter("vCustomerID", MySqlDbType.Int32);
            customerIdParam.Direction = ParameterDirection.Input;
            customerIdParam.Value = customerId;

            MySqlParameter startDteParam = new MySqlParameter("vStartDate", MySqlDbType.DateTime);
            startDteParam.Direction = ParameterDirection.Input;
            startDteParam.Value = startDate;

            MySqlParameter endDteParam = new MySqlParameter("vEndDate", MySqlDbType.DateTime);
            endDteParam.Direction = ParameterDirection.Input;
            endDteParam.Value = endDate;

            MySqlParameter inclCanceledParam = new MySqlParameter("vIncludeCanceled", MySqlDbType.Int16);
            inclCanceledParam.Direction = ParameterDirection.Input;
            inclCanceledParam.Value = includeCanceled;

            MySqlParameter pendingOnlyParam = new MySqlParameter("vPendingOnly", MySqlDbType.Int16);
            pendingOnlyParam.Direction = ParameterDirection.Input;
            pendingOnlyParam.Value = pendingOnly;

            MySqlParameter excludePendingParam = new MySqlParameter("vExcludePending", MySqlDbType.Int16);
            excludePendingParam.Direction = ParameterDirection.Input;
            excludePendingParam.Value = excludePending;

            MySqlParameter[] spParams = new MySqlParameter[] {
                customerIdParam,
                startDteParam,
                endDteParam,
                inclCanceledParam,
                pendingOnlyParam,
                excludePendingParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL bookedeventsget(@vCustomerID, @vStartDate, @vEndDate, @vIncludeCanceled, @vPendingOnly, @vExcludePending)");

            string commandText = sb.ToString();

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<BookedEventsGet>(commandText, spParams);
        }

        // For Formilae
        public int AddBookingEvent(int eventId, int organizerClientId, MySql.Data.Types.MySqlDateTime eventDate, MySql.Data.Types.MySqlDateTime startTime,
            string firstName, string lastName, string email, string cellPhoneNumber, int timezoneOffset, string sessionKey)
        {
            return AddBookingEvent(eventId, organizerClientId, eventDate, startTime, 0, -1, firstName, lastName, email, cellPhoneNumber, 
                "", 0, timezoneOffset, sessionKey);
        }

        public int AddBookingEvent(int eventId, int organizerClientId, MySql.Data.Types.MySqlDateTime eventDate, MySql.Data.Types.MySqlDateTime startTime,
            int pending, int clientId, string firstName, string lastName, string email, string cellPhoneNumber, string phoneNumber,
            int canceled, int timezoneOffset, string sessionKey)
        {
            firstName = String.IsNullOrEmpty(firstName) ? String.Empty : firstName;
            lastName = String.IsNullOrEmpty(lastName) ? String.Empty : lastName;
            email = String.IsNullOrEmpty(email) ? String.Empty : email;
            cellPhoneNumber = String.IsNullOrEmpty(cellPhoneNumber) ? String.Empty : cellPhoneNumber;
            phoneNumber = String.IsNullOrEmpty(phoneNumber) ? String.Empty : phoneNumber;

            MySqlParameter eventIdParam = new MySqlParameter("vEventID", MySqlDbType.Int32);
            eventIdParam.Direction = ParameterDirection.Input;
            eventIdParam.Value = eventId;

            MySqlParameter organizerClientIdParam = new MySqlParameter("vOrganizerClientID", MySqlDbType.Int32);
            organizerClientIdParam.Direction = ParameterDirection.Input;
            organizerClientIdParam.Value = organizerClientId;

            MySqlParameter eventDteParam = new MySqlParameter("vEventDate", MySqlDbType.DateTime);
            eventDteParam.Direction = ParameterDirection.Input;
            eventDteParam.Value = eventDate;

            MySqlParameter startTimeParam = new MySqlParameter("vStartTime", MySqlDbType.DateTime);
            startTimeParam.Direction = ParameterDirection.Input;
            startTimeParam.Value = startTime;

            MySqlParameter pendingParam = new MySqlParameter("vPending", MySqlDbType.Int16);
            pendingParam.Direction = ParameterDirection.Input;
            pendingParam.Value = pending;

            MySqlParameter clientIdParam = new MySqlParameter("vClientID", MySqlDbType.Int32);
            clientIdParam.Direction = ParameterDirection.Input;
            clientIdParam.Value = clientId;

            MySqlParameter firstNameParam = new MySqlParameter("vFirstName", MySqlDbType.String);
            firstNameParam.Direction = ParameterDirection.Input;
            firstNameParam.Value = firstName;

            MySqlParameter lastNameParam = new MySqlParameter("vLastName", MySqlDbType.String);
            lastNameParam.Direction = ParameterDirection.Input;
            lastNameParam.Value = lastName;

            MySqlParameter emailParam = new MySqlParameter("vEmail", MySqlDbType.String);
            emailParam.Direction = ParameterDirection.Input;
            emailParam.Value = email;

            MySqlParameter cellParam = new MySqlParameter("vCellPhoneNumber", MySqlDbType.String);
            cellParam.Direction = ParameterDirection.Input;
            cellParam.Value = cellPhoneNumber;

            MySqlParameter phoneParam = new MySqlParameter("vPhoneNumber", MySqlDbType.String);
            phoneParam.Direction = ParameterDirection.Input;
            phoneParam.Value = phoneNumber;

            MySqlParameter canceledParam = new MySqlParameter("vCanceled", MySqlDbType.Int16);
            canceledParam.Direction = ParameterDirection.Input;
            canceledParam.Value = canceled;

            MySqlParameter timezneOffsetParam = new MySqlParameter("vTimezoneOffset", MySqlDbType.Int16);
            timezneOffsetParam.Direction = ParameterDirection.Input;
            timezneOffsetParam.Value = timezoneOffset;

            MySqlParameter sessionKyParam = new MySqlParameter("vSessionKey", MySqlDbType.String);
            sessionKyParam.Direction = ParameterDirection.Input;
            sessionKyParam.Value = sessionKey;

            MySqlParameter[] spParams = new MySqlParameter[] {
                eventIdParam,
                organizerClientIdParam,
                eventDteParam,
                startTimeParam,
                pendingParam,
                clientIdParam,
                firstNameParam,
                lastNameParam,
                emailParam,
                cellParam,
                phoneParam,
                canceledParam,
                timezneOffsetParam,
                sessionKyParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL bookedeventadd(@vEventID, @vOrganizerClientID, @vEventDate, @vStartTime, @vPending, @vClientID, " +
                "@vFirstName, @vLastName, @vEmail, @vCellPhoneNumber, @vPhoneNumber, @vCanceled, @vTimezoneOffset, @vSessionKey)");

            string commandText = sb.ToString();
            this.Database.ExecuteSqlCommand(commandText, spParams);

            var result = Convert.ToInt32(ClientHelper.GetSessionInsertOutput());
            return result;
        }

        public virtual ObjectResult<EventGet> GetEvent(Nullable<int> eventId, Nullable<int> customerId)
        {
            return GetEvent(eventId, customerId, 1);
        }

        public virtual ObjectResult<EventGet> GetEvent(Nullable<int> eventId, Nullable<int> customerId, Nullable<int> activeOnly)
        {
            MySqlParameter eventIdParam = new MySqlParameter("vEventID", MySqlDbType.Int32);
            eventIdParam.Direction = ParameterDirection.Input;
            eventIdParam.Value = eventId;

            MySqlParameter customerIDParam = new MySqlParameter("vCustomerID", MySqlDbType.Int32);
            customerIDParam.Direction = ParameterDirection.Input;
            customerIDParam.Value = customerId;

            MySqlParameter activeOnlyParam = new MySqlParameter("vActiveOnly", MySqlDbType.Int16);
            activeOnlyParam.Direction = ParameterDirection.Input;
            activeOnlyParam.Value = activeOnly;

            MySqlParameter[] spParams = new MySqlParameter[] {
                eventIdParam,
                customerIDParam,
                activeOnlyParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL eventget(@vEventID, @vCustomerID, @vActiveOnly)");

            string commandText = sb.ToString();

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<EventGet>(commandText, spParams);
        }

        public virtual ObjectResult<EventOrganizersGet> GetEventOrganizers(Nullable<int> eventId, Nullable<int> activeOnly)
        {
            MySqlParameter eventIdParam = new MySqlParameter("vEventID", MySqlDbType.Int32);
            eventIdParam.Direction = ParameterDirection.Input;
            eventIdParam.Value = eventId;

            MySqlParameter activeOnlyParam = new MySqlParameter("vActiveOnly", MySqlDbType.Int16);
            activeOnlyParam.Direction = ParameterDirection.Input;
            activeOnlyParam.Value = activeOnly;

            MySqlParameter[] spParams = new MySqlParameter[] {
                eventIdParam,
                activeOnlyParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL eventorganizersget(@vEventID, @vActiveOnly)");

            string commandText = sb.ToString();

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<EventOrganizersGet>(commandText, spParams);
        }

        public virtual ObjectResult<CustomerCurrentDateGet> GetCustomerCurrentDate(Nullable<int> customerId)
        {
            MySqlParameter customerIDParam = new MySqlParameter("vCustomerID", MySqlDbType.Int32);
            customerIDParam.Direction = ParameterDirection.Input;
            customerIDParam.Value = customerId;

            MySqlParameter[] spParams = new MySqlParameter[] {
                customerIDParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL customercurrentdateget(@vCustomerID)");

            string commandText = sb.ToString();

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<CustomerCurrentDateGet>(commandText, spParams);
        }

        public virtual ObjectResult<EventsGet> GetEvents(Nullable<int> customerId)
        {
            MySqlParameter customerIDParam = new MySqlParameter("vCustomerID", MySqlDbType.Int32);
            customerIDParam.Direction = ParameterDirection.Input;
            customerIDParam.Value = customerId;

            MySqlParameter[] spParams = new MySqlParameter[] {
                customerIDParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL eventsget(@vCustomerID)");

            string commandText = sb.ToString();

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<EventsGet>(commandText, spParams);
        }

        public void EventDelete(int eventID)
        {
            MySqlParameter eventIDParam = new MySqlParameter("vEventID", MySqlDbType.Int32);
            eventIDParam.Direction = ParameterDirection.Input;
            eventIDParam.Value = eventID;

            MySqlParameter[] spParams = new MySqlParameter[] {
                eventIDParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL DeleteEvent(@vEventID)");

            string commandText = sb.ToString();
            this.Database.ExecuteSqlCommand(commandText, spParams);
        }

        public virtual ObjectResult<EventBlockedDaysForwardGet> EventBlockedDaysForwardGet(Nullable<int> activeOnly)
        {
            MySqlParameter activeOnlyParam = new MySqlParameter("vActiveOnly", MySqlDbType.Int16);
            activeOnlyParam.Direction = ParameterDirection.Input;
            activeOnlyParam.Value = activeOnly;

            MySqlParameter[] spParams = new MySqlParameter[] {
                activeOnlyParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL eventblockeddaysforwardget(@vActiveOnly)");

            string commandText = sb.ToString();

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<EventBlockedDaysForwardGet>(commandText, spParams);
        }

        public void EventAdd(int clientId, string eventName, string description, string location,
            string locationDetails, int duration, decimal price, int addressId,
            string confirmationMsgFooter, string cancellationMsgFooter, string forwardingWebAddress,
            int blockedDaysForward, int active)
        {
            var sessionKey = RandomHelper.GenerateGUID();

            EventAdd(clientId, eventName, description, location, locationDetails, duration,
                price, addressId, confirmationMsgFooter, cancellationMsgFooter, forwardingWebAddress,
                blockedDaysForward, active, sessionKey);
        }

        public void EventAdd(int clientId, string eventName, string description, string location, 
            string locationDetails, int duration, decimal price, int addressId, 
            string confirmationMsgFooter, string cancellationMsgFooter, string forwardingWebAddress, 
            int blockedDaysForward, int active, string sessionKey)
        {

            eventName = String.IsNullOrEmpty(eventName) ? String.Empty : eventName;
            description = String.IsNullOrEmpty(description) ? String.Empty : description;
            location = String.IsNullOrEmpty(location) ? String.Empty : location;
            locationDetails = String.IsNullOrEmpty(locationDetails) ? String.Empty : locationDetails;
            confirmationMsgFooter = String.IsNullOrEmpty(confirmationMsgFooter) ? String.Empty : confirmationMsgFooter;
            cancellationMsgFooter = String.IsNullOrEmpty(cancellationMsgFooter) ? String.Empty : cancellationMsgFooter;
            forwardingWebAddress = String.IsNullOrEmpty(forwardingWebAddress) ? String.Empty : forwardingWebAddress;
            sessionKey = String.IsNullOrEmpty(sessionKey) ? String.Empty : sessionKey;

            MySqlParameter clientIdParam = new MySqlParameter("vClientID", MySqlDbType.Int32);
            clientIdParam.Direction = ParameterDirection.Input;
            clientIdParam.Value = clientId;

            MySqlParameter eventNameParam = new MySqlParameter("vEventName", MySqlDbType.String);
            eventNameParam.Direction = ParameterDirection.Input;
            eventNameParam.Value = eventName;

            MySqlParameter descParam = new MySqlParameter("vDescription", MySqlDbType.String);
            descParam.Direction = ParameterDirection.Input;
            descParam.Value = description;

            MySqlParameter locationParam = new MySqlParameter("vLocation", MySqlDbType.String);
            locationParam.Direction = ParameterDirection.Input;
            locationParam.Value = location;

            MySqlParameter locationDtlsParam = new MySqlParameter("vLocationDetails", MySqlDbType.String);
            locationDtlsParam.Direction = ParameterDirection.Input;
            locationDtlsParam.Value = locationDetails;

            MySqlParameter durationParam = new MySqlParameter("vDuration", MySqlDbType.Int32);
            durationParam.Direction = ParameterDirection.Input;
            durationParam.Value = duration;

            MySqlParameter priceParam = new MySqlParameter("vPrice", MySqlDbType.Decimal);
            priceParam.Direction = ParameterDirection.Input;
            priceParam.Value = price;

            MySqlParameter addressIdParam = new MySqlParameter("vAddressID", MySqlDbType.Int32);
            addressIdParam.Direction = ParameterDirection.Input;
            addressIdParam.Value = addressId;

            MySqlParameter confirmMsgFtrParam = new MySqlParameter("vConfirmationMsgFooter", MySqlDbType.String);
            confirmMsgFtrParam.Direction = ParameterDirection.Input;
            confirmMsgFtrParam.Value = confirmationMsgFooter;

            MySqlParameter cancelMsgFtrParam = new MySqlParameter("vCancellationMsgFooter", MySqlDbType.String);
            cancelMsgFtrParam.Direction = ParameterDirection.Input;
            cancelMsgFtrParam.Value = cancellationMsgFooter;

            MySqlParameter forwardingWebAddrParam = new MySqlParameter("vForwardingWebAddress", MySqlDbType.String);
            forwardingWebAddrParam.Direction = ParameterDirection.Input;
            forwardingWebAddrParam.Value = forwardingWebAddress;

            MySqlParameter blckdDaysForwParam = new MySqlParameter("vBlockedDaysForwardID", MySqlDbType.Int32);
            blckdDaysForwParam.Direction = ParameterDirection.Input;
            blckdDaysForwParam.Value = blockedDaysForward;

            MySqlParameter activeParam = new MySqlParameter("vActive", MySqlDbType.Int16);
            activeParam.Direction = ParameterDirection.Input;
            activeParam.Value = active;

            MySqlParameter sessionKeyParam = new MySqlParameter("vSessionKey", MySqlDbType.String);
            sessionKeyParam.Direction = ParameterDirection.Input;
            sessionKeyParam.Value = sessionKey;

            MySqlParameter[] spParams = new MySqlParameter[] {
                clientIdParam,
                eventNameParam,
                descParam,
                locationParam,
                locationDtlsParam,
                durationParam,
                priceParam,
                addressIdParam,
                confirmMsgFtrParam,
                cancelMsgFtrParam,
                forwardingWebAddrParam,
                blckdDaysForwParam,
                activeParam,
                sessionKeyParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL EventAdd(@vClientID, @vEventName, @vDescription, @vLocation, @vLocationDetails, @vDuration, @vPrice, @vAddressID, @vConfirmationMsgFooter, @vCancellationMsgFooter, @vForwardingWebAddress, @vBlockedDaysForwardID, @vActive, @vSessionKey)");

            string commandText = sb.ToString();
            this.Database.ExecuteSqlCommand(commandText, spParams);
        }

        public void EventUpdate(int eventID, int clientId, string eventName, string description, string location,
            string locationDetails, int duration, decimal price, int addressId,
            string confirmationMsgFooter, string cancellationMsgFooter, string forwardingWebAddress,
            int blockedDaysForward, int active)
        {

            eventName = String.IsNullOrEmpty(eventName) ? String.Empty : eventName;
            description = String.IsNullOrEmpty(description) ? String.Empty : description;
            location = String.IsNullOrEmpty(location) ? String.Empty : location;
            locationDetails = String.IsNullOrEmpty(locationDetails) ? String.Empty : locationDetails;
            confirmationMsgFooter = String.IsNullOrEmpty(confirmationMsgFooter) ? String.Empty : confirmationMsgFooter;
            cancellationMsgFooter = String.IsNullOrEmpty(cancellationMsgFooter) ? String.Empty : cancellationMsgFooter;
            forwardingWebAddress = String.IsNullOrEmpty(forwardingWebAddress) ? String.Empty : forwardingWebAddress;

            MySqlParameter eventIdParam = new MySqlParameter("vEventID", MySqlDbType.Int32);
            eventIdParam.Direction = ParameterDirection.Input;
            eventIdParam.Value = eventID;

            MySqlParameter clientIdParam = new MySqlParameter("vClientID", MySqlDbType.Int32);
            clientIdParam.Direction = ParameterDirection.Input;
            clientIdParam.Value = clientId;

            MySqlParameter eventNameParam = new MySqlParameter("vEventName", MySqlDbType.String);
            eventNameParam.Direction = ParameterDirection.Input;
            eventNameParam.Value = eventName;

            MySqlParameter descParam = new MySqlParameter("vDescription", MySqlDbType.String);
            descParam.Direction = ParameterDirection.Input;
            descParam.Value = description;

            MySqlParameter locationParam = new MySqlParameter("vLocation", MySqlDbType.String);
            locationParam.Direction = ParameterDirection.Input;
            locationParam.Value = location;

            MySqlParameter locationDtlsParam = new MySqlParameter("vLocationDetails", MySqlDbType.String);
            locationDtlsParam.Direction = ParameterDirection.Input;
            locationDtlsParam.Value = locationDetails;

            MySqlParameter durationParam = new MySqlParameter("vDuration", MySqlDbType.Int32);
            durationParam.Direction = ParameterDirection.Input;
            durationParam.Value = duration;

            MySqlParameter priceParam = new MySqlParameter("vPrice", MySqlDbType.Decimal);
            priceParam.Direction = ParameterDirection.Input;
            priceParam.Value = price;

            MySqlParameter addressIdParam = new MySqlParameter("vAddressID", MySqlDbType.Int32);
            addressIdParam.Direction = ParameterDirection.Input;
            addressIdParam.Value = addressId;

            MySqlParameter confirmMsgFtrParam = new MySqlParameter("vConfirmationMsgFooter", MySqlDbType.String);
            confirmMsgFtrParam.Direction = ParameterDirection.Input;
            confirmMsgFtrParam.Value = confirmationMsgFooter;

            MySqlParameter cancelMsgFtrParam = new MySqlParameter("vCancellationMsgFooter", MySqlDbType.String);
            cancelMsgFtrParam.Direction = ParameterDirection.Input;
            cancelMsgFtrParam.Value = cancellationMsgFooter;

            MySqlParameter forwardingWebAddrParam = new MySqlParameter("vForwardingWebAddress", MySqlDbType.String);
            forwardingWebAddrParam.Direction = ParameterDirection.Input;
            forwardingWebAddrParam.Value = forwardingWebAddress;

            MySqlParameter blckdDaysForwParam = new MySqlParameter("vBlockedDaysForwardID", MySqlDbType.Int32);
            blckdDaysForwParam.Direction = ParameterDirection.Input;
            blckdDaysForwParam.Value = blockedDaysForward;

            MySqlParameter activeParam = new MySqlParameter("vActive", MySqlDbType.Int16);
            activeParam.Direction = ParameterDirection.Input;
            activeParam.Value = active;

            MySqlParameter[] spParams = new MySqlParameter[] {
                eventIdParam,
                clientIdParam,
                eventNameParam,
                descParam,
                locationParam,
                locationDtlsParam,
                durationParam,
                priceParam,
                addressIdParam,
                confirmMsgFtrParam,
                cancelMsgFtrParam,
                forwardingWebAddrParam,
                blckdDaysForwParam,
                activeParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL EventUpdate(@vEventID, @vClientID, @vEventName, @vDescription, @vLocation, @vLocationDetails, @vDuration, @vPrice, @vAddressID, @vConfirmationMsgFooter, @vCancellationMsgFooter, @vForwardingWebAddress, @vBlockedDaysForwardID, @vActive)");

            string commandText = sb.ToString();
            this.Database.ExecuteSqlCommand(commandText, spParams);
        }

        public void EventOrganizerDelete(int eventId, int clientId)
        {
            MySqlParameter eventIDParam = new MySqlParameter("vEventID", MySqlDbType.Int32);
            eventIDParam.Direction = ParameterDirection.Input;
            eventIDParam.Value = eventId;

            MySqlParameter clientIdParam = new MySqlParameter("vClientID", MySqlDbType.Int32);
            clientIdParam.Direction = ParameterDirection.Input;
            clientIdParam.Value = clientId;

            MySqlParameter[] spParams = new MySqlParameter[] {
                eventIDParam,
                clientIdParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL eventorganizerdeactivate(@vEventID, @vClientID)");

            string commandText = sb.ToString();
            this.Database.ExecuteSqlCommand(commandText, spParams);
        }

        public void EventOrganizerAdd(int eventId, int clientId, string title)
        {
            title = String.IsNullOrEmpty(title) ? String.Empty : title;

            MySqlParameter eventIDParam = new MySqlParameter("vEventID", MySqlDbType.Int32);
            eventIDParam.Direction = ParameterDirection.Input;
            eventIDParam.Value = eventId;

            MySqlParameter clientIdParam = new MySqlParameter("vClientID", MySqlDbType.Int32);
            clientIdParam.Direction = ParameterDirection.Input;
            clientIdParam.Value = clientId;

            MySqlParameter titleParam = new MySqlParameter("vTitle", MySqlDbType.String);
            titleParam.Direction = ParameterDirection.Input;
            titleParam.Value = title;

            MySqlParameter[] spParams = new MySqlParameter[] {
                eventIDParam,
                clientIdParam,
                titleParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL eventorganizeradd(@vEventID, @vClientID, @vTitle)");

            string commandText = sb.ToString();
            this.Database.ExecuteSqlCommand(commandText, spParams);
        }

        public void EventOrganizerUpdate(int eventId, int clientId, string title)
        {
            title = String.IsNullOrEmpty(title) ? String.Empty : title;

            MySqlParameter eventIDParam = new MySqlParameter("vEventID", MySqlDbType.Int32);
            eventIDParam.Direction = ParameterDirection.Input;
            eventIDParam.Value = eventId;

            MySqlParameter clientIdParam = new MySqlParameter("vClientID", MySqlDbType.Int32);
            clientIdParam.Direction = ParameterDirection.Input;
            clientIdParam.Value = clientId;

            MySqlParameter titleParam = new MySqlParameter("vTitle", MySqlDbType.String);
            titleParam.Direction = ParameterDirection.Input;
            titleParam.Value = title;

            MySqlParameter[] spParams = new MySqlParameter[] {
                eventIDParam,
                clientIdParam,
                titleParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL eventorganizerupdate(@vEventID, @vClientID, @vTitle)");

            string commandText = sb.ToString();
            this.Database.ExecuteSqlCommand(commandText, spParams);
        }

        public virtual ObjectResult<AdminUsersByRoleGet> GetAdminUsersByRole(Nullable<int> customerId, string role)
        {
            MySqlParameter customerIDParam = new MySqlParameter("vCustomerID", MySqlDbType.Int32);
            customerIDParam.Direction = ParameterDirection.Input;
            customerIDParam.Value = customerId;

            MySqlParameter roleParam = new MySqlParameter("vRole", MySqlDbType.String);
            roleParam.Direction = ParameterDirection.Input;
            roleParam.Value = role;

            MySqlParameter[] spParams = new MySqlParameter[] {
                customerIDParam,
                roleParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL admin_usersbyroleget(@vCustomerID, @vRole)");

            string commandText = sb.ToString();
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<AdminUsersByRoleGet>(commandText, spParams);
        }

        public void UpdateForm(int formId, string formName, string formCode, int active, string identifierUnlockMsg,
            string coverletterBulletItemText,
            string approvalInstructions, int supplementPageNo, int supplementPageSectionCount,
            string supplementPageMsg, string supplementPageFieldMsg, int supplementPageTextCharsMax,
            string requireUnitNoPattern, int allowMultipleCopies, int hideSections, int authorizeWithoutLogin,
            string header, string hiddenFormCompleted, int isPropertyAddress,
            int headerPlain, string notApplicableText)
        {
            UpdateForm(formId, 0, formName, formCode, active, identifierUnlockMsg, "", "", coverletterBulletItemText, approvalInstructions, supplementPageNo,
                supplementPageSectionCount, supplementPageMsg, supplementPageFieldMsg, supplementPageTextCharsMax, requireUnitNoPattern, allowMultipleCopies,
                hideSections, authorizeWithoutLogin, header, "", hiddenFormCompleted, isPropertyAddress, headerPlain, "", 0, notApplicableText);
        }

        public void UpdateForm(int formId, int formBuilderUpdate, string formName, string formCode, int active, string identifierUnlockMsg,
        string sourceFileName, string destinationFileName, string coverletterBulletItemText,
        string approvalInstructions, int supplementPageNo, int supplementPageSectionCount,
        string supplementPageMsg, string supplementPageFieldMsg, int supplementPageTextCharsMax,
        string requireUnitNoPattern, int allowMultipleCopies, int hideSections, int authorizeWithoutLogin,
        string header, string pdfFontSize, string hiddenFormCompleted, int isPropertyAddress,
        int headerPlain, string formBuilderJson, int generatesPDF, string notApplicableText)
        {
            formName = String.IsNullOrEmpty(formName) ? String.Empty : formName;
            formCode = String.IsNullOrEmpty(formCode) ? String.Empty : formCode;
            identifierUnlockMsg = String.IsNullOrEmpty(identifierUnlockMsg) ? String.Empty : identifierUnlockMsg;
            sourceFileName = String.IsNullOrEmpty(sourceFileName) ? String.Empty : sourceFileName;
            destinationFileName = String.IsNullOrEmpty(destinationFileName) ? String.Empty : destinationFileName;
            coverletterBulletItemText = String.IsNullOrEmpty(coverletterBulletItemText) ? String.Empty : coverletterBulletItemText;
            approvalInstructions = String.IsNullOrEmpty(approvalInstructions) ? String.Empty : approvalInstructions;
            supplementPageMsg = String.IsNullOrEmpty(supplementPageMsg) ? String.Empty : supplementPageMsg;
            supplementPageFieldMsg = String.IsNullOrEmpty(supplementPageFieldMsg) ? String.Empty : supplementPageFieldMsg;
            requireUnitNoPattern = String.IsNullOrEmpty(requireUnitNoPattern) ? String.Empty : requireUnitNoPattern;
            header = String.IsNullOrEmpty(header) ? String.Empty : header;
            pdfFontSize = String.IsNullOrEmpty(pdfFontSize) ? String.Empty : pdfFontSize;
            hiddenFormCompleted = String.IsNullOrEmpty(hiddenFormCompleted) ? String.Empty : hiddenFormCompleted;
            formBuilderJson = String.IsNullOrEmpty(formBuilderJson) ? String.Empty : formBuilderJson;
            notApplicableText = String.IsNullOrEmpty(notApplicableText) ? String.Empty : notApplicableText;

            MySqlParameter formIdParam = new MySqlParameter("pFormID", MySqlDbType.Int32);
            formIdParam.Direction = ParameterDirection.Input;
            formIdParam.Value = formId;

            MySqlParameter formBldrUpdateParam = new MySqlParameter("pFormBuilderUpdate", MySqlDbType.Int16);
            formBldrUpdateParam.Direction = ParameterDirection.Input;
            formBldrUpdateParam.Value = formBuilderUpdate;

            MySqlParameter formNameParam = new MySqlParameter("pFormName", MySqlDbType.String);
            formNameParam.Direction = ParameterDirection.Input;
            formNameParam.Value = formName;

            MySqlParameter formCodeParam = new MySqlParameter("pFormCode", MySqlDbType.String);
            formCodeParam.Direction = ParameterDirection.Input;
            formCodeParam.Value = formCode;

            MySqlParameter activeParam = new MySqlParameter("pActive", MySqlDbType.Int32);
            activeParam.Direction = ParameterDirection.Input;
            activeParam.Value = active;

            MySqlParameter identifierUnlockMsgParam = new MySqlParameter("pIdentifierUnlockMsg", MySqlDbType.String);
            identifierUnlockMsgParam.Direction = ParameterDirection.Input;
            identifierUnlockMsgParam.Value = identifierUnlockMsg;

            MySqlParameter sourceFileNameParam = new MySqlParameter("pSourceFileName", MySqlDbType.String);
            sourceFileNameParam.Direction = ParameterDirection.Input;
            sourceFileNameParam.Value = sourceFileName;

            MySqlParameter destinationFileNameParam = new MySqlParameter("pDestinationFileName", MySqlDbType.String);
            destinationFileNameParam.Direction = ParameterDirection.Input;
            destinationFileNameParam.Value = destinationFileName;

            MySqlParameter coverletterBulletItmTxtParam = new MySqlParameter("pCoverletterBulletItemText", MySqlDbType.String);
            coverletterBulletItmTxtParam.Direction = ParameterDirection.Input;
            coverletterBulletItmTxtParam.Value = coverletterBulletItemText;

            MySqlParameter apprvlInstructionsParam = new MySqlParameter("pApprovalInstructions", MySqlDbType.String);
            apprvlInstructionsParam.Direction = ParameterDirection.Input;
            apprvlInstructionsParam.Value = approvalInstructions;

            MySqlParameter supplPageNoParam = new MySqlParameter("pSupplementPageNo", MySqlDbType.Int32);
            supplPageNoParam.Direction = ParameterDirection.Input;
            supplPageNoParam.Value = supplementPageNo;

            MySqlParameter supplementPageSectionCountParam = new MySqlParameter("pSupplementPageSectionCount", MySqlDbType.Int32);
            supplementPageSectionCountParam.Direction = ParameterDirection.Input;
            supplementPageSectionCountParam.Value = supplementPageSectionCount;

            MySqlParameter supplPageMsgParam = new MySqlParameter("pSupplementPageMsg", MySqlDbType.String);
            supplPageMsgParam.Direction = ParameterDirection.Input;
            supplPageMsgParam.Value = supplementPageMsg;

            MySqlParameter supplementPageFieldMsgParam = new MySqlParameter("pSupplementPageFieldMsg", MySqlDbType.String);
            supplementPageFieldMsgParam.Direction = ParameterDirection.Input;
            supplementPageFieldMsgParam.Value = supplementPageFieldMsg;

            MySqlParameter supplPageTextCharsMaxParam = new MySqlParameter("pSupplementPageTextCharsMax", MySqlDbType.Int32);
            supplPageTextCharsMaxParam.Direction = ParameterDirection.Input;
            supplPageTextCharsMaxParam.Value = supplementPageTextCharsMax;

            MySqlParameter requireUnitNoPatternParam = new MySqlParameter("pRequireUnitNoPattern", MySqlDbType.String);
            requireUnitNoPatternParam.Direction = ParameterDirection.Input;
            requireUnitNoPatternParam.Value = requireUnitNoPattern;

            MySqlParameter allowMultplCopiesParam = new MySqlParameter("pAllowMultipleCopies", MySqlDbType.Int16);
            allowMultplCopiesParam.Direction = ParameterDirection.Input;
            allowMultplCopiesParam.Value = allowMultipleCopies;

            MySqlParameter hideSectionsParam = new MySqlParameter("pHideSections", MySqlDbType.Int16);
            hideSectionsParam.Direction = ParameterDirection.Input;
            hideSectionsParam.Value = hideSections;

            MySqlParameter authorizeWithoutLoginParam = new MySqlParameter("pAuthorizeWithoutLogin", MySqlDbType.Int16);
            authorizeWithoutLoginParam.Direction = ParameterDirection.Input;
            authorizeWithoutLoginParam.Value = authorizeWithoutLogin;

            MySqlParameter headerParam = new MySqlParameter("pHeader", MySqlDbType.String);
            headerParam.Direction = ParameterDirection.Input;
            headerParam.Value = header;

            MySqlParameter pdfFontSizeParam = new MySqlParameter("pPDFFontSize", MySqlDbType.String);
            pdfFontSizeParam.Direction = ParameterDirection.Input;
            pdfFontSizeParam.Value = pdfFontSize;

            MySqlParameter hiddenFormCompletedParam = new MySqlParameter("pHiddenFormCompleted", MySqlDbType.String);
            hiddenFormCompletedParam.Direction = ParameterDirection.Input;
            hiddenFormCompletedParam.Value = hiddenFormCompleted;

            MySqlParameter isPropertyAddressParam = new MySqlParameter("pIsPropertyAddress", MySqlDbType.Int16);
            isPropertyAddressParam.Direction = ParameterDirection.Input;
            isPropertyAddressParam.Value = isPropertyAddress;

            MySqlParameter headerPlainParam = new MySqlParameter("pHeaderPlain", MySqlDbType.Int16);
            headerPlainParam.Direction = ParameterDirection.Input;
            headerPlainParam.Value = headerPlain;

            MySqlParameter formBuilderJsonParam = new MySqlParameter("pFormBuilderJson", MySqlDbType.String);
            formBuilderJsonParam.Direction = ParameterDirection.Input;
            formBuilderJsonParam.Value = formBuilderJson;

            MySqlParameter genPDFParam = new MySqlParameter("pGeneratesPDF", MySqlDbType.Int16);
            genPDFParam.Direction = ParameterDirection.Input;
            genPDFParam.Value = generatesPDF;

            MySqlParameter naTextParam = new MySqlParameter("pNotApplicableText", MySqlDbType.String);
            naTextParam.Direction = ParameterDirection.Input;
            naTextParam.Value = notApplicableText;

            MySqlParameter[] spParams = new MySqlParameter[] {
                formIdParam,
                formBldrUpdateParam,
                formNameParam,
                formCodeParam,
                activeParam,
                identifierUnlockMsgParam,
                sourceFileNameParam,
                destinationFileNameParam,
                coverletterBulletItmTxtParam,
                apprvlInstructionsParam,
                supplPageNoParam,
                supplementPageSectionCountParam,
                supplPageMsgParam,
                supplementPageFieldMsgParam,
                supplPageTextCharsMaxParam,
                requireUnitNoPatternParam,
                allowMultplCopiesParam,
                hideSectionsParam,
                authorizeWithoutLoginParam,
                headerParam,
                pdfFontSizeParam,
                hiddenFormCompletedParam,
                isPropertyAddressParam,
                headerPlainParam,
                formBuilderJsonParam,
                genPDFParam,
                naTextParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL admin_formupdate(@pFormId, @pFormBuilderUpdate, @pFormName, @pFormCode, @pActive, @pIdentifierUnlockMsg, @pSourceFileName, @pDestinationFileName, @pCoverletterBulletItemText, @pApprovalInstructions, @pSupplementPageNo, @pSupplementPageSectionCount, @pSupplementPageMsg, @pSupplementPageFieldMsg, @pSupplementPageTextCharsMax, @pRequireUnitNoPattern, @pAllowMultipleCopies, @pHideSections, @pAuthorizeWithoutLogin, @pHeader, @pPDFFontSize, @pHiddenFormCompleted, @pIsPropertyAddress, @pHeaderPlain, @pFormBuilderJson, @pGeneratesPDF, @pNotApplicableText)");

            string commandText = sb.ToString();
            this.Database.ExecuteSqlCommand(commandText, spParams);
        }

        public int AddForm(int customerId, string formName, string formCode, string identifierUnlockMsg,
            string coverletterBulletItemText,
            string approvalInstructions, int supplementPageNo, int supplementPageSectionCount,
            string supplementPageMsg, string supplementPageFieldMsg, int supplementPageTextCharsMax,
            string requireUnitNoPattern, int allowMultipleCopies, int hideSections, int authorizeWithoutLogin,
            string header, string hiddenFormCompleted, int isPropertyAddress,
            int headerPlain, string notApplicableText)
        {
            var sessionKey = RandomHelper.GenerateGUID();
            return AddForm(customerId, formName, formCode, identifierUnlockMsg, "", "", coverletterBulletItemText,
            approvalInstructions, supplementPageNo, supplementPageSectionCount, supplementPageMsg, supplementPageFieldMsg, supplementPageTextCharsMax,
            requireUnitNoPattern, allowMultipleCopies, hideSections, authorizeWithoutLogin, header, "", hiddenFormCompleted, isPropertyAddress,
            headerPlain, "", 0, notApplicableText, sessionKey);
        }

        public int AddForm(int customerId, string formName, string formCode, string identifierUnlockMsg,
            string sourceFileName, string destinationFileName, string coverletterBulletItemText,
            string approvalInstructions, int supplementPageNo, int supplementPageSectionCount,
            string supplementPageMsg, string supplementPageFieldMsg, int supplementPageTextCharsMax,
            string requireUnitNoPattern, int allowMultipleCopies, int hideSections, int authorizeWithoutLogin,
            string header, string pdfFontSize, string hiddenFormCompleted, int isPropertyAddress,
            int headerPlain, string formBuilderJson, int generatesPDF, string notApplicableText)
        {
            var sessionKey = RandomHelper.GenerateGUID();
            return AddForm(customerId, formName, formCode, identifierUnlockMsg, sourceFileName, destinationFileName, coverletterBulletItemText,
            approvalInstructions, supplementPageNo, supplementPageSectionCount, supplementPageMsg, supplementPageFieldMsg, supplementPageTextCharsMax,
            requireUnitNoPattern, allowMultipleCopies, hideSections, authorizeWithoutLogin, header, pdfFontSize, hiddenFormCompleted, isPropertyAddress,
            headerPlain, formBuilderJson, generatesPDF, notApplicableText, sessionKey);
        }

        public int AddForm(int customerId, string formName, string formCode, string identifierUnlockMsg,
        string sourceFileName, string destinationFileName, string coverletterBulletItemText,
        string approvalInstructions, int supplementPageNo, int supplementPageSectionCount,
        string supplementPageMsg, string supplementPageFieldMsg, int supplementPageTextCharsMax,
        string requireUnitNoPattern, int allowMultipleCopies, int hideSections, int authorizeWithoutLogin,
        string header, string pdfFontSize, string hiddenFormCompleted, int isPropertyAddress,
        int headerPlain, string formBuilderJson, int generatesPDF, string notApplicableText, string sessionKey)
        {
            formName = String.IsNullOrEmpty(formName) ? String.Empty : formName;
            formCode = String.IsNullOrEmpty(formCode) ? String.Empty : formCode;
            identifierUnlockMsg = String.IsNullOrEmpty(identifierUnlockMsg) ? String.Empty : identifierUnlockMsg;
            sourceFileName = String.IsNullOrEmpty(sourceFileName) ? String.Empty : sourceFileName;
            destinationFileName = String.IsNullOrEmpty(destinationFileName) ? String.Empty : destinationFileName;
            coverletterBulletItemText = String.IsNullOrEmpty(coverletterBulletItemText) ? String.Empty : coverletterBulletItemText;
            approvalInstructions = String.IsNullOrEmpty(approvalInstructions) ? String.Empty : approvalInstructions;
            supplementPageMsg = String.IsNullOrEmpty(supplementPageMsg) ? String.Empty : supplementPageMsg;
            supplementPageFieldMsg = String.IsNullOrEmpty(supplementPageFieldMsg) ? String.Empty : supplementPageFieldMsg;
            requireUnitNoPattern = String.IsNullOrEmpty(requireUnitNoPattern) ? String.Empty : requireUnitNoPattern;
            header = String.IsNullOrEmpty(header) ? String.Empty : header;
            pdfFontSize = String.IsNullOrEmpty(pdfFontSize) ? String.Empty : pdfFontSize;
            hiddenFormCompleted = String.IsNullOrEmpty(hiddenFormCompleted) ? String.Empty : hiddenFormCompleted;
            formBuilderJson = String.IsNullOrEmpty(formBuilderJson) ? String.Empty : formBuilderJson;
            notApplicableText = String.IsNullOrEmpty(notApplicableText) ? String.Empty : notApplicableText;
            sessionKey = String.IsNullOrEmpty(sessionKey) ? String.Empty : sessionKey;

            MySqlParameter customerIdParam = new MySqlParameter("pCustomerID", MySqlDbType.Int32);
            customerIdParam.Direction = ParameterDirection.Input;
            customerIdParam.Value = customerId;

            MySqlParameter formNameParam = new MySqlParameter("pFormName", MySqlDbType.String);
            formNameParam.Direction = ParameterDirection.Input;
            formNameParam.Value = formName;

            MySqlParameter formCodeParam = new MySqlParameter("pFormCode", MySqlDbType.String);
            formCodeParam.Direction = ParameterDirection.Input;
            formCodeParam.Value = formCode;

            MySqlParameter identifierUnlockMsgParam = new MySqlParameter("pIdentifierUnlockMsg", MySqlDbType.String);
            identifierUnlockMsgParam.Direction = ParameterDirection.Input;
            identifierUnlockMsgParam.Value = identifierUnlockMsg;

            MySqlParameter sourceFileNameParam = new MySqlParameter("pSourceFileName", MySqlDbType.String);
            sourceFileNameParam.Direction = ParameterDirection.Input;
            sourceFileNameParam.Value = sourceFileName;

            MySqlParameter destinationFileNameParam = new MySqlParameter("pDestinationFileName", MySqlDbType.String);
            destinationFileNameParam.Direction = ParameterDirection.Input;
            destinationFileNameParam.Value = destinationFileName;

            MySqlParameter coverletterBulletItmTxtParam = new MySqlParameter("pCoverletterBulletItemText", MySqlDbType.String);
            coverletterBulletItmTxtParam.Direction = ParameterDirection.Input;
            coverletterBulletItmTxtParam.Value = coverletterBulletItemText;

            MySqlParameter apprvlInstructionsParam = new MySqlParameter("pApprovalInstructions", MySqlDbType.String);
            apprvlInstructionsParam.Direction = ParameterDirection.Input;
            apprvlInstructionsParam.Value = approvalInstructions;

            MySqlParameter supplPageNoParam = new MySqlParameter("pSupplementPageNo", MySqlDbType.Int32);
            supplPageNoParam.Direction = ParameterDirection.Input;
            supplPageNoParam.Value = supplementPageNo;

            MySqlParameter supplementPageSectionCountParam = new MySqlParameter("pSupplementPageSectionCount", MySqlDbType.Int32);
            supplementPageSectionCountParam.Direction = ParameterDirection.Input;
            supplementPageSectionCountParam.Value = supplementPageSectionCount;

            MySqlParameter supplPageMsgParam = new MySqlParameter("pSupplementPageMsg", MySqlDbType.String);
            supplPageMsgParam.Direction = ParameterDirection.Input;
            supplPageMsgParam.Value = supplementPageMsg;

            MySqlParameter supplementPageFieldMsgParam = new MySqlParameter("pSupplementPageFieldMsg", MySqlDbType.String);
            supplementPageFieldMsgParam.Direction = ParameterDirection.Input;
            supplementPageFieldMsgParam.Value = supplementPageFieldMsg;

            MySqlParameter supplPageTextCharsMaxParam = new MySqlParameter("pSupplementPageTextCharsMax", MySqlDbType.Int32);
            supplPageTextCharsMaxParam.Direction = ParameterDirection.Input;
            supplPageTextCharsMaxParam.Value = supplementPageTextCharsMax;

            MySqlParameter requireUnitNoPatternParam = new MySqlParameter("pRequireUnitNoPattern", MySqlDbType.String);
            requireUnitNoPatternParam.Direction = ParameterDirection.Input;
            requireUnitNoPatternParam.Value = requireUnitNoPattern;

            MySqlParameter allowMultplCopiesParam = new MySqlParameter("pAllowMultipleCopies", MySqlDbType.Int16);
            allowMultplCopiesParam.Direction = ParameterDirection.Input;
            allowMultplCopiesParam.Value = allowMultipleCopies;

            MySqlParameter hideSectionsParam = new MySqlParameter("pHideSections", MySqlDbType.Int16);
            hideSectionsParam.Direction = ParameterDirection.Input;
            hideSectionsParam.Value = hideSections;

            MySqlParameter authorizeWithoutLoginParam = new MySqlParameter("pAuthorizeWithoutLogin", MySqlDbType.Int16);
            authorizeWithoutLoginParam.Direction = ParameterDirection.Input;
            authorizeWithoutLoginParam.Value = authorizeWithoutLogin;

            MySqlParameter headerParam = new MySqlParameter("pHeader", MySqlDbType.String);
            headerParam.Direction = ParameterDirection.Input;
            headerParam.Value = header;

            MySqlParameter pdfFontSizeParam = new MySqlParameter("pPDFFontSize", MySqlDbType.String);
            pdfFontSizeParam.Direction = ParameterDirection.Input;
            pdfFontSizeParam.Value = pdfFontSize;

            MySqlParameter hiddenFormCompletedParam = new MySqlParameter("pHiddenFormCompleted", MySqlDbType.String);
            hiddenFormCompletedParam.Direction = ParameterDirection.Input;
            hiddenFormCompletedParam.Value = hiddenFormCompleted;

            MySqlParameter isPropertyAddressParam = new MySqlParameter("pIsPropertyAddress", MySqlDbType.Int16);
            isPropertyAddressParam.Direction = ParameterDirection.Input;
            isPropertyAddressParam.Value = isPropertyAddress;

            MySqlParameter headerPlainParam = new MySqlParameter("pHeaderPlain", MySqlDbType.Int16);
            headerPlainParam.Direction = ParameterDirection.Input;
            headerPlainParam.Value = headerPlain;

            MySqlParameter formBuilderJsonParam = new MySqlParameter("pFormBuilderJson", MySqlDbType.String);
            formBuilderJsonParam.Direction = ParameterDirection.Input;
            formBuilderJsonParam.Value = formBuilderJson;

            MySqlParameter genPDFParam = new MySqlParameter("pGeneratesPDF", MySqlDbType.Int16);
            genPDFParam.Direction = ParameterDirection.Input;
            genPDFParam.Value = generatesPDF;

            MySqlParameter naTextParam = new MySqlParameter("pNotApplicableText", MySqlDbType.String);
            naTextParam.Direction = ParameterDirection.Input;
            naTextParam.Value = notApplicableText;

            MySqlParameter sessionKyParam = new MySqlParameter("pSessionKey", MySqlDbType.String);
            sessionKyParam.Direction = ParameterDirection.Input;
            sessionKyParam.Value = sessionKey;

            MySqlParameter[] spParams = new MySqlParameter[] {
                customerIdParam,
                formNameParam,
                formCodeParam,
                identifierUnlockMsgParam,
                sourceFileNameParam,
                destinationFileNameParam,
                coverletterBulletItmTxtParam,
                apprvlInstructionsParam,
                supplPageNoParam,
                supplementPageSectionCountParam,
                supplPageMsgParam,
                supplementPageFieldMsgParam,
                supplPageTextCharsMaxParam,
                requireUnitNoPatternParam,
                allowMultplCopiesParam,
                hideSectionsParam,
                authorizeWithoutLoginParam,
                headerParam,
                pdfFontSizeParam,
                hiddenFormCompletedParam,
                isPropertyAddressParam,
                headerPlainParam,
                formBuilderJsonParam,
                genPDFParam,
                naTextParam,
                sessionKyParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL admin_formadd(@pCustomerId, @pFormName, @pFormCode, @pIdentifierUnlockMsg, @pSourceFileName, @pDestinationFileName, @pCoverletterBulletItemText, @pApprovalInstructions, @pSupplementPageNo, @pSupplementPageSectionCount, @pSupplementPageMsg, @pSupplementPageFieldMsg, @pSupplementPageTextCharsMax, @pRequireUnitNoPattern, @pAllowMultipleCopies, @pHideSections, @pAuthorizeWithoutLogin, @pHeader, @pPDFFontSize, @pHiddenFormCompleted, @pIsPropertyAddress, @pHeaderPlain, @pFormBuilderJson, @pGeneratesPDF, @pNotApplicableText, @pSessionKey)");

            string commandText = sb.ToString();
            this.Database.ExecuteSqlCommand(commandText, spParams);

            var result = Convert.ToInt32(ClientHelper.GetSessionInsertOutput(sessionKey));
            return result;
        }

        public void FormDelete(int formId)
        {
            MySqlParameter formIdParam = new MySqlParameter("vFormID", MySqlDbType.Int32);
            formIdParam.Direction = ParameterDirection.Input;
            formIdParam.Value = formId;

            MySqlParameter[] spParams = new MySqlParameter[] {
                formIdParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL deleteform(@vFormID)");

            string commandText = sb.ToString();
            this.Database.ExecuteSqlCommand(commandText, spParams);
        }

        public virtual ObjectResult<AdminFormsGet> GetForms(Nullable<int> clientId)
        {
            MySqlParameter userIDParam = new MySqlParameter("vClientID", MySqlDbType.Int32);
            userIDParam.Direction = ParameterDirection.Input;
            userIDParam.Value = clientId;

            MySqlParameter[] spParams = new MySqlParameter[] {
                userIDParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL admin_formsget(@vClientID)");

            string commandText = sb.ToString();

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<AdminFormsGet>(commandText, spParams);
        }

        public virtual ObjectResult<AdminFormGet> GetForm(Nullable<int> formId)
        {
            MySqlParameter formIdParam = new MySqlParameter("vFormID", MySqlDbType.Int32);
            formIdParam.Direction = ParameterDirection.Input;
            formIdParam.Value = formId;

            MySqlParameter[] spParams = new MySqlParameter[] {
                formIdParam
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("CALL admin_formget(@vFormID)");

            string commandText = sb.ToString();

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<AdminFormGet>(commandText, spParams);
        }
    }
}