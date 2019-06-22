namespace ShipleySwine
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SellingPoint")]
    public partial class SellingPoint
    {
        [Key]
        [Column(TypeName = "numeric")]
        public decimal SellingPoints_Id { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Boar_Id { get; set; }

        public virtual Boar Boar { get; set; }

        public virtual ICollection<SellingPoint1> SellingPoints { get; set; }
    }
}
