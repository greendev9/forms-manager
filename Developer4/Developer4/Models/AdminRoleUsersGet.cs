using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedAssemblies.Models
{
    public class AdminRoleUsersGet
    {
        public int ID { get; set; }
        public string FullName { get; set; }
        public string Avatar { get; set; }
        public int? Active { get; set; }
        public string Email { get; set; }
        public int? RoleID { get; set; }
        public int Checked { get; set; }
    }
}
