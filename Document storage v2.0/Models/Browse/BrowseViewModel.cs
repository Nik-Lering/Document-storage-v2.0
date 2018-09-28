using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Document_storage_v2.Models
{
    public class BrowseViewModel
    {
        [Required]
        public Users Owner { get; set; }

        [Required]
        public ICollection<Documents> Documents { get; set; }
    }
}