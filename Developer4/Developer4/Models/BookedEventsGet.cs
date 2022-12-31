using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SharedAssemblies.Models
{
    public class BookedEventsGet
    {
        public int ID { get; set; }
        public int EventID { get; set; }
        public string EventDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string StartDateTime { get; set; }
        public string EndDateTime { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Cell { get; set; }
        public string Phone { get; set; }
        public int Pending { get; set; }
        public int Canceled { get; set; }
        public int TimezoneOffSet { get; set; }
        public string EventName { get; set; }
        public string Description { get; set; }
        public string OrganizerFirstName { get; set; }
        public string OrganizerLastName { get; set; }
        public string OrganizerClientID { get; set; }
        public int Duration { get; set; }

    }
}