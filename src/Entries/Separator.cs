using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComicsCatalog.Entries
{
    class Separator : IEntry
    {
        private string label;

        public Separator(string label)
        {
            this.label = label;
        }

        public string GetImage()
        {
            return null;
        }
        public string GetLabel()
        {
            return label;
        }
        public string GetSubLabel()
        {
            return null;
        }
    }
}
