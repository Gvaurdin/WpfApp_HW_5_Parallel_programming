using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfApp_HW_5_Parallel_programming
{
    /// <summary>
    /// Логика взаимодействия для MainWindow_Task_2.xaml
    /// </summary>
    public partial class MainWindow_Task_2 : Window
    {
        // переменная для отслеживания наличия дубликатов
        private bool isFind = false;
        public MainWindow_Task_2()
        {
            InitializeComponent();
        }

        private void SourceDirectoryBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            // создание диалогового окна для выбора директории
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                // отображение диалогового окна и получение результата выбора
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                // если пользователь выбрал директорию и нажал "OK"
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    // установка выбранного пути в текстовое поле для исходной директории
                    SourceDirectoryTextBox.Text = dialog.SelectedPath;
                    // установка выбранного пути в метку для отображения выбранной директории
                    SourceDirectoryLabel.Content = dialog.SelectedPath;
                }
            }
        }

        private void DestinationDirectoryBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    DestinationDirectoryTextBox.Text = dialog.SelectedPath;
                    DestinationDirectoryLabel.Content = dialog.SelectedPath;
                }
            }
        }

        private async void CheckDuplicatesButton_Click(object sender, RoutedEventArgs e)
        {
            // получение путей исходной и целевой директорий из текстовых полей
            string sourceDirectory = SourceDirectoryTextBox.Text;
            string destinationDirectory = DestinationDirectoryTextBox.Text;

            // проверка, что обе директории были выбраны
            if (string.IsNullOrWhiteSpace(sourceDirectory) || string.IsNullOrWhiteSpace(destinationDirectory))
            {
                //вывод сообщения об ошибке, если одна из директорий не выбрана
                MessageBox.Show("Выберите исходную и целевую директории");
                return;
            }

            try
            {
                //вызов асинхронного метода для проверки и перемещения дубликатов
                await CheckAndMoveDuplicatesAsync(sourceDirectory, destinationDirectory);
                // проверка наличия дубликатов и вывод сообщения об успешном завершении
                if (isFind)
                    MessageBox.Show("Проверка завершена. Оригинальные файлы перемещены в целевую директорию.");
            }
            catch (Exception ex)
            {
                // вывод сообщения об ошибке, если произошла исключительная ситуация
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private async Task CheckAndMoveDuplicatesAsync(string sourceDirectory, string destinationDirectory)
        {
            // искусственная задержка в 2 секунды для имитации выполнения операции
            await Task.Delay(2000);

            // получение списка файлов в исходной директории
            var files = Directory.GetFiles(sourceDirectory);

            // группировка файлов по размеру
            var groupedFiles = files.GroupBy(file => new FileInfo(file).Length);

            // список для хранения имен перемещенных файлов
            var movedFiles = new List<string>();

            // перебор групп файлов
            foreach (var group in groupedFiles)
            {
                // если в группе больше одного файла
                if (group.Count() > 1)
                {
                    // выбор первого файла в группе как оригинального
                    var originalFile = group.First();

                    //выбор всех остальных файлов в группе как дубликатов
                    var duplicateFiles = group.Skip(1);

                    //перебор дубликатов
                    foreach (var duplicateFile in duplicateFiles)
                    {
                        //формирование пути для перемещения дубликата в целевую директорию
                        string destinationFilePath = Path.Combine(destinationDirectory, Path.GetFileName(duplicateFile));

                        // проверка, что файл еще не был перемещен
                        if (!File.Exists(destinationFilePath))
                        {
                            // перемещение файла в целевую директорию
                            File.Move(duplicateFile, destinationFilePath);
                            //добавление имени перемещенного файла в список
                            movedFiles.Add(Path.GetFileName(destinationFilePath));
                        }
                    }
                }
            }

            // если были перемещены какие-либо файлы
            if (movedFiles.Any())
            {
                // формирование сообщения о перемещенных файлах
                string movedFilesMessage = string.Join("\n", movedFiles);
                // вывод сообщения об успешном перемещении файлов
                MessageBox.Show($"Перемещены файлы:\n{movedFilesMessage}", "Перемещение файлов");
                // установка флага наличия дубликатов в true
                isFind = true;
            }
            else
            {
                // вывод сообщения о том, что дубликаты не были найдены
                MessageBox.Show("Нет дубликатов для перемещения.", "Перемещение файлов");
            }
        }
    }
}
