using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SharedAssemblies.Models
{
    public class EventsGet
    {
        public int ID { get; set; }
        public string EventName { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public int Duration { get; set; }
        public int Active { get; set; }
        public int OrganizerCount { get; set; }
    }
}