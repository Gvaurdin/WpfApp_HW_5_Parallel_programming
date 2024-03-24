using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp_HW_5_Parallel_programming
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private bool isAnalyzing = false;
        private CancellationTokenSource cancellationTokenSource; // класс для отмены операции анализа
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void AnalyzeButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isAnalyzing)
            {
                isAnalyzing = true;
                AnalyzeButton.Content = "Остановить";
                cancellationTokenSource = new CancellationTokenSource();

                string text = TextInput.Text;

                // проверка состояния чек боксов и запись логики в переменную bool
                bool includeSentences = SentencesCheckBox.IsChecked == true;
                bool includeWords = WordsCheckBox.IsChecked == true;
                bool includeQuestionSentences = QuestionSentencesCheckBox.IsChecked == true;
                bool includeExclamationSentences = ExclamationSentencesCheckBox.IsChecked == true;

                await AnalyzeText(text, includeSentences, includeWords, includeQuestionSentences, includeExclamationSentences, cancellationTokenSource.Token);

                isAnalyzing = false;
                AnalyzeButton.Content = "Анализировать";
                StatusLabel.Content = "";
            }
            else
            {
                isAnalyzing = false;
                cancellationTokenSource.Cancel();
            }
        }

        private async Task AnalyzeText(string text, bool includeSentences, bool includeWords, bool includeQuestionSentences, bool includeExclamationSentences, CancellationToken cancellationToken)
        {
            try
            {
                StatusLabel.Content = "Выполняется анализ текста...";
                await Task.Delay(1000, cancellationToken); // искусственная задержка для имитации длительного анализа

                string report = "";

                if (includeSentences)
                {
                    StatusLabel.Content = "Анализируем количество предложений";
                    await Task.Delay(1000, cancellationToken);
                    int sentenceCount = CountSentences(text);
                    report += $"Количество предложений: {sentenceCount}\n";
                }

                if (includeWords)
                {
                    StatusLabel.Content = "Анализируем количество слов";
                    await Task.Delay(1000, cancellationToken);
                    int wordCount = CountWords(text);
                    report += $"Количество слов: {wordCount}\n";
                }

                if (includeQuestionSentences)
                {
                    StatusLabel.Content = "Анализируем количество вопросительных предложений";
                    await Task.Delay(1000, cancellationToken);
                    int questionSentenceCount = CountQuestionSentences(text);
                    report += $"Количество вопросительных предложений: {questionSentenceCount}\n";
                }

                if (includeExclamationSentences)
                {
                    StatusLabel.Content = "Анализируем количество восклицательных предложений";
                    await Task.Delay(1000, cancellationToken);
                    int exclamationSentenceCount = CountExclamationSentences(text);
                    report += $"Количество восклицательных предложений: {exclamationSentenceCount}\n";
                }

                Dispatcher.Invoke(() =>
                {
                    if (!string.IsNullOrWhiteSpace(report))
                    {
                        if (ShowOnScreenRadioButton.IsChecked == true)
                        {
                            MessageBox.Show(report, "Отчет");
                        }
                        else if (SaveToFileRadioButton.IsChecked == true)
                        {
                            SaveReportToFile(report);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Выберите хотя бы один тип информации для включения в отчет", "Предупреждение");
                    }
                });
            }
            catch (OperationCanceledException)
            {
                // Обработка отмены операции
                MessageBox.Show("Анализ текста отменен", "Отмена");
            }
        }

        // методы для поиска предложений , слов, вопросительных предложений, восклицательных предложений
        private int CountSentences(string text)
        {
            return Regex.Matches(text, @"[.!?]").Count;
        }

        private int CountWords(string text)
        {
            return Regex.Matches(text, @"\b\w+\b").Count;
        }

        private int CountQuestionSentences(string text)
        {
            return Regex.Matches(text, @"\b\w+[?]").Count;
        }

        private int CountExclamationSentences(string text)
        {
            return Regex.Matches(text, @"\b\w+[!]").Count;
        }

        private void SaveReportToFile(string report)
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, report);
                MessageBox.Show("Отчет сохранен в файл", "Сохранение");
            }
        }
    }
}
