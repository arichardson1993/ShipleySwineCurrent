namespace ShipleySwine
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class BredGuilt
    {
        [StringLength(255)]
        public string Breed { get; set; }

        [Key]
        public byte BreedId { get; set; }

        public int? Price { get; set; }

        [StringLength(255)]
        public string EarNotch { get; set; }

        public bool? BlackCrossGilt { get; set; }

        public DateTime? DOB { get; set; }

        public byte? LitterSize { get; set; }

        [StringLength(255)]
        public string Sire { get; set; }

        public bool? SireXBred { get; set; }

        [StringLength(255)]
        public string DamEN { get; set; }

        [StringLength(255)]
        public string SireOfDam { get; set; }

        public bool? SireOfDamXBred { get; set; }

        public DateTime? DateBred { get; set; }

        [StringLength(255)]
        public string SvcSire { get; set; }

        public bool? SvcSireXBred { get; set; }

        public DateTime? DueDate { get; set; }
    }
}
