using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SharedAssemblies.ViewModels
{
    public class EventViewModel
    {
        public string id { get; set; }
        public string title { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string duration { get; set; }
        public string location { get; set; }
        public string description { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string organizer { get; set; }
        public string date { get; set; }
        public string eventName { get; set; }
    }
}