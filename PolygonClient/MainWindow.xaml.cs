using Newtonsoft.Json;
using Polygon.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Windows;
using Point = Polygon.Models.Point;

namespace PolygonClient
{
    public partial class MainWindow : Window
    {
        private const string ApiUrl = "https://localhost:7098/Shapes/AddShape";
        private List<Shape> _shapes;

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
                _shapes = JsonConvert.DeserializeObject<List<Shape>>(json);

                if (_shapes != null)
                {
                    double averageArea = _shapes.Average(shape => shape.Area);

                    foreach (var shape in _shapes)
                    {
                        shape.Area = CalculateArea(shape.Points);
                        shape.Perimeter = CalculatePerimeter(shape.Points);
                        shape.LongestSide = CalculateLongestSide(shape.Points);
                        shape.Color = shape.Area > averageArea ? 0xFF0000FF : 0xFF000000; // Синій для вище середнього, чорний для інших
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

        private double CalculateArea(List<Point> points)
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

        private double CalculatePerimeter(List<Point> points)
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

        private double CalculateLongestSide(List<Point> points)
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

        private async void SendDataToServer(string json)
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

        private void SendDataButton_Click(object sender, RoutedEventArgs e)
        {
            if (_shapes != null && _shapes.Count > 0)
            {
                var serializedShapes = JsonConvert.SerializeObject(_shapes[0]); // Ви можете вибрати інші дані для відправки
                SendDataToServer(serializedShapes);
            }
            else
            {
                MessageBox.Show("Не обрано жодного файлу або дані не оброблені.");
            }
        }
    }
}
