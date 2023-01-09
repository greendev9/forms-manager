using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SharedAssemblies.Models
{
    public class AdminFormGet
    {
        public int ID { get; set; }
        [Required]
        [StringLength(150)]
        [DisplayName("Form Name")]
        public string FormName { get; set; }
        [Required]
        [StringLength(10)]
        [DisplayName("Form Code")]
        public string FormCode { get; set; }
        public int Active { get; set; }
        [DisplayName("Authorization Without Login")]
        public int AuthorizeWithoutLogin { get; set; }
        [DisplayName("Group Form Unlock Message")]
        public string IdentifierUnlockMsg { get; set; }
        public string SourceFileName { get; set; }
        public string DestinationFileName { get; set; }
        [DisplayName("Cover Letter Bullet Item Text")]
        public string CoverletterBulletItemText { get; set; }
        [DataType(DataType.MultilineText)]
        [DisplayName("Approval Instructions")]
        [AllowHtml]
        public string ApprovalInstructions { get; set; }
        [DisplayName("Supplement Page Number")]
        public int? SupplementPageNo { get; set; }
        [DisplayName("Supplement Page Section Count")]
        public int SupplementPageSectionCount { get; set; }
        [DisplayName("Refer to Custom Supplemental Page Message")]
        public string SupplementPageMsg { get; set; }
        [DisplayName("Refer to Supplement Page Message")]
        public string SupplementPageFieldMsg { get; set; }
        [DisplayName("Custom Supplemental Page Maximum Characters")]
        public int? SupplementPageTextCharsMax { get; set; }
        public string RequireUnitNoPattern { get; set; }
        [DisplayName("Allow Multiple Copies")]
        public int AllowMultipleCopies { get; set; }
        [DisplayName("Hide Sections")]
        public int HideSections { get; set; }
        [DataType(DataType.MultilineText)]
        [DisplayName("Header")]
        [AllowHtml]
        public string Header { get; set; }
        [DataType(DataType.MultilineText)]
        [DisplayName("Confirmation Message (Surveys Only)")]
        [AllowHtml]
        public string HiddenFormCompleted { get; set; }
        [DisplayName("Property Address")]
        public int IsPropertyAddress { get; set; }
        [DisplayName("Header Border and Coloring Hidden")]
        public int HeaderPlain { get; set; }
        public string FormBuilderJson { get; set; }
        public int GeneratesPDF { get; set; }
        public int FormilaeForm { get; set; }
        [Required]
        [StringLength(50)]
        [DisplayName("Not Applicable Text ")]
        public string NotApplicableText { get; set; }
    }
}