namespace XBDMRelay.Forms
{
    partial class MainForm
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
            RootMenuStrip = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            exitApplicationToolStripMenuItem = new ToolStripMenuItem();
            optionsToolStripMenuItem = new ToolStripMenuItem();
            LabelXboxIPAddress = new Label();
            TextBoxXboxIPAddress = new TextBox();
            TextBoxXboxPort = new TextBox();
            label1 = new Label();
            RichTextBoxOutput = new RichTextBox();
            ButtonToggleListener = new Button();
            TextBoxProxyIP = new TextBox();
            label2 = new Label();
            RootStatusStrip = new StatusStrip();
            StatusLabelCurrentStatus = new ToolStripStatusLabel();
            StatusLabelSeperator = new ToolStripStatusLabel();
            StatusLabelLnCol = new ToolStripStatusLabel();
            RootMenuStrip.SuspendLayout();
            RootStatusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // RootMenuStrip
            // 
            RootMenuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, optionsToolStripMenuItem });
            RootMenuStrip.Location = new Point(0, 0);
            RootMenuStrip.Name = "RootMenuStrip";
            RootMenuStrip.Size = new Size(745, 24);
            RootMenuStrip.TabIndex = 0;
            RootMenuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { exitApplicationToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // exitApplicationToolStripMenuItem
            // 
            exitApplicationToolStripMenuItem.Name = "exitApplicationToolStripMenuItem";
            exitApplicationToolStripMenuItem.ShortcutKeys = Keys.Alt | Keys.F4;
            exitApplicationToolStripMenuItem.Size = new Size(198, 22);
            exitApplicationToolStripMenuItem.Text = "Exit Application";
            exitApplicationToolStripMenuItem.Click += exitApplicationToolStripMenuItem_Click;
            // 
            // optionsToolStripMenuItem
            // 
            optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            optionsToolStripMenuItem.Size = new Size(61, 20);
            optionsToolStripMenuItem.Text = "Options";
            // 
            // LabelXboxIPAddress
            // 
            LabelXboxIPAddress.AutoSize = true;
            LabelXboxIPAddress.Location = new Point(21, 33);
            LabelXboxIPAddress.Name = "LabelXboxIPAddress";
            LabelXboxIPAddress.Size = new Size(76, 15);
            LabelXboxIPAddress.TabIndex = 1;
            LabelXboxIPAddress.Text = "Your Xbox IP:";
            // 
            // TextBoxXboxIPAddress
            // 
            TextBoxXboxIPAddress.Location = new Point(103, 30);
            TextBoxXboxIPAddress.Name = "TextBoxXboxIPAddress";
            TextBoxXboxIPAddress.PlaceholderText = "192.168.4.61";
            TextBoxXboxIPAddress.Size = new Size(104, 23);
            TextBoxXboxIPAddress.TabIndex = 2;
            // 
            // TextBoxXboxPort
            // 
            TextBoxXboxPort.Location = new Point(280, 30);
            TextBoxXboxPort.Name = "TextBoxXboxPort";
            TextBoxXboxPort.Size = new Size(64, 23);
            TextBoxXboxPort.TabIndex = 4;
            TextBoxXboxPort.Text = "730";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(213, 33);
            label1.Name = "label1";
            label1.Size = new Size(61, 15);
            label1.TabIndex = 3;
            label1.Text = "Xbox Port:";
            // 
            // RichTextBoxOutput
            // 
            RichTextBoxOutput.Location = new Point(12, 59);
            RichTextBoxOutput.Name = "RichTextBoxOutput";
            RichTextBoxOutput.Size = new Size(721, 370);
            RichTextBoxOutput.TabIndex = 5;
            RichTextBoxOutput.Text = "";
            RichTextBoxOutput.SelectionChanged += RichTextBoxOutput_SelectionChanged;
            // 
            // ButtonToggleListener
            // 
            ButtonToggleListener.Location = new Point(572, 30);
            ButtonToggleListener.Name = "ButtonToggleListener";
            ButtonToggleListener.Size = new Size(161, 23);
            ButtonToggleListener.TabIndex = 6;
            ButtonToggleListener.Text = "Start Listening";
            ButtonToggleListener.UseVisualStyleBackColor = true;
            ButtonToggleListener.Click += ButtonToggleListener_Click;
            // 
            // TextBoxProxyIP
            // 
            TextBoxProxyIP.Location = new Point(437, 30);
            TextBoxProxyIP.Name = "TextBoxProxyIP";
            TextBoxProxyIP.ReadOnly = true;
            TextBoxProxyIP.Size = new Size(104, 23);
            TextBoxProxyIP.TabIndex = 8;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(350, 33);
            label2.Name = "label2";
            label2.Size = new Size(81, 15);
            label2.TabIndex = 7;
            label2.Text = "Proxy Xbox IP:";
            // 
            // RootStatusStrip
            // 
            RootStatusStrip.Items.AddRange(new ToolStripItem[] { StatusLabelCurrentStatus, StatusLabelSeperator, StatusLabelLnCol });
            RootStatusStrip.Location = new Point(0, 438);
            RootStatusStrip.Name = "RootStatusStrip";
            RootStatusStrip.Size = new Size(745, 22);
            RootStatusStrip.TabIndex = 9;
            RootStatusStrip.Text = "statusStrip1";
            // 
            // StatusLabelCurrentStatus
            // 
            StatusLabelCurrentStatus.Name = "StatusLabelCurrentStatus";
            StatusLabelCurrentStatus.Size = new Size(95, 17);
            StatusLabelCurrentStatus.Text = "Status: Waiting...";
            // 
            // StatusLabelSeperator
            // 
            StatusLabelSeperator.Name = "StatusLabelSeperator";
            StatusLabelSeperator.Size = new Size(585, 17);
            StatusLabelSeperator.Spring = true;
            // 
            // StatusLabelLnCol
            // 
            StatusLabelLnCol.Name = "StatusLabelLnCol";
            StatusLabelLnCol.RightToLeft = RightToLeft.No;
            StatusLabelLnCol.Size = new Size(50, 17);
            StatusLabelLnCol.Text = "Ln: ,Col:";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(745, 460);
            Controls.Add(RootStatusStrip);
            Controls.Add(TextBoxProxyIP);
            Controls.Add(label2);
            Controls.Add(ButtonToggleListener);
            Controls.Add(RichTextBoxOutput);
            Controls.Add(TextBoxXboxPort);
            Controls.Add(label1);
            Controls.Add(TextBoxXboxIPAddress);
            Controls.Add(LabelXboxIPAddress);
            Controls.Add(RootMenuStrip);
            MainMenuStrip = RootMenuStrip;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "XBDMRelay - A TCP/UDP Listener for Xbox 360 Consoles";
            RootMenuStrip.ResumeLayout(false);
            RootMenuStrip.PerformLayout();
            RootStatusStrip.ResumeLayout(false);
            RootStatusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip RootMenuStrip;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem exitApplicationToolStripMenuItem;
        private Label LabelXboxIPAddress;
        private TextBox TextBoxXboxIPAddress;
        private TextBox TextBoxXboxPort;
        private Label label1;
        private RichTextBox RichTextBoxOutput;
        private Button ButtonToggleListener;
        private TextBox TextBoxProxyIP;
        private Label label2;
        private StatusStrip RootStatusStrip;
        private ToolStripStatusLabel StatusLabelLnCol;
        private ToolStripStatusLabel StatusLabelCurrentStatus;
        private ToolStripStatusLabel StatusLabelSeperator;
        private ToolStripMenuItem optionsToolStripMenuItem;
    }
}