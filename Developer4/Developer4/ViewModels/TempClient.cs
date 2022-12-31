using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Admin.ViewModels
{
    public class TempClient
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string IsAdministrator { get; set; }
        public string ErrMsg { get; set; }
    }
}