using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;

using static Sendy.Theme.Images;
using static Sendy.OfficeWorker.Extensions;
using System;
using System.Windows.Input;
using System.Data;

namespace Sendy.MVVM.View
{
    /// <summary>
    /// Логика взаимодействия для DirectView.xaml
    /// </summary>



    public partial class DirectView : UserControl
    {
        public DirectView()
        {
            InitializeComponent();
        }

        //private UserControls.Complete complete = null;

        public static string PathExcel { get; private set; }
        public static string PathFolder { get; private set; }
        public static DataTable DT { get; set; }

        private async void FileDropPanel_Drop(object sender, DragEventArgs e)
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
                else if (Directory.Exists(path))
                {
                    PathFolder = path;
                    folderIcon.Source = Image(true, FOLDER);
                }
            }

            if (PathExcel != null && PathFolder != null)
            {
                IProgress<double> progress = new Progress<double>(ReportProgress);
                directProgress.Visibility = Visibility.Visible;
                try
                {
                    await Task.Run(() =>
                    {
                        OfficeWorker.Sender send = new OfficeWorker.Sender();
                        DT = send.DBGrab(PathExcel, progress);
                    });

                    DirectTableView dtV = new DirectTableView();
                    dtV.DBView.DataContext = DT;

                    ViewModel.DirectViewModel dVM = new ViewModel.DirectViewModel();
                    dVM.CurrentView = dVM.DirectTableVM;
                    directContent.Content = dVM.CurrentView;

                }
                catch (Exception ex)
                {
                    excelIcon.Source = Image(false, XLSX);
                    folderIcon.Source = Image(false, FOLDER);
                    PathExcel = null;
                    PathFolder = null;
                    directProgress.Visibility = Visibility.Hidden;
                    directProgress.Value = 0;
                    MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    excelIcon.Source = Image(false, XLSX);
                    folderIcon.Source = Image(false, FOLDER);
                    directProgress.Visibility = Visibility.Hidden;
                    directProgress.Value = 0;
                }
            }
        }

        private void ReportProgress(double value)
        {
            directProgress.Value = value;
        }

        private void FileDropPanel_DragOver(object sender, DragEventArgs e)
        {
            DropBorder.BorderThickness = new Thickness(7, 7, 7, 7);
        }

        private void FileDropPanel_DragLeave(object sender, DragEventArgs e)
        {
            DropBorder.BorderThickness = new Thickness(3, 3, 3, 3);
        }
    }
}
