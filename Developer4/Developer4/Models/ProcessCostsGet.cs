using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedAssemblies.Models
{
    public class ProcessCostsGet
    {
        public int ID { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public decimal Price { get; set; }
        public int SortOrder { get; set; }
        public int Active { get; set; }
    }
}
