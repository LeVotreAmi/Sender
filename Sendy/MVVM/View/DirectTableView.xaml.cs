using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace Sendy.MVVM.View
{
    public partial class DirectTableView : UserControl
    {
        private DataTable DT { get; set; }
        public DirectTableView()
        {
            InitializeComponent();
        }

        private void DTVUC_Loaded(object sender, RoutedEventArgs e)
        {
            DT = DirectView.DT;
            if (DT != null)
            {
                DBView.DataContext = DT;
            }
            else
            {
                DBView.ItemsSource = "Error, base don't upload or empty.";
            }
        }

        private void SendMailBtn_Click(object sender, RoutedEventArgs e)
        {
            /**
             *  
             * Test outlook signatures with local images
             * 
            **/
            OfficeWorker.Sender send = new OfficeWorker.Sender();
            string UserProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            //Format src for input img in body
            //Regex rgx = new Regex("src[^\"]* \"[^\"]*\"");
            Regex rgx = new Regex("src[^\"]*\"[^/]*/");
            string str = "<img border=0 width=243 height=68 src = \"files/image001.png\" alt = \"cid:image002.png@01D4404C.C00DD090\" v: shapes = \"Рисунок_x0020_2\" > ";
            string newStr = rgx.Replace(str, "src=\"cid:");

            Outlook.Application OApp = new Outlook.Application();
            Outlook.MailItem mailItem = OApp.CreateItem(Outlook.OlItemType.olMailItem);
            mailItem.To = "levotreami@gmail.com";
            mailItem.Subject = "Test OL signatures";
            mailItem.BodyFormat = Outlook.OlBodyFormat.olFormatHTML;
            mailItem.Attachments.Add(UserProfile + "\\Downloads\\image001.png", Outlook.OlAttachmentType.olByValue, 0, "image001.png");
            mailItem.HTMLBody = "<img src=\"cid:image001.png\">"; //send.ReadSignature()[1];
            mailItem.Importance = Outlook.OlImportance.olImportanceNormal;
            mailItem.Send();
            OApp = null;
            mailItem = null;
        }

        private void DBView_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }
    }
}
