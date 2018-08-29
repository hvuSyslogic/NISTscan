using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NISTscan
{
    public partial class BatchControl : Form
    {
        public BatchControl()
        {
            InitializeComponent();
        }

        public BatchControl(BatchSettings settings)
        {
            InitializeComponent();
        }

        public void Update(string message)
        {
            if (!this.IsDisposed)
            {
                tboStatus.AppendText(message);
            }
        }

        public void Cancel()
        {
            Update("\r\nBatch Cancelled!\r\n");
            this.Close();
        }

        public void Continue()
        {
            buttonContinue.Enabled = true;

            // Change to RED
            tboStatus.BackColor = Color.Red;
            tboStatus.ForeColor = Color.White;
        }

        private void buttonContinue_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
