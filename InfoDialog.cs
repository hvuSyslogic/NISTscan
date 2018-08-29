using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace NISTscan
{
	/// <summary>
	/// Summary description for InfoDialog.
	/// </summary>
	public class InfoDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label lblInfo;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public InfoDialog(string message)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.lblInfo.Text = message;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.lblInfo = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// lblInfo
			// 
			this.lblInfo.Location = new System.Drawing.Point(11, 7);
			this.lblInfo.Name = "lblInfo";
			this.lblInfo.Size = new System.Drawing.Size(270, 94);
			this.lblInfo.TabIndex = 0;
			this.lblInfo.Text = "Working... Please wait.";
			this.lblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lblInfo.TextChanged += new System.EventHandler(this.lblInfo_TextChanged);
			// 
			// InfoDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 109);
			this.ControlBox = false;
			this.Controls.Add(this.lblInfo);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "InfoDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.TopMost = true;
			this.ResumeLayout(false);

		}
		#endregion

		private void lblInfo_TextChanged(object sender, System.EventArgs e)
		{
			this.Refresh();
			Application.DoEvents();
		}
	}
}

