using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SharedAssemblies.Models
{
    public class AvailableClientEventsGet
    {
        public int ID { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string StartDateTime { get; set; }
        public string EndDateTime { get; set; }
        public string StartTimeDisplay { get; set; }
        public string EndTimeDisplay { get; set; }
        public string StartDateTimeDisplay { get; set; }
        public string EndDateTimeDisplay { get; set; }
        public int DateDifference { get; set; }  // Means that due to the timezone difference, the date falls on a different day then the customer booking date
    }
}