namespace ShipleySwine
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SellingPoints")]
    public partial class SellingPoint1
    {
        [StringLength(255)]
        public string SellingPoint { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? SellingPoints_Id { get; set; }

        public int ID { get; set; }

        public virtual SellingPoint SellingPoints { get; set; }
    }
}
