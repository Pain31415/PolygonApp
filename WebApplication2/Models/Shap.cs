namespace PolygonClient.Models
{
    public class Shape
    {
        public int Id { get; set; }
        public uint Color { get; set; }
        public List<Point> Points { get; set; } = new List<Point>();
        public double Area { get; set; }
        public double LongestSide { get; set; }
        public double Perimeter { get; set; }
    }

    public class Point
    {
        public int Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public int ShapeId { get; set; } // Foreign key for Shape
    }
}
