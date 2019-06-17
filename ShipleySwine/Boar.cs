namespace ShipleySwine
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Boar
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Boar()
        {
            SellingPoints = new HashSet<SellingPoint>();
        }

        [Key]
        [Column(TypeName = "numeric")]
        public decimal Boar_Id { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        public DateTime? Farrowed { get; set; }

        public byte? LitterSize { get; set; }

        public byte? Price { get; set; }

        public bool? GuaranteedSettle { get; set; }

        [StringLength(255)]
        public string StressTest { get; set; }

        [StringLength(255)]
        public string NameNoSpaces { get; set; }

        [StringLength(255)]
        public string Sire { get; set; }

        [StringLength(255)]
        public string SireFull { get; set; }

        [StringLength(255)]
        public string Dam { get; set; }

        [StringLength(255)]
        public string DamFull { get; set; }

        [StringLength(255)]
        public string Breed { get; set; }

        public byte? Order { get; set; }

        public bool? Featured { get; set; }

        public short? FeaturedOrder { get; set; }

        [StringLength(255)]
        public string EarNotch { get; set; }

        [StringLength(255)]
        public string RegNum { get; set; }

        [StringLength(255)]
        public string TestData { get; set; }

        [StringLength(255)]
        public string BredBy { get; set; }

        [StringLength(255)]
        public string OwnedBy { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SellingPoint> SellingPoints { get; set; }
    }
}
