using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SharedAssemblies.Models
{
    public class CustomerSettingsGet
    {
        public int ID { get; set; }
        public string SettingName { get; set; }
        public string Path { get; set; }
        public string SettingValue { get; set; }
        public string SettingValueRaw { get; set; }  // The customer setting (raw value)
        public string DataType { get; set; }
        public int Required { get; set; }
        public string DefaultValue { get; set; }
    }
}