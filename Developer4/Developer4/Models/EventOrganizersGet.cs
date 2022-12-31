using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SharedAssemblies.Models
{
    public class EventOrganizersGet
    {
        public int ID { get; set; }
        public int EventID { get; set; }
        public int ClientID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Active { get; set; }
        public string Title { get; set; }
        public string FullName { get; set; }
    }
}