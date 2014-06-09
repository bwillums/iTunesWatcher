using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iTunesWatcher
{
    public class QueuedFile
    {
        public string FullPath { get; set; }
        public DateTime LastChecked { get; set; }
    }
}