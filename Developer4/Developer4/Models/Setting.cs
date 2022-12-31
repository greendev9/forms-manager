using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SharedAssemblies.Models
{
    public class Setting
    {
        public int ID { get; set; }
        public string SettingName { get; set; }
        public string DataType { get; set; }
        public string DefaultValue { get; set; }
        public string Description { get; set; }
        public int Required { get; set; }
        public string Path { get; set; }
        public string LastUpdateDate { get; set; }
        public int Active { get; set; }
    }
}