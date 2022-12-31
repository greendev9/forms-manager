using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SharedAssemblies.Models
{
    public class UserPermissionsGet
    {
        public int ID { get; set; }
        public string PermissionName { get; set; }
        public string RoleName { get; set; }
        public string LastUpdateDate { get; set; }
        public string Description { get; set; }
    }
}