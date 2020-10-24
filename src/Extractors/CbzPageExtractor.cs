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
                var orderedEntries =
                    from entry in archive.Entries
                    orderby entry.Key
                    select entry;
                
                int cpt = 0;
                foreach (var page in orderedEntries) {
                    if (Path.GetExtension(page.Key).ToLower() != ".jpg" && Path.GetExtension(page.Key).ToLower() != ".png") continue;
                    if (cpt == 0 || orderedEntries.Last() == page) {
                        var tmpFile = Path.GetTempFileName();
                        page.WriteToFile(tmpFile);
                        return tmpFile;
                    }
                    cpt++;
                }
                return null;
            }
        }
    }
}
