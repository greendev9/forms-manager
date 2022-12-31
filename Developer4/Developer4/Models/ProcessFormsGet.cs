using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedAssemblies.Models
{
    public class ProcessFormsGet
    {
        public int ID { get; set; }
        public int FormID { get; set; }
        public string FormName { get; set; }
        public string FormCode { get; set; }
        public int AsNeeded { get; set; }
        public int Optional { get; set; }
        public int FormilaeProcessForm { get; set; }
        public int ProcessID { get; set; }
        public int Active { get; set; }
    }
}
