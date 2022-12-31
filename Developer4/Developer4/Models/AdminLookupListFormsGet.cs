using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedAssemblies.Models
{
    public class AdminLookupListFormsGet
    {
        public int ID { get; set; }
        public int FormID { get; set; }
        public string FormCode { get; set; }
        public string FormName { get; set; }
    }
}
