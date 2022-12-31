using SharedAssemblies.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Admin.ViewModels
{
    public class ProcessViewModel
    {
        private static ProcessViewModel _Instance;

        public static ProcessViewModel Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new ProcessViewModel();
                }

                return _Instance;
            }
        }

        public int ProcessId { get; set; }
        public int FormilaeProcess { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "This field can not be longer than 50 characters.")]
        //[DisplayName("Process Name")]
        public string ProcessName { get; set; }
        [Required]
        [StringLength(500, ErrorMessage = "Summary can not be longer than 500 characters.")]
        [DataType(DataType.MultilineText)]
        public string Summary { get; set; }
        [StringLength(500, ErrorMessage = "Summary can not be longer than 500 characters.")]
        [DataType(DataType.MultilineText)]
        [DisplayName("Government Mailing Address")]
        public string GovernmentMailingAddress1 { get; set; }
        [StringLength(255, ErrorMessage = "Packet Title can not be longer than 255 characters.")]
        [DisplayName("Packet Title")]
        public string PacketTitle { get; set; }
        [DisplayName("Requires Admin Approval")]
        public int RequiresAdminApproval { get; set; }
        [DisplayName("Generates Email")]
        public int GeneratesEmail { get; set; }
        [StringLength(2050, ErrorMessage = "External Redirect can not be longer than 2050 characters.")]
        [DisplayName("External Redirect")]
        public string ExternalRedirect { get; set; }
        public int HideFromServicesPurchasedList { get; set; }
        [DataType(DataType.MultilineText)]
        [DisplayName("Form List Header Message")]
        [AllowHtml]
        public string FormListHeaderMessage { get; set; }
        [DataType(DataType.MultilineText)]
        [DisplayName("Welcome Email")]
        [AllowHtml]
        public string WelcomeEmail { get; set; }
        [DisplayName("Public Survey")]
        public int NoEmailAuth { get; set; }
        [DisplayName("User Supplies Email")]
        public int UserSuppliesEmail { get; set; }
        [DisplayName("Email Field Required")]
        public int EmailSurveyEmailRequired { get; set; }
        [DisplayName("Human Verification")]
        public int HumanVerifyNeeded { get; set; }
        [DisplayName("Show Name Field")]
        public int EmailSurveyNameUse { get; set; }
        [DisplayName("Name Field Required")]
        public int EmailSurveyNameRequired { get; set; }
        [DisplayName("Show Phone Field")]
        public int EmailSurveyPhoneUse { get; set; }
        [DisplayName("Phone Field Required")]
        public int EmailSurveyPhoneRequired { get; set; }
        [DataType(DataType.MultilineText)]
        [DisplayName("Start Page Header Text")]
        [AllowHtml]
        public string EmailSurveyStartHeaderText { get; set; }
        public string EmailSurveyImage { get; set; }
        [DisplayName("Begin Without Registration")]
        public int BeginWithoutRegistration { get; set; }
        [DisplayName("Hyperlink Directory")]
        public string HyperlinkDirectory { get; set; }
        [DisplayName("Generates PDF")]
        public int QuestionnaireGeneratePDF { get; set; }

        public IList<ProcessCostsViewModel> ProcessCosts { get; set; }
        public IList<ProcessFormsGet> ProcessForms { get; set; }
    }

    public class ProcessCostsViewModel
    {
        public int ID { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public decimal Price { get; set; }
        public int SortOrder { get; set; }
        public int Active { get; set; }
    }
}