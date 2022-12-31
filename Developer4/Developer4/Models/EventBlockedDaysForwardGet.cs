using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SharedAssemblies.Models
{
    public class EventBlockedDaysForwardGet
    {
        public int ID { get; set; }
        public int Days { get; set; }
        public string Label { get; set; }
        public int Active { get; set; }
    }
}