using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SharedAssemblies.Models
{
    public class SupportTicketsGet
    {
        public int ID { get; set; }
        public string AvatarFile { get; set; }
        public string Subject { get; set; }
        public string Status { get; set; }
        public int Closed { get; set; }
        public int Priority { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}