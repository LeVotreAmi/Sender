using System;
using System.Windows;
using System.Windows.Input;

namespace Sendy
{
    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();
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

        private void BtnMin(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}
