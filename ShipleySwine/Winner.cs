namespace ShipleySwine
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Winner
    {
        public byte ID { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(255)]
        public string Won { get; set; }

        [StringLength(255)]
        public string Pedigree { get; set; }

        [StringLength(255)]
        public string Act { get; set; }

        [StringLength(255)]
        public string ImageURL { get; set; }

        public int WinnerId { get; set; }
    }
}
