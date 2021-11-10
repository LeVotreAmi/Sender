using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;

namespace Sendy.UserControls
{
    /// <summary>
    /// Логика взаимодействия для DirectTable.xaml
    /// </summary>
    public partial class DirectTable : Window
    {
        private string path { get; set; }
        public DirectTable(string path)
        {
            InitializeComponent();
            this.path = path;
        }

        private void LoadData(string path)
        {
            //OfficeWorker.Sender send = new OfficeWorker.Sender();
            //DBView.DataContext = send.DBGrab(path);
        }

        private void DirectWindow_Closed(object sender, EventArgs e)
        {
            Window mainWindow = Application.Current.MainWindow;
            mainWindow.Show();
        }

        private void DirectWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData(path);
        }
    }
}