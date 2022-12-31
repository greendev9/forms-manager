using SharedAssemblies.Models;
using System.Collections.Generic;

namespace Admin.ViewModels
{
    public class RoleManagerViewModel
    {
        public List<AdminRoleUsersGet> AdminRoleUsersGet { get; set; }
        public List<AdminRolePermissionsGet> AdminRolePermissionsGet { get; set; }
    }
}