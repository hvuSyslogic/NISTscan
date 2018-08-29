using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace NISTscan
{
    public partial class BatchConfig : Form
    {
        public BatchSettings settings;
        
        public BatchConfig()
        {
            InitializeComponent();
        }

        public BatchConfig(BatchSettings bs, string[] colorspaces, List<string> resolutions)
        {
            InitializeComponent();

            settings = bs;
            
            foreach (string cs in colorspaces)
            {
                lboColorSpace.Items.Add(cs);
            }
            foreach (string r in resolutions)
            {
                lboResolution.Items.Add(r);
            }
        }

        private void tboStartNum_Validating (object sender, CancelEventArgs e)
        {
            if (!Regex.IsMatch(tboStartNum.Text, @"\d+", RegexOptions.IgnoreCase))
            {
                MessageBox.Show(@"Filename does not match regular expression: \d+", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
            else if (Convert.ToInt32(tboStartNum.Text) < 0) 
            {
                MessageBox.Show("Starting value must be positive!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            // Validate, save and return to main form
            settings.validated = true;
            
            // Scan Count
            if (cbxScanCount.Checked)
            {
                if (cboStartEnd.SelectedIndex == 0)
                {
                    settings.append = false;
                    settings.prepend = true;
                }
                else
                {
                    settings.append = true;
                    settings.prepend = false;
                }
                settings.sequence = tboStartNum.Text; 
            }

            // Get selected resolutons/colorspaces (must select at least one each)
            if (lboColorSpace.SelectedIndices.Count < 1)
            {
                // Invalidate -- send message to user
                MessageBox.Show("Must select at least one colorspace!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                settings.validated = false;
            }
            else
            {
                settings.colorspaces = lboColorSpace.SelectedIndices;
            }

            if (lboResolution.SelectedIndices.Count < 1)
            {
                // Invalidate -- send message to user
                MessageBox.Show("Must select at least one resolution!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                settings.validated = false;
            }
            else
            {
                settings.resolutions = lboResolution.SelectedIndices;
            }

            if (settings.validated)
            {
                this.Close();
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            // Close and return to main form
            this.Close();
        }

        private void cbxScanCount_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxScanCount.Checked)
            {
                cboStartEnd.Enabled = true;
                tboStartNum.Enabled = true;

                // Default to start of file
                cboStartEnd.SelectedIndex = 0;
            }
            else
            {
                cboStartEnd.Enabled = false;
                tboStartNum.Enabled = false;
            }
        }
    }
}
