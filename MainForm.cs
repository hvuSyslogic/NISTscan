using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Reflection;
using Atalasoft.Twain;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ImageMagick;


namespace NISTscan
{
    public class Form1 : System.Windows.Forms.Form
    {
        #region UI Controls

        private TabControl tabImages;
        private Panel panel1;
        private Label label1;
        private MenuStrip menuMain;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem acquireToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ComboBox cboPixelType;
        private Label label3;
        private ComboBox cboTransferMethod;
        private Label label2;
        private ComboBox cboDevice;
        private ComboBox cboResolution;
        private Label label5;
        private ComboBox cboBitDepth;
        private Label label4;
        private CheckBox chkHideProgress;
        private CheckBox chkHideInterface;
        private CheckBox chkKeepInterfaceOpen;
        private CheckBox chkModalAcquire;
        private ToolStripMenuItem acquireCustomInterfaceToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem2;
        private ComboBox cboFrameSize;
        private Label label6;
        private ToolStripMenuItem capabilityInformationToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;

        #endregion

        private bool _keepUIOpen = false;
        private InfoDialog _infoDialog = null;
        private string _fileDialogFilter = "";
        private Atalasoft.Twain.Acquisition acquisition;
        private Device device = null;
        private ToolStripMenuItem saveAcquireParametersToolStripMenuItem;
        private ToolStripMenuItem acquireFromParametersToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem3;
        private int _scanCount;
        private bool _updatingValues;
        private TextBox tboFilename1;
        private System.DirectoryServices.DirectorySearcher directorySearcher1;
        private Button buttonAcquire;
        private Button buttonPreview;
        private Label label8;
        private ComboBox cboFormat;
        private Label label_fn1;
        private Label label9;
        private CheckBox chkAppendFilename;
        private bool _preview;
        private PictureBox picBox;
        private Panel previewPanel;
        private Point mouseDown;
        private bool selecting;
        private Rectangle selection = new Rectangle();
        private Brush selectionBrush = new SolidBrush(Color.FromArgb(128, 72, 145, 220));
        private Pen selectionPen = new Pen(Color.GreenYellow);
        private Pen ROIPen = new Pen(Color.Blue);
        private Pen deletionPen = new Pen(Color.Red);
        private Label label_fn2;
        private TextBox tboFilename2;
        private Label label_fn14;
        private TextBox tboFilename14;
        private Label label_fn13;
        private TextBox tboFilename13;
        private Label label_fn12;
        private TextBox tboFilename12;
        private Label label_fn11;
        private TextBox tboFilename11;
        private Label label_fn10;
        private TextBox tboFilename10;
        private Label label_fn9;
        private TextBox tboFilename9;
        private Label label_fn8;
        private TextBox tboFilename8;
        private Label label_fn7;
        private TextBox tboFilename7;
        private Label label_fn6;
        private TextBox tboFilename6;
        private Label label_fn5;
        private TextBox tboFilename5;
        private Label label_fn4;
        private TextBox tboFilename4;
        private Label label_fn3;
        private TextBox tboFilename3;
        private List<Rectangle> ROIs = new List<Rectangle>();
        private List<char> ORIs = new List<char>();
        private string regex_string = "";
        private Button buttonBatchScan;
        private Button buttonBatchConfig;
        private bool strict_regex = false;
        private string regex_message;
        private List<string> resolutions = new List<string>();
        private string[] colorspaces = { "8-bit Grayscale", "16-bit Grayscale", "24-bit Color" };
        private string[] bitDepths = { "8", "16", "24" };
        private bool append_imageinfo = false;
        private bool autoconfig_batch = false;
        private string savepath = "";
        private bool _8bit = false;
        private bool _16bit = false;
        private bool _24bit = false;
        private List<TextBox> activeTBOs = new List<TextBox>();
        private Label label10;
        private Label label7;
        private int preview_resolution = 75; // 75 PPI
        private List<string> filesToDelete = new List<string>();
        private List<string> filesToCopy = new List<string>();
        private string scannerID = "S01";
        private bool movingROI = false;
        private Rectangle ROItomove;
        private int ROIindex;
        private Point ROIoffset = new Point();
        private bool ROIresizing = false;
        private Rectangle ROItoresize;
        private int _memoryCallbackImageSize = 0;
        private byte[] _memoryCallbackImage = new byte[0];
        private int _scanWidth;
        private int _scanHeight;
        private int _bytesPerRow;
        private int _bytesWritten;
        private List<byte> _memoryCallbackImage2 = new List<byte>();


        public BatchSettings batchSettings = new BatchSettings();


        public Form1()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.acquisition = new Acquisition();

            this.acquisition.AsynchronousException += new AsynchronousExceptionEventHandler(this.acquisition_AsynchronousException);
            this.acquisition.AcquireCanceled += new EventHandler(acquisition_AcquireCanceled);
            this.acquisition.AcquireFinished += new EventHandler(acquisition_AcquireFinished);
            this.acquisition.DeviceEvent += new DeviceEventHandler(acquisition_DeviceEvent);
            this.acquisition.FileTransfer += new FileTransferEventHandler(acquisition_FileTransfer);
            this.acquisition.ImageAcquired += new ImageAcquiredEventHandler(acquisition_ImageAcquired);
            this.acquisition.MemoryDataTransfer += new MemoryDataTransferEventHandler(acquisition_MemoryTransfer);

            ShowInfoDialog("Loading devices...");
            LoadDeviceNames();
            HideInfoDialog();

            // NIST
            InitializeControls();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.device != null && this.device.State > TwainState.Loaded)
                this.device.Close();

