using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Admin.ViewModels
{
    public class TempFormQuestion
    {
        public string FormCode { get; set; }
        public string FormLabel { get; set; }
        public string QuestionNo { get; set; }
        public string QuestionOfficial { get; set; }
        public string SectionName { get; set; }
        public string SubSectionName { get; set; }
        public string ChoiceText { get; set; }
        public string MaxChars { get; set; }
        public string IsSingleCheckbox { get; set; }
        public string SortOrder { get; set; }
        public string PageNo { get; set; }
        public string MultiSectionName { get; set; }
        public string MultiSectionNo { get; set; }
        public string MultiSectionQuestion { get; set; }
        public string ErrMsg { get; set; }

        public string MaxCharsDB
        {
            get
            {
                return String.IsNullOrEmpty(MaxChars) ? "50" : MaxChars;
            }
        }

        public string IsSingleCheckboxDB {
            get
            {
                var value = !String.IsNullOrEmpty(IsSingleCheckbox) ? IsSingleCheckbox : String.Empty;
                return (value == "1" || value.ToLower() == "yes" || value.ToLower() == "y" || value.ToLower() == "x") ? "1" : "0";
            }
        }

        public string PageNoDB {
            get
            {
                return String.IsNullOrEmpty(PageNo) ? "1" : PageNo;
            }
        }

        public bool MultiSectionInvalid
        {
            get
            {
                bool result = false;
                if (!string.IsNullOrEmpty(MultiSectionName) && String.IsNullOrEmpty(MultiSectionNo))
                {
                    result = true;
                }
                return result;
            }
        }

        // Need to count all unique values so client can leave the real value blank
        public string FormLabelProtected
        {
            get
            {
                //return ChoiceText.Contains("MultiSelect:") || ChoiceText.Contains("RadioList:") ? String.Concat("FrmLbl", SortOrder) : FormLabel;

                // No longer making the form label required because of email generated forms.
                return ChoiceText.Contains("MultiSelect:") || ChoiceText.Contains("RadioList:") || String.IsNullOrEmpty(FormLabel) ? String.Concat("FrmLbl", SortOrder) : FormLabel;
            }
        }
    }
}