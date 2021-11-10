using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Sendy.UserControls
{
    public partial class Complete : Window
    {
        public Complete(Theme.MenuItems item)
        {
            InitializeComponent();
            switch(item)
            {
                case Theme.MenuItems.Create:
                    ContentBtnOk.Background = new SolidColorBrush(Theme.Colors.GREEN);
                    ContentLbl.Content = "Create done";
                    break;
                case Theme.MenuItems.Convert:
                    ContentBtnOk.Background = new SolidColorBrush(Theme.Colors.RED);
                    ContentLbl.Content = "Convert done";
                    break;
                case Theme.MenuItems.Direct:
                    ContentBtnOk.Background = new SolidColorBrush(Theme.Colors.YELLOW);
                    ContentLbl.Content = "Direct done";
                    break;
                default:
                    ContentBtnOk.Background = new SolidColorBrush(Theme.Colors.DEFAULT);
                    ContentLbl.Content = "Default";
                    break;
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TitleBarMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void BtnClose(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
    }
}
