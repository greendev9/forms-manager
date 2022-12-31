using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedAssemblies.Models
{
    public class ProcessGet
    {
        public int ID { get; set; }
        public string ProcessName { get; set; }
        public string WelcomeEmail { get; set; }
        public string Summary { get; set; }
        public string GovernmentMailingAddress1 { get; set; }
        public string PacketTitle { get; set; }
        public int RequiresAdminApproval { get; set; }
        public int GeneratesEmail { get; set; }
        public string ExternalRedirect { get; set; }
        public string FormListHeaderMessage { get; set; }
        public int FormilaeProcess { get; set; }
        public int HideFromServicesPurchasedList { get; set; }
        public int UserSuppliesEmail { get; set; }
        public int NoEmailAuth { get; set; }
        public int HumanVerifyNeeded { get; set; }
        public string EmailSurveyImage { get; set; }
        public int EmailSurveyPhoneUse { get; set; }
        public int EmailSurveyNameUse { get; set; }
        public int EmailSurveyNameRequired { get; set; }
        public int EmailSurveyPhoneRequired { get; set; }
        public int EmailSurveyEmailRequired { get; set; }
        public string EmailSurveyStartHeaderText { get; set; }
        public int BeginWithoutRegistration { get; set; }
        public string HyperlinkDirectory { get; set; }
        public int QuestionnaireGeneratePDF { get; set; }

    }
}
