using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShipleySwine.ViewModels
{
    public class ContactEmailViewModel
    {
        [Required]
        [StringLength(100)]
        public string fullname { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(254)]
        public string email { get; set; }

        [Required]
        [StringLength(50)]
        public string phone { get; set; }

        [Required]
        [StringLength(150)]
        public string subject { get; set; }

        [Required]
        [StringLength(4000)]
        public string comments { get; set; }

        // Honeypot: real users never see or fill this field.
        public string website { get; set; }
    }
}
