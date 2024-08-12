using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PolygonClient
{
    public partial class MainWindow : Window
    {
        private const string ApiUrl = "https://localhost:7098/Shapes/AddShape";
        private List<Polygon.Models.Shape>? _shapes;
        private double _scaleFactor = 50;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "JSON Files (*.json)|*.json",
                DefaultExt = ".json"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                FilePathTextBox.Text = openFileDialog.FileName;
                ProcessFile(openFileDialog.FileName);
            }
        }

        private void ProcessFile(string filePath)
        {
            try
            {
                var json = File.ReadAllText(filePath);
                _shapes = JsonConvert.DeserializeObject<List<Polygon.Models.Shape>>(json);

                if (_shapes != null)
                {
                    double averageArea = _shapes.Average(shape => shape.Area);

                    foreach (var shape in _shapes)
                    {
                        // Calculate areas, perimeters, and longest sides for each shape
                        shape.Area = CalculateArea(shape.Points);
                        shape.Perimeter = CalculatePerimeter(shape.Points);
                        shape.LongestSide = CalculateLongestSide(shape.Points);

                        // Example logic for color
                        shape.Color = (uint)ColorToArgb(
                            255, // Alpha (fully opaque)
                            (byte)(shape.Area > averageArea ? 0 : 255), // Red
                            (byte)(shape.Area > averageArea ? 255 : 0), // Green
                            0 // Blue
                        );
                    }

                    PolygonsListBox.ItemsSource = _shapes;
                }
                else
                {
                    MessageBox.Show("Не вдалося десеріалізувати файл JSON.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка обробки файлу: {ex.Message}");
            }
        }

        private double CalculateArea(List<Polygon.Models.Point> points)
        {
            if (points.Count < 3)
                return 0.0;

            double area = 0.0;
            int n = points.Count;
            for (int i = 0; i < n; i++)
            {
                int j = (i + 1) % n;
                area += points[i].X * points[j].Y;
                area -= points[j].X * points[i].Y;
            }
            area = Math.Abs(area) / 2.0;
            return area;
        }

        private double CalculatePerimeter(List<Polygon.Models.Point> points)
        {
            double perimeter = 0.0;
            int n = points.Count;
            for (int i = 0; i < n; i++)
            {
                int j = (i + 1) % n;
                double dx = points[j].X - points[i].X;
                double dy = points[j].Y - points[i].Y;
                perimeter += Math.Sqrt(dx * dx + dy * dy);
            }
            return perimeter;
        }

        private double CalculateLongestSide(List<Polygon.Models.Point> points)
        {
            double longestSide = 0.0;
            int n = points.Count;
            for (int i = 0; i < n; i++)
            {
                int j = (i + 1) % n;
                double dx = points[j].X - points[i].X;
                double dy = points[j].Y - points[i].Y;
                double sideLength = Math.Sqrt(dx * dx + dy * dy);
                if (sideLength > longestSide)
                    longestSide = sideLength;
            }
            return longestSide;
        }

        private async void SendDataButton_Click(object sender, RoutedEventArgs e)
        {
            if (_shapes != null)
            {
                var json = JsonConvert.SerializeObject(_shapes);
                await SendDataToServer(json);
            }
            else
            {
                MessageBox.Show("Немає даних для відправлення.");
            }
        }

        private async Task SendDataToServer(string json)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(ApiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Дані успішно надіслано на сервер.");
                    }
                    else
                    {
                        MessageBox.Show("Не вдалося надіслати дані на сервер.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при відправці даних на сервер: {ex.Message}");
            }
        }

        private void DisplayShapeButton_Click(object sender, RoutedEventArgs e)
        {
            if (_shapes != null && _shapes.Count > 0)
            {
                ShapeCanvas.Children.Clear();
                ShapeCanvas.Children.Add(CreateCoordinateGrid());

                double minX = _shapes.SelectMany(shape => shape.Points).Min(p => p.X);
                double maxX = _shapes.SelectMany(shape => shape.Points).Max(p => p.X);
                double minY = _shapes.SelectMany(shape => shape.Points).Min(p => p.Y);
                double maxY = _shapes.SelectMany(shape => shape.Points).Max(p => p.Y);

                double canvasCenterX = ShapeCanvas.ActualWidth / 2;
                double canvasCenterY = ShapeCanvas.ActualHeight / 2;

                foreach (var shape in _shapes)
                {
                    var points = shape.Points.Select(p => new System.Windows.Point(p.X * _scaleFactor, p.Y * _scaleFactor)).ToList();

                    double shapeWidth = maxX - minX;
                    double shapeHeight = maxY - minY;

                    double offsetX = canvasCenterX - (minX + shapeWidth / 2) * _scaleFactor;
                    double offsetY = canvasCenterY - (minY + shapeHeight / 2) * _scaleFactor;

                    var polygon = new System.Windows.Shapes.Polygon
                    {
                        Points = new PointCollection(points.Select(p => new System.Windows.Point(p.X + offsetX, p.Y + offsetY))),
                        Stroke = Brushes.Black,
                        Fill = new SolidColorBrush(Color.FromArgb(
                            (byte)(shape.Color >> 24),
                            (byte)(shape.Color >> 16),
                            (byte)(shape.Color >> 8),
                            (byte)shape.Color
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

        private Canvas CreateCoordinateGrid()
        {
            var gridCanvas = new Canvas
            {
                Width = ShapeCanvas.ActualWidth,
                Height = ShapeCanvas.ActualHeight
            };

            double step = 50;
            for (double x = 0; x < gridCanvas.Width; x += step)
            {
                var line = new Line
                {
                    X1 = x,
                    Y1 = 0,
                    X2 = x,
                    Y2 = gridCanvas.Height,
                    Stroke = Brushes.LightGray,
                    StrokeThickness = 1
                };
                gridCanvas.Children.Add(line);

                var text = new TextBlock
                {
                    Text = x.ToString("0"),
                    Foreground = Brushes.Black,
                    FontSize = 10,
                    Margin = new Thickness(x + 2, 0, 0, 0)
                };
                Canvas.SetLeft(text, x);
                Canvas.SetTop(text, -20);
                gridCanvas.Children.Add(text);
            }

            for (double y = 0; y < gridCanvas.Height; y += step)
            {
                var line = new Line
                {
                    X1 = 0,
                    Y1 = y,
                    X2 = gridCanvas.Width,
                    Y2 = y,
                    Stroke = Brushes.LightGray,
                    StrokeThickness = 1
                };
                gridCanvas.Children.Add(line);

                var text = new TextBlock
                {
                    Text = y.ToString("0"),
                    Foreground = Brushes.Black,
                    FontSize = 10,
                    Margin = new Thickness(0, y - 10, 0, 0)
                };
                Canvas.SetLeft(text, -30);
                Canvas.SetTop(text, y);
                gridCanvas.Children.Add(text);
            }

            return gridCanvas;
        }

        private uint ColorToArgb(byte alpha, byte red, byte green, byte blue)
        {
            return (uint)((alpha << 24) | (red << 16) | (green << 8) | blue);
        }
    }
}
