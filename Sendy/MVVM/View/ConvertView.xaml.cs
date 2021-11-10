using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WinForms = System.Windows.Forms;

namespace Sendy.MVVM.View
{
    /// <summary>
    /// Логика взаимодействия для ConvertView.xaml
    /// </summary>
    public partial class ConvertView : UserControl
    {
        public ConvertView()
        {
            InitializeComponent();
        }

        private string PathFolder { get; set; }

        private OfficeWorker.Converter converter = new OfficeWorker.Converter();
        UserControls.Complete complete = null;

        private async void fileDropPanel_Drop(object sender, DragEventArgs e)
        {
            DropBorder.BorderThickness = new Thickness(3, 3, 3, 3);
            string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);
            List<string> WordFiles = new List<string>();
            if (Regex.IsMatch(paths[0], @"\.(docx)"))
            {
                foreach (string file in paths)
                {
                    if (!Regex.IsMatch(file, @"(\$|\~)"))
                    {
                        WordFiles.Add(file);
                    }
                }
            }
            else if(Directory.Exists(paths[0]))
            {
                WordFiles.AddRange(Directory.GetFiles(paths[0]));
            }

            if (folderCheck.IsChecked == true)
            {
                try
                {
                    PathFolder = Path.GetDirectoryName(WordFiles.First()) + "\\";
                }
                catch
                {
                    MessageBox.Show("Для конвертации требуется дропнуть папку с файлами .docx или файл/файлы", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                using (WinForms.FolderBrowserDialog fbd = new WinForms.FolderBrowserDialog())
                {
                    if (fbd.ShowDialog() == WinForms.DialogResult.OK)
                    {
                        PathFolder = fbd.SelectedPath + "\\";
                    }
                }
            }

            if (PathFolder != null && WordFiles.Count >= 1)
            {
                IProgress<double> progress = new Progress<double>(ReportProgress);
                convertProgress.Visibility = Visibility.Visible;
                try
                {
                    await Task.Run(() =>
                    {
                        converter.Convert(WordFiles, PathFolder, progress);
                    });
                    complete = new UserControls.Complete(Theme.MenuItems.Convert);
                    complete.Show();
                }
                catch (Exception ex)
                {
                    PathFolder = null;
                    convertProgress.Value = 0;
                    convertProgress.Visibility = Visibility.Hidden;
                    MessageBox.Show(ex.Message, ex.Source);
                }
                finally
                {
                    convertProgress.Value = 0;
                    convertProgress.Visibility = Visibility.Hidden;
                    PathFolder = null;
                }
            }
            //timeLbl.Content = OfficeWorker.Converter.SpentTime;
        }

        private void ReportProgress(double value)
        {
            convertProgress.Value = value;
        }

        private void fileDropPanel_DragOver(object sender, DragEventArgs e)
        {
            DropBorder.BorderThickness = new Thickness(7, 7, 7, 7);
        }

        private void fileDropPanel_DragLeave(object sender, DragEventArgs e)
        {
            DropBorder.BorderThickness = new Thickness(3, 3, 3, 3);
        }
    }
}
