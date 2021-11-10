using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static Sendy.Theme.Images;
using static Sendy.OfficeWorker.Extensions;

namespace Sendy.MVVM.View
{
    /// <summary>
    /// Логика взаимодействия для CreateView.xaml
    /// </summary>
    public partial class CreateView : UserControl
    {
        public CreateView()
        {
            InitializeComponent();
        }
        public static string PathExcel { get; private set; }
        public static string PathWord { get; private set; }
        public static string PathFolder { get; private set; }

        UserControls.Complete complete = null;

        async private void fileDropPanel_Drop(object sender, DragEventArgs e)
        {
            DropBorder.BorderThickness = new Thickness(3, 3, 3, 3);

            string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);

            foreach (string path in paths)
            {
                string extension = Path.GetExtension(path);
                if (extension.Equals(XLSX))
                {
                    PathExcel = path;
                    excelIcon.Source = Image(true, XLSX);
                }
                else if(extension.Equals(DOCX))
                {
                    PathWord = path;
                    wordIcon.Source = Image(true, DOCX);
                }
                else if(Directory.Exists(path))
                {
                    PathFolder = path;
                    folderIcon.Source = Image(true, FOLDER);
                }
            }

            if (PathExcel != null && PathWord != null && PathFolder != null)
            {
                IProgress<double> progress = new Progress<double>(ReportProgress);
                createProgress.Visibility = Visibility.Visible;
                try
                {
                    await Task.Run(() =>
                    {
                        OfficeWorker.Creater creater = new OfficeWorker.Creater();
                        creater.CreateFilesFromTemplate(progress);
                    });
                    complete = new UserControls.Complete(Theme.MenuItems.Create);
                    complete.Show();
                }
                catch (Exception ex)
                {
                    excelIcon.Source = Image(false, XLSX);
                    wordIcon.Source = Image(false, DOCX);
                    folderIcon.Source = Image(false, FOLDER);
                    PathExcel = null;
                    PathWord = null;
                    PathFolder = null;
                    createProgress.Value = 0;
                    createProgress.Visibility = Visibility.Hidden;
                    MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    excelIcon.Source = Image(false, XLSX);
                    wordIcon.Source = Image(false, DOCX);
                    folderIcon.Source = Image(false, FOLDER);
                    PathExcel = null;
                    PathWord = null;
                    PathFolder = null;
                    createProgress.Value = 0;
                    createProgress.Visibility = Visibility.Hidden;
                }
            }
        }

        private void fileDropPanel_DragOver(object sender, DragEventArgs e)
        {
            DropBorder.BorderThickness = new Thickness(7, 7, 7, 7);
        }

        private void fileDropPanel_DragLeave(object sender, DragEventArgs e)
        {
            DropBorder.BorderThickness = new Thickness(3, 3, 3, 3);
        }

        private void ReportProgress(double value)
        {
            createProgress.Value = value;
        }
    }
}
