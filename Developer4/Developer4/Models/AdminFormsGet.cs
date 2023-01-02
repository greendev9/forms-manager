using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SharedAssemblies.Models
{
    public class AdminFormsGet
    {
        public int ID { get; set; }
        public string FormName { get; set; }
        public string FormCode { get; set; }
        public int Active { get; set; }
        public int HideSections { get; set; }
        public string HiddenFormCompleted { get; set; }
        public int IsPropertyAddress { get; set; }
        public int HeaderPlain { get; set; }
        public int GeneratesPDF { get; set; }
        public int FormilaeForm { get; set; }
        public int FormBuilderUse { get; set; }
    }
}


