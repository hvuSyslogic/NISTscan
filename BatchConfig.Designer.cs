namespace NISTscan
{
    partial class BatchConfig
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BatchConfig));
            this.label1 = new System.Windows.Forms.Label();
            this.lboColorSpace = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lboResolution = new System.Windows.Forms.ListBox();
            this.cboStartEnd = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tboStartNum = new System.Windows.Forms.TextBox();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.cbxScanCount = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(334, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Choose the capture settings to iterate through in an automated batch:";
            // 
            // lboColorSpace
            // 
            this.lboColorSpace.FormattingEnabled = true;
            this.lboColorSpace.Location = new System.Drawing.Point(13, 107);
            this.lboColorSpace.Name = "lboColorSpace";
            this.lboColorSpace.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.lboColorSpace.Size = new System.Drawing.Size(120, 95);
            this.lboColorSpace.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Color Space";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(201, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Resolution";
            // 
            // lboResolution
            // 
            this.lboResolution.FormattingEnabled = true;
            this.lboResolution.Location = new System.Drawing.Point(201, 107);
            this.lboResolution.Name = "lboResolution";
            this.lboResolution.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.lboResolution.Size = new System.Drawing.Size(120, 95);
            this.lboResolution.TabIndex = 3;
            // 
            // cboStartEnd
            // 
            this.cboStartEnd.Enabled = false;
            this.cboStartEnd.FormattingEnabled = true;
            this.cboStartEnd.Items.AddRange(new object[] {
            "start",
            "end"});
            this.cboStartEnd.Location = new System.Drawing.Point(133, 39);
            this.cboStartEnd.Name = "cboStartEnd";
            this.cboStartEnd.Size = new System.Drawing.Size(96, 21);
            this.cboStartEnd.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(234, 43);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(123, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "of filename, starting with:";
            // 
            // tboStartNum
            // 
            this.tboStartNum.Enabled = false;
            this.tboStartNum.Location = new System.Drawing.Point(361, 41);
            this.tboStartNum.Name = "tboStartNum";
            this.tboStartNum.Size = new System.Drawing.Size(57, 20);
            this.tboStartNum.TabIndex = 9;
            this.tboStartNum.Text = "0001";
            this.tboStartNum.Validating += new System.ComponentModel.CancelEventHandler(this.tboStartNum_Validating);
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(93, 227);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 10;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(179, 227);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 11;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // cbxScanCount
            // 
            this.cbxScanCount.AutoSize = true;
            this.cbxScanCount.Location = new System.Drawing.Point(17, 42);
            this.cbxScanCount.Name = "cbxScanCount";
            this.cbxScanCount.Size = new System.Drawing.Size(113, 17);
            this.cbxScanCount.TabIndex = 12;
            this.cbxScanCount.Text = "Add scan count to";
            this.cbxScanCount.UseVisualStyleBackColor = true;
            this.cbxScanCount.CheckedChanged += new System.EventHandler(this.cbxScanCount_CheckedChanged);
            // 
            // BatchConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 262);
            this.Controls.Add(this.cbxScanCount);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.tboStartNum);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cboStartEnd);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lboResolution);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lboColorSpace);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BatchConfig";
            this.Text = "Batch Configuration";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lboColorSpace;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox lboResolution;
        private System.Windows.Forms.ComboBox cboStartEnd;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tboStartNum;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.CheckBox cbxScanCount;

    }
}