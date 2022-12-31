using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SharedAssemblies.Models
{
    public class Client
    {
        public int ID { get; set; }
        public int CustomerID { get; set; }

        [Required]
        [StringLength(25, ErrorMessage = "Password can not be longer than 25 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "First Name can not be longer than 50 characters.")]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Last Name can not be longer than 50 characters.")]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [DisplayName("Login E-mail")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Login Email is not a valid address.")]
        [StringLength(75, ErrorMessage = "Login Email can not be longer than 75 characters.")]
        public string Email { get; set; }

        public int Active { get; set; }
        public string LastLoginDate { get; set; }
        public string LastLoginIP { get; set; }
        public string CreatedDate { get; set; }

        [DisplayName("Login E-mail")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Login Email is not a valid address.")]
        [StringLength(75, ErrorMessage = "Login Email can not be longer than 75 characters.")]
        public string NewEmail { get; set; }

        public string NewEmailPassword { get; set; }
        public string ApprovedPassword { get; set; }
        public int Denied { get; set; }
        public int Admin { get; set; }

        public int? InitialInquiryProcessID { get; set; }
        public string PhoneNumber { get; set; }

        public int AcceptsCookies { get; set; }
        public string AdminAuthCode { get; set; }
        public string Avatar { get; set; }
    }
}