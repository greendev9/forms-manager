using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SharedAssemblies.Models
{
    public class Form_Question
    {
        public int ID { get; set; }
        public int FormID { get; set; }
        public string QuestionOfficial { get; set; }
        public int? FormLookupID { get; set; }
        public string DateActive { get; set; }
        public string DateInactive { get; set; }
    }
}