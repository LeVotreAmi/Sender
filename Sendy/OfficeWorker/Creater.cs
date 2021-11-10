using System;
using Word = Microsoft.Office.Interop.Word;
using Excel = Microsoft.Office.Interop.Excel;
using System.Text.RegularExpressions;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;

namespace Sendy.OfficeWorker
{
    class Creater : Office
    {
        private Word.Application WApp = null;
        private Word.Document WDoc = null;
        private Excel.Application EApp = null;
        private Excel.Workbook EBook = null;
        private Excel.Worksheet ESheet = null;

        private string PathWord = MVVM.View.CreateView.PathWord;
        private string PathFolder = MVVM.View.CreateView.PathFolder;
        private string PathExcel = MVVM.View.CreateView.PathExcel;

        //private bool IsChangedDB = false;
        private int NumOfDuplicate = 0;
        private string path = "";

        private object missing = System.Reflection.Missing.Value;
        public Creater()
        {
            try
            {
                WApp = new Word.Application();
                EApp = new Excel.Application();
                OpenDocs();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message + "\nВозникла ошибка при открытии файлов.\n");
            }
        }

        private void OpenDocs()
        {
            try
            {
                WDoc = WApp.Documents.Open(PathWord, false, true, false, missing, missing, true, missing, missing, missing, missing, false, false, missing, true, missing);
                EBook = EApp.Workbooks.Open(PathExcel, false, false, missing, missing, missing, true, missing, missing, false, false, missing, false, missing, false);
                ESheet = EBook.Sheets[1];
            }
            catch
            {
                throw;
            }
        }
        public void CreateFilesFromTemplate(IProgress<double> progress)
        {
            bool hasContent = false;
            foreach (Excel.Worksheet sheet in EBook.Worksheets)
            {
                Excel.Range range = sheet.UsedRange;
                if (range != null)
                {
                    Excel.Range found = range.Cells.Find("*", missing, missing, missing, Excel.XlSearchOrder.xlByRows, Excel.XlSearchDirection.xlNext, missing, missing, missing);
                    if (found != null) hasContent = true;
                    CloseApp(found);
                    CloseApp(range);
                }
            }

            if (!hasContent)
            {
                release();
                throw new Exception("Excel file: no content");
            }

            int lastCol = 0;
            int lastRow = 0;
            string[] headers = null;
            string[] row = null;
            try
            {
                lastCol = ESheet.Cells.Find("*", System.Reflection.Missing.Value,
                    System.Reflection.Missing.Value, System.Reflection.Missing.Value,
                    Excel.XlSearchOrder.xlByColumns, Excel.XlSearchDirection.xlPrevious,
                    false, System.Reflection.Missing.Value, System.Reflection.Missing.Value).Column;

                lastRow = ESheet.Cells.Find("*", System.Reflection.Missing.Value,
                    System.Reflection.Missing.Value, System.Reflection.Missing.Value,
                    Excel.XlSearchOrder.xlByRows, Excel.XlSearchDirection.xlPrevious,
                    false, System.Reflection.Missing.Value, System.Reflection.Missing.Value).Row;

                headers = new string[lastCol - 1];

                for (int i = 1; i < lastCol; i++)
                {
                    headers[i - 1] = ESheet.Cells[1, i].Text.ToString();
                }

                DeleteInvalidCharacters(lastRow, lastCol);

                for(int i = 2; i <= lastRow; i++)
                {
                    string fileName = ESheet.Cells[i, lastCol].Text.ToString();
                    row = new string[lastCol - 1];
                    for (int j = 1; j < lastCol; j++)
                    {
                        row[j - 1] = ESheet.Cells[i, j].Text.ToString();
                    }
                    ReplaceAndSave(headers, row, fileName);
                    progress.Report(100.0 * i / lastRow);
                    row = null;
                }
            }
            catch (Exception e)
            {
                release();
                throw new Exception(e.Message + "\nFile's broken\n");
            }
            release();
        }

