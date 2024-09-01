using Newtonsoft.Json;
using PolygonClient.Extensions;
using PolygonClient.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PolygonClient.ViewModels
{

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private ShapeService _shapeService = new ShapeService();

        private string openedfileName;
        public string OpenedFileName
        {
            get => openedfileName;
            set
            {
                openedfileName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OpenedFileName)));
            }
        }
        public ICommand SelectFileCommand => selectFileCommand; 
        public ICommand selectFileCommand;

        public ICommand SendDataCommand => sendDataCommand;
        public ICommand sendDataCommand;
        public ICommand DisplayShapeCommand => displayShapeCommand;
        public ICommand displayShapeCommand;

        public ObservableCollection<Polygon.Models.Shape> Shapes { get;}
        public ShapeCanvasViewModel CanvasViewModel { get;}

        public MainWindowViewModel()
        {
            CanvasViewModel = new ShapeCanvasViewModel();
            selectFileCommand = new RelayCommand(p => SelectFile());
            Shapes = new ObservableCollection<Polygon.Models.Shape>();
            openedfileName = string.Empty;
            sendDataCommand = new RelayCommand(p => SendData());
            displayShapeCommand = new RelayCommand(p => CanvasViewModel.DisplayShapes(Shapes));
        }
        private void SelectFile()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "JSON Files (*.json)|*.json",
                DefaultExt = ".json"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                OpenedFileName = openFileDialog.FileName;
                ProcessFile(openFileDialog.FileName);
            }
        }
        private void ProcessFile(string filePath)
        {
            try
            {
                var json = File.ReadAllText(filePath);
                var shapes = JsonConvert.DeserializeObject<List<Polygon.Models.Shape>>(json);
                if (shapes != null)
                {
                    foreach (var shape in shapes)
                    {
                        shape.Area = shape.CalculateArea();
                        shape.Perimeter = shape.CalculatePerimeter();
                        shape.LongestSide = shape.CalculateLongestSide();
                        Shapes.Add(shape);
                    }
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
        public async void SendData()
        {
            if (Shapes != null)
            {
                await _shapeService.SendDataToServer (Shapes);
            }
            else
            {
                MessageBox.Show("Немає даних для відправлення.");
            }
        }
    }
}
