using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SharedAssemblies.Models
{
    public class Customer_Setting
    {
        public int ID { get; set; }
        public int CustomerID { get; set; }
        public int SettingID { get; set; }
        public string SettingValue { get; set; }
        public string LastUpdateDate { get; set; }
    }
}