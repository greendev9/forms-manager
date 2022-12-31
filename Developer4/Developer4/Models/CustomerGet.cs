using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SharedAssemblies.Models
{
    public class CustomerGet
    {
        public int ID { get; set; }
        [Required]
        [StringLength(150)]
        [DataType(DataType.Text)]
        [Display(Name = "Business Name")]
        public string BusinessName { get; set; }
        [StringLength(50)]
        [DataType(DataType.Text)]
        [Display(Name = "Business Subtitle")]
        public string BusinessSubTitle { get; set; }
        [Required]
        [StringLength(75)]
        [DataType(DataType.Text)]
        [Display(Name = "Contact Firstname")]
        public string ContactFirstName { get; set; }
        [Required]
        [StringLength(75)]
        [DataType(DataType.Text)]
        [Display(Name = "Contact Lastname")]
        public string ContactLastName { get; set; }
        [Required]
        [StringLength(200)]
        [EmailAddress]
        [Display(Name = "Contact Email")]
        public string ContactEmail { get; set; }
        [Display(Name = "Customer Code")]
        public int CustomerCode { get; set; }
        [StringLength(255)]
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }
        [StringLength(75)]
        [DataType(DataType.Text)]
        public string City { get; set; }
        public string State { get; set; }
        [StringLength(20)]
        public string Zip { get; set; }
        [StringLength(50)]
        public string Phone { get; set; }
        [StringLength(100)]
        public string Province { get; set; }
        [StringLength(45)]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }
        [StringLength(25)]
        public string Country { get; set; }
        [Display(Name = "Logo File")]
        public string LogoFile { get; set; }
        [AllowHtml]
        [DataType(DataType.MultilineText)]
        [Display(Name = "About Us")]
        public string AboutUsText { get; set; }

        public string returnUrl { get; set; }
    }
}
