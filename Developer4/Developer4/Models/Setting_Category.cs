using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SharedAssemblies.Models
{
    public class Setting_Category
    {
        public int ID { get; set; }
        public int SettingID { get; set; }
        public int CategoryID { get; set; }
        public string LastUpdateDate { get; set; }
    }
}