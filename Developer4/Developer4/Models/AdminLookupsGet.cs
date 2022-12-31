using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedAssemblies.Models
{
    public class AdminLookupsGet
    {
        public int ID { get; set; }
        public int LookupID { get; set; }
        public string ListName { get; set; }
        public int NoRecords { get; set; }
        public int InternalList { get; set; }
    }
}
