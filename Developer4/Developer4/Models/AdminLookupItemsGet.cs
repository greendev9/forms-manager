using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedAssemblies.Models
{
    public class AdminLookupItemsGet
    {
        public int ID { get; set; }
        public string KeyValue { get; set; }
        public string DisplayValue { get; set; }
        public int Active { get; set; }
    }
}
