using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SharedAssemblies.Models
{
    public class EventGet
    {
        public int ID { get; set; }
        [Required]
        [StringLength(255)]
        [DisplayName("Event Name")]
        public string EventName { get; set; }
        [Required]
        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [Required]
        [StringLength(100)]
        public string Location { get; set; }
        [StringLength(1000)]
        [DataType(DataType.MultilineText)]
        [DisplayName("Location Details")]
        public string LocationDetails { get; set; }
        [Required]
        public int Duration { get; set; }
        [Required]
        public decimal Price { get; set; }
        public int UTCTimezoneOffset { get; set; }
        [StringLength(500)]
        [RegularExpression(@"^https?:\/\/(.*)", ErrorMessage = "Please begins with http:// or https://")]
        [DisplayName("Forwarding Web Address")]
        public string ForwardingWebAddress { get; set; }
        [StringLength(1000)]
        [DataType(DataType.MultilineText)]
        [DisplayName("Confirmation Message Footer")]
        public string ConfirmationMsgFooter { get; set; }
        [StringLength(1000)]
        [DataType(DataType.MultilineText)]
        [DisplayName("Cancellation Message Footer")]
        public string CancellationMsgFooter { get; set; }
        [DisplayName("Blocked Days Forward")]
        public int BlockedDaysForwardID { get; set; }
        public int Active { get; set; }
    }
}