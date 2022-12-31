using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SharedAssemblies.Models
{
    public class ProcessFormGet
    {
        public int ID { get; set; }
        public int ProcessID { get; set; }
        public string ProcessName { get; set; }
        public string FormName { get; set; }
        [AllowHtml]
        [DataType(DataType.MultilineText)]
        [DisplayName("Top Message")]
        public string FormMessageTop { get; set; }
        [DisplayName("Top Message Border and Coloring Hidden")]
        public int FormMessageTopPlain { get; set; }
        [DisplayName("Display Results")]
        public int DisplayResults { get; set; }
        [AllowHtml]
        [DataType(DataType.MultilineText)]
        [DisplayName("Results Page Top Message")]
        public string ResultsMessageTop { get; set; }
        public int IsSurveyOrQuestionnaire { get; set; }
        public int? FormGroupID { get; set; }
        [DisplayName("Add to Process Automatically")]
        public int AutoAddToService { get; set; }
        [DisplayName("Requires Admin Approval")]
        public int RequiresAdminApproval { get; set; }
        [DisplayName("Allow Additional Attachment")]
        public int AllowAdditionalAttachment { get; set; }
        public int Required { get; set; }
        [DisplayName("Display on Service List")]
        public int ServiceListDisplay { get; set; }
        [AllowHtml]
        [DisplayName("Form Approved Opening Email Body")]
        public string FormApprovedOpeningEmailBodyMsg { get; set; }
        [AllowHtml]
        [DisplayName("Form Approved Footer Message")]
        public string ApprovedEmailIfYouHaveQuestionsFooter { get; set; }
        [DisplayName("Form Complete Page Delegate")]
        public string LastPageDelegateName { get; set; }
    }
}
