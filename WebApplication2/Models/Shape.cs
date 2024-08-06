using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Polygon.Models
{
    public class Shape
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public uint Color { get; set; }
        public List<Point> Points { get; set; } = new List<Point>();
        public double Area { get; set; }
        public double LongestSide { get; set; }
        public double Perimeter { get; set; }
    }

    public class Point
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public int PolygonId { get; set; }
    }
}
