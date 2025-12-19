using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShipleySwine.Models
{
    public class FileUpload
    {
        [DataType(DataType.Upload)]
        [Display(Name = "Upload File")]
        [Required(ErrorMessage = "Please choose file to upload.")]
        public string file { get; set; }
        public string breed { get; set; }
        public string NameNoSpace { get; set; }

    }
}