using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedAssemblies.Models
{
    public class ProcessesGet
    {
        public int ID { get; set; }
        public string ProcessName { get; set; }
        public int Active { get; set; }
        public decimal Price { get; set; }
        public int ProcessFormsCount { get; set; }
        public int FormilaeProcess { get; set; }
        public string SurveyLinkDirectory { get; set; }
        public int ShowSurveyFormLink { get; set; }
        public int? SurveyProcessFormID { get; set; }
    }
}
