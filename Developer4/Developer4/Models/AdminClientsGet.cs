using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SharedAssemblies.Models
{
    public class AdminClientsGet
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int Active { get; set; }
        public string LastLoginDate { get; set; }
        public string LastLoginIP { get; set; }
        public string Avatar { get; set; }
        public int Admin { get; set; }
        public string HomePhone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Occupation { get; set; }
        public string EmergencyContact { get; set; }
        public string DOB { get; set; }
        public string PreferredMethodOfComm { get; set; }
    }
}