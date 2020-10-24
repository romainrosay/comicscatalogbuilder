using System;

namespace ComicsCatalog
{
    public class NewStatusEventArgs : EventArgs
    {
        public NewStatusEventArgs(string message) {
            Message = message;
        }
        public string Message { get; set; }
    }
}
