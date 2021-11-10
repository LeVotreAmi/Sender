using System;
using System.Windows.Media.Imaging;

using static Sendy.OfficeWorker.Extensions;

namespace Sendy.Theme
{
    public class Images
    {
        public static BitmapImage Image(bool PathExist, string Extension)
        {
            if (PathExist)
            {
                switch (Extension)
                {
                    case XLSX:
                        return new BitmapImage(new Uri("/Resources/Images/excelIconOk.png", UriKind.Relative));
                    case DOCX:
                        return new BitmapImage(new Uri("/Resources/Images/wordIconOk.png", UriKind.Relative));
                    case FOLDER:
                        return new BitmapImage(new Uri("/Resources/Images/folderIconOk.png", UriKind.Relative));
                    default:
                        return new BitmapImage(new Uri("/Resources/Images/default.png", UriKind.Relative));
                }
            }
            else
            {
                switch (Extension)
                {
                    case XLSX:
                        return new BitmapImage(new Uri("/Resources/Images/excelIcon.png", UriKind.Relative));
                    case DOCX:
                        return new BitmapImage(new Uri("/Resources/Images/wordIcon.png", UriKind.Relative));
                    case FOLDER:
                        return new BitmapImage(new Uri("/Resources/Images/folderIcon.png", UriKind.Relative));
                    default:
                        return new BitmapImage(new Uri("/Resources/Images/default.png", UriKind.Relative));
                }
            }
        }
    }
}
