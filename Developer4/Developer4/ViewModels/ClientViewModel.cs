using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Admin.ViewModels
{
    public class ClientViewModel
    {
        public int ID { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "First Name can not be longer than 50 characters.")]
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Last Name can not be longer than 50 characters.")]
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        [StringLength(20)]
        [DisplayName("Cell Phone Number")]
        public string PhoneNumber { get; set; }
        [StringLength(20)]
        [DisplayName("Home Phone Number")]
        public string HomePhone { get; set; }
        [Required]
        [DisplayName("Login E-mail")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Login Email is not a valid address.")]
        [StringLength(75, ErrorMessage = "Login Email can not be longer than 75 characters.")]
        public string Email { get; set; }
        [StringLength(255)]
        [DataType(DataType.MultilineText)]
        [DisplayName("Emergency Contact")]
        public string EmergencyContact { get; set; }
        [StringLength(255)]
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }
        [StringLength(50)]
        public string City { get; set; }
        public string State { get; set; }
        [StringLength(10)]
        [DisplayName("Zip Code")]
        public string ZipCode { get; set; }
        [StringLength(155)]
        public string Occupation { get; set; }
        [DisplayName("Date of Birth")]
        public string DOB { get; set; }
        [StringLength(20)]
        [DisplayName("Preferred Method of Communication")]
        public string PreferredMethodOfComm { get; set; }
        public int Active { get; set; }
        public int Admin { get; set; }
        public string Avatar { get; set; }
        public string LastLoginDate { get; set; }
        public string LastLoginIP { get; set; }

        [DisplayName("Role(s)")]
        public string[] Roles { get; set; }
        public List<SelectListItem> AllRoles { get; set; }
    }
}