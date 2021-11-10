using System;
using System.Windows;
using System.Windows.Input;

namespace Sendy.UserControls
{
    public partial class CheckGender : Window
    {
        public static bool male { get; private set; }
        public static bool checkPress { get; private set; }
        public CheckGender(string name)
        {
            InitializeComponent();
            ContentLbl.Content = name + " мужчина?";
        }

        private void BtnTrue_Click(object sender, RoutedEventArgs e)
        {
            male = true;
            checkPress = !checkPress;
            Close();
        }

        private void BtnFalse_Click(object sender, RoutedEventArgs e)
        {
            male = false;
            checkPress = !checkPress;
            Close();
        }

        private void CheckWin_Closed(object sender, System.EventArgs e)
        {
            checkPress = !checkPress;
        }

        private void TitleBarMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void BtnClose(object sender, MouseButtonEventArgs e)
        {
            GC.Collect();
            Close();
        }
    }
}
