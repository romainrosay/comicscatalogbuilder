using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComicsCatalog.Entries
{
    interface IEntry
    {
        string GetLabel();
        string GetSubLabel();
        string GetImage();
    }
}
