namespace ShipleySwine
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ShipleySwineContext : DbContext
    {
        public ShipleySwineContext()
            : base("name=ShipleySwineContext")
        {
        }

        public virtual DbSet<Boar> Boars { get; set; }
        public virtual DbSet<FAQ> FAQs { get; set; }
        public virtual DbSet<SellingPoint> SellingPoints { get; set; }
        public virtual DbSet<SellingPoint1> SellingPoints1 { get; set; }
        public virtual DbSet<Winner> Winners { get; set; }
        public virtual DbSet<BredGuilt> BredGilts { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Boar>()
                .Property(e => e.Boar_Id)
                .HasPrecision(20, 0);

            modelBuilder.Entity<SellingPoint>()
                .Property(e => e.SellingPoints_Id)
                .HasPrecision(20, 0);

            modelBuilder.Entity<SellingPoint>()
                .Property(e => e.Boar_Id)
                .HasPrecision(20, 0);

            modelBuilder.Entity<SellingPoint1>()
                .Property(e => e.SellingPoints_Id)
                .HasPrecision(20, 0);

            modelBuilder.Entity<BredGuilt>()
                .Property(e => e.BredGuiltId)
                .HasPrecision(20, 0);
        }
    }
}
