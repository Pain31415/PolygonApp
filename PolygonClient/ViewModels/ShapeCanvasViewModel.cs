using Polygon.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PolygonClient.ViewModels
{
    public class ShapeCanvasViewModel : INotifyPropertyChanged
    {
        public Canvas ShapeCanvas { get; }
        public ShapeCanvasViewModel()
        {
            ShapeCanvas = new Canvas();
        }
        public void DisplayShapes(ObservableCollection<Polygon.Models.Shape> shapes)
        {
            double averageArea = shapes.Average(shape => shape.Area);
            var renderedshapes = new List<Polygon.Models.Shape>();
            foreach (var shape in shapes)
            {
                var color = ColorBasedOnAverageArea(shape.Area, averageArea);
                renderedshapes.Add(new Polygon.Models.Shape
                {
                    Points = shape.Points,
                    Color = color,
                    Area = shape.Area,
                    Perimeter = shape.Perimeter
                });
            }
            RenderShapes(renderedshapes);
        }

        private uint ColorBasedOnAverageArea(double area, double averageArea)
        {
                if (area < averageArea)
                {
                    return ColorToArgb(255, 0, 255, 0);
                }
                else if (area > averageArea)
                {
                    return ColorToArgb(255, 255, 0, 0);
                }
                else
                {
                    return ColorToArgb(255, 255, 255, 0);
                }
        }

        private void CreateCoordinateGrid()
        {
            
            double step = 20;
            for (double x = 0; x < ShapeCanvas.ActualWidth; x += step)
            {
                var line = new Line
                {
                    X1 = x,
                    Y1 = 0,
                    X2 = x,
                    Y2 = ShapeCanvas.ActualHeight,
                    Stroke = Brushes.LightGray,
                    StrokeThickness = 1
                };
                ShapeCanvas.Children.Add(line);

                var text = new TextBlock
                {
                    Text = x.ToString("0"),
                    Foreground = Brushes.Black,
                    FontSize = 10,
                    Margin = new Thickness(x + 2, 0, 0, 0)
                };
                Canvas.SetLeft(text, x);
                Canvas.SetTop(text, -20);
                ShapeCanvas.Children.Add(text);
            }

            for (double y = 0; y < ShapeCanvas.ActualHeight; y += step)
            {
                var line = new Line
                {
                    X1 = 0,
                    Y1 = y,
                    X2 = ShapeCanvas.ActualWidth,
                    Y2 = y,
                    Stroke = Brushes.LightGray,
                    StrokeThickness = 1
                };
                ShapeCanvas.Children.Add(line);

                var text = new TextBlock
                {
                    Text = y.ToString("0"),
                    Foreground = Brushes.Black,
                    FontSize = 10,
                    Margin = new Thickness(0, y - 10, 0, 0)
                };
                Canvas.SetLeft(text, -30);
                Canvas.SetTop(text, y);
                ShapeCanvas.Children.Add(text);
            }
        }

        private uint ColorToArgb(byte alpha, byte red, byte green, byte blue)
        {
            return (uint)((alpha << 24) | (red << 16) | (green << 8) | blue);
        }

        private void RenderShapes(List<Polygon.Models.Shape> shapes)
        {
            if (shapes != null && shapes.Any())
            {
                ShapeCanvas.Children.Clear();
                CreateCoordinateGrid();

                double minX = shapes.SelectMany(shape => shape.Points).Min(p => p.X);
                double maxX = shapes.SelectMany(shape => shape.Points).Max(p => p.X);
                double minY = shapes.SelectMany(shape => shape.Points).Min(p => p.Y);
                double maxY = shapes.SelectMany(shape => shape.Points).Max(p => p.Y);

                double shapeWidth = maxX - minX;
                double shapeHeight = maxY - minY;

                double canvasWidth = ShapeCanvas.ActualWidth;
                double canvasHeight = ShapeCanvas.ActualHeight;

                double scaleX = canvasWidth / shapeWidth;
                double scaleY = canvasHeight / shapeHeight;
                double scale = Math.Min(scaleX, scaleY);

                double offsetX = (canvasWidth - shapeWidth * scale) / 2;
                double offsetY = (canvasHeight - shapeHeight * scale) / 2;

                foreach (var shape in shapes)
                {
                    var points = shape.Points.Select(p => new System.Windows.Point((p.X - minX) * scale + offsetX, (p.Y - minY) * scale + offsetY)).ToList();

                    var polygon = new System.Windows.Shapes.Polygon
                    {
                        Points = new PointCollection(points),
                        Stroke = Brushes.Black,
                        Fill = new SolidColorBrush(Color.FromArgb(
                            (byte)(shape.Color >> 24),
                            (byte)((shape.Color >> 16) & 0xFF),
                            (byte)((shape.Color >> 8) & 0xFF),
                            (byte)(shape.Color & 0xFF)
                        )),
                        StrokeThickness = 2
                    };

                    ShapeCanvas.Children.Add(polygon);
                }
            }
            else
            {
                MessageBox.Show("Фігури не завантажені.");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}