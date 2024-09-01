using Polygon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolygonClient.Extensions
{
    public static class ShapeExtensions
    {
        public static double CalculateArea(this Shape shape)
        {
            if (shape.Points.Count < 3)
                return 0.0;

            double area = 0.0;
            int n = shape.Points.Count;
            for (int i = 0; i < n; i++)
            {
                int j = (i + 1) % n;
                area += shape.Points[i].X * shape.Points[j].Y;
                area -= shape.Points[j].X * shape.Points[i].Y;
            }
            area = Math.Abs(area) / 2.0;
            return area;
        }

        public static double CalculatePerimeter(this Shape shape)
        {
            double perimeter = 0.0;
            int n = shape.Points.Count;
            for (int i = 0; i < n; i++)
            {
                int j = (i + 1) % n;
                double dx = shape.Points[j].X - shape.Points[i].X;
                double dy = shape.Points[j].Y - shape.Points[i].Y;
                perimeter += Math.Sqrt(dx * dx + dy * dy);
            }
            return perimeter;
        }

        public static double CalculateLongestSide(this Shape shape)
        {
            double longestSide = 0.0;
            int n = shape.Points.Count;
            for (int i = 0; i < n; i++)
            {
                int j = (i + 1) % n;
                double dx = shape.Points[j].X - shape.Points[i].X;
                double dy = shape.Points[j].Y - shape.Points[i].Y;
                double sideLength = Math.Sqrt(dx * dx + dy * dy);
                if (sideLength > longestSide)
                    longestSide = sideLength;
            }
            return longestSide;
        }
    }
}
