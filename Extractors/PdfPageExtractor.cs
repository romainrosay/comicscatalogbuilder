using Apitron.PDF.Rasterizer;
using Apitron.PDF.Rasterizer.Configuration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ComicsCatalog
{
    class PdfPageExtractor : IPageExtractor
    {
        private static List<String> ext = new List<string> { "jpg", "png" };

        public string ExtractPage(string filePath, int pageNumber)
        {
            // open and load the file
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                Document document = new Document(fs);
                RenderingSettings settings = new RenderingSettings();
                Page currentPage = document.Pages[0];
                using (Bitmap bitmap = currentPage.Render((int)currentPage.Width, (int)currentPage.Height, settings))
                {
                    var tmpFile = Path.GetTempFileName();
                    bitmap.Save(tmpFile, ImageFormat.Png);
                    return tmpFile;
                }
            }
        }
    }
}