using Microsoft.EntityFrameworkCore;

namespace PolygonClient.Models
{
    public class PolygonContext : DbContext
    {
        public PolygonContext(DbContextOptions<PolygonContext> options) : base(options) { }

        public DbSet<Shape> Shapes { get; set; }
        public DbSet<Point> Points { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Point>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Shape>()
                .HasMany(p => p.Points)
                .WithOne()
                .HasForeignKey(p => p.ShapeId);
        }
    }
}
