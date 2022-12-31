using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedAssemblies.Models
{
    public class AdminRolePermissionsGet
    {
        public int ID { get; set; }
        public int? RoleID { get; set; }
        public int PermissionID { get; set; }
        public string PermissionName { get; set; }
        public string Description { get; set; }
        public int Checked { get; set; }
        public int? CustomerID { get; set; }
    }
}
