using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Word = Microsoft.Office.Interop.Word;
using Excel = Microsoft.Office.Interop.Excel;
using System.Text.RegularExpressions;
using System.IO;

namespace Sendy.OfficeWorker
{
    class Converter : Office
    {
        Word.Application WApp;
        Word.Document WDoc;
        Object missing = System.Reflection.Missing.Value;

        //public static string SpentTime { get; set; }
        //private string beforeTime { get; set; }
        //private string afterTime { get; set; }

        public void Convert(List<string> PathWord, string PathFolder, IProgress<double> progress)
        {
            try
            {
                //beforeTime = DateTime.Now.ToString("T");
                WApp = new Word.Application();
                WApp.Visible = false;
                int count = PathWord.Count;
                int i = 0;
                foreach (string file in PathWord)
                {
                    string FileName = Path.GetFileNameWithoutExtension(file);
                    WDoc = WApp.Documents.Open(file, false, true, false, missing, missing, true, missing, missing, missing, missing, false, false, missing, true, missing);
                    WDoc.ExportAsFixedFormat(PathFolder + FileName, Word.WdExportFormat.wdExportFormatPDF, false, Word.WdExportOptimizeFor.wdExportOptimizeForOnScreen, Word.WdExportRange.wdExportAllDocument, 1, 1, Word.WdExportItem.wdExportDocumentContent, false, false, Word.WdExportCreateBookmarks.wdExportCreateNoBookmarks, false, false, false, missing);
                    WDoc.Close();
                    CloseApp(WDoc);
                    ++i;
                    progress.Report(100 * i / PathWord.Count);
                }
                //afterTime = DateTime.Now.ToString("T");
                //SpentTime = DateTime.Parse(afterTime).Subtract(DateTime.Parse(beforeTime)).ToString();
            }
            catch
            {
                //afterTime = DateTime.Now.ToString("T");
                //SpentTime = DateTime.Parse(afterTime).Subtract(DateTime.Parse(beforeTime)).ToString();
                release();
                throw;
            }
            release();
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

        }
    }
}