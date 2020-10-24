using System;

namespace ComicsCatalog
{
    public interface IPageExtractor
    {
        string ExtractPage(string filePath, int pageNumber);
    }
}
