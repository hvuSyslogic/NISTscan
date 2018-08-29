using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace NISTscan
{
    public class BatchSettings
    {
        public bool append;
        public bool prepend;
        public string sequence;
        public ListBox.SelectedIndexCollection colorspaces;
        public ListBox.SelectedIndexCollection resolutions;
        public bool validated;

        public BatchSettings()
        {
            append = false;
            prepend = false;
            sequence = "-1";
            validated = false;
        }
    }
}
