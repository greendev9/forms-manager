using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SharedAssemblies.Models
{
    public class AdminFormGet
    {
        public int ID { get; set; }
        public string FormName { get; set; }
        public string FormCode { get; set; }
        public int Active { get; set; }
        public int AuthorizeWithoutLogin { get; set; }
        public string IdentifierUnlockMsg { get; set; }
        public string SourceFileName { get; set; }
        public string DestinationFileName { get; set; }
        public string CoverletterBulletItemText { get; set; }
        public string ApprovalInstructions { get; set; }
        public int? SupplementPageNo { get; set; }
        public int SupplementPageSectionCount { get; set; }
        public string SupplementPageMsg { get; set; }
        public string SupplementPageFieldMsg { get; set; }
        public int? SupplementPageTextCharsMax { get; set; }
        public string RequireUnitNoPattern { get; set; }
        public int AllowMultipleCopies { get; set; }
        public int HideSections { get; set; }
        public string Header { get; set; }
        public string HiddenFormCompleted { get; set; }
        public int IsPropertyAddress { get; set; }
        public int HeaderPlain { get; set; }
        public string FormBuilderJson { get; set; }
        public int GeneratesPDF { get; set; }
        public int FormilaeForm { get; set; }
        public string NotApplicableText { get; set; }
    }
}