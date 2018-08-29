using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace NISTscan{
    public partial class AboutDialog : Form
    {
        public AboutDialog()
        {
            InitializeComponent();

            Assembly asm = Assembly.Load("Atalasoft.DotTwain");
            this.lblVersion.Text = "Version: " + asm.GetName().Version.ToString();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("www.atalasoft.com");
        }

        private void downloadHelpLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("www.atalasoft.com/support/dotimage/help/install");
        }

        private void demoGalleryLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("www.atalasoft.com/Support/Sample-Applications");
        }

        private void downloadDotImageLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("www.atalasoft.com/products/download/dotimage");
        }
    }
}

