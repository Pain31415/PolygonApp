using Microsoft.EntityFrameworkCore;

namespace Polygon.Models
{
    public class PolygonContext : DbContext
    {
        public PolygonContext(DbContextOptions<PolygonContext> options) : base(options) { }
        public DbSet<Shape> Polygons { get; set; }
        public DbSet<Point> Points { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Shape>()
                .HasKey(s => s.Id);

            modelBuilder.Entity<Shape>()
                .Property(s => s.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Point>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Point>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Point>()
                .HasOne<Shape>()
                .WithMany(s => s.Points)
                .HasForeignKey(p => p.PolygonId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
