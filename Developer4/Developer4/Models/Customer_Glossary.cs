using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SharedAssemblies.Models
{
    public class Customer_Glossary
    {
        public int ID { get; set; }
        public int CustomerID { get; set; }
        public int GlossaryID { get; set; }
        public string Text { get; set; }
        public string LastUpdateDate { get; set; }
    }
}