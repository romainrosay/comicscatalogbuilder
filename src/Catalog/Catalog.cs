using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using ComicsCatalog.Entries;
using System.Windows.Forms;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;

namespace ComicsCatalog
{
    class Catalog
    {
        private static List<String> ext = new List<string> { "cbr", "cbz", "pdf" };
        private static int catalogPageWidth = 1920;
        private static int catalogPageHeight = 2550;
        private static int maxItemsPerPage = 9;
        private static int catalogRowsCount = 3;
        private static int catalogColumnsCount = 3;
        private static int catalogHorizontalMarginInPixels = 50;
        private static int catalogVerticalMarginInPixels = 80;

        private List<IEntry> entries;
        private List<string> pages;
        
        private string currentOutputFilePath;

        public static string currentRootPath;

        #region NewStatusEvent
        public delegate void NewStatusEventHandler(object sender, NewStatusEventArgs e);
        public event NewStatusEventHandler NewStatusSent;
        protected virtual void SendNewStatus(NewStatusEventArgs e)
        {
            NewStatusEventHandler handler = NewStatusSent;
            handler?.Invoke(this, e);
        }
        #endregion

        public void BuildCatalog(string rootPath, string outputFilePath) {
            try
            {
                currentRootPath = rootPath;
                currentOutputFilePath = outputFilePath;

                if (!Directory.Exists(rootPath)) throw new Exception(rootPath + " isn't a valid directory");

                SendNewStatus(new NewStatusEventArgs("Catalog building started"));

                entries = new List<IEntry>();               
                ParseDirectory(rootPath);
                CreatePages();

                SendNewStatus(new NewStatusEventArgs("Catalog building ended"));
            }
            catch (Exception ex)
            {
                throw new Exception("Catalog building exception: " + ex.Message);
            }
        }

        private void ParseDirectory(string dirPath) { 
            if (!Directory.Exists(dirPath)) SendNewStatus(new NewStatusEventArgs("Skipping " + dirPath));
            List<string> dirs = new List<string>(Directory.EnumerateDirectories(dirPath));
            if (dirs.Count > 0)
            {
                foreach (var dir in dirs)
                {
                    SendNewStatus(new NewStatusEventArgs("Processing " + GetDirectoryNameFromPath(dir)));
                    ParseDirectory(dir);
                }
            }
            else {
                LoadComics(dirPath);
            }
        }

        private void LoadComics(string path) {
            try
            {
                //Listing comics
                var foundComics = Directory
                    .EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
                    .Where(s => ext.Contains(Path.GetExtension(s).TrimStart('.').ToLowerInvariant()));
                //Initialising comic
                Comic firstComic = new Comic(foundComics.First());
                entries.Add(firstComic);
            }
            catch (Exception ex) {
                SendNewStatus(new NewStatusEventArgs("Skipping comic: " + ex.Message));
            }
        }

        private void CreatePages() {
            pages = new List<string>();
            for (int i = 0; i < entries.Count; i = i + maxItemsPerPage)
            {
                List<IEntry> items = entries.Skip(i).Take(maxItemsPerPage).ToList();
                pages.Add(CreateAPage(items));
            }

            using (var archive = ZipArchive.Create())
            {
                int c = 0;
                foreach (string page in pages)
                {
                    archive.AddEntry(c + ".jpg", new FileInfo(page));
                    c++;
                }
                using (FileStream output = File.Create(currentOutputFilePath))
                {
                    archive.SaveTo(output);
                }
            }
            
        }

        private string CreateAPage(List<IEntry> entries) {
            using (var bmp = new Bitmap(catalogPageWidth, catalogPageHeight))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.Clear(Color.Black);
                    int currentColumn = 0;
                    int currentLine = 0;
                    float thumbWidth = (catalogPageWidth / catalogColumnsCount) - (1.5f *catalogHorizontalMarginInPixels);
                    float thumbHeight = (catalogPageHeight / catalogRowsCount) - (1.5f * catalogVerticalMarginInPixels);
                    foreach (var entry in entries) {
                        //Comic
                        if (entry.GetImage() != null)
                        {
                            using (var firstPageBmp = new Bitmap(entry.GetImage()))
                            {
                                //Draw cover
                                float x = (currentColumn * thumbWidth) + ((currentColumn + 1) * catalogHorizontalMarginInPixels);
                                float y = (currentLine * thumbHeight) + ((currentLine + 1) * catalogVerticalMarginInPixels);
                                g.DrawImage(firstPageBmp, x, y, thumbWidth, thumbHeight);

                                //Write text
                                g.DrawString(entry.GetSubLabel(), new Font("Tahoma", 14), Brushes.LightGray, x, y + thumbHeight + 5);
                                //Write text
                                g.DrawString(entry.GetLabel(), new Font("Tahoma", 18), Brushes.White, x, y + thumbHeight + 25);
                            }
                        }
                        //Separator
                        else {
                            int x = (int)(currentColumn * thumbWidth) + ((currentColumn + 1) * catalogHorizontalMarginInPixels);
                            int y = (int)(currentLine * thumbHeight) + ((currentLine + 1) * catalogVerticalMarginInPixels);
                            //Write text
                            Rectangle rect = new Rectangle(x, y, (int)thumbWidth, (int)thumbHeight);
                            TextFormatFlags flags = TextFormatFlags.HorizontalCenter |
                                TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak;
                            TextRenderer.DrawText(g, entry.GetLabel(), new Font("Tahoma", 30), rect, Color.White, flags);
                            g.DrawRectangle(Pens.LightGray, rect);
                        }

                        //Calculate next position
                        if (currentColumn + 1 >= catalogColumnsCount)
                        {
                            currentColumn = 0;
                            currentLine++;
                        }
                        else
                        {
                            currentColumn++;
                        }
                    }
                }    
                var memStream = new MemoryStream();
                var fileName = Path.GetTempFileName();
                bmp.Save(fileName, ImageFormat.Jpeg);
                return fileName;
            }
        }

        private void ClearTmpFiles() {
            foreach (var entry in entries)
            {
                //Comic
                if (entry.GetImage() != null) File.Delete(entry.GetImage());
            }

        }

        private string GetDirectoryNameFromPath(string dir) {
            return $"{dir.Substring(dir.LastIndexOf(Path.DirectorySeparatorChar) + 1)}";
        }
    }
}
