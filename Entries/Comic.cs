using ComicsCatalog.Entries;
using System;
using System.Collections.Generic;
using System.IO;

namespace ComicsCatalog
{
    class Comic : IEntry
    {
        private string filePath;
        private string firstPageExtractedPath;
        private string name;

        IPageExtractor extractor;
        public Comic(string path) {
            filePath = path;
            name = new DirectoryInfo(System.IO.Path.GetDirectoryName(filePath)).Name;
            string ext = Path.GetExtension(filePath);
            ext = ext.Substring(1,1).ToUpper() + ext.Substring(2).ToLower();
            var type = Type.GetType("ComicsCatalog." + ext + "PageExtractor");
            if (type == null) throw new Exception("No page extractor for " + ext + " extension");
            extractor = (IPageExtractor)Activator.CreateInstance(type);
            ExtractFirstPage();
        }

        private string GetReadablePath() {
            string folder = Path.GetDirectoryName(filePath);
            folder = Path.GetDirectoryName(folder);
            string toRemove = Catalog.currentRootPath;
            if (!toRemove.EndsWith("\\")) toRemove += "\\";
            string result = folder.Replace(toRemove, "").Replace("\\", " | ");
            return result;
        }

        private string ExtractFirstPage() {
            firstPageExtractedPath = extractor.ExtractPage(filePath, 0);
            return firstPageExtractedPath;
        }

        public string GetLabel()
        {
            return name;
        }

        public string GetSubLabel()
        {
            return GetReadablePath();
        }

        public string GetImage()
        {
            return firstPageExtractedPath;
        }
    }
}
