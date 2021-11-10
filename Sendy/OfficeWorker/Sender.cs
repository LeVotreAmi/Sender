using System;
using System.Data;
using System.IO;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using Outlook = Microsoft.Office.Interop.Outlook;
using Redemption;
namespace Sendy.OfficeWorker
{
    public class Sender : Office
    {
        private Excel.Application EApp = null;
        private Excel.Workbook EBook = null;
        private Excel.Worksheet ESheet = null;
        private Outlook.Application OApp = null;
        private readonly object Missing = System.Reflection.Missing.Value;

        public DataTable DBGrab(string path, IProgress<double> progress)
        {
            try
            {
                EApp = new Excel.Application();
                EBook = EApp.Workbooks.Open(path, false, false, Missing, Missing, Missing, true, Missing, Missing, false, false, Missing, false, Missing, false);
                ESheet = EBook.Sheets[1];
            }
            catch
            {
                Release();
                throw;
            }

            bool hasContent = false;
            foreach (Excel.Worksheet sheet in EBook.Worksheets)
            {
                Excel.Range range = sheet.UsedRange;
                if (range != null)
                {
                    Excel.Range found = range.Cells.Find("*", Missing, Missing, Missing, Excel.XlSearchOrder.xlByRows, Excel.XlSearchDirection.xlNext, Missing, Missing, Missing);
                    if (found != null)
                    {
                        hasContent = true;
                    }

                    CloseApp(found);
                    CloseApp(range);
                }
            }

            if (!hasContent)
            {
                Release();
                throw new Exception("Excel file: no content");
            }

            int lastCol = 0;
            int lastRow = 0;

            lastCol = ESheet.Cells.Find("*", System.Reflection.Missing.Value,
                    System.Reflection.Missing.Value, System.Reflection.Missing.Value,
                    Excel.XlSearchOrder.xlByColumns, Excel.XlSearchDirection.xlPrevious,
                    false, System.Reflection.Missing.Value, System.Reflection.Missing.Value).Column;

            lastRow = ESheet.Cells.Find("*", System.Reflection.Missing.Value,
                System.Reflection.Missing.Value, System.Reflection.Missing.Value,
                Excel.XlSearchOrder.xlByRows, Excel.XlSearchDirection.xlPrevious,
                false, System.Reflection.Missing.Value, System.Reflection.Missing.Value).Row;

            /*
            string[,] Data = new string[lastRow, lastCol];

            for (int i = 1; i <= lastRow; i++)
            {
                for(int j = 1; j <= lastCol; j++)
                {
                    Data[i - 1, j - 1] = ESheet.Cells[i, j].Text.ToString();
                }
            }
            */

            DataTable Data = new DataTable();
            for (int i = 1; i <= lastCol; i++)
            {
                Data.Columns.Add(ESheet.Cells[1, i].Text.ToString());
            }
            for (int i = 2; i <= lastRow; i++)
            {
                DataRow row = Data.NewRow();
                for (int j = 1; j <= lastCol; j++)
                {
                    row[j - 1] = ESheet.Cells[i, j].Text.ToString();
                }
                Data.Rows.Add(row);
                progress.Report(100 * i / lastRow);
            }
            Release();
            return Data;
        }

        public void SendMail(DataTable DT)
        {
            try
            {
                string[] signatures = ReadSignature();
                RDOSession session = new RDOSession();
                session.Logon();
                var signature = session.Signatures.Item("MASS");
                RDOMail Mail = session.GetDefaultFolder(rdoDefaultFolders.olFolderOutbox).Items.Add("IPM.Note");
                Mail.Subject = "test RDO";
                Mail.HTMLBody = "<div style='background: #ff0000' width='50' height='50'><p>Test</p></div>" + signature.HTMLBody;
                Mail.Recipients.Add("levotreami@gmail.com");
                Mail.Attachments.Add(@"C:\Users\levotreami\Desktop\qwe.docx");
                Mail.DownloadPictures = true;
                Mail.Save();
                Mail.Send();
                //OApp = new Outlook.Application();
                //Outlook.MailItem MailItem = OApp.Application.CreateItem(Outlook.OlItemType.olMailItem);
                //int count = DT.Rows.Count;
                //for (int i = 1; i <= count; i++)
                //{
                    //MailItem.To = DT.Rows[i]["Email"].ToString();
                //}
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentNullException("Failed connection to outlook.");
            }
        }

        //thx for signature: https://stackoverflow.com/a/6454131
        public string[] ReadSignature()
        {
            string appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Microsoft\\Signatures";
            DirectoryInfo diInfo = new DirectoryInfo(appDataDir);
            string[] signatures = null;

            if (diInfo.Exists)
            {
                FileInfo[] fiSignature = diInfo.GetFiles("*.htm");
                int count = fiSignature.Length;
                signatures = new string[count];
                if (count > 0)
                {
                    for(int i = 0; i < count; i++)
                    {
                        StreamReader sr = new StreamReader(fiSignature[i].FullName, Encoding.Default);
                        signatures[i] = sr.ReadToEnd();

                        if (!string.IsNullOrEmpty(signatures[i]))
                        {
                            string fileName = fiSignature[i].Name.Replace(fiSignature[i].Extension, string.Empty);
                            signatures[i] = signatures[i].Replace(fileName + "_files/", appDataDir + "/" + fileName + "_files/");
                        }
                    }
                }
            }
            return signatures;
        }

        private void Release()
        {
            if (EBook != null)
            {
                try
                {
                    EBook.Close();
                    CloseApp(EBook);
                }
                catch
                {

                }
            }

            if (EApp != null)
            {
                try
                {
                    EApp.Quit();
                    CloseApp(EApp);
                }
                catch
                {

                }
            }

            if (OApp != null)
            {
                try
                {
                    OApp.Quit();
                    CloseApp(OApp);
                }
                catch
                {

                }
            }
        }
    }
}
