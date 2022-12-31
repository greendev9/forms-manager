using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SharedAssemblies.Models
{
    public class Form
    {
        public int ID { get; set; }
        public int? CustomerID { get; set; }
        public string FormName { get; set; }
        public string FormCode { get; set; }
        public string DateActive { get; set; }
        public string DateInactive { get; set; }
    }
}