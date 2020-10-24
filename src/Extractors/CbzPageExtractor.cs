using SharpCompress.Archives;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace ComicsCatalog
{
    class CbzPageExtractor : IPageExtractor
    {
        private static List<String> ext = new List<string> { "jpg", "png" };

        public string ExtractPage(string filePath, int pageNumber)
        {
            using (var archive = ArchiveFactory.Open(filePath))
            {
                var pages = archive.Entries.Where(s => ext.Contains(Path.GetExtension(s.Key).TrimStart('.').ToLowerInvariant()));
                var tmpFile = Path.GetTempFileName();
                pages.ElementAt(Math.Min(pageNumber, pages.Count())).WriteToFile(tmpFile);
                return tmpFile;
            }
        }
    }
}
