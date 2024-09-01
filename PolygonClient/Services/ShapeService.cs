using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using Polygon.Models;

namespace PolygonClient.Services
{
    public class ShapeService
    {
        private const string ApiUrl = "https://localhost:7098/Shapes/AddShape";
        public async Task SendDataToServer(IEnumerable<Shape> shapes)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var json = JsonConvert.SerializeObject(shapes);
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
    }
}
