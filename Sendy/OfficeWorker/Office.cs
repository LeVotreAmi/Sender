using System;
using System.IO;

namespace Sendy.OfficeWorker
{
    public abstract class Office
    {
        //public static string SpentTime { get; set; }
        //private string beforeTime { get; set; }
        //private string afterTime { get; set; }

        public void CloseApp(Object obj)
        {
            try
            {
                if (System.Runtime.InteropServices.Marshal.IsComObject(obj))
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(obj);
                }
            }
            catch
            {
            }
            obj = null;
        }

        public bool FileIsOpen(string Path)
        {
            FileStream file = null;

            try
            {
                file = File.Open(Path, FileMode.Open, FileAccess.Read, FileShare.None);
                if (file != null)
                {
                    file.Close();
                    file.Dispose();
                }
                return false;
            }
            catch (IOException)
            {
                if (file != null)
                {
                    file.Close();
                    file.Dispose();
                }
                return true;
            }
        }

        public static string NFileName(string Path, int NumberOfDuplicate, string Extension)
        {
            if (!File.Exists(Path
                                  + (NumberOfDuplicate > 0 ? " (" + NumberOfDuplicate.ToString() + ")" : "")
                                  + Extension))
            {
                if (NumberOfDuplicate >= 1)
                {
                    Path += " (" + NumberOfDuplicate.ToString() + ")";
                }
                return Path;
            }
            else
            {
                ++NumberOfDuplicate;
                return NFileName(Path, NumberOfDuplicate, Extension);
            }
        }
    }
}