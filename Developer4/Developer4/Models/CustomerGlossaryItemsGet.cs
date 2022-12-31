using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SharedAssemblies.Models
{
    public class CustomerGlossaryItemsGet
    {
        public int ID { get; set; }
        public string LookupKey { get; set; }
        public string DefaultText { get; set; }
        public string CustomerValue { get; set; }
        public string ValueWithDefault { get; set; }
        public int Required { get; set; }
        public int HtmlAllowed { get; set; }
        public string HyperlinkElements { get; set; }
        public string Description { get; set; }
        public string TokensAvailable { get; set; }
    }
}