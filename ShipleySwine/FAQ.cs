namespace ShipleySwine
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FAQ")]
    public partial class FAQ
    {
        public byte ID { get; set; }

        [StringLength(255)]
        public string Question { get; set; }

        [StringLength(255)]
        public string Answer { get; set; }
    }
}
