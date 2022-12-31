using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Admin.ViewModels
{
    public class LookupViewModel
    {
        public int LookupID { get; set; }
        [Required]
        [StringLength(maximumLength: 200)]
        [DisplayName("List Name")]
        public string ListName { get; set; }

        public IList<LookupItemsViewModel> LookupItems { get; set; }
    }

    public class LookupItemsViewModel
    {
        public int ID { get; set; }
        public string KeyValue { get; set; }
        public string DisplayValue { get; set; }
        public int Active { get; set; }
    }
}