        private void ReplaceAndSave(string[] Keys, string[] Data, string FileName)
        {
            WDoc = WApp.Documents.Open(PathWord, false, true, false, missing, missing, true, missing, missing, missing, missing, false, false, missing, true, missing);
            path = PathFolder + "\\" + FileName;
            for (int i = 0; i < Keys.Length; i++)
            {
                CheckGender(Keys[i], Data[i]);
                WDoc.Content.Find.Execute(FindText: "{" + Keys[i] + "}", ReplaceWith: Data[i]);
            }

            string NPath = NFileName(path, NumOfDuplicate, Extensions.DOCX);

            WDoc.SaveAs2(NPath);
            WDoc.Close();
        }

        private void DeleteInvalidCharacters(int LastRow, int LastCol)
        {
            string[] fileNames = new string[LastRow];
            for(int i = 0; i < LastRow; i++)
            {
                fileNames[i] = ESheet.Cells[i + 1, LastCol].Text.ToString();
            }

            for(int i = 0; i < fileNames.Length; i++)
            {
                string[] matches = Regex.Matches(fileNames[i], @"(\s{1}|)\w(|\s{1}|)").Cast<Match>().Select(n => n.Value).ToArray();
                fileNames[i] = string.Join("", matches);
            }

            for(int i = 0; i < fileNames.Length; i++)
            {
                ESheet.Cells[i + 1, LastCol].Value = fileNames[i];
            }
            EBook.Save();
        }

        private void CheckGender(string Key, string Name)
        {
            if (Key.Equals("Name"))
            {
                string[] FMSName = Name.Split(' ');
                string middleName = FMSName[FMSName.Length - 1];
                if (middleName.Length > 3)
                {
                    string ending = middleName.Remove(0, middleName.Length - 3);
                    if (ending.Equals("вич"))
                    {
                        WDoc.Content.Find.Execute(FindText: "{%End%}", ReplaceWith: "ый");
                    }
                    else if (ending.Equals("вна"))
                    {
                        WDoc.Content.Find.Execute(FindText: "{%End%}", ReplaceWith: "ая");
                    }
                    else
                    {
                        bool result = false;
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            UserControls.CheckGender checkGender = new UserControls.CheckGender(Name);
                            checkGender.Show();
                        });
                        while (!UserControls.CheckGender.checkPress)
                        {
                            result = UserControls.CheckGender.male;
                        }
                        //MessageBoxResult result = MessageBox.Show("Программе не удалось определить пол для: " + Name + "\nМужчина?", "Не удалось определить пол", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result)
                        {
                            WDoc.Content.Find.Execute(FindText: "{%End%}", ReplaceWith: "ый");
                        }
                        else
                        {
                            WDoc.Content.Find.Execute(FindText: "{%End%}", ReplaceWith: "ая");
                        }
                    }
                }
                else
                {
                    //MessageBoxResult result = MessageBox.Show("Программе не удалось определить пол для: " + Name + "\nМужчина?", "Не удалось определить пол", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    bool result = false;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        UserControls.CheckGender checkGender = new UserControls.CheckGender(Name);
                        checkGender.Show();
                    });
                    while(!UserControls.CheckGender.checkPress)
                    {
                        result = UserControls.CheckGender.male;
                    }
                    if (result)
                    {
                        WDoc.Content.Find.Execute(FindText: "{%End%}", ReplaceWith: "ый");
                    }
                    else
                    {
                        WDoc.Content.Find.Execute(FindText: "{%End%}", ReplaceWith: "ая");
                    }
                }
            }
        }

        private void release()
        {
            if (WDoc != null)
            {
                try
                {
                    WDoc.Close(false);
                    CloseApp(WDoc);
                }
                catch
                {

                }
            }

            if (WApp != null)
            {
                try
                {
                    WApp.Quit();
                    CloseApp(WApp);
                }
                catch
                {

                }
            }

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
        }
    }
}