            this.acquisition.Dispose();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.device != null) this.device.Close();
                if (this.acquisition != null) this.acquisition.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.tabImages = new System.Windows.Forms.TabControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label10 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.buttonBatchScan = new System.Windows.Forms.Button();
            this.buttonBatchConfig = new System.Windows.Forms.Button();
            this.label_fn14 = new System.Windows.Forms.Label();
            this.tboFilename14 = new System.Windows.Forms.TextBox();
            this.label_fn13 = new System.Windows.Forms.Label();
            this.tboFilename13 = new System.Windows.Forms.TextBox();
            this.label_fn12 = new System.Windows.Forms.Label();
            this.tboFilename12 = new System.Windows.Forms.TextBox();
            this.label_fn11 = new System.Windows.Forms.Label();
            this.tboFilename11 = new System.Windows.Forms.TextBox();
            this.label_fn10 = new System.Windows.Forms.Label();
            this.tboFilename10 = new System.Windows.Forms.TextBox();
            this.label_fn9 = new System.Windows.Forms.Label();
            this.tboFilename9 = new System.Windows.Forms.TextBox();
            this.label_fn8 = new System.Windows.Forms.Label();
            this.tboFilename8 = new System.Windows.Forms.TextBox();
            this.label_fn7 = new System.Windows.Forms.Label();
            this.tboFilename7 = new System.Windows.Forms.TextBox();
            this.label_fn6 = new System.Windows.Forms.Label();
            this.tboFilename6 = new System.Windows.Forms.TextBox();
            this.label_fn5 = new System.Windows.Forms.Label();
            this.tboFilename5 = new System.Windows.Forms.TextBox();
            this.label_fn4 = new System.Windows.Forms.Label();
            this.tboFilename4 = new System.Windows.Forms.TextBox();
            this.label_fn3 = new System.Windows.Forms.Label();
            this.tboFilename3 = new System.Windows.Forms.TextBox();
            this.label_fn2 = new System.Windows.Forms.Label();
            this.tboFilename2 = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.chkAppendFilename = new System.Windows.Forms.CheckBox();
            this.buttonAcquire = new System.Windows.Forms.Button();
            this.buttonPreview = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.cboFormat = new System.Windows.Forms.ComboBox();
            this.label_fn1 = new System.Windows.Forms.Label();
            this.tboFilename1 = new System.Windows.Forms.TextBox();
            this.cboFrameSize = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.chkKeepInterfaceOpen = new System.Windows.Forms.CheckBox();
            this.chkModalAcquire = new System.Windows.Forms.CheckBox();
            this.chkHideProgress = new System.Windows.Forms.CheckBox();
            this.chkHideInterface = new System.Windows.Forms.CheckBox();
            this.cboResolution = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cboBitDepth = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cboPixelType = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cboTransferMethod = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cboDevice = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.menuMain = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.acquireToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.acquireCustomInterfaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveAcquireParametersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.acquireFromParametersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.capabilityInformationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.directorySearcher1 = new System.DirectoryServices.DirectorySearcher();
            this.picBox = new System.Windows.Forms.PictureBox();
            this.previewPanel = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.menuMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBox)).BeginInit();
            this.previewPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabImages
            // 
            this.tabImages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabImages.Enabled = false;
            this.tabImages.Location = new System.Drawing.Point(250, 24);
            this.tabImages.Name = "tabImages";
            this.tabImages.SelectedIndex = 0;
            this.tabImages.Size = new System.Drawing.Size(640, 884);
            this.tabImages.TabIndex = 0;
            this.tabImages.Visible = false;
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.buttonBatchScan);
            this.panel1.Controls.Add(this.buttonBatchConfig);
            this.panel1.Controls.Add(this.label_fn14);
            this.panel1.Controls.Add(this.tboFilename14);
            this.panel1.Controls.Add(this.label_fn13);
            this.panel1.Controls.Add(this.tboFilename13);
            this.panel1.Controls.Add(this.label_fn12);
            this.panel1.Controls.Add(this.tboFilename12);
            this.panel1.Controls.Add(this.label_fn11);
            this.panel1.Controls.Add(this.tboFilename11);
            this.panel1.Controls.Add(this.label_fn10);
            this.panel1.Controls.Add(this.tboFilename10);
            this.panel1.Controls.Add(this.label_fn9);
            this.panel1.Controls.Add(this.tboFilename9);
            this.panel1.Controls.Add(this.label_fn8);
            this.panel1.Controls.Add(this.tboFilename8);
            this.panel1.Controls.Add(this.label_fn7);
            this.panel1.Controls.Add(this.tboFilename7);
            this.panel1.Controls.Add(this.label_fn6);
            this.panel1.Controls.Add(this.tboFilename6);
            this.panel1.Controls.Add(this.label_fn5);
            this.panel1.Controls.Add(this.tboFilename5);
            this.panel1.Controls.Add(this.label_fn4);
            this.panel1.Controls.Add(this.tboFilename4);
            this.panel1.Controls.Add(this.label_fn3);
            this.panel1.Controls.Add(this.tboFilename3);
            this.panel1.Controls.Add(this.label_fn2);
            this.panel1.Controls.Add(this.tboFilename2);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.chkAppendFilename);
            this.panel1.Controls.Add(this.buttonAcquire);
            this.panel1.Controls.Add(this.buttonPreview);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.cboFormat);
            this.panel1.Controls.Add(this.label_fn1);
            this.panel1.Controls.Add(this.tboFilename1);
            this.panel1.Controls.Add(this.cboFrameSize);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.chkKeepInterfaceOpen);
            this.panel1.Controls.Add(this.chkModalAcquire);
            this.panel1.Controls.Add(this.chkHideProgress);
            this.panel1.Controls.Add(this.chkHideInterface);
            this.panel1.Controls.Add(this.cboResolution);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.cboBitDepth);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.cboPixelType);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.cboTransferMethod);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.cboDevice);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(250, 884);
            this.panel1.TabIndex = 1;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Enabled = false;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.Color.Red;
            this.label10.Location = new System.Drawing.Point(12, 290);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(67, 13);
            this.label10.TabIndex = 56;
            this.label10.Text = "[CUSTOM]";
            this.label10.Visible = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Enabled = false;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(12, 275);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(131, 13);
            this.label7.TabIndex = 55;
            this.label7.Text = "Required File Pattern:";
            this.label7.Visible = false;
            // 
            // buttonBatchScan
            // 
            this.buttonBatchScan.Enabled = false;
            this.buttonBatchScan.Location = new System.Drawing.Point(135, 185);
            this.buttonBatchScan.Name = "buttonBatchScan";
            this.buttonBatchScan.Size = new System.Drawing.Size(100, 25);
            this.buttonBatchScan.TabIndex = 54;
            this.buttonBatchScan.Text = "Batch Scan";
            this.buttonBatchScan.UseVisualStyleBackColor = true;
            this.buttonBatchScan.Click += new System.EventHandler(this.buttonBatchScan_Click);
            // 
            // buttonBatchConfig
            // 
            this.buttonBatchConfig.Location = new System.Drawing.Point(15, 185);
            this.buttonBatchConfig.Name = "buttonBatchConfig";
            this.buttonBatchConfig.Size = new System.Drawing.Size(100, 25);
            this.buttonBatchConfig.TabIndex = 53;
            this.buttonBatchConfig.Text = "Batch Config";
            this.buttonBatchConfig.UseVisualStyleBackColor = true;
            this.buttonBatchConfig.Click += new System.EventHandler(this.buttonBatchConfig_Click);
            // 
            // label_fn14
            // 
            this.label_fn14.AutoSize = true;
            this.label_fn14.Enabled = false;
            this.label_fn14.Location = new System.Drawing.Point(12, 840);
            this.label_fn14.Name = "label_fn14";
            this.label_fn14.Size = new System.Drawing.Size(82, 13);
            this.label_fn14.TabIndex = 52;
            this.label_fn14.Text = "FILENAME #14";
            this.label_fn14.Visible = false;
            // 
            // tboFilename14
            // 
            this.tboFilename14.Enabled = false;
            this.tboFilename14.Location = new System.Drawing.Point(15, 857);
            this.tboFilename14.Name = "tboFilename14";
            this.tboFilename14.Size = new System.Drawing.Size(162, 20);
            this.tboFilename14.TabIndex = 51;
            this.tboFilename14.Visible = false;
            this.tboFilename14.TextChanged += new System.EventHandler(this.tboFilename14_TextChanged);
            // 
            // label_fn13
            // 
            this.label_fn13.AutoSize = true;
            this.label_fn13.Enabled = false;
            this.label_fn13.Location = new System.Drawing.Point(12, 800);
            this.label_fn13.Name = "label_fn13";
            this.label_fn13.Size = new System.Drawing.Size(82, 13);
            this.label_fn13.TabIndex = 50;
            this.label_fn13.Text = "FILENAME #13";
            this.label_fn13.Visible = false;
            // 
            // tboFilename13
            // 
            this.tboFilename13.Enabled = false;
            this.tboFilename13.Location = new System.Drawing.Point(15, 817);
            this.tboFilename13.Name = "tboFilename13";
            this.tboFilename13.Size = new System.Drawing.Size(162, 20);
            this.tboFilename13.TabIndex = 49;
            this.tboFilename13.Visible = false;
            this.tboFilename13.TextChanged += new System.EventHandler(this.tboFilename13_TextChanged);
            // 
            // label_fn12
            // 
            this.label_fn12.AutoSize = true;
            this.label_fn12.Enabled = false;
            this.label_fn12.Location = new System.Drawing.Point(12, 760);
            this.label_fn12.Name = "label_fn12";
            this.label_fn12.Size = new System.Drawing.Size(82, 13);
            this.label_fn12.TabIndex = 48;
            this.label_fn12.Text = "FILENAME #12";
            this.label_fn12.Visible = false;
            // 
            // tboFilename12
            // 
            this.tboFilename12.Enabled = false;
            this.tboFilename12.Location = new System.Drawing.Point(15, 777);
            this.tboFilename12.Name = "tboFilename12";
            this.tboFilename12.Size = new System.Drawing.Size(162, 20);
            this.tboFilename12.TabIndex = 47;
            this.tboFilename12.Visible = false;
            this.tboFilename12.TextChanged += new System.EventHandler(this.tboFilename12_TextChanged);
            // 
            // label_fn11
            // 
            this.label_fn11.AutoSize = true;
            this.label_fn11.Enabled = false;
            this.label_fn11.Location = new System.Drawing.Point(12, 720);
            this.label_fn11.Name = "label_fn11";
            this.label_fn11.Size = new System.Drawing.Size(82, 13);
            this.label_fn11.TabIndex = 46;
            this.label_fn11.Text = "FILENAME #11";
            this.label_fn11.Visible = false;
            // 
            // tboFilename11
            // 
            this.tboFilename11.Enabled = false;
            this.tboFilename11.Location = new System.Drawing.Point(15, 737);
            this.tboFilename11.Name = "tboFilename11";
            this.tboFilename11.Size = new System.Drawing.Size(162, 20);
            this.tboFilename11.TabIndex = 45;
            this.tboFilename11.Visible = false;
            this.tboFilename11.TextChanged += new System.EventHandler(this.tboFilename11_TextChanged);
            // 
            // label_fn10
            // 
            this.label_fn10.AutoSize = true;
            this.label_fn10.Enabled = false;
            this.label_fn10.Location = new System.Drawing.Point(12, 680);
            this.label_fn10.Name = "label_fn10";
            this.label_fn10.Size = new System.Drawing.Size(82, 13);
            this.label_fn10.TabIndex = 44;
            this.label_fn10.Text = "FILENAME #10";
            this.label_fn10.Visible = false;
            // 
            // tboFilename10
            // 
            this.tboFilename10.Enabled = false;
            this.tboFilename10.Location = new System.Drawing.Point(15, 697);
            this.tboFilename10.Name = "tboFilename10";
            this.tboFilename10.Size = new System.Drawing.Size(162, 20);
            this.tboFilename10.TabIndex = 43;
            this.tboFilename10.Visible = false;
            this.tboFilename10.TextChanged += new System.EventHandler(this.tboFilename10_TextChanged);
            // 
            // label_fn9
            // 
            this.label_fn9.AutoSize = true;
            this.label_fn9.Enabled = false;
            this.label_fn9.Location = new System.Drawing.Point(12, 640);
            this.label_fn9.Name = "label_fn9";
            this.label_fn9.Size = new System.Drawing.Size(76, 13);
            this.label_fn9.TabIndex = 42;
            this.label_fn9.Text = "FILENAME #9";
            this.label_fn9.Visible = false;
            // 
            // tboFilename9
            // 
            this.tboFilename9.Enabled = false;
            this.tboFilename9.Location = new System.Drawing.Point(15, 657);
            this.tboFilename9.Name = "tboFilename9";
            this.tboFilename9.Size = new System.Drawing.Size(162, 20);
            this.tboFilename9.TabIndex = 41;
            this.tboFilename9.Visible = false;
            this.tboFilename9.TextChanged += new System.EventHandler(this.tboFilename9_TextChanged);
            // 
            // label_fn8
            // 
            this.label_fn8.AutoSize = true;
            this.label_fn8.Enabled = false;
            this.label_fn8.Location = new System.Drawing.Point(12, 600);
            this.label_fn8.Name = "label_fn8";
            this.label_fn8.Size = new System.Drawing.Size(76, 13);
            this.label_fn8.TabIndex = 40;
            this.label_fn8.Text = "FILENAME #8";
            this.label_fn8.Visible = false;
            // 
            // tboFilename8
            // 
            this.tboFilename8.Enabled = false;
            this.tboFilename8.Location = new System.Drawing.Point(15, 617);
            this.tboFilename8.Name = "tboFilename8";
            this.tboFilename8.Size = new System.Drawing.Size(162, 20);
            this.tboFilename8.TabIndex = 39;
            this.tboFilename8.Visible = false;
            this.tboFilename8.TextChanged += new System.EventHandler(this.tboFilename8_TextChanged);
            // 
            // label_fn7
            // 
            this.label_fn7.AutoSize = true;
            this.label_fn7.Enabled = false;
            this.label_fn7.Location = new System.Drawing.Point(12, 560);
            this.label_fn7.Name = "label_fn7";
            this.label_fn7.Size = new System.Drawing.Size(76, 13);
            this.label_fn7.TabIndex = 38;
            this.label_fn7.Text = "FILENAME #7";
            this.label_fn7.Visible = false;
            // 
            // tboFilename7
            // 
            this.tboFilename7.Enabled = false;
            this.tboFilename7.Location = new System.Drawing.Point(15, 577);
            this.tboFilename7.Name = "tboFilename7";
            this.tboFilename7.Size = new System.Drawing.Size(162, 20);
            this.tboFilename7.TabIndex = 37;
            this.tboFilename7.Visible = false;
            this.tboFilename7.TextChanged += new System.EventHandler(this.tboFilename7_TextChanged);
            // 
            // label_fn6
            // 
            this.label_fn6.AutoSize = true;
            this.label_fn6.Enabled = false;
            this.label_fn6.Location = new System.Drawing.Point(12, 520);
            this.label_fn6.Name = "label_fn6";
            this.label_fn6.Size = new System.Drawing.Size(76, 13);
            this.label_fn6.TabIndex = 36;
            this.label_fn6.Text = "FILENAME #6";
            this.label_fn6.Visible = false;
            // 
            // tboFilename6
            // 
            this.tboFilename6.Enabled = false;
            this.tboFilename6.Location = new System.Drawing.Point(15, 537);
            this.tboFilename6.Name = "tboFilename6";
            this.tboFilename6.Size = new System.Drawing.Size(162, 20);
            this.tboFilename6.TabIndex = 35;
            this.tboFilename6.Visible = false;
            this.tboFilename6.TextChanged += new System.EventHandler(this.tboFilename6_TextChanged);
            // 
            // label_fn5
            // 
            this.label_fn5.AutoSize = true;
            this.label_fn5.Enabled = false;
            this.label_fn5.Location = new System.Drawing.Point(12, 480);
            this.label_fn5.Name = "label_fn5";
            this.label_fn5.Size = new System.Drawing.Size(76, 13);
            this.label_fn5.TabIndex = 34;
            this.label_fn5.Text = "FILENAME #5";
            this.label_fn5.Visible = false;
            // 
            // tboFilename5
            // 
            this.tboFilename5.Enabled = false;
            this.tboFilename5.Location = new System.Drawing.Point(15, 497);
            this.tboFilename5.Name = "tboFilename5";
            this.tboFilename5.Size = new System.Drawing.Size(162, 20);
            this.tboFilename5.TabIndex = 33;
            this.tboFilename5.Visible = false;
            this.tboFilename5.TextChanged += new System.EventHandler(this.tboFilename5_TextChanged);
            // 
            // label_fn4
            // 
            this.label_fn4.AutoSize = true;
            this.label_fn4.Enabled = false;
            this.label_fn4.Location = new System.Drawing.Point(12, 440);
            this.label_fn4.Name = "label_fn4";
            this.label_fn4.Size = new System.Drawing.Size(76, 13);
            this.label_fn4.TabIndex = 32;
            this.label_fn4.Text = "FILENAME #4";
            this.label_fn4.Visible = false;
            // 
            // tboFilename4
            // 
            this.tboFilename4.Enabled = false;
            this.tboFilename4.Location = new System.Drawing.Point(15, 457);
            this.tboFilename4.Name = "tboFilename4";
            this.tboFilename4.Size = new System.Drawing.Size(162, 20);
            this.tboFilename4.TabIndex = 31;
            this.tboFilename4.Visible = false;
            this.tboFilename4.TextChanged += new System.EventHandler(this.tboFilename4_TextChanged);
            // 
            // label_fn3
            // 
            this.label_fn3.AutoSize = true;
            this.label_fn3.Enabled = false;
            this.label_fn3.Location = new System.Drawing.Point(12, 400);
            this.label_fn3.Name = "label_fn3";
            this.label_fn3.Size = new System.Drawing.Size(76, 13);
            this.label_fn3.TabIndex = 30;
            this.label_fn3.Text = "FILENAME #3";
            this.label_fn3.Visible = false;
            // 
            // tboFilename3
            // 
            this.tboFilename3.Enabled = false;
            this.tboFilename3.Location = new System.Drawing.Point(15, 417);
            this.tboFilename3.Name = "tboFilename3";
            this.tboFilename3.Size = new System.Drawing.Size(162, 20);
            this.tboFilename3.TabIndex = 29;
            this.tboFilename3.Visible = false;
            this.tboFilename3.TextChanged += new System.EventHandler(this.tboFilename3_TextChanged);
            // 
            // label_fn2
            // 
            this.label_fn2.AutoSize = true;
            this.label_fn2.Enabled = false;
            this.label_fn2.Location = new System.Drawing.Point(12, 360);
            this.label_fn2.Name = "label_fn2";
            this.label_fn2.Size = new System.Drawing.Size(76, 13);
            this.label_fn2.TabIndex = 28;
            this.label_fn2.Text = "FILENAME #2";
            this.label_fn2.Visible = false;
            // 
            // tboFilename2
            // 
            this.tboFilename2.Enabled = false;
            this.tboFilename2.Location = new System.Drawing.Point(15, 377);
            this.tboFilename2.Name = "tboFilename2";
            this.tboFilename2.Size = new System.Drawing.Size(162, 20);
            this.tboFilename2.TabIndex = 27;
            this.tboFilename2.Visible = false;
            this.tboFilename2.TextChanged += new System.EventHandler(this.tboFilename2_TextChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(33, 250);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(215, 13);
            this.label9.TabIndex = 19;
            this.label9.Text = "Ex: [FILENAME]_500PPI_8BPP_1CH";
            // 
            // chkAppendFilename
            // 
            this.chkAppendFilename.AutoSize = true;
            this.chkAppendFilename.Location = new System.Drawing.Point(15, 230);
            this.chkAppendFilename.Name = "chkAppendFilename";
            this.chkAppendFilename.Size = new System.Drawing.Size(205, 17);
            this.chkAppendFilename.TabIndex = 25;
            this.chkAppendFilename.Text = "Append image attributes to filename(s)";
            this.chkAppendFilename.UseVisualStyleBackColor = true;
            this.chkAppendFilename.CheckedChanged += new System.EventHandler(this.chkAppendFilename_CheckedChanged);
            // 
            // buttonAcquire
            // 
            this.buttonAcquire.Enabled = false;
            this.buttonAcquire.Location = new System.Drawing.Point(135, 150);
            this.buttonAcquire.Name = "buttonAcquire";
            this.buttonAcquire.Size = new System.Drawing.Size(100, 25);
            this.buttonAcquire.TabIndex = 24;
            this.buttonAcquire.Text = "Single Scan";
            this.buttonAcquire.UseVisualStyleBackColor = true;
            this.buttonAcquire.Click += new System.EventHandler(this.buttonAcquire_Click);
            // 
            // buttonPreview
            // 
            this.buttonPreview.Location = new System.Drawing.Point(15, 150);
            this.buttonPreview.Name = "buttonPreview";
            this.buttonPreview.Size = new System.Drawing.Size(100, 25);
            this.buttonPreview.TabIndex = 23;
            this.buttonPreview.Text = "Preview";
            this.buttonPreview.UseVisualStyleBackColor = true;
            this.buttonPreview.Click += new System.EventHandler(this.buttonPreview_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(182, 320);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(52, 13);
            this.label8.TabIndex = 22;
            this.label8.Text = "FORMAT";
            // 
            // cboFormat
            // 
            this.cboFormat.FormattingEnabled = true;
            this.cboFormat.Items.AddRange(new object[] {
            ".BMP",
            ".PNG",
            ".TIFF"});
            this.cboFormat.Location = new System.Drawing.Point(185, 337);
            this.cboFormat.Name = "cboFormat";
            this.cboFormat.Size = new System.Drawing.Size(50, 21);
            this.cboFormat.TabIndex = 21;
            this.cboFormat.Text = ".BMP";
            // 
            // label_fn1
            // 
            this.label_fn1.AutoSize = true;
            this.label_fn1.Location = new System.Drawing.Point(12, 320);
            this.label_fn1.Name = "label_fn1";
            this.label_fn1.Size = new System.Drawing.Size(60, 13);
            this.label_fn1.TabIndex = 20;
            this.label_fn1.Text = "FILENAME";
            // 
            // tboFilename1
            // 
            this.tboFilename1.Location = new System.Drawing.Point(15, 337);
            this.tboFilename1.Name = "tboFilename1";
            this.tboFilename1.Size = new System.Drawing.Size(162, 20);
            this.tboFilename1.TabIndex = 26;
            this.tboFilename1.TextChanged += new System.EventHandler(this.tboFilename1_TextChanged);
            // 
            // cboFrameSize
            // 
            this.cboFrameSize.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboFrameSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFrameSize.Enabled = false;
            this.cboFrameSize.FormattingEnabled = true;
            this.cboFrameSize.Location = new System.Drawing.Point(15, 1075);
            this.cboFrameSize.Name = "cboFrameSize";
            this.cboFrameSize.Size = new System.Drawing.Size(0, 21);
            this.cboFrameSize.TabIndex = 18;
            this.cboFrameSize.Visible = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Enabled = false;
            this.label6.Location = new System.Drawing.Point(12, 1064);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(62, 13);
            this.label6.TabIndex = 17;
            this.label6.Text = "Frame Size:";
            this.label6.Visible = false;
            // 
            // chkKeepInterfaceOpen
            // 
            this.chkKeepInterfaceOpen.AutoSize = true;
            this.chkKeepInterfaceOpen.Enabled = false;
            this.chkKeepInterfaceOpen.Location = new System.Drawing.Point(15, 1147);
            this.chkKeepInterfaceOpen.Name = "chkKeepInterfaceOpen";
            this.chkKeepInterfaceOpen.Size = new System.Drawing.Size(125, 17);
            this.chkKeepInterfaceOpen.TabIndex = 16;
            this.chkKeepInterfaceOpen.Text = "Keep Interface Open";
            this.chkKeepInterfaceOpen.UseVisualStyleBackColor = true;
            this.chkKeepInterfaceOpen.Visible = false;
            this.chkKeepInterfaceOpen.CheckedChanged += new System.EventHandler(this.chkKeepInterfaceOpen_CheckedChanged);
            // 
            // chkModalAcquire
            // 
            this.chkModalAcquire.AutoSize = true;
            this.chkModalAcquire.Enabled = false;
            this.chkModalAcquire.Location = new System.Drawing.Point(15, 1131);
            this.chkModalAcquire.Name = "chkModalAcquire";
            this.chkModalAcquire.Size = new System.Drawing.Size(94, 17);
            this.chkModalAcquire.TabIndex = 15;
            this.chkModalAcquire.Text = "Modal Acquire";
            this.chkModalAcquire.UseVisualStyleBackColor = true;
            this.chkModalAcquire.Visible = false;
            // 
            // chkHideProgress
            // 
            this.chkHideProgress.AutoSize = true;
            this.chkHideProgress.Enabled = false;
            this.chkHideProgress.Location = new System.Drawing.Point(15, 1115);
            this.chkHideProgress.Name = "chkHideProgress";
            this.chkHideProgress.Size = new System.Drawing.Size(125, 17);
            this.chkHideProgress.TabIndex = 14;
            this.chkHideProgress.Text = "Hide Progress Dialog";
            this.chkHideProgress.UseVisualStyleBackColor = true;
            this.chkHideProgress.Visible = false;
            // 
            // chkHideInterface
            // 
            this.chkHideInterface.AutoSize = true;
            this.chkHideInterface.Enabled = false;
            this.chkHideInterface.Location = new System.Drawing.Point(15, 1099);
            this.chkHideInterface.Name = "chkHideInterface";
            this.chkHideInterface.Size = new System.Drawing.Size(130, 17);
            this.chkHideInterface.TabIndex = 13;
            this.chkHideInterface.Text = "Hide Device Interface";
            this.chkHideInterface.UseVisualStyleBackColor = true;
            this.chkHideInterface.Visible = false;
            this.chkHideInterface.CheckedChanged += new System.EventHandler(this.chkHideInterface_CheckedChanged);
            // 
            // cboResolution
            // 
            this.cboResolution.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboResolution.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboResolution.FormattingEnabled = true;
            this.cboResolution.Location = new System.Drawing.Point(15, 115);
            this.cboResolution.Name = "cboResolution";
            this.cboResolution.Size = new System.Drawing.Size(196, 21);
            this.cboResolution.TabIndex = 12;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 100);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Resolution:";
            // 
            // cboBitDepth
            // 
            this.cboBitDepth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboBitDepth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBitDepth.Enabled = false;
            this.cboBitDepth.FormattingEnabled = true;
            this.cboBitDepth.Location = new System.Drawing.Point(15, 1007);
            this.cboBitDepth.Name = "cboBitDepth";
            this.cboBitDepth.Size = new System.Drawing.Size(0, 21);
            this.cboBitDepth.TabIndex = 10;
            this.cboBitDepth.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Enabled = false;
            this.label4.Location = new System.Drawing.Point(12, 995);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Bit Depth:";
            this.label4.Visible = false;
            // 
            // cboPixelType
            // 
            this.cboPixelType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboPixelType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPixelType.FormattingEnabled = true;
            this.cboPixelType.Location = new System.Drawing.Point(15, 70);
            this.cboPixelType.Name = "cboPixelType";
            this.cboPixelType.Size = new System.Drawing.Size(196, 21);
            this.cboPixelType.TabIndex = 8;
            this.cboPixelType.SelectedIndexChanged += new System.EventHandler(this.cboPixelType_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Color Space:";
            // 
            // cboTransferMethod
            // 
            this.cboTransferMethod.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboTransferMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTransferMethod.Enabled = false;
            this.cboTransferMethod.FormattingEnabled = true;
            this.cboTransferMethod.Location = new System.Drawing.Point(15, 1042);
            this.cboTransferMethod.Name = "cboTransferMethod";
            this.cboTransferMethod.Size = new System.Drawing.Size(0, 21);
            this.cboTransferMethod.TabIndex = 6;
            this.cboTransferMethod.Visible = false;
            this.cboTransferMethod.SelectedIndexChanged += new System.EventHandler(this.cboTransferMethod_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Enabled = false;
            this.label2.Location = new System.Drawing.Point(12, 1031);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Transfer Method:";
            this.label2.Visible = false;
            // 
            // cboDevice
            // 
            this.cboDevice.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDevice.FormattingEnabled = true;
            this.cboDevice.Items.AddRange(new object[] {
            "Select Source..."});
            this.cboDevice.Location = new System.Drawing.Point(15, 25);
            this.cboDevice.Name = "cboDevice";
            this.cboDevice.Size = new System.Drawing.Size(196, 21);
            this.cboDevice.TabIndex = 4;
            this.cboDevice.SelectedIndexChanged += new System.EventHandler(this.cboDevice_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Device:";
            // 
            // menuMain
            // 
            this.menuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuMain.Location = new System.Drawing.Point(0, 0);
            this.menuMain.Name = "menuMain";
            this.menuMain.Size = new System.Drawing.Size(890, 24);
            this.menuMain.TabIndex = 2;
            this.menuMain.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.acquireToolStripMenuItem,
            this.acquireCustomInterfaceToolStripMenuItem,
            this.toolStripMenuItem1,
            this.saveAcquireParametersToolStripMenuItem,
            this.acquireFromParametersToolStripMenuItem,
            this.toolStripMenuItem2,
            this.capabilityInformationToolStripMenuItem,
            this.toolStripMenuItem3,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Enabled = false;
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            this.fileToolStripMenuItem.Visible = false;
            // 
            // acquireToolStripMenuItem
            // 
            this.acquireToolStripMenuItem.Name = "acquireToolStripMenuItem";
            this.acquireToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.acquireToolStripMenuItem.Text = "&Acquire";
            this.acquireToolStripMenuItem.Click += new System.EventHandler(this.acquireToolStripMenuItem_Click);
            // 
            // acquireCustomInterfaceToolStripMenuItem
            // 
            this.acquireCustomInterfaceToolStripMenuItem.Name = "acquireCustomInterfaceToolStripMenuItem";
            this.acquireCustomInterfaceToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.acquireCustomInterfaceToolStripMenuItem.Text = "Acquire (Custom Interface)";
            this.acquireCustomInterfaceToolStripMenuItem.Click += new System.EventHandler(this.acquireCustomInterfaceToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(214, 6);
            // 
            // saveAcquireParametersToolStripMenuItem
            // 
            this.saveAcquireParametersToolStripMenuItem.Name = "saveAcquireParametersToolStripMenuItem";
            this.saveAcquireParametersToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.saveAcquireParametersToolStripMenuItem.Text = "Save Acquire Parameters";
            this.saveAcquireParametersToolStripMenuItem.Click += new System.EventHandler(this.saveAcquireParametersToolStripMenuItem_Click);
            // 
            // acquireFromParametersToolStripMenuItem
            // 
            this.acquireFromParametersToolStripMenuItem.Name = "acquireFromParametersToolStripMenuItem";
            this.acquireFromParametersToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.acquireFromParametersToolStripMenuItem.Text = "Acquire From Parameters";
            this.acquireFromParametersToolStripMenuItem.Click += new System.EventHandler(this.acquireFromParametersToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(214, 6);
            // 
            // capabilityInformationToolStripMenuItem
            // 
            this.capabilityInformationToolStripMenuItem.Name = "capabilityInformationToolStripMenuItem";
            this.capabilityInformationToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.capabilityInformationToolStripMenuItem.Text = "Capability Information...";
            this.capabilityInformationToolStripMenuItem.Click += new System.EventHandler(this.capabilityInformationToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(214, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Enabled = false;
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            this.helpToolStripMenuItem.Visible = false;
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.aboutToolStripMenuItem.Text = "&About...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // directorySearcher1
            // 
            this.directorySearcher1.ClientTimeout = System.TimeSpan.Parse("-00:00:01");
            this.directorySearcher1.ServerPageTimeLimit = System.TimeSpan.Parse("-00:00:01");
            this.directorySearcher1.ServerTimeLimit = System.TimeSpan.Parse("-00:00:01");
            // 
            // picBox
            // 
            this.picBox.Location = new System.Drawing.Point(0, 0);
            this.picBox.MinimumSize = new System.Drawing.Size(85, 117);
            this.picBox.Name = "picBox";
            this.picBox.Size = new System.Drawing.Size(638, 878);
            this.picBox.TabIndex = 0;
            this.picBox.TabStop = false;
            this.picBox.Paint += new System.Windows.Forms.PaintEventHandler(this.picBox_Paint);
            this.picBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picBox_MouseDown);
            this.picBox.MouseEnter += new System.EventHandler(this.picBox_MouseEnter);
            this.picBox.MouseLeave += new System.EventHandler(this.picBox_MouseLeave);
            this.picBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picBox_MouseMove);
            this.picBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picBox_MouseUp);
            // 
            // previewPanel
            // 
            this.previewPanel.AutoSize = true;
            this.previewPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.previewPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.previewPanel.Controls.Add(this.picBox);
            this.previewPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previewPanel.Location = new System.Drawing.Point(250, 24);
            this.previewPanel.MinimumSize = new System.Drawing.Size(85, 117);
            this.previewPanel.Name = "previewPanel";
            this.previewPanel.Size = new System.Drawing.Size(640, 884);
            this.previewPanel.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(890, 908);
            this.Controls.Add(this.previewPanel);
            this.Controls.Add(this.tabImages);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuMain;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "NISTscan";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.menuMain.ResumeLayout(false);
            this.menuMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBox)).EndInit();
            this.previewPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                //Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n\r\nStack Trace:\r\n" + ex.StackTrace, "Exception");
            }
        }

        #region Acquisition Events

        private void acquisition_AcquireCanceled(object sender, System.EventArgs e)
        {
            // If we are keeping the interface open between scans,
            // the user will have to click 'Cancel' to close it.
            if (this._keepUIOpen)
                this.device.Disable();

            this.device.Close();
            EnableDisableMenus(true);
        }

        private void acquisition_AcquireFinished(object sender, System.EventArgs e)
        {
            // This is the only event where you should call the Close method.
            // AcquireFinished fires after all images have been acquired.
            // Note that you can call Close anytime if you need to cancel
            // any pending transfers and do a quick shutdown of your application.
            if (this._keepUIOpen == false)
            {
                this.device.Close();
                EnableDisableMenus(true);
            }
        }

        private void acquisition_ImageAcquired(object sender, Atalasoft.Twain.AcquireEventArgs e)
        {
            _scanCount++;

            if (_preview)
            {
                /* DEBUG -- original code 
                // Add a new tab for the image.
                TabPage tab = new TabPage();
                tab.AutoScroll = true;
                PictureBox pic = new PictureBox();
                tab.Controls.Add(pic);
                this.tabImages.TabPages.Add(tab);

                if (e.Image != null)
                {
                    tab.Text = "Scan " + _scanCount.ToString();
                    pic.Image = e.Image;
                }
                else if (e.FileName != null && File.Exists(e.FileName))
                {
                    tab.Text = Path.GetFileName(e.FileName);

                    using (FileStream fs = new FileStream(e.FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        pic.Image = new Bitmap(fs);
                    }
                }

                pic.Width = pic.Image.Width;
                pic.Height = pic.Image.Height;
                 */

                /* New code -- draw on previewPanel 
                PictureBox pic = new PictureBox();
                previewPanel.Controls.Add(pic);
                if (e.Image != null)
                {
                    pic.Image = e.Image;
                    pic.Width = pic.Image.Width;
                    pic.Height = pic.Image.Height;
                }
                */

                /* Newer code -- draw on picturebox */
                if (e.Image != null)
                {
                    picBox.Image = e.Image;
                }
            }
            else if (ROIs.Count >= 0)
            {
                // Process ROI's
                Rectangle rect;
                string temp_filename, filename, filepath;

                temp_filename = e.FileName;

                if (_16bit)
                {
                    //MessageBox.Show("16-bit image saved to: " + e.FileName, "[DEBUG]", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    if (!filesToCopy.Contains(temp_filename))
                    {
                        filesToCopy.Add(temp_filename);
                    }
                }
                else if (ROIs.Count == 0)
                {
                    if (!filesToCopy.Contains(temp_filename))
                    {
                        filesToCopy.Add(temp_filename);
                    }
                }
                else
                {
                    /* DEBUG 
                BatchControl bc = new BatchControl(batchSettings);
                bc.Show();
                bc.Update("[DEBUG] ROI Processing\r\n\r\n");
                bc.Update("e.Filename = " + e.FileName + "\r\n");
                */

                    for (int r = 0; r < ROIs.Count; r++)
                    {
                        rect = ROIs[r];

                        switch (r)
                        {
                            case 0:
                                filename = e.FileName.Replace("TEMP_SCAN", tboFilename1.Text);
                                break;
                            case 1:
                                filename = e.FileName.Replace("TEMP_SCAN", tboFilename2.Text);
                                break;
                            case 2:
                                filename = e.FileName.Replace("TEMP_SCAN", tboFilename3.Text);
                                break;
                            case 3:
                                filename = e.FileName.Replace("TEMP_SCAN", tboFilename4.Text);
                                break;
                            case 4:
                                filename = e.FileName.Replace("TEMP_SCAN", tboFilename5.Text);
                                break;
                            case 5:
                                filename = e.FileName.Replace("TEMP_SCAN", tboFilename6.Text);
                                break;
                            case 6:
                                filename = e.FileName.Replace("TEMP_SCAN", tboFilename7.Text);
                                break;
                            case 7:
                                filename = e.FileName.Replace("TEMP_SCAN", tboFilename8.Text);
                                break;
                            case 8:
                                filename = e.FileName.Replace("TEMP_SCAN", tboFilename9.Text);
                                break;
                            case 9:
                                filename = e.FileName.Replace("TEMP_SCAN", tboFilename10.Text);
                                break;
                            case 10:
                                filename = e.FileName.Replace("TEMP_SCAN", tboFilename11.Text);
                                break;
                            case 11:
                                filename = e.FileName.Replace("TEMP_SCAN", tboFilename12.Text);
                                break;
                            case 12:
                                filename = e.FileName.Replace("TEMP_SCAN", tboFilename13.Text);
                                break;
                            case 13:
                                filename = e.FileName.Replace("TEMP_SCAN", tboFilename14.Text);
                                break;

                            default:
                                filename = e.FileName;
                                break;
                        }

                        filepath = savepath + filename;


                        /* DEBUG -- Need read settings for ImageMagick?
                        MagickReadSettings settings = new MagickReadSettings();
                        settings.Width = _scanWidth;
                        settings.Height = _scanHeight;
                        //settings.Format = MagickFormat.Gray; // Not needed?
                         */

                        // Crop and rotate
                        using (MagickImage image = new MagickImage(e.FileName))
                        {
                            // DEBUG: Shallow copy
                            IMagickImage temp_img = image;
                            
                            // Clone image?
                            //IMagickImage temp_img = image.Clone();

                            // Dispose of original image once cloned?
                            //image.Dispose();
                            //GC.Collect();


                            // Scale ROI coordinates
                            int x, y, width, height, x_offset, y_offset;

                            double scaleFactor = this.device.Resolution.X / preview_resolution; // 100

                            x_offset = Convert.ToInt32(this.device.Frame.X / scaleFactor);
                            y_offset = Convert.ToInt32(this.device.Frame.Y / scaleFactor);
                            x = Convert.ToInt32(rect.X * scaleFactor) - Convert.ToInt32(this.device.Frame.X); // 100 = preview resolution
                            y = Convert.ToInt32(rect.Y * scaleFactor) - Convert.ToInt32(this.device.Frame.Y); // 100 = preview resolution
                            width = Convert.ToInt32(rect.Width * scaleFactor); // 100 = preview resolution
                            height = Convert.ToInt32(rect.Height * scaleFactor); // 100 = preview resolution

                            // Check values
                            if (x < 0)
                            {
                                x = 0;
                            }
                            if (y < 0)
                            {
                                y = 0;
                            }
                            if ((x + width) > Convert.ToInt32(this.device.Frame.X + this.device.Frame.Width))// - 2) // NOTE: Check these offsets (see if you can remove the "-2")
                            {
                                width = Convert.ToInt32(this.device.Frame.X + this.device.Frame.Width);// -2;
                            }
                            if ((y + height) > Convert.ToInt32(this.device.Frame.Y + this.device.Frame.Height))// - 2)
                            {
                                height = Convert.ToInt32(this.device.Frame.Y + this.device.Frame.Height);// -2;
                            }

                            temp_img.Crop(x, y, width, height);
                            temp_img.RePage(); // Recommended by API?

                            // Rotate
                            if (ORIs[r] != 'T')
                            {
                                if (ORIs[r] == 'L')
                                {
                                    temp_img.Transpose();
                                    temp_img.Flop(); // Flop is a horizontal flip
                                }
                                if (ORIs[r] == 'R')
                                {
                                    temp_img.Transpose();
                                    temp_img.Flip();
                                }
                                if (ORIs[r] == 'B')
                                {
                                    temp_img.Flip();
                                    temp_img.Flop(); // Flop is a horizontal flip
                                }
                            }

                            // Write Output Image
                            temp_img.Write(filepath);

                            // Clean-up?
                            temp_img.Dispose();
                            GC.Collect();
                        }

                        /* DEBUG -- Try EmguCV
                        try
                        {
                            Mat cv_img = CvInvoke.Imread(e.FileName, ImreadModes.Unchanged);

                            if (cv_img.IsEmpty)
                            {
                                MessageBox.Show("There was an error loading the image: " + e.FileName, "[DEBUG]", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                // DEBUG
                                //MessageBox.Show("CV Image Loaded: " + e.FileName, "[DEBUG]", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            }


                            // DEBUG 
                            //ImageViewer.Show(cv_img, "Emgu Test Window");
                            //ImageViewer im = new ImageViewer(cv_img, "Emgu Test Window");
                            //im.ShowDialog();
                            

                            // Scale ROI coordinates
                            int x, y, width, height, x_offset, y_offset;

                            double scaleFactor = this.device.Resolution.X / preview_resolution; // 100

                            x_offset = Convert.ToInt32(this.device.Frame.X / scaleFactor);
                            y_offset = Convert.ToInt32(this.device.Frame.Y / scaleFactor);

                            //x = (rect.X - x_offset) * (Convert.ToInt32(this.device.Resolution.X) / preview_resolution); // 100 = preview resolution
                            //y = (rect.Y - y_offset) * (Convert.ToInt32(this.device.Resolution.X) / preview_resolution); // 100 = preview resolution
                            x = Convert.ToInt32(rect.X * scaleFactor) - Convert.ToInt32(this.device.Frame.X); // 100 = preview resolution
                            y = Convert.ToInt32(rect.Y * scaleFactor) - Convert.ToInt32(this.device.Frame.Y); // 100 = preview resolution
                            //width = rect.Width * (Convert.ToInt32(this.device.Resolution.X) / preview_resolution); // 100 = preview resolution
                            //height = rect.Height * (Convert.ToInt32(this.device.Resolution.X) / preview_resolution); // 100 = preview resolution
                            width = Convert.ToInt32(rect.Width * scaleFactor); // 100 = preview resolution
                            height = Convert.ToInt32(rect.Height * scaleFactor); // 100 = preview resolution

                            // Check values
                            if (x < 0)
                            {
                                x = 0;
                            }
                            if (y < 0)
                            {
                                y = 0;
                            }
                            if ((x + width) > Convert.ToInt32(this.device.Frame.X + this.device.Frame.Width))// - 2) // NOTE: Check these offsets (see if you can remove the "-2")
                            {
                                width = Convert.ToInt32(this.device.Frame.X + this.device.Frame.Width);// -2;
                            }
                            if ((y + height) > Convert.ToInt32(this.device.Frame.Y + this.device.Frame.Height))// - 2)
                            {
                                height = Convert.ToInt32(this.device.Frame.Y + this.device.Frame.Height);// -2;
                            }

                            Rectangle cropRect = new Rectangle(x, y, width, height);

                            // OpenCV Crop
                            // crop the resulting image
                            using (Mat cropped = new Mat())
                            {
                                CvInvoke.GetRectSubPix(cv_img, cropRect.Size, new PointF(cropRect.X + (cropRect.Width / 2), cropRect.Y + (cropRect.Height / 2)), cropped);
                                cv_img.Dispose();

                                // Rotate the image
                                //Mat rotMat = new Mat();
                                using (Mat transposed = new Mat())
                                using (Mat rotated = new Mat())
                                {
                                    if (ORIs[r] != 'T')
                                    {
                                        int deg = 0;

                                        if (ORIs[r] == 'L')
                                        {
                                            deg = 90;

                                            CvInvoke.Transpose(cropped, transposed);
                                            CvInvoke.Flip(transposed, rotated, FlipType.Horizontal);
                                        }
                                        if (ORIs[r] == 'R')
                                        {
                                            deg = -90;

                                            CvInvoke.Transpose(cropped, transposed);
                                            CvInvoke.Flip(transposed, rotated, FlipType.None);
                                        }
                                        if (ORIs[r] == 'B')
                                        {
                                            deg = 180;

                                            CvInvoke.Flip(cropped, transposed, FlipType.Vertical);
                                            CvInvoke.Flip(transposed, rotated, FlipType.Horizontal);
                                        }


                                        //CvInvoke.GetRotationMatrix2D(new PointF(x + (width /2), y + (height / 2)), deg, 1.0, rotMat);
                                        //CvInvoke.WarpAffine(cropped, rotated, rotMat, rotated.Size, Inter.Cubic, Warp.Default, BorderType.Constant);


                                        // Write rotated image
                                        CvInvoke.Imwrite(filepath, rotated);
                                        rotated.Dispose();
                                    }
                                    else
                                    {
                                        // Write cropped image
                                        CvInvoke.Imwrite(filepath, cropped);
                                        cropped.Dispose();
                                    }

                                    // Necessary?
                                    //transposed.Dispose();
                                    //rotated.Dispose();
                                }

                                // Nessary?
                                //cropped.Dispose();
                            }

                            // Clean-Up -- Unnecessary?
                            //cv_img.Dispose();
                            //rotMat.Dispose();
                        }
                        catch (Exception ex)
                        {
                            //MessageBox.Show("Exception caught: " + ex.Message, "[DEBUG]", MessageBoxButtons.OK, MessageBoxIcon.Information);


                            // Copy/rename temp_filename
                            if (!filesToCopy.Contains(temp_filename))
                            {
                                filesToCopy.Add(temp_filename);
                            }
                        }
                        finally
                        {
                            // placeholder
                        }
                        */


                        // DEBUG
                        //CvInvoke.Imwrite("test_cropped.bmp", cropped);

                        /* DEBUG --  Segment & Save (the old way)
                    
                        // DEBUG 
                        //bc.Update("Filepath (1) = " + filepath + "\r\n");

                        //this.device.Frame = new Rectangle(Convert.ToInt32(min_x * scaleFactor), Convert.ToInt32(min_y * scaleFactor), Convert.ToInt32((max_x - min_x) * scaleFactor), Convert.ToInt32((max_y - min_y) * scaleFactor));

                        // DEBUG
                        //bc.Update("Frame = " + this.device.Frame.X.ToString() + "," + this.device.Frame.Y.ToString() + "\r\n");

                        // Scale ROI coordinates
                        int x, y, width, height, x_offset, y_offset;

                        double scaleFactor = this.device.Resolution.X / preview_resolution; // 100

                        x_offset = Convert.ToInt32(this.device.Frame.X / scaleFactor);
                        y_offset = Convert.ToInt32(this.device.Frame.Y / scaleFactor);

                        //x = (rect.X - x_offset) * (Convert.ToInt32(this.device.Resolution.X) / preview_resolution); // 100 = preview resolution
                        //y = (rect.Y - y_offset) * (Convert.ToInt32(this.device.Resolution.X) / preview_resolution); // 100 = preview resolution
                        x = Convert.ToInt32(rect.X * scaleFactor) - Convert.ToInt32(this.device.Frame.X); // 100 = preview resolution
                        y = Convert.ToInt32(rect.Y * scaleFactor)- Convert.ToInt32(this.device.Frame.Y); // 100 = preview resolution
                        //width = rect.Width * (Convert.ToInt32(this.device.Resolution.X) / preview_resolution); // 100 = preview resolution
                        //height = rect.Height * (Convert.ToInt32(this.device.Resolution.X) / preview_resolution); // 100 = preview resolution
                        width = Convert.ToInt32(rect.Width * scaleFactor); // 100 = preview resolution
                        height = Convert.ToInt32(rect.Height * scaleFactor); // 100 = preview resolution

                        // Check values
                        if (x < 0)
                        {
                            x = 0;
                        }
                        if (y < 0)
                        {
                            y = 0;
                        }
                        if ((x + width) > Convert.ToInt32(this.device.Frame.X + this.device.Frame.Width) - 2)
                        {
                            width = Convert.ToInt32(this.device.Frame.X + this.device.Frame.Width) - 2;
                        }
                        if ((y + height) > Convert.ToInt32(this.device.Frame.Y + this.device.Frame.Height) - 2)
                        {
                            height = Convert.ToInt32(this.device.Frame.Y + this.device.Frame.Height) - 2;
                        }

                        ProcessStartInfo psi = new ProcessStartInfo();
                        psi.WorkingDirectory = Directory.GetCurrentDirectory();
                        psi.FileName = "quickrop.exe";
                        //psi.Arguments = e.FileName + " " + rect.X.ToString() + " " + rect.Y.ToString() + " " + rect.Width.ToString() + " " + rect.Height.ToString() + " " + filepath; //+ @" C:\TEMP\testcrop" + r.ToString() + ".bmp";
                        psi.Arguments = e.FileName + " " + x.ToString() + " " + y.ToString() + " " + width.ToString() + " " + height.ToString() + " " + filepath;
                        psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

                        // DEBUG
                        //bc.Update("Filepath (2) = " + filepath + "\r\n");

                        // DEBUG
                        //bc.Update("Sending Command: " + psi.FileName + psi.Arguments + "\r\n");
                    

                        Process.Start(psi);

                        if (ORIs[r] != 'T')
                        {
                            int deg = 0;

                            if (ORIs[r] == 'L')
                            {
                                deg = 90;
                            }
                            if (ORIs[r] == 'R')
                            {
                                deg = -90;
                            }
                            if (ORIs[r] == 'B')
                            {
                                deg = 180;
                            }

                            // DEBUG -- rotate with imrotate 
                            psi.FileName = "imrotate.exe";
                            //psi.Arguments = e.FileName + " " + rect.X.ToString() + " " + rect.Y.ToString() + " " + rect.Width.ToString() + " " + rect.Height.ToString() + " " + filepath; //+ @" C:\TEMP\testcrop" + r.ToString() + ".bmp";
                            psi.Arguments = filepath + " " + deg.ToString() + " " + filepath;
                            psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                         

                            // DEBUG -- rotate with convert
                            //psi.FileName = "convert.exe";
                            //psi.Arguments = filepath + " -rotate " + deg.ToString() + " " + filepath;
                            //psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                        

                            // DEBUG 
                            //bc.Update("Filepath (3) = " + filepath + "\r\n");

                            // DEBUG 
                            //bc.Update("Sending Command: " + psi.FileName + psi.Arguments + "\r\n");
                        
                            Process.Start(psi);
                        }
                    */
                    }
                }


                // Copy any files that need to be copy before deleting
                foreach (string f in filesToCopy)
                {
                    string new_filename = f;

                    /* DEBUG */
                    if (ROIs.Count > 0)
                    {
                        for (int r = 0; r < ROIs.Count; r++)
                        {
                            //new_filename = new_filename.Replace("TEMP_SCAN", ("ROI_TO_PROCESS_" + tboFilename1.Text));
                            new_filename = f;

                            // DEBUG
                            //MessageBox.Show("Processing ROI #: " + (r+1), "[DEBUG]", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            switch (r)
                            {
                                case 0:
                                    new_filename = new_filename.Replace("TEMP_SCAN", ("ROI_TO_PROCESS_" + tboFilename1.Text));
                                    break;
                                case 1:
                                    new_filename = new_filename.Replace("TEMP_SCAN", ("ROI_TO_PROCESS_" + tboFilename2.Text));
                                    break;
                                case 2:
                                    new_filename = new_filename.Replace("TEMP_SCAN", ("ROI_TO_PROCESS_" + tboFilename3.Text));
                                    break;
                                case 3:
                                    new_filename = new_filename.Replace("TEMP_SCAN", ("ROI_TO_PROCESS_" + tboFilename4.Text));
                                    break;
                                case 4:
                                    new_filename = new_filename.Replace("TEMP_SCAN", ("ROI_TO_PROCESS_" + tboFilename5.Text));
                                    break;
                                case 5:
                                    new_filename = new_filename.Replace("TEMP_SCAN", ("ROI_TO_PROCESS_" + tboFilename6.Text));
                                    break;
                                case 6:
                                    new_filename = new_filename.Replace("TEMP_SCAN", ("ROI_TO_PROCESS_" + tboFilename7.Text));
                                    break;
                                case 7:
                                    new_filename = new_filename.Replace("TEMP_SCAN", ("ROI_TO_PROCESS_" + tboFilename8.Text));
                                    break;
                                case 8:
                                    new_filename = new_filename.Replace("TEMP_SCAN", ("ROI_TO_PROCESS_" + tboFilename9.Text));
                                    break;
                                case 9:
                                    new_filename = new_filename.Replace("TEMP_SCAN", ("ROI_TO_PROCESS_" + tboFilename10.Text));
                                    break;
                                case 10:
                                    new_filename = new_filename.Replace("TEMP_SCAN", ("ROI_TO_PROCESS_" + tboFilename11.Text));
                                    break;
                                case 11:
                                    new_filename = new_filename.Replace("TEMP_SCAN", ("ROI_TO_PROCESS_" + tboFilename12.Text));
                                    break;
                                case 12:
                                    new_filename = new_filename.Replace("TEMP_SCAN", ("ROI_TO_PROCESS_" + tboFilename13.Text));
                                    break;
                                case 13:
                                    new_filename = new_filename.Replace("TEMP_SCAN", ("ROI_TO_PROCESS_" + tboFilename14.Text));
                                    break;
                                default:
                                    new_filename = e.FileName;
                                    break;
                            }

                            // DEBUG
                            //MessageBox.Show("Filename is: " + new_filename, "[DEBUG]", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            /* DEBUG -- Copying files with OpenCV imread/imwrite??
                            Mat tmp_img = CvInvoke.Imread(f, ImreadModes.Unchanged);

                            if (tmp_img.IsEmpty)
                            {
                                MessageBox.Show("There was an error loading the image: " + f, "[DEBUG]", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }

                            CvInvoke.Imwrite((savepath + new_filename), tmp_img);
                            tmp_img.Dispose();
                             */
                            // DEBUG -- Copy/rename file directly instead
                            File.Copy(f, (savepath + new_filename));

                            // Write ROIs out to file (or script to process them?)
                            string ROIfile = savepath + new_filename;
                            ROIfile += ".ROI";
                            using (System.IO.StreamWriter outfile = new System.IO.StreamWriter(ROIfile))
                            {
                                string line = ORIs[r].ToString() + "," + ROIs[r].X.ToString() + "," + ROIs[r].Y.ToString() + "," + ROIs[r].Width.ToString() + "," + ROIs[r].Height.ToString();
                                outfile.WriteLine(line);

                                //MessageBox.Show("Writing line to ROI file: " + line, "[DEBUG]", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                outfile.Close();
                            }

                            //File.Copy(ROIfile, (savepath + ROIfile));
                            //filesToDelete.Add(ROIfile);
                        }
                    }
                    else
                    {
                        new_filename = new_filename.Replace("TEMP_SCAN", tboFilename1.Text);

                        /* DEBUG -- Copying files with OpenCV imread/imwrite??
                       Mat tmp_img = CvInvoke.Imread(f, ImreadModes.Unchanged);

                       if (tmp_img.IsEmpty)
                       {
                           MessageBox.Show("There was an error loading the image: " + f, "[DEBUG]", MessageBoxButtons.OK, MessageBoxIcon.Information);
                       }

                       CvInvoke.Imwrite((savepath + new_filename), tmp_img);
                       tmp_img.Dispose();
                         */
                        // DEBUG -- Copy/rename file directly instead
                        File.Copy(f, (savepath + new_filename));
                    }

                    // Remove item
                    filesToCopy.Remove(f);
                    filesToDelete.Add(f);
                }

                // Delete TEMP file
                filesToDelete.Add(temp_filename);
            }
        }


        private void acquisition_FileTransfer(object sender, Atalasoft.Twain.FileTransferEventArgs e)
        {
            // This will fire before a file transfer takes place to
            // allow you to set the filename and format of this image.
            // Once the file is acquired and saved, the ImageAcquired
            // event fires with the filename of the image just saved.

            /* ORIGINAL CODE 
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = this._fileDialogFilter;
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                // Delete the existing file.
                try
                {
                    if (File.Exists(dlg.FileName))
                        File.Delete(dlg.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("There was an error trying to delete the file.\r\n\r\n" + ex.Message, "Delete File Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    e.CancelPending = true;
                    return;
                }

                e.FileName = dlg.FileName;
                string[] filters = dlg.Filter.Split('|');
                switch(filters[(dlg.FilterIndex - 1) * 2])
                {
                    case "TIFF (*.tif)":
                        e.FileFormat = SourceImageFormat.Tiff;
                        break;
                    case "JPEG (*.jpg)":
                        e.FileFormat = SourceImageFormat.Jfif;
                        break;
                    case "PNG (*.png)":
                        e.FileFormat = SourceImageFormat.Png;
                        break;
                    case "Multipage TIFF (*.tif)":
                        e.FileFormat = SourceImageFormat.TiffMulti;
                        break;
                    case "XBM (*.xbm)":
                        e.FileFormat = SourceImageFormat.Xbm;
                        break;
                    case "PDF (*.pdf)":
                        e.FileFormat = SourceImageFormat.Pdf;
                        break;
                    case "JPEG 2000 (*.jp2)":
                        e.FileFormat = SourceImageFormat.Jpeg2000;
                        break;
                    default:
                        e.FileFormat = SourceImageFormat.Bmp;
                        break;
                }
            }
            else
                e.CancelPending = true;
			
            dlg.Dispose();
             */

            /* DEBUG -- disable single scan/direct save (make sure file is touched by OpenCV first
            if (tboFilename2.Enabled == false) // Single scan
            {
                if (append_imageinfo)
                {
                    //e.FileName = tboFilename1.Text + "_" + this.cboResolution.SelectedItem + "PPI_" + this.device.BitDepth.ToString() + "BPP_";

                    // DEBUG
                    //e.FileName = savepath + tboFilename1.Text + "_" + Convert.ToInt32(this.device.Resolution.X).ToString() + "PPI_" + this.device.BitDepth.ToString() + "BPP_";
                     
                    if (!_16bit)
                    {
                        e.FileName = savepath + tboFilename1.Text + "_" + scannerID + "_" + Convert.ToInt32(this.device.Resolution.X).ToString() + "PPI_" + this.device.BitDepth.ToString() + "BPP_";
                    }
                    else
                    {
                        e.FileName = savepath + tboFilename1.Text + "_" + scannerID + "_" + Convert.ToInt32(this.device.Resolution.X).ToString() + "PPI_" + "16BPP_";
                    }
                     

                    if (this.device.PixelType == ImagePixelType.Grayscale)
                    {
                        e.FileName += "1CH";
                    }
                    else
                    {
                        e.FileName += "3CH";
                    }
                    e.FileName += cboFormat.Text.ToLower();
                    //e.FileFormat = SourceImageFormat.Tiff;
                    e.FileFormat = SourceImageFormat.Png;
                }
                else
                {
                    e.FileName = savepath + tboFilename1.Text + cboFormat.Text.ToLower();
                    e.FileFormat = SourceImageFormat.Tiff;
                }
                
            }
            else*/
            {
                // Scan to temp file, then segment and save ROI's

                /* DEBUG
                   e.FileName = "TEMP_SCAN" + "_" + Convert.ToInt32(this.device.Resolution.X).ToString() + "PPI_" + this.device.BitDepth.ToString() + "BPP_";
                    */
                if (_8bit)
                {
                    e.FileName = "TEMP_SCAN" + "_" + scannerID + "_" + Convert.ToInt32(this.device.Resolution.X).ToString() + "PPI_8BPP_1CH";
                }
                else if (_16bit)
                {
                    e.FileName = "TEMP_SCAN" + "_" + scannerID + "_" + Convert.ToInt32(this.device.Resolution.X).ToString() + "PPI_16BPP_1CH";
                }
                else if (_24bit)
                {
                    e.FileName = "TEMP_SCAN" + "_" + scannerID + "_" + Convert.ToInt32(this.device.Resolution.X).ToString() + "PPI_24BPP_3CH";
                }
                else // Default/fallback
                {
                    e.FileName = "TEMP_SCAN" + "_" + scannerID + "_" + Convert.ToInt32(this.device.Resolution.X).ToString() + "PPI_" + this.device.BitDepth.ToString() + "BPP_";
                    if (this.device.PixelType == ImagePixelType.Grayscale)
                    {
                        e.FileName += "1CH";
                    }
                    else
                    {
                        e.FileName += "3CH";
                    }
                }
                e.FileName += cboFormat.Text.ToLower();

                // DEBUG -- Hardcode TIFF?
                e.FileFormat = SourceImageFormat.Tiff;
            }

            // DEBUG
            //MessageBox.Show("Writing filename: " + e.FileName + "\r\nFile Format Set to: " + e.FileFormat.ToString(), "[DEBUG]", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //MessageBox.Show("(Later) Bit Depth set to: " + this.device.BitDepth.ToString(), "[DEBUG]", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private void acquisition_DeviceEvent(object sender, Atalasoft.Twain.DeviceEventArgs e)
        {
            // One of the many device events has fired.
            // You will only receive the events you have set using
            // the Device.DeviceEvents property.
            if (e.Event == DeviceEventFlags.PaperJam)
                MessageBox.Show(this, "Paper jam!!!");
            else
                MessageBox.Show(this, "Device Event:  " + e.Event.ToString());
        }

        private void acquisition_AsynchronousException(object sender, Atalasoft.Twain.AsynchronousExceptionEventArgs e)
        {
            // Make sure you close the connection when there is an error during a scan.
            this.device.Close();
            MessageBox.Show(this, e.Exception.Message, "Asynchronous Exception");
        }

        #endregion

        #region Private Methods

        private List<ComboBox> _disabledControls = new List<ComboBox>();
        private void EnableDisableMenus(bool enabled)
        {
            this.fileToolStripMenuItem.Enabled = enabled;

            this.cboDevice.Enabled = enabled;
            this.cboTransferMethod.Enabled = enabled;

            // We do this so controls that are disabled 
            if (enabled)
            {
                int len = _disabledControls.Count;
                for (int i = 0; i < len; i++)
                {
                    ComboBox box = _disabledControls[i];
                    box.Enabled = true;
                }
            }
            else
            {
                if (this.cboPixelType.Enabled)
                {
                    this.cboPixelType.Enabled = false;
                    _disabledControls.Add(this.cboPixelType);
                }

                if (this.cboBitDepth.Enabled)
                {
                    this.cboBitDepth.Enabled = false;
                    _disabledControls.Add(this.cboBitDepth);
                }

                if (this.cboResolution.Enabled)
                {
                    this.cboResolution.Enabled = false;
                    _disabledControls.Add(this.cboResolution);
                }

                if (this.cboFrameSize.Enabled)
                {
                    this.cboFrameSize.Enabled = false;
                    _disabledControls.Add(this.cboFrameSize);
                }
            }
        }

        private void ClearTabs()
        {
            // Calling TabPages.Clear will not release the memory
            // used by the images, so we have to dispose them manually.
            foreach (TabPage page in this.tabImages.TabPages)
            {
                PictureBox pic = page.Controls[0] as PictureBox;
                if (pic != null)
                {
                    page.Controls.Remove(pic);
                    if (pic.Image != null) pic.Image.Dispose();
                    pic.Dispose();
                }
            }

            this.tabImages.TabPages.Clear();
        }

        private void ShowInfoDialog(string message)
        {
            this._infoDialog = new InfoDialog("Loading devices...");
            Rectangle rc = Screen.PrimaryScreen.WorkingArea;
            this._infoDialog.Show();
            Application.DoEvents();
        }

        private void HideInfoDialog()
        {
            this._infoDialog.Close();
            this._infoDialog.Dispose();
            this._infoDialog = null;
        }

        private void LoadDeviceNames()
        {
            // Never assume that a system has any acquisition devices.
            if (!this.acquisition.SystemHasTwain || this.acquisition.Devices.Count == 0)
                return;

            string defaultDevice = this.acquisition.Devices.Default.Identity.ProductName;

            // Add a menu item for each device.
            foreach (Device dev in this.acquisition.Devices)
            {
                this.cboDevice.Items.Add(dev.Identity.ProductName);
            }

            int index = this.cboDevice.Items.IndexOf(defaultDevice);
            if (index > -1) this.cboDevice.SelectedIndex = index;
        }

        private void FillDeviceInformation()
        {
            if (this.device == null) return;

            this.Cursor = Cursors.WaitCursor;
            if (this.device.TryOpen())
            {
                try
                {
                    _updatingValues = true;
                    UpdateTransferMethodValues();
                    UpdatePixelTypeValues();
                    UpdateBitDepthValues();
                    UpdateResolutionValues();
                    UpdateFrameSizeValues();
                }
                finally
                {
                    _updatingValues = false;
                    this.device.Close();
                }
            }
            this.Cursor = Cursors.Default;
        }

        private void UpdateTransferMethodValues()
        {
            this.cboTransferMethod.Items.Clear();
            if (this.device.QueryCapability(DeviceCapability.ICAP_XFERMECH, true))
            {
                this.cboTransferMethod.Enabled = true;

                TwainTransferMethod[] methods = this.device.GetSupportedTransferMethods();
                foreach (TwainTransferMethod method in methods)
                    this.cboTransferMethod.Items.Add(method);

                int index = this.cboTransferMethod.Items.IndexOf(this.device.TransferMethod);
                if (index > -1) this.cboTransferMethod.SelectedIndex = index;
            }
            else
                this.cboTransferMethod.Enabled = false;
        }

        private void UpdatePixelTypeValues()
        {
            this.cboPixelType.Items.Clear();
            if (this.device.QueryCapability(DeviceCapability.ICAP_PIXELTYPE, true))
            {
                this.cboPixelType.Enabled = true;

                ImagePixelType[] pixelTypes = this.device.GetSupportedPixelTypes();
                foreach (ImagePixelType pt in pixelTypes)
                    this.cboPixelType.Items.Add(pt);

                int index = this.cboPixelType.Items.IndexOf(this.device.PixelType);
                if (index > -1) this.cboPixelType.SelectedIndex = index;
            }
            else
                this.cboPixelType.Enabled = false;

        }

        private void UpdateBitDepthValues()
        {
            this.cboBitDepth.Items.Clear();
            if (this.device.QueryCapability(DeviceCapability.ICAP_BITDEPTH, true))
            {
                this.cboBitDepth.Enabled = true;

                int[] bitDepths = this.device.GetSupportedBitDepths();
                foreach (int bd in bitDepths)
                    this.cboBitDepth.Items.Add(bd);

                int index = this.cboBitDepth.Items.IndexOf(this.device.BitDepth);
                if (index > -1) this.cboBitDepth.SelectedIndex = index;
            }
            else
                this.cboBitDepth.Enabled = false;
        }

        private void UpdateResolutionValues()
        {
            this.cboResolution.Items.Clear();
            if (this.device.QueryCapability(DeviceCapability.ICAP_XRESOLUTION, true))
            {
                this.cboResolution.Enabled = true;

                TwainResolution[] resolutions = this.device.GetSupportedResolutions();
                foreach (TwainResolution res in resolutions)
                    this.cboResolution.Items.Add(res.X.ToString() + ", " + res.Y.ToString());

                TwainResolution resolution = this.device.Resolution;
                int index = this.cboResolution.Items.IndexOf(resolution.X.ToString() + ", " + resolution.Y.ToString());
                if (index > -1) this.cboResolution.SelectedIndex = index;
            }
            else
                this.cboResolution.Enabled = false;
        }

        private void UpdateFrameSizeValues()
        {
            this.cboFrameSize.Items.Clear();
            if (this.device.QueryCapability(DeviceCapability.ICAP_SUPPORTEDSIZES, true))
            {
                this.cboFrameSize.Enabled = true;

                StaticFrameSizeType[] frameSizes = this.device.GetSupportedFrameSizes();
                foreach (StaticFrameSizeType fs in frameSizes)
                    this.cboFrameSize.Items.Add(fs);

                int index = this.cboFrameSize.Items.IndexOf(this.device.FrameSize);
                if (index > -1) this.cboFrameSize.SelectedIndex = index;
            }
            else
                this.cboFrameSize.Enabled = false;
        }

        private void SetResolutionValue()
        {
            if (this.cboResolution.Enabled == false)
                return;

            string[] val = this.cboResolution.Text.Split(',');
            float x, y;
            if (float.TryParse(val[0].Trim(), out x) && float.TryParse(val[1].Trim(), out y))
                this.device.Resolution = new TwainResolution(x, y);
        }

        #endregion

        #region Menu Events

        private void acquireToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Perform a normal acquire.
            ClearTabs();

            if (this.device == null)
                this.device = this.acquisition.Devices.Default;

            if (this.device.TryOpen())
            {
                if (this.cboTransferMethod.Enabled && this.cboTransferMethod.SelectedIndex != -1)
                    this.device.TransferMethod = (TwainTransferMethod)this.cboTransferMethod.SelectedItem;
                if (this.cboPixelType.Enabled && this.cboPixelType.SelectedIndex != -1)
                    this.device.PixelType = (ImagePixelType)this.cboPixelType.SelectedItem;
                if (this.cboBitDepth.Enabled && this.cboBitDepth.SelectedIndex != -1)
                    this.device.BitDepth = (int)this.cboBitDepth.SelectedItem;

                SetResolutionValue();

                if (this.cboFrameSize.Enabled && this.cboFrameSize.SelectedIndex != -1)
                    this.device.FrameSize = (StaticFrameSizeType)this.cboFrameSize.SelectedItem;

                this.device.HideInterface = this.chkHideInterface.Checked;
                this.device.ModalAcquire = this.chkModalAcquire.Checked;
                this.device.DisplayProgressIndicator = !this.chkHideProgress.Checked;

                EnableDisableMenus(false);

                // If the transfer method is FILE, get a list of supported file types.
                TwainTransferMethod tm = this.device.TransferMethod;
                if (tm == TwainTransferMethod.TWSX_FILE || tm == TwainTransferMethod.TWSX_FILE2)
                {
                    _fileDialogFilter = "";
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();

                    SourceImageFormat[] formats = this.device.GetSupportedImageFormats();
                    foreach (SourceImageFormat format in formats)
                    {
                        switch (format)
                        {
                            case SourceImageFormat.Bmp:
                                sb.Append("Bitmap (*.bmp)|*.bmp|");
                                break;
                            case SourceImageFormat.Jfif:
                                sb.Append("JPEG (*.jpg)|*.jpg|");
                                break;
                            case SourceImageFormat.Jpeg2000:
                                sb.Append("JPEG 2000 (*.jp2)|*.jp2|");
                                break;
                            case SourceImageFormat.Pdf:
                                sb.Append("PDF (*.pdf)|*.pdf|");
                                break;
                            case SourceImageFormat.Png:
                                sb.Append("PNG (*.png)|*.png|");
                                break;
                            case SourceImageFormat.Tiff:
                                sb.Append("TIFF (*.tif)|*.tif|");
                                break;
                            case SourceImageFormat.TiffMulti:
                                sb.Append("Multipage TIFF (*.tif)|*.tif|");
                                break;
                            case SourceImageFormat.Xbm:
                                sb.Append("XBM (*.xbm)|*.xbm|");
                                break;
                        }
                    }

                    _fileDialogFilter = sb.ToString(0, sb.Length - 1);
                }

                // If you want to keep the interface open between scans, 
                // use Enable instead of Acquire and call Disable in AcquireCanceled.
                if (this._keepUIOpen)
                    this.device.Enable();
                else
                    this.device.Acquire();
            }
            else
                MessageBox.Show("We were unable to open a connection to the device.", "Connection Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void acquireCustomInterfaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearTabs();

            // Use our custom interface instead of the driver interface.
            // Note that the driver always has the option to show its own interface
            // even when asked to hide it.

            if (this.device == null)
                this.device = this.acquisition.Devices.Default;

            CustomInterface ci = null;

            try
            {
                if (this.device.TryOpen())
                {
                    this.device.HideInterface = true;
                    this.device.DisplayProgressIndicator = false;

                    ci = new CustomInterface(this.device);
                    if (ci.ShowDialog(this) == DialogResult.OK)
                    {
                        EnableDisableMenus(false);
                        this.device.Acquire();
                    }
                    else
                        this.device.Close();
                }
                else
                    MessageBox.Show("We were unable to open a connection to the device.", "Connection Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message);
                this.device.Close();
            }
            finally
            {
                if (ci != null)
                    ci.Dispose();
            }
        }

        private void saveAcquireParametersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // This will show the scanner interface without scanning to
            // allow users to select the settings they want to use in
            // a future scan.  It will then save those settings to file.

            if (this.device == null) return;

            if (this.device.TryOpen())
            {
                try
                {
                    if (this.device.EnableInterfaceOnly)
                    {
                        if (this.device.ShowUserInterface())
                        {
                            using (SaveFileDialog dlg = new SaveFileDialog())
                            {
                                bool customData = this.device.QueryCapability(DeviceCapability.CAP_CUSTOMDSDATA, false);

                                dlg.Filter = "Twain Parameters|*." + (customData ? "ini" : "xml");
                                if (dlg.ShowDialog(this) == DialogResult.OK)
                                {
                                    using (FileStream fs = new FileStream(dlg.FileName, FileMode.Create, FileAccess.Write, FileShare.None))
                                    {
                                        // CAP_CUSTOMDSDATA is read only, so make sure to pass 'false' for 'canSet'.
                                        if (customData)
                                            this.device.SaveParameters(fs);
                                        else
                                            this.device.SaveXmlParameters(fs);
                                    }
                                }
                            }
                        }
                    }
                    else
                        MessageBox.Show("The device does not support this feature.", "Not Supported", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                finally
                {
                    this.device.Close();
                }
            }
            else
                MessageBox.Show("We were unable to connect to the device.", "Connection Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void acquireFromParametersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // This will load a previously saved setting file and hide
            // the scanner interface when scanning.

            if (this.device.TryOpen())
            {
                try
                {
                    using (OpenFileDialog dlg = new OpenFileDialog())
                    {
                        bool customData = this.device.QueryCapability(DeviceCapability.CAP_CUSTOMDSDATA, false);

                        dlg.Filter = "Twain Parameters|*." + (customData ? "ini" : "xml");
                        if (dlg.ShowDialog(this) == DialogResult.OK)
                        {
                            using (FileStream fs = new FileStream(dlg.FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                if (customData)
                                    this.device.LoadParameters(fs);
                                else
                                    this.device.LoadXmlParameters(fs);
                            }

                            EnableDisableMenus(false);

                            this.device.HideInterface = true;
                            this.device.DisplayProgressIndicator = false;
                            this.device.Acquire();
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.device.Close();
                    MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void capabilityInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.device == null) return;

            if (this.device.TryOpen())
            {
                try
                {
                    DeviceCapabilityInformation info = new DeviceCapabilityInformation(this.device);
                    info.ShowDialog(this);
                    info.Dispose();
                }
                finally
                {
                    this.device.Close();
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutDialog dlg = new AboutDialog();
            dlg.ShowDialog(this);
            dlg.Dispose();
        }

        #endregion

        #region ComboBox Events

        private void cboDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cboDevice.Text == "Select Source...")
            {
                // Display the select source dialog.
                // If the user cancels the dialog the return value is the default device.
                Device dev = this.acquisition.ShowSelectSource();
                if (dev == null)
                    return;

                this.device = dev;
            }
            else
                this.device = this.acquisition.Devices.GetDevice(this.cboDevice.Text);

            /* NIST -- disable FillDeviceInformation
            FillDeviceInformation();
             */
        }

        // HERE -- update "SelectedIndexChanged" functions

        private void cboTransferMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_updatingValues) return;

            /* DEBUG -- disable for now 
            this.Cursor = Cursors.WaitCursor;
            if (this.device.TryOpen())
            {
                try
                {
                    this.device.TransferMethod = (TwainTransferMethod)this.cboTransferMethod.SelectedItem;

                    _updatingValues = true;
                    UpdatePixelTypeValues();
                    UpdateBitDepthValues();
                }
                finally
                {
                    _updatingValues = false;
                    this.device.Close();
                }
            }
            this.Cursor = Cursors.Default;
             */
        }

        private void cboPixelType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_updatingValues) return;

            /* DEBUG -- disable for now
            this.Cursor = Cursors.WaitCursor;
            if (this.device.TryOpen())
            {
                try
                {
                    this.device.PixelType = (ImagePixelType)this.cboPixelType.SelectedItem;

                    _updatingValues = true;
                    UpdateBitDepthValues();
                }
                finally
                {
                    _updatingValues = false;
                    this.device.Close();
                }
            }
            this.Cursor = Cursors.Default;
             */
        }

        #endregion

        // You can't have both Keep Interface Open and Hide Interface enabled.
        private void chkKeepInterfaceOpen_CheckedChanged(object sender, EventArgs e)
        {
            this._keepUIOpen = this.chkKeepInterfaceOpen.Checked;
            if (_keepUIOpen) this.chkHideInterface.Checked = false;
            this.chkHideInterface.Enabled = !_keepUIOpen;
        }

        private void chkHideInterface_CheckedChanged(object sender, EventArgs e)
        {
            bool disable = !this.chkHideInterface.Checked;
            if (disable) this.chkKeepInterfaceOpen.Checked = false;
            this.chkKeepInterfaceOpen.Enabled = disable;
        }

        #region NIST

        private void ReadINI(string file)
        {
            if (File.Exists(file))
            {
                string[] configLines = File.ReadAllLines(file);

                /* Process lines */
                for (int i = 0; i < configLines.Length; i++)
                {
                    if (configLines[i].Length > 0)
                    {
                        string[] split = configLines[i].Split(new Char[] { '=' });

                        if (split[0] == "Resolutions")
                        {
                            string[] subsplit = split[1].Split(new Char[] { ',' });
                            foreach (string s in subsplit)
                            {
                                resolutions.Add(s);
                            }
                        }
                        else if (split[0] == "Append Filenames")
                        {
                            if (split[1] == "Enable")
                            {
                                chkAppendFilename.Checked = true;
                                chkAppendFilename.Enabled = false;
                                label9.ForeColor = System.Drawing.Color.LightGray;
                                append_imageinfo = true;
                            }
                        }
                        else if (split[0] == "Filename Pattern")
                        {
                            regex_string = split[1];
                            //regex_string.TrimEnd(new char[] { '\n' });
                        }
                        else if (split[0] == "Pattern Message")
                        {
                            regex_message = split[1];
                            label7.Enabled = true;
                            label7.Visible = true;
                            label10.Text = regex_message;
                            label10.Enabled = true;
                            label10.Visible = true;
                        }
                        else if (split[0] == "Save Path")
                        {
                            savepath = split[1];

                            // Test & create path
                            try
                            {
                                Directory.CreateDirectory(savepath);

                                if (!Regex.IsMatch(savepath, "\\$")) // if path doesn't end with "\" 
                                {
                                    savepath += "\\";
                                 
                                    // DEBUG
                                    //MessageBox.Show("Save Path =" + savepath, "[DEBUG] SAVEPATH", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            catch
                            {
                                MessageBox.Show("Bad Save Directory in .INI file:" + savepath, "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Environment.Exit(0);
                            }
                        }
                        else if (split[0] == "Autoconfig Batch")
                        {
                            if (split[1] == "Enable")
                            {
                                autoconfig_batch = true;
                                buttonBatchConfig.Enabled = false;
                                buttonBatchScan.Enabled = true;

                                // Disable other UI buttons
                                cboPixelType.Enabled = false;
                                cboResolution.Enabled = false;

                                batchSettings.append = false;
                                batchSettings.prepend = false;
                                batchSettings.sequence = "-1";
                                batchSettings.validated = true;
                            }
                        }
                        else if (split[0] == "Format Lock")
                        {
                            string format = "." + split[1];
                            cboFormat.SelectedItem = cboFormat.Items[cboFormat.Items.IndexOf(format)];
                            cboFormat.Text = format;
                            cboFormat.Enabled = false;

                            // DEBUG
                            //MessageBox.Show("cboFormat.Text = " + cboFormat.Text.ToString(), "[DEBUG]", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else if (split[0] == "Scanner ID")
                        {
                            scannerID = split[1];
                        }
                        else if (split[0].StartsWith("#"))
                        {
                            // Ignore comment line
                        }
                        else
                        {
                            MessageBox.Show("Corrupted INI file: " + file, "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Environment.Exit(0);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Failed to load INI file: " + file, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /* NIST */
        private void InitializeControls()
        {
            _updatingValues = true;

            // Add tbofilename1 to list of activeTBOs on initialize
            activeTBOs.Add(tboFilename1);

            ReadINI("NISTScan.ini");

            /* DEBUG ColorSpace drop-down disabled -- currently defined by INI file
            this.cboPixelType.Items.Add("8-bit Grayscale");
            this.cboPixelType.Items.Add("16-bit Grayscale");
            this.cboPixelType.Items.Add("24-bit Color");
             */
            foreach (string cs in colorspaces)
            {
                this.cboPixelType.Items.Add(cs);
            }
            this.cboPixelType.SelectedIndex = 0; // 8-bit Grayscale default

            if (resolutions.Count < 1) // If not already defined by INI file
            {
                resolutions.Add("500");
                resolutions.Add("1000");
                resolutions.Add("2000");
            }
            foreach (string r in resolutions)
            {
                this.cboResolution.Items.Add(r);
            }
            this.cboResolution.SelectedIndex = 0; // 500 ppi default

            _updatingValues = false;
        }

        #region Textbox Validation

        private bool validateTextBoxes()
        {
            bool valid = true;
            List<string> errors = new List<string>();
                      

            for (int t = 0; t < activeTBOs.Count; t++)
            {
                // Check against previous
                for (int u = 0; u < t; u++)
                {
                    if (u != t) // No need to check against same box...
                    {
                        if (activeTBOs[u].Text == activeTBOs[t].Text)
                        {
                            valid = false;
                            string err = "Filename #" + (u + 1).ToString() + " and Filename #" + (t + 1).ToString() + " must not match!";
                            if (!errors.Contains(err)) // Avoid duplciate messages
                            {
                                errors.Add(err);
                            }
                            activeTBOs[u].ForeColor = Color.Red;
                            activeTBOs[t].ForeColor = Color.Red;
                        }
                    }

                }

                // Match against regex
                if (regex_string != "")
                {
                    if (!Regex.IsMatch(activeTBOs[t].Text, regex_string, RegexOptions.IgnoreCase))
                    {
                        valid = false;
                        errors.Add("Filename #" + (t + 1).ToString() + " does not match required file pattern: " + regex_message);

                        // Text = red/bold
                        activeTBOs[t].ForeColor = Color.Red;
                    }
                }
            }

            if (!valid)
            {
                string error = "";

                foreach (string s in errors)
                {
                    error += s + "\n";
                }
                MessageBox.Show(error, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            return valid;
        }

        #endregion

        /* NIST */
        private void buttonBatchConfig_Click(object sender, EventArgs e)
        {
            // Open batch config form, save settings to some kind of data structure
            BatchConfig config = new BatchConfig(batchSettings, colorspaces, resolutions);
            config.ShowDialog();

            if (batchSettings.validated)
            {
                // Enable Batch Scan button
                buttonBatchScan.Enabled = true;
            }
        }
      

        private bool validateFilepath(string filepath)
        {
            if (File.Exists(filepath))
            {
                /* DEBUG -- disable option to overwrite existing file
                DialogResult result = MessageBox.Show("File already exists: " + filepath + "\r\nOverwrite?", "File Exists", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == System.Windows.Forms.DialogResult.No)
                {
                    return false;
                }
                else
                {
                    return true;
                }
                */

                MessageBox.Show("File already exists: " + filepath + "\r\nPlease choose a new name and try again.", "File Exists", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            return true;
        }

        private bool validateBatchFile(string filename)
        {
            string file;

            for (int b = 0; b < bitDepths.Length; b++)
            {
                for (int r = 0; r < resolutions.Count; r++)
                {
                    file = savepath + filename + "_" + scannerID + "_" + resolutions[r] + "PPI_" + bitDepths[b] + "BPP_";
                    if (bitDepths[b] == "8" || bitDepths[b] == "16")
                    {
                        file += "1CH";
                    }
                    else
                    {
                        file += "3CH";
                    }
                    file += cboFormat.Text.ToLower();

                    if (!validateFilepath(file))
                    {
                        return false;
                    }
                }
            }

            // Passed checks
            return true;
        }

        private bool validateBatch()
        {
            string file;

            if (autoconfig_batch && append_imageinfo)
            {
                if (!validateBatchFile(tboFilename1.Text))
                {
                    tboFilename1.ForeColor = Color.Red;
                    return false;
                }

                if (tboFilename2.Enabled)
                {
                    if (tboFilename2.Text.Length > 0)
                    {
                        if (!validateBatchFile(tboFilename2.Text))
                        {
                            tboFilename2.ForeColor = Color.Red;
                            return false;
                        }
                    }
                    else
                    {
                        MessageBox.Show("You must enter all filenames for batch scanning!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }

                if (tboFilename3.Enabled)
                {
                    if (tboFilename3.Text.Length > 0)
                    {
                        if (!validateBatchFile(tboFilename3.Text))
                        {
                            tboFilename3.ForeColor = Color.Red;
                            return false;
                        }
                    }
                    else
                    {
                        MessageBox.Show("You must enter all filenames for batch scanning!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }

                if (tboFilename4.Enabled)
                {
                    if (tboFilename4.Text.Length > 0)
                    {
                        if (!validateBatchFile(tboFilename4.Text))
                        {
                            tboFilename4.ForeColor = Color.Red;
                            return false;
                        }
                    }
                    else
                    {
                        MessageBox.Show("You must enter all filenames for batch scanning!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }

                if (tboFilename5.Enabled)
                {
                    if (tboFilename5.Text.Length > 0)
                    {
                        if (!validateBatchFile(tboFilename5.Text))
                        {
                            tboFilename5.ForeColor = Color.Red;
                            return false;
                        }
                    }
                    else
                    {
                        MessageBox.Show("You must enter all filenames for batch scanning!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }

                if (tboFilename6.Enabled)
                {
                    if (tboFilename6.Text.Length > 0)
                    {
                        if (!validateBatchFile(tboFilename6.Text))
                        {
                            tboFilename6.ForeColor = Color.Red;
                            return false;
                        }
                    }
                    else
                    {
                        MessageBox.Show("You must enter all filenames for batch scanning!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }

                if (tboFilename7.Enabled)
                {
                    if (tboFilename7.Text.Length > 0)
                    {
                        if (!validateBatchFile(tboFilename7.Text))
                        {
                            tboFilename7.ForeColor = Color.Red;
                            return false;
                        }
                    }
                    else
                    {
                        MessageBox.Show("You must enter all filenames for batch scanning!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }

                if (tboFilename8.Enabled)
                {
                    if (tboFilename8.Text.Length > 0)
                    {
                        if (!validateBatchFile(tboFilename8.Text))
                        {
                            tboFilename8.ForeColor = Color.Red;
                            return false;
                        }
                    }
                    else
                    {
                        MessageBox.Show("You must enter all filenames for batch scanning!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }

                if (tboFilename9.Enabled)
                {
                    if (tboFilename9.Text.Length > 0)
                    {
                        if (!validateBatchFile(tboFilename9.Text))
                        {
                            tboFilename9.ForeColor = Color.Red;
                            return false;
                        }
                    }
                    else
                    {
                        MessageBox.Show("You must enter all filenames for batch scanning!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }

                if (tboFilename10.Enabled)
                {
                    if (tboFilename10.Text.Length > 0)
                    {
                        if (!validateBatchFile(tboFilename10.Text))
                        {
                            tboFilename10.ForeColor = Color.Red;
                            return false;
                        }
                    }
                    else
                    {
                        MessageBox.Show("You must enter all filenames for batch scanning!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }

                if (tboFilename11.Enabled)
                {
                    if (tboFilename11.Text.Length > 0)
                    {
                        if (!validateBatchFile(tboFilename11.Text))
                        {
                            tboFilename11.ForeColor = Color.Red;
                            return false;
                        }
                    }
                    else
                    {
                        MessageBox.Show("You must enter all filenames for batch scanning!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }

                if (tboFilename12.Enabled)
                {
                    if (tboFilename12.Text.Length > 0)
                    {
                        if (!validateBatchFile(tboFilename12.Text))
                        {
                            tboFilename12.ForeColor = Color.Red;
                            return false;
                        }
                    }
                    else
                    {
                        MessageBox.Show("You must enter all filenames for batch scanning!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }

                if (tboFilename13.Enabled)
                {
                    if (tboFilename13.Text.Length > 0)
                    {
                        if (!validateBatchFile(tboFilename13.Text))
                        {
                            tboFilename13.ForeColor = Color.Red;
                            return false;
                        }
                    }
                    else
                    {
                        MessageBox.Show("You must enter all filenames for batch scanning!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }

                if (tboFilename14.Enabled)
                {
                    if (tboFilename14.Text.Length > 0)
                    {
                        if (!validateBatchFile(tboFilename14.Text))
                        {
                            tboFilename14.ForeColor = Color.Red;
                            return false;
                        }
                    }
                    else
                    {
                        MessageBox.Show("You must enter all filenames for batch scanning!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }

            }
            else 
            {
                file = savepath + tboFilename1.Text + cboFormat.Text.ToLower();

                if (!validateFilepath(file))
                {
                    return false;
                }
            }

            // Passed all checks
            return true;
        }

        private void buttonBatchScan_Click(object sender, EventArgs e)
        {
            // Validate text boxes
            if (validateTextBoxes())
            {
                // Read batch config and execute
                if (batchSettings.validated && validateBatch())
                {
                    // Generate batch control/status dialog
                    BatchControl batchControl = new BatchControl(batchSettings);
                    batchControl.Show();

                    /* Begin batch */
                    if (autoconfig_batch)
                    {
                        for (int cs = 0; cs < colorspaces.Length; cs++)
                        {
                            for (int r = 0; r < resolutions.Count; r++)
                            {
                                if (!batchControl.IsDisposed)
                                {
                                    if (cs != -1) // DEBUG -- enable all depths
                                    //if (cs != 1) // DEBUG -- disable 16-bit
                                    //if (cs == 0) // DEBUG -- 8-bit only mode
                                    //if (cs == 1) // DEBUG -- 16-bit only mode
                                    //if (cs == 2) // DEBUG -- 24-bit only mode
                                    {
                                        batchControl.Update("Beginning scan: " + cboPixelType.Items[cs] + " @ " + cboResolution.Items[r] + "...\r\n");
                                        batchAcquire(r, cs);
                                    }
                                }
                            }
                            /* Delete TEMP_SCAN files (if necessary) */
                            foreach (string f in filesToDelete)
                            {
                                File.Delete(f);
                            }
                            filesToDelete.Clear();
                        }
                    }
                    else
                    {
                        foreach (int cs in batchSettings.colorspaces)
                        {
                            foreach (int r in batchSettings.resolutions)
                            {
                                if (!batchControl.IsDisposed)
                                {
                                    batchControl.Update("Beginning scan: " + cboPixelType.Items[cs] + " @ " + cboResolution.Items[r] + "...\r\n");
                                    batchAcquire(r, cs);
                                }
                            }
                        }
                    }

                    batchControl.Update("\r\nBatch Complete!");
                    batchControl.Continue();

                    /* Wait for continue button */
                    batchControl.Visible = false;
                    batchControl.ShowDialog();

                    // Reset interface
                    ResetUI();
                }
            }
        }

        private void ResetUI()
        {
            // Clear ROI's
            ROIs.Clear();
            ORIs.Clear();

            // Clear picbox
            picBox.Image = null;
            picBox.Refresh();

            // Clear all filenames
            tboFilename1.Clear();
            tboFilename2.Clear();
            tboFilename3.Clear();
            tboFilename4.Clear();
            tboFilename5.Clear();
            tboFilename6.Clear();
            tboFilename7.Clear();
            tboFilename8.Clear();
            tboFilename9.Clear();
            tboFilename10.Clear();
            tboFilename11.Clear();
            tboFilename12.Clear();
            tboFilename13.Clear();
            tboFilename14.Clear();

            // Re-disable filename textboxes
            tboFilename2.Enabled = false;
            tboFilename2.Visible = false;
            tboFilename3.Enabled = false;
            tboFilename3.Visible = false;
            tboFilename4.Enabled = false;
            tboFilename4.Visible = false;
            tboFilename5.Enabled = false;
            tboFilename5.Visible = false;
            tboFilename6.Enabled = false;
            tboFilename6.Visible = false;
            tboFilename7.Enabled = false;
            tboFilename7.Visible = false;
            tboFilename8.Enabled = false;
            tboFilename8.Visible = false;
            tboFilename9.Enabled = false;
            tboFilename9.Visible = false;
            tboFilename10.Enabled = false;
            tboFilename10.Visible = false;
            tboFilename11.Enabled = false;
            tboFilename11.Visible = false;
            tboFilename12.Enabled = false;
            tboFilename12.Visible = false;
            tboFilename13.Enabled = false;
            tboFilename13.Visible = false;
            tboFilename14.Enabled = false;
            tboFilename14.Visible = false;

            label_fn2.Visible = false;
            label_fn3.Visible = false;
            label_fn4.Visible = false;
            label_fn5.Visible = false;
            label_fn6.Visible = false;
            label_fn7.Visible = false;
            label_fn8.Visible = false;
            label_fn9.Visible = false;
            label_fn10.Visible = false;
            label_fn11.Visible = false;
            label_fn12.Visible = false;
            label_fn13.Visible = false;
            label_fn14.Visible = false;

            //Clear activeTBOs
            activeTBOs.Clear();
            activeTBOs.Add(tboFilename1); // Filename1 is always active

        }

        private void batchAcquire(int resolution, int colorspace)
        {
            // Settings
            _preview = false;

            if (this.device == null)
                this.device = this.acquisition.Devices.Default;

            if (this.device.TryOpen())
            {
                // Static options
                TwainTransferMethod[] methods = this.device.GetSupportedTransferMethods();

                foreach (TwainTransferMethod method in methods)
                {
                    if (method == TwainTransferMethod.TWSX_FILE2)
                    {
                        // Use TWSX_FILE2 when possible. 
                        this.device.TransferMethod = method;
                        break;
                    }
                    if (method == TwainTransferMethod.TWSX_FILE)
                        this.device.TransferMethod = method;
                }

                /* Device options for NISTscan */
                this.device.AutoBrightness = false; // Turn off auto-exposure
                this.device.AutomaticBorderDetection = false; // Turn off auto-crop
                this.device.AutomaticDeskew = false; // Turn off auto-deskew
                this.device.AutomaticRotate = false; // Turn off auto-rotate
                this.device.AutoScan = false; // Turn off auto-scan (auto capture one after another?)
                this.device.AutoSize = TwainAutoSize.TWAS_NONE; // Turn off auto-sizing
                this.device.Compression = CompressionMode.None; // No compression

                /* Interface options */
                this.device.HideInterface = true;
                this.device.ModalAcquire = true; // We want modal so nothing else happens while a scan is occurring (especially in batch mode)
                this.device.DisplayProgressIndicator = true;

                /* DEBUG -- Leftover from Atalasoft example */
                EnableDisableMenus(false);

                /* Set colorspace */
                switch (colorspace)
                {
                    case 0: // 8-bit grayscale
                        this.device.PixelType = ImagePixelType.Grayscale;
                        // DEBUG
                        this.device.TransferMethod = TwainTransferMethod.TWSX_MEMORY; // NOTE: Transfer mode must be set before Bit Depth!
                        this.device.BitDepth = 8;
                        _8bit = true;
                        _16bit = false;
                        _24bit = false;
                        break;
                    case 1: // 16-bit grayscale
                        /* DEBUG */
                        this.device.PixelType = ImagePixelType.Grayscale;
                        this.device.TransferMethod = TwainTransferMethod.TWSX_MEMORY; // NOTE: Transfer mode must be set before Bit Depth!
                        this.device.BitDepth = 16;
                        _16bit = true;
                        _8bit = false;
                        _24bit = false;
                        break;
                    case 2: // 24-bit color
                        this.device.PixelType = ImagePixelType.Color;
                        // DEBUG
                        this.device.TransferMethod = TwainTransferMethod.TWSX_MEMORY; // NOTE: Transfer mode must be set before Bit Depth!
                        this.device.BitDepth = 24;
                        _24bit = true;
                        _8bit = false;
                        _16bit = false;
                        break;
                    default: // default to 8bpp
                        this.device.PixelType = ImagePixelType.Grayscale;
                        this.device.BitDepth = 8;
                        // DEBUG
                        _16bit = false;
                        break;
                }

                // DEBUG -- Useful for troubleshooting errors in setting bit depth for some devices
                //MessageBox.Show("Bit Depth Set to: " + this.device.BitDepth.ToString(), "[DEBUG]", MessageBoxButtons.OK, MessageBoxIcon.Information);

                /* Set resolution */
                this.device.Resolution = new TwainResolution(Convert.ToInt32(this.cboResolution.Items[resolution]), Convert.ToInt32(this.cboResolution.Items[resolution]));

                /* Set scan area */
                int max_x = 0, max_y = 0, min_x = int.MaxValue, min_y = int.MaxValue;
                double scaleFactor = this.device.Resolution.X / preview_resolution; // 100
                this.device.Units = UnitType.Pixels;

                if (ROIs.Count > 0)
                {
                    // Find largest and smallest X and Y values from ROIs
                    foreach (Rectangle r in ROIs)
                    {
                        if ((r.X + r.Width) > max_x)
                        {
                            max_x = (r.X + r.Width);
                        }
                        if ((r.Y + r.Height) > max_y)
                        {
                            max_y = (r.Y + r.Height);
                        }
                        if (r.X < min_x)
                        {
                            min_x = r.X;
                        }
                        if (r.Y < min_y)
                        {
                            min_y = r.Y;
                        }
                    }

                    this.device.Frame = new Rectangle(Convert.ToInt32(min_x * scaleFactor), Convert.ToInt32(min_y * scaleFactor), Convert.ToInt32((max_x - min_x) * scaleFactor), Convert.ToInt32((max_y - min_y) * scaleFactor));
                }

                // If you want to keep the interface open between scans, 
                // use Enable instead of Acquire and call Disable in AcquireCanceled.
                if (this._keepUIOpen)
                    this.device.Enable();
                else
                    this.device.Acquire();
            }
        }

        /* NIST */
        private void buttonPreview_Click(object sender, EventArgs e)
        {
            // Preview Acquisition
            _preview = true;
            ClearTabs();

            if (this.device == null)
                this.device = this.acquisition.Devices.Default;

            if (this.device.TryOpen())
            {
                // Check and set parameters
                this.device.TransferMethod = TwainTransferMethod.TWSX_NATIVE; // Preview can be NATIVE, but full acquisition will have to be FILE or MEMORY

                switch (this.cboPixelType.SelectedIndex)
                {
                    case 0: // 8-bit grayscale
                        this.device.PixelType = ImagePixelType.Grayscale;
                        this.device.BitDepth = 8;
                        break;
                    case 1: // 16-bit grayscale
                        this.device.PixelType = ImagePixelType.Grayscale;
                        this.device.BitDepth = 16;
                        break;
                    case 2: // 24-bit color
                        this.device.PixelType = ImagePixelType.Color;
                        this.device.BitDepth = 24;
                        break;
                    default: // default to 8bpp
                        this.device.PixelType = ImagePixelType.Grayscale;
                        this.device.BitDepth = 8;
                        break;
                }


                this.device.Resolution = new TwainResolution(preview_resolution, preview_resolution); // Suggest preview @ 50 or 100 PPI


                /* Device options for NISTscan */
                this.device.AutoBrightness = false; // Turn off auto-exposure
                this.device.AutomaticBorderDetection = false; // Turn off auto-crop
                this.device.AutomaticDeskew = false; // Turn off auto-deskew
                this.device.AutomaticRotate = false; // Turn off auto-rotate
                this.device.AutoScan = false; // Turn off auto-scan (auto capture one after another?)
                this.device.AutoSize = TwainAutoSize.TWAS_NONE; // Turn off auto-sizing
                this.device.Compression = CompressionMode.None; // No compression

                /* Interface options */
                this.device.HideInterface = true;
                this.device.ModalAcquire = false;
                this.device.DisplayProgressIndicator = true;

                /* DEBUG -- Leftover from Atalasoft example */
                EnableDisableMenus(false);


                // If you want to keep the interface open between scans, 
                // use Enable instead of Acquire and call Disable in AcquireCanceled.
                if (this._keepUIOpen)
                    this.device.Enable();
                else
                    this.device.Acquire();
            }
            else
                MessageBox.Show("We were unable to open a connection to the device.", "Connection Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void buttonAcquire_Click(object sender, EventArgs e)
        {
            // Full Acquisition
            _preview = false;
            ClearTabs();

            
            if (this.device == null)
                this.device = this.acquisition.Devices.Default;

            if (this.device.TryOpen())
            {
                // Check and set parameters
                TwainTransferMethod[] methods = this.device.GetSupportedTransferMethods();

                foreach (TwainTransferMethod method in methods)
                {
                    if (method == TwainTransferMethod.TWSX_FILE2)
                    {
                        // Use TWSX_FILE2 when possible. 
                        this.device.TransferMethod = method;
                        break;
                    }
                    if (method == TwainTransferMethod.TWSX_FILE)
                        this.device.TransferMethod = method;
                }

                switch (this.cboPixelType.SelectedIndex)
                {
                    case 0: // 8-bit grayscale
                        this.device.PixelType = ImagePixelType.Grayscale;
                        this.device.BitDepth = 8;
                        break;
                    case 1: // 16-bit grayscale
                        this.device.PixelType = ImagePixelType.Grayscale;
                        this.device.BitDepth = 16;
                        break;
                    case 2: // 24-bit color
                        this.device.PixelType = ImagePixelType.Color;
                        this.device.BitDepth = 24;
                        break;
                    default: // default to 8bpp
                        this.device.PixelType = ImagePixelType.Grayscale;
                        this.device.BitDepth = 8;
                        break;
                }


                this.device.Resolution = new TwainResolution(Convert.ToInt32(this.cboResolution.SelectedItem), Convert.ToInt32(this.cboResolution.SelectedItem));

                /* Device options for NISTscan */
                this.device.AutoBrightness = false; // Turn off auto-exposure
                this.device.AutomaticBorderDetection = false; // Turn off auto-crop
                this.device.AutomaticDeskew = false; // Turn off auto-deskew
                this.device.AutomaticRotate = false; // Turn off auto-rotate
                this.device.AutoScan = false; // Turn off auto-scan (auto capture one after another?)
                this.device.AutoSize = TwainAutoSize.TWAS_NONE; // Turn off auto-sizing
                this.device.Compression = CompressionMode.None; // No compression

                /* Interface options */
                this.device.HideInterface = true;
                this.device.ModalAcquire = false;
                this.device.DisplayProgressIndicator = true;

                /* DEBUG -- Leftover from Atalasoft example */
                EnableDisableMenus(false);

                /* Set scan area */
                int max_x = 0, max_y = 0;
                double scaleFactor = this.device.Resolution.X / preview_resolution; //  
                this.device.Units = UnitType.Pixels;

                if (ROIs.Count > 0)
                {
                    // Find largest X and Y values from ROIs
                    foreach (Rectangle r in ROIs)
                    {
                        if ((r.X + r.Width) > max_x)
                        {
                            max_x = (r.X + r.Width);
                        }
                        if ((r.Y + r.Height) > max_y)
                        {
                            max_y = (r.Y + r.Height);
                        }
                    }

                    this.device.Frame = new Rectangle(0, 0, Convert.ToInt32(max_x * scaleFactor), Convert.ToInt32(max_y * scaleFactor));
                }

                // If you want to keep the interface open between scans, 
                // use Enable instead of Acquire and call Disable in AcquireCanceled.
                if (this._keepUIOpen)
                    this.device.Enable();
                else
                    this.device.Acquire();
            }
            else
                MessageBox.Show("We were unable to open a connection to the device.", "Connection Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        private void picBox_MouseEnter(object sender, EventArgs e)
        {
            Cursor = Cursors.Cross;
        }

        private void picBox_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Arrow;
        }

        private void picBox_MouseDown(object sender, MouseEventArgs e)
        {
            MouseEventArgs mouse = e as MouseEventArgs;

            if (mouse.Button == MouseButtons.Left)
            {
                Rectangle testrect = new Rectangle(mouse.X, mouse.Y, 1, 1);

                if (ROIs.Count > 0)
                {
                    selecting = true;

                    for (int r = 0; r < ROIs.Count; r++)
                    {
                        if (ROIs[r].Contains(testrect))
                        {
                            selecting = false;

                            /* Orientation box offsets */
                            int wOffset = ROIs[r].Width / 5;
                            int hOffset = ROIs[r].Height / 5;

                            Rectangle rSubLR = new Rectangle(((ROIs[r].X + ROIs[r].Width) - wOffset), ((ROIs[r].Y + ROIs[r].Height) - hOffset), wOffset, hOffset);

                            // Define "central" rectangle for moving
                            Rectangle rCentral = new Rectangle((ROIs[r].X + wOffset), (ROIs[r].Y + hOffset), (ROIs[r].Width - (wOffset * 2)), (ROIs[r].Height - (hOffset * 2)));

                            if (rSubLR.Contains(testrect))
                            {
                                selecting = false;
                                movingROI = false;
                                ROIresizing = true;
                                ROItoresize = ROIs[r];
                                ROIindex = r;
                                ROIoffset.X = ROIs[r].Width;
                                ROIoffset.Y = ROIs[r].Height;

                                break;
                            }
                            else if (rCentral.Contains(testrect))
                            {
                                selecting = false;
                                movingROI = true;
                                ROItomove = ROIs[r];
                                ROIindex = r;
                                ROIoffset.X = Math.Abs(ROIs[r].X - mouse.X);
                                ROIoffset.Y = Math.Abs(ROIs[r].Y - mouse.Y);

                                break;
                            }
                        }
                    }
                }
                else
                {
                    selecting = true;
                }

                mouseDown = new Point(mouse.X, mouse.Y);
            }

        }

        private void picBox_MouseUp(object sender, MouseEventArgs e)
        {
            MouseEventArgs mouse = e as MouseEventArgs;

            if (movingROI)
            {
                Cursor = Cursors.Cross;
                movingROI = false;
            }
            else if (ROIresizing)
            {
                Cursor = Cursors.Cross;
                ROIresizing = false;
            }
            else if (selecting)  //(selecting && (selection.Width >= 10 && selection.Height >= 10)) // DEBUG: Enable to set min size for ROI at 10x10
            {
                Point tempEndPoint = e.Location;
                selection.Location = new Point(Math.Min(mouseDown.X, tempEndPoint.X), Math.Min(mouseDown.Y, tempEndPoint.Y));
                selection.Size = new Size(Math.Abs(mouseDown.X - tempEndPoint.X), Math.Abs(mouseDown.Y - tempEndPoint.Y));
                picBox.Invalidate();

                if (selection.Width >= 10 && selection.Height >= 10)
                {
                    // Check/correct ROI boundaries and make sure they stay inside picBox
                    if (selection.X < 0)
                    {
                        selection.X = 0;
                    }
                    if (selection.Y < 0)
                    {
                        selection.Y = 0;
                    }
                    if ((selection.X + selection.Width) > picBox.Width)
                    {
                        selection.Width = picBox.Width - selection.X;
                    }
                    if ((selection.Y + selection.Height) > picBox.Height)
                    {
                        selection.Height = picBox.Height - selection.Y;
                    }


                    ROIs.Add(selection);
                    ORIs.Add('T');
                    selecting = false;
                    picBox.Invalidate();

                    // Add textboxes
                    switch (ROIs.Count)
                    {
                        case 2:
                            if (!Regex.IsMatch(label_fn1.Text, "#1$"))
                            {
                                label_fn1.Text += " #1";
                            }
                            label_fn2.Enabled = true;
                            label_fn2.Visible = true;
                            tboFilename2.Enabled = true;
                            tboFilename2.Visible = true;
                            activeTBOs.Add(tboFilename2);
                            break;
                        case 3:
                            label_fn3.Enabled = true;
                            label_fn3.Visible = true;
                            tboFilename3.Enabled = true;
                            tboFilename3.Visible = true;
                            activeTBOs.Add(tboFilename3);
                            break;
                        case 4:
                            label_fn4.Enabled = true;
                            label_fn4.Visible = true;
                            tboFilename4.Enabled = true;
                            tboFilename4.Visible = true;
                            activeTBOs.Add(tboFilename4);
                            break;
                        case 5:
                            label_fn5.Enabled = true;
                            label_fn5.Visible = true;
                            tboFilename5.Enabled = true;
                            tboFilename5.Visible = true;
                            activeTBOs.Add(tboFilename5);
                            break;
                        case 6:
                            label_fn6.Enabled = true;
                            label_fn6.Visible = true;
                            tboFilename6.Enabled = true;
                            tboFilename6.Visible = true;
                            activeTBOs.Add(tboFilename6);
                            break;
                        case 7:
                            label_fn7.Enabled = true;
                            label_fn7.Visible = true;
                            tboFilename7.Enabled = true;
                            tboFilename7.Visible = true;
                            activeTBOs.Add(tboFilename7);
                            break;
                        case 8:
                            label_fn8.Enabled = true;
                            label_fn8.Visible = true;
                            tboFilename8.Enabled = true;
                            tboFilename8.Visible = true;
                            activeTBOs.Add(tboFilename8);
                            break;
                        case 9:
                            label_fn9.Enabled = true;
                            label_fn9.Visible = true;
                            tboFilename9.Enabled = true;
                            tboFilename9.Visible = true;
                            activeTBOs.Add(tboFilename9);
                            break;
                        case 10:
                            label_fn10.Enabled = true;
                            label_fn10.Visible = true;
                            tboFilename10.Enabled = true;
                            tboFilename10.Visible = true;
                            activeTBOs.Add(tboFilename10);
                            break;
                        case 11:
                            label_fn11.Enabled = true;
                            label_fn11.Visible = true;
                            tboFilename11.Enabled = true;
                            tboFilename11.Visible = true;
                            activeTBOs.Add(tboFilename11);
                            break;
                        case 12:
                            label_fn12.Enabled = true;
                            label_fn12.Visible = true;
                            tboFilename12.Enabled = true;
                            tboFilename12.Visible = true;
                            activeTBOs.Add(tboFilename12);
                            break;
                        case 13:
                            label_fn13.Enabled = true;
                            label_fn13.Visible = true;
                            tboFilename13.Enabled = true;
                            tboFilename13.Visible = true;
                            activeTBOs.Add(tboFilename13);
                            break;
                        case 14:
                            label_fn14.Enabled = true;
                            label_fn14.Visible = true;
                            tboFilename14.Enabled = true;
                            tboFilename14.Visible = true;
                            activeTBOs.Add(tboFilename14);
                            break;
                    }
                }
                selecting = false;
            }
            else if (mouse.Button == MouseButtons.Right)
            {
                Rectangle testrect = new Rectangle(mouse.X, mouse.Y, 1, 1);

                if (ROIs.Count > 0)
                {
                    for (int r = 0; r < ROIs.Count; r++)
                    {
                        if (ROIs[r].Contains(testrect))
                        {
                            // Prompt user to confirm delete
                            DialogResult result = MessageBox.Show("Are you sure you want to delete this ROI?", "Delete ROI?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == System.Windows.Forms.DialogResult.Yes)
                            {
                                ROIs.RemoveAt(r);
                                ORIs.RemoveAt(r);
                                this.Refresh();

                                switch (ROIs.Count)
                                {
                                    case 1:
                                        if (Regex.IsMatch(label_fn1.Text, "#1$"))
                                        {
                                            label_fn1.Text = "Filename";
                                        }
                                        label_fn2.Enabled = false;
                                        label_fn2.Visible = false;
                                        tboFilename2.Enabled = false;
                                        tboFilename2.Visible = false;
                                        activeTBOs.Remove(tboFilename2);
                                        break;
                                    case 2:
                                        label_fn3.Enabled = false;
                                        label_fn3.Visible = false;
                                        tboFilename3.Enabled = false;
                                        tboFilename3.Visible = false;
                                        activeTBOs.Remove(tboFilename3);
                                        break;
                                    case 3:
                                        label_fn4.Enabled = false;
                                        label_fn4.Visible = false;
                                        tboFilename4.Enabled = false;
                                        tboFilename4.Visible = false;
                                        activeTBOs.Remove(tboFilename4);
                                        break;
                                    case 4:
                                        label_fn5.Enabled = false;
                                        label_fn5.Visible = false;
                                        tboFilename5.Enabled = false;
                                        tboFilename5.Visible = false;
                                        activeTBOs.Remove(tboFilename5);
                                        break;
                                    case 5:
                                        label_fn6.Enabled = false;
                                        label_fn6.Visible = false;
                                        tboFilename6.Enabled = false;
                                        tboFilename6.Visible = false;
                                        activeTBOs.Remove(tboFilename6);
                                        break;
                                    case 6:
                                        label_fn7.Enabled = false;
                                        label_fn7.Visible = false;
                                        tboFilename7.Enabled = false;
                                        tboFilename7.Visible = false;
                                        activeTBOs.Remove(tboFilename7);
                                        break;
                                    case 7:
                                        label_fn8.Enabled = false;
                                        label_fn8.Visible = false;
                                        tboFilename8.Enabled = false;
                                        tboFilename8.Visible = false;
                                        activeTBOs.Remove(tboFilename8);
                                        break;
                                    case 8:
                                        label_fn9.Enabled = false;
                                        label_fn9.Visible = false;
                                        tboFilename9.Enabled = false;
                                        tboFilename9.Visible = false;
                                        activeTBOs.Remove(tboFilename9);
                                        break;
                                    case 9:
                                        label_fn10.Enabled = false;
                                        label_fn10.Visible = false;
                                        tboFilename10.Enabled = false;
                                        tboFilename10.Visible = false;
                                        activeTBOs.Remove(tboFilename10);
                                        break;
                                    case 10:
                                        label_fn11.Enabled = false;
                                        label_fn11.Visible = false;
                                        tboFilename11.Enabled = false;
                                        tboFilename11.Visible = false;
                                        activeTBOs.Remove(tboFilename11);
                                        break;
                                    case 11:
                                        label_fn12.Enabled = false;
                                        label_fn12.Visible = false;
                                        tboFilename12.Enabled = false;
                                        tboFilename12.Visible = false;
                                        activeTBOs.Remove(tboFilename12);
                                        break;
                                    case 12:
                                        label_fn13.Enabled = false;
                                        label_fn13.Visible = false;
                                        tboFilename13.Enabled = false;
                                        tboFilename13.Visible = false;
                                        activeTBOs.Remove(tboFilename13);
                                        break;
                                    case 13:
                                        label_fn14.Enabled = false;
                                        label_fn14.Visible = false;
                                        tboFilename14.Enabled = false;
                                        tboFilename14.Visible = false;
                                        activeTBOs.Remove(tboFilename14);
                                        break;

                                }
                            }
                        }
                    }
                }
            }
            else // A left-click inside of an ROI
            {
                Rectangle testRect = new Rectangle(mouse.X, mouse.Y, 1, 1);

                if (ROIs.Count > 0)
                {
                    for (int r = 0; r < ROIs.Count; r++)
                    {
                        if (ROIs[r].Contains(testRect))
                        {
                            int wOffset = ROIs[r].Width / 5;
                            int hOffset = ROIs[r].Height / 5;

                            /* Define subRects */
                            Rectangle rSubT = new Rectangle(ROIs[r].X + wOffset, ROIs[r].Y, ROIs[r].Width - (wOffset * 2), hOffset);
                            Rectangle rSubL = new Rectangle(ROIs[r].X, ROIs[r].Y + hOffset, wOffset, ROIs[r].Height - (hOffset * 2));
                            Rectangle rSubR = new Rectangle((ROIs[r].X + ROIs[r].Width) - wOffset, ROIs[r].Y + hOffset, wOffset, ROIs[r].Height - (hOffset * 2));
                            Rectangle rSubB = new Rectangle(ROIs[r].X + wOffset, (ROIs[r].Y + ROIs[r].Height) - hOffset, ROIs[r].Width - (wOffset * 2), hOffset);



                            if (rSubT.Contains(testRect))
                            {
                                // Change Orientation to "T"
                                ORIs[r] = 'T';
                                picBox.Invalidate();
                            }
                            else if (rSubL.Contains(testRect))
                            {
                                // Change Orientation to "L"
                                ORIs[r] = 'L';
                                picBox.Invalidate();
                            }
                            else if (rSubR.Contains(testRect))
                            {
                                // Change Orientation to "R"
                                ORIs[r] = 'R';
                                picBox.Invalidate();
                            }
                            else if (rSubB.Contains(testRect))
                            {
                                // Change Orientation to "B"
                                ORIs[r] = 'B';
                                picBox.Invalidate();
                            }
                        }
                    }
                }
            }

        }

        private void picBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            if (selecting)
            {
                Point tempEndPoint = e.Location;
                selection.Location = new Point(Math.Min(mouseDown.X, tempEndPoint.X), Math.Min(mouseDown.Y, tempEndPoint.Y));
                selection.Size = new Size(Math.Abs(mouseDown.X - tempEndPoint.X), Math.Abs(mouseDown.Y - tempEndPoint.Y));
                picBox.Invalidate();
            }
            else if (movingROI)
            {
                Cursor = Cursors.SizeAll;

                int newX = e.Location.X - ROIoffset.X;
                int newY = e.Location.Y - ROIoffset.Y;

                if (newX < 0)
                    newX = 0;
                if (newY < 0)
                    newY = 0;

                if ((newX + ROItomove.Width) > picBox.Width)
                {
                    newX = (picBox.Width - ROItomove.Width);
                }
                if ((newY + ROItomove.Height) > picBox.Height)
                {
                    newY = (picBox.Height - ROItomove.Height);
                }


                ROItomove.X = newX;
                ROItomove.Y = newY;

                ROIs[ROIindex] = ROItomove;
                picBox.Invalidate();
            }
            else if (ROIresizing)
            {
                Cursor = Cursors.SizeNWSE;

                int newW = ROIoffset.X - (mouseDown.X - e.Location.X);
                int newH = ROIoffset.Y - (mouseDown.Y - e.Location.Y);

                if (newW < 10)
                    newW = 10;
                if (newH < 10)
                    newH = 10;

                if ((newW + ROItoresize.X) > picBox.Width)
                {
                    newW = (picBox.Width - ROItoresize.X);
                }
                if ((newH + ROItoresize.Y) > picBox.Height)
                {
                    newH = (picBox.Height - ROItoresize.Y);
                }

                ROItoresize.Width = newW;
                ROItoresize.Height = newH;
                                                
                ROIs[ROIindex] = ROItoresize;
                picBox.Invalidate();
            }

        }

        private void picBox_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            if (picBox.Image != null)
            {
                int rc = 1;

                StringFormat sfH = new StringFormat();
                sfH.Alignment = StringAlignment.Center;
                sfH.LineAlignment = StringAlignment.Center;


                StringFormat sfV = new StringFormat();
                sfV.Alignment = StringAlignment.Center;
                sfV.LineAlignment = StringAlignment.Center;
                sfV.FormatFlags = StringFormatFlags.DirectionVertical;

                foreach (Rectangle r in ROIs)
                {
                    // Draw main rectangle
                    e.Graphics.DrawRectangle(ROIPen, r);

                    /* Define sub-rectangles */
                    int wOffset = r.Width / 5;
                    int hOffset = r.Height / 5;

                    /* Define subRects */
                    Rectangle rsubT = new Rectangle(r.X + wOffset, r.Y, r.Width - (wOffset * 2), hOffset);
                    Rectangle rsubL = new Rectangle(r.X, r.Y + hOffset, wOffset, r.Height - (hOffset * 2));
                    Rectangle rsubR = new Rectangle((r.X + r.Width) - wOffset, r.Y + hOffset, wOffset, r.Height - (hOffset * 2));
                    Rectangle rsubB = new Rectangle(r.X + wOffset, (r.Y + r.Height) - hOffset, r.Width - (wOffset * 2), hOffset);

                    ROIPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                    e.Graphics.DrawRectangle(ROIPen, rsubT);
                    e.Graphics.DrawRectangle(ROIPen, rsubL);
                    e.Graphics.DrawRectangle(ROIPen, rsubR);
                    e.Graphics.DrawRectangle(ROIPen, rsubB);
                    ROIPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;

                    e.Graphics.DrawString(rc.ToString(), new Font("Calibri", (r.Height >= r.Width ? r.Height : r.Width) / 4), Brushes.Blue, new Point((r.X + (r.Width / 2)) + 2, (r.Y + (r.Height / 2)) + 2), sfH);
                    e.Graphics.DrawString(rc++.ToString(), new Font("Calibri", (r.Height >= r.Width ? r.Height : r.Width) / 4), Brushes.White, new Point((r.X + (r.Width / 2)), (r.Y + (r.Height / 2))), sfH);

                    if (ORIs[rc - 2] == 'T')
                    {
                        e.Graphics.DrawString("↑", new Font("Calibri", Convert.ToInt32(rsubT.Height / 1.5)), Brushes.White, rsubT, sfH);
                        e.Graphics.DrawString("↑", new Font("Calibri", Convert.ToInt32(rsubT.Height / 1.5)), Brushes.Blue, new Rectangle(rsubT.X - 2, rsubT.Y - 2, rsubT.Width, rsubT.Height), sfH);
                    }
                    else if (ORIs[rc - 2] == 'L')
                    {
                        e.Graphics.DrawString("←", new Font("Calibri", Convert.ToInt32(rsubL.Width / 1.5)), Brushes.White, rsubL, sfH);
                        e.Graphics.DrawString("←", new Font("Calibri", Convert.ToInt32(rsubL.Width / 1.5)), Brushes.Blue, new Rectangle(rsubL.X - 2, rsubL.Y - 2, rsubL.Width, rsubL.Height), sfH);

                    }
                    else if (ORIs[rc - 2] == 'R')
                    {
                        e.Graphics.DrawString("→", new Font("Calibri", Convert.ToInt32(rsubR.Width / 1.5)), Brushes.White, rsubR, sfH);
                        e.Graphics.DrawString("→", new Font("Calibri", Convert.ToInt32(rsubR.Width / 1.5)), Brushes.Blue, new Rectangle(rsubR.X - 2, rsubR.Y - 2, rsubR.Width, rsubR.Height), sfH);
                    }
                    else if (ORIs[rc - 2] == 'B')
                    {
                        e.Graphics.DrawString("↓", new Font("Calibri", Convert.ToInt32(rsubB.Height / 1.5)), Brushes.White, rsubB, sfH);
                        e.Graphics.DrawString("↓", new Font("Calibri", Convert.ToInt32(rsubB.Height / 1.5)), Brushes.Blue, new Rectangle(rsubB.X - 2, rsubB.Y - 2, rsubB.Width, rsubB.Height), sfH);
                    }
                }

                if (selecting && selection != null && selection.Width > 0 && selection.Height > 0)
                {
                    e.Graphics.FillRectangle(selectionBrush, selection);
                }
            }
        }

        private void chkAppendFilename_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAppendFilename.Checked)
            {
                append_imageinfo = true;
            }
            else
            {
                append_imageinfo = false;
            }
        }

        #endregion

        private void tboFilename1_TextChanged(object sender, EventArgs e)
        {
            tboFilename1.ForeColor = TextBoxBase.DefaultForeColor;
        }

        private void tboFilename2_TextChanged(object sender, EventArgs e)
        {
            tboFilename2.ForeColor = TextBoxBase.DefaultForeColor;
        }

        private void tboFilename3_TextChanged(object sender, EventArgs e)
        {
            tboFilename3.ForeColor = TextBoxBase.DefaultForeColor;
        }

        private void tboFilename4_TextChanged(object sender, EventArgs e)
        {
            tboFilename4.ForeColor = TextBoxBase.DefaultForeColor;
        }

        private void tboFilename5_TextChanged(object sender, EventArgs e)
        {
            tboFilename5.ForeColor = TextBoxBase.DefaultForeColor;
        }

        private void tboFilename6_TextChanged(object sender, EventArgs e)
        {
            tboFilename6.ForeColor = TextBoxBase.DefaultForeColor;
        }

        private void tboFilename7_TextChanged(object sender, EventArgs e)
        {
            tboFilename7.ForeColor = TextBoxBase.DefaultForeColor;
        }

        private void tboFilename8_TextChanged(object sender, EventArgs e)
        {
            tboFilename8.ForeColor = TextBoxBase.DefaultForeColor;
        }

        private void tboFilename9_TextChanged(object sender, EventArgs e)
        {
            tboFilename9.ForeColor = TextBoxBase.DefaultForeColor;
        }

        private void tboFilename10_TextChanged(object sender, EventArgs e)
        {
            tboFilename10.ForeColor = TextBoxBase.DefaultForeColor;
        }

        private void tboFilename11_TextChanged(object sender, EventArgs e)
        {
            tboFilename11.ForeColor = TextBoxBase.DefaultForeColor;
        }

        private void tboFilename12_TextChanged(object sender, EventArgs e)
        {
            tboFilename12.ForeColor = TextBoxBase.DefaultForeColor;
        }

        private void tboFilename13_TextChanged(object sender, EventArgs e)
        {
            tboFilename13.ForeColor = TextBoxBase.DefaultForeColor;
        }

        private void tboFilename14_TextChanged(object sender, EventArgs e)
        {
            tboFilename14.ForeColor = TextBoxBase.DefaultForeColor;
        }

        private void acquisition_MemoryTransfer(object sender, Atalasoft.Twain.MemoryDataTransferEventArgs e)
        {
            string tempfile = _24bit ? "temp_scan.rgb" : "temp_scan.gray";
                  
            try
            {
                if (e.Data != IntPtr.Zero) // If not final loop
                {
                    // Write data to  buffer (callbackimage)
                    e.DataHandled = true;

                    _scanWidth = e.Columns;
                    _scanHeight += e.Rows;
                    _bytesPerRow = e.BytesPerRow;
                    _bytesWritten = _scanWidth * e.Rows;

                    /* Truncating bytes -- will only work one row at a time */
                    int expectedBytes = _scanWidth * (_8bit ? 1 : _16bit ? 2 : 3);
                    int correctBytes = expectedBytes * e.Rows;

                    System.Array.Resize(ref _memoryCallbackImage, _memoryCallbackImageSize + correctBytes);

                    byte[] dataCopy = new byte[e.BytesWritten];
                    System.Runtime.InteropServices.Marshal.Copy(e.Data, dataCopy, 0, e.BytesWritten);

                    for (int i = 0; i < e.Rows; i++)
                    {
                        System.Array.Copy(dataCopy, i * _bytesPerRow, _memoryCallbackImage, _memoryCallbackImageSize, expectedBytes);
                        _memoryCallbackImageSize += expectedBytes;
                    }
                    
                    dataCopy = new byte[0];
                    GC.Collect();

                    using (var stream = new FileStream(tempfile, FileMode.Append))
                    {
                        stream.Write(_memoryCallbackImage, 0, _memoryCallbackImage.Length);
                    }
                    
                    // Reset memoryCallbackImage and memoryCallbackImageSize
                    _memoryCallbackImage = new byte[0];
                    _memoryCallbackImageSize = 0;
                    GC.Collect();

                }
                else
                {
                        // Force clean-up (if necessary)
                        _memoryCallbackImage = new byte[0];
                        GC.Collect();

                        MagickReadSettings settings = new MagickReadSettings();
                        settings.SetDefine("size", _scanWidth.ToString() + "x" + _scanHeight.ToString());
                        settings.SetDefine("depth", _16bit ? "16" : "8");
                        settings.Density = new ImageMagick.Density(this.device.Resolution.X, this.device.Resolution.Y);
                        if (_24bit)
                            settings.SetDefine("type", "Truecolor");

                        // Additional Clean-Up (done with _memoryCallBack)
                        _memoryCallbackImageSize = 0;
                        _scanWidth = 0;
                        _scanHeight = 0;

                        // Process image with ImageMagick (convert and/or segment)
                        using (MagickImage image = new MagickImage(tempfile, settings))
                        {
                            string new_filename;

                            // Process with ImageMagick
                            if (ROIs.Count == 0)
                            {
                                // Save whole scan to DEST DIR
                                new_filename = tboFilename1.Text + "_" + scannerID + "_" + Convert.ToInt32(this.device.Resolution.X).ToString() + "PPI_";
                                new_filename += _8bit ? "8BPP_1CH" : _16bit ? "16BPP_1CH" : "24BPP_3CH";
                                new_filename += cboFormat.Text.ToLower();
                                image.Write(savepath + new_filename);
                            }
                            else
                            {
                                // Process ROIs 
                                for (int r = 0; r < ROIs.Count; r++)
                                {
                                    Rectangle rect = ROIs[r];

                                    switch (r)
                                    {
                                        case 0:
                                            new_filename = tboFilename1.Text + "_" + scannerID + "_" + Convert.ToInt32(this.device.Resolution.X).ToString() + "PPI_" + (_8bit ? "8BPP_1CH" : _16bit ? "16BPP_1CH" : "24BPP_3CH") + cboFormat.Text.ToLower();
                                            break;
                                        case 1:
                                            new_filename = tboFilename2.Text + "_" + scannerID + "_" + Convert.ToInt32(this.device.Resolution.X).ToString() + "PPI_" + (_8bit ? "8BPP_1CH" : _16bit ? "16BPP_1CH" : "24BPP_3CH") + cboFormat.Text.ToLower();
                                            break;
                                        case 2:
                                            new_filename = tboFilename3.Text + "_" + scannerID + "_" + Convert.ToInt32(this.device.Resolution.X).ToString() + "PPI_" + (_8bit ? "8BPP_1CH" : _16bit ? "16BPP_1CH" : "24BPP_3CH") + cboFormat.Text.ToLower();
                                            break;
                                        case 3:
                                            new_filename = tboFilename4.Text + "_" + scannerID + "_" + Convert.ToInt32(this.device.Resolution.X).ToString() + "PPI_" + (_8bit ? "8BPP_1CH" : _16bit ? "16BPP_1CH" : "24BPP_3CH") + cboFormat.Text.ToLower();
                                            break;
                                        case 4:
                                            new_filename = tboFilename5.Text + "_" + scannerID + "_" + Convert.ToInt32(this.device.Resolution.X).ToString() + "PPI_" + (_8bit ? "8BPP_1CH" : _16bit ? "16BPP_1CH" : "24BPP_3CH") + cboFormat.Text.ToLower();
                                            break;
                                        case 5:
                                            new_filename = tboFilename6.Text + "_" + scannerID + "_" + Convert.ToInt32(this.device.Resolution.X).ToString() + "PPI_" + (_8bit ? "8BPP_1CH" : _16bit ? "16BPP_1CH" : "24BPP_3CH") + cboFormat.Text.ToLower();
                                            break;
                                        case 6:
                                            new_filename = tboFilename7.Text + "_" + scannerID + "_" + Convert.ToInt32(this.device.Resolution.X).ToString() + "PPI_" + (_8bit ? "8BPP_1CH" : _16bit ? "16BPP_1CH" : "24BPP_3CH") + cboFormat.Text.ToLower();
                                            break;
                                        case 7:
                                            new_filename = tboFilename8.Text + "_" + scannerID + "_" + Convert.ToInt32(this.device.Resolution.X).ToString() + "PPI_" + (_8bit ? "8BPP_1CH" : _16bit ? "16BPP_1CH" : "24BPP_3CH") + cboFormat.Text.ToLower();
                                            break;
                                        case 8:
                                            new_filename = tboFilename9.Text + "_" + scannerID + "_" + Convert.ToInt32(this.device.Resolution.X).ToString() + "PPI_" + (_8bit ? "8BPP_1CH" : _16bit ? "16BPP_1CH" : "24BPP_3CH") + cboFormat.Text.ToLower();
                                            break;
                                        case 9:
                                            new_filename = tboFilename10.Text + "_" + scannerID + "_" + Convert.ToInt32(this.device.Resolution.X).ToString() + "PPI_" + (_8bit ? "8BPP_1CH" : _16bit ? "16BPP_1CH" : "24BPP_3CH") + cboFormat.Text.ToLower();
                                            break;
                                        case 10:
                                            new_filename = tboFilename11.Text + "_" + scannerID + "_" + Convert.ToInt32(this.device.Resolution.X).ToString() + "PPI_" + (_8bit ? "8BPP_1CH" : _16bit ? "16BPP_1CH" : "24BPP_3CH") + cboFormat.Text.ToLower();
                                            break;
                                        case 11:
                                            new_filename = tboFilename12.Text + "_" + scannerID + "_" + Convert.ToInt32(this.device.Resolution.X).ToString() + "PPI_" + (_8bit ? "8BPP_1CH" : _16bit ? "16BPP_1CH" : "24BPP_3CH") + cboFormat.Text.ToLower();
                                            break;
                                        case 12:
                                            new_filename = tboFilename13.Text + "_" + scannerID + "_" + Convert.ToInt32(this.device.Resolution.X).ToString() + "PPI_" + (_8bit ? "8BPP_1CH" : _16bit ? "16BPP_1CH" : "24BPP_3CH") + cboFormat.Text.ToLower();
                                            break;
                                        case 13:
                                            new_filename = tboFilename14.Text + "_" + scannerID + "_" + Convert.ToInt32(this.device.Resolution.X).ToString() + "PPI_" + (_8bit ? "8BPP_1CH" : _16bit ? "16BPP_1CH" : "24BPP_3CH") + cboFormat.Text.ToLower();
                                            break;

                                        default:
                                            new_filename = "test.tiff";
                                            break;
                                    }

                                    string filepath = savepath + new_filename;

                                    // Crop and rotate
                                    IMagickImage temp_img = image.Clone();
                                    
                                    // Scale ROI coordinates
                                    int x, y, width, height, x_offset, y_offset;

                                    double scaleFactor = this.device.Resolution.X / preview_resolution; // Reminder: preview resolution = 100

                                    x_offset = Convert.ToInt32(this.device.Frame.X / scaleFactor);
                                    y_offset = Convert.ToInt32(this.device.Frame.Y / scaleFactor);
                                    x = Convert.ToInt32(rect.X * scaleFactor) - Convert.ToInt32(this.device.Frame.X); 
                                    y = Convert.ToInt32(rect.Y * scaleFactor) - Convert.ToInt32(this.device.Frame.Y); 
                                    width = Convert.ToInt32(rect.Width * scaleFactor); 
                                    height = Convert.ToInt32(rect.Height * scaleFactor); 

                                    // Check values
                                    if (x < 0)
                                    {
                                        x = 0;
                                    }
                                    if (y < 0)
                                    {
                                        y = 0;
                                    }
                                    if ((x + width) > Convert.ToInt32(this.device.Frame.X + this.device.Frame.Width))
                                    {
                                        width = Convert.ToInt32(this.device.Frame.X + this.device.Frame.Width);
                                    }
                                    if ((y + height) > Convert.ToInt32(this.device.Frame.Y + this.device.Frame.Height))
                                    {
                                        height = Convert.ToInt32(this.device.Frame.Y + this.device.Frame.Height);
                                    }

                                    temp_img.Crop(x, y, width, height);
                                    temp_img.RePage(); // Recommended by API documentation

                                    // Rotate
                                    if (ORIs[r] != 'T')
                                    {
                                        if (ORIs[r] == 'L')
                                        {
                                            temp_img.Transpose();
                                            temp_img.Flop(); // Reminder: Flop is a horizontal flip
                                        }
                                        if (ORIs[r] == 'R')
                                        {
                                            temp_img.Transpose();
                                            temp_img.Flip();
                                        }
                                        if (ORIs[r] == 'B')
                                        {
                                            temp_img.Flip();
                                            temp_img.Flop(); // Reminder: Flop is a horizontal flip
                                        }
                                    }

                                    // Write Output Image
                                    temp_img.Write(filepath);
                                }
                            }
                        }

                        File.Delete(tempfile);

                        /* Reset previous image after writing result (for next batch scan?) */
                        _memoryCallbackImageSize = 0;
                        _memoryCallbackImage = new byte[0];
                        _scanWidth = 0;
                        _scanHeight = 0;
                        return;
                    }

                    /* Reset previous image after writing result (for next batch scan) */
                    _memoryCallbackImageSize = 0;
                    _memoryCallbackImage = new byte[0];
                    _scanWidth = 0;
                    _scanHeight = 0;
                    return;
            }
            catch (Exception ex)
            {
                // Delete temp gray file
                File.Delete(tempfile);
            }
            finally
            {
                // Placeholder
            }
        }

        private IntPtr DWordAlignData(IntPtr data, int rowStride, int height, int bitsPerPixel, int bytesPerRow)
        {
            IntPtr alignedData = NativeMethods.GlobalAlloc(0, rowStride * height);

            try
            {
                unsafe
                {
                    byte* src = (byte*)data;
                    byte* dest = (byte*)alignedData;

                    for (int h = 0; h < height; h++)
                    {
                        NativeMethods.CopyMemory((IntPtr)dest, (IntPtr)src, bytesPerRow);
                        src += bytesPerRow;
                        dest += rowStride;
                    }
                }
            }
            catch
            {
                NativeMethods.GlobalFree(alignedData);
                alignedData = IntPtr.Zero;
            }
            finally
            {
                NativeMethods.GlobalFree(data);
            }

            return alignedData;
        }

    } /* END OF CLASS */
} /* END OF NAMESPACE */

