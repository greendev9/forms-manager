using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SharedAssemblies.Models
{
    public class Glossary
    {
        public int ID { get; set; }
        public string LookupKey { get; set; }
        public int IncludeInSessionObject { get; set; }
        public string WhereUsed { get; set; }
        public string DefaultText { get; set; }
        public string Description { get; set; }
        public int Required { get; set; }
        public int HtmlAllowed { get; set; }
        public string HyperlinkElements { get; set; }
        public string LastUpdateDate { get; set; }
    }
}