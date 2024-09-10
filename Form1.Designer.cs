
using System.Drawing;
using System;
using System.Windows.Forms;

namespace KeepRunning
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.BrowseButton = new System.Windows.Forms.Button();
            this.MonitorFileButton = new System.Windows.Forms.PictureBox();
            this.FileChosen = new System.Windows.Forms.Label();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.registerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.linkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ProcessName = new System.Windows.Forms.TextBox();
            this.timerComboBox = new System.Windows.Forms.ComboBox();
            this.statusPanel = new System.Windows.Forms.Label();
            this.SaveButton = new System.Windows.Forms.Button();
            this.HelpLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.MonitorFileButton)).BeginInit();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // BrowseButton
            // 
            this.BrowseButton.Location = new System.Drawing.Point(356, 18);
            this.BrowseButton.Name = "BrowseButton";
            this.BrowseButton.Size = new System.Drawing.Size(131, 24);
            this.BrowseButton.TabIndex = 0;
            this.BrowseButton.Text = global::KeepRunning.Properties.Resources.ChooseFile;
            this.BrowseButton.UseVisualStyleBackColor = true;
            this.BrowseButton.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // MonitorFileButton
            // 
            this.MonitorFileButton.Image = global::KeepRunning.Properties.Resources.timer;
            this.MonitorFileButton.Location = new System.Drawing.Point(230, 60);
            this.MonitorFileButton.Name = "MonitorFileButton";
            this.MonitorFileButton.Size = new System.Drawing.Size(40, 40);
            this.MonitorFileButton.TabIndex = 5;
            this.MonitorFileButton.TabStop = false;
            this.MonitorFileButton.Click += new System.EventHandler(this.MonitorFileButton_Click);
            // 
            // FileChosen
            // 
            this.FileChosen.BackColor = System.Drawing.Color.LightSlateGray;
            this.FileChosen.ForeColor = System.Drawing.Color.White;
            this.FileChosen.Location = new System.Drawing.Point(12, 20);
            this.FileChosen.Name = "FileChosen";
            this.FileChosen.Size = new System.Drawing.Size(340, 20);
            this.FileChosen.TabIndex = 7;
            this.FileChosen.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.FileChosen.TextChanged += new System.EventHandler(this.FileChosen_TextChanged);
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.contextMenuStrip;
            this.notifyIcon.Icon = global::KeepRunning.Properties.Resources.keeprunning;
            this.notifyIcon.Text = global::KeepRunning.Properties.Resources.AppName;
            this.notifyIcon.Visible = true;
            this.notifyIcon.Click += new System.EventHandler(this.NotifyIcon_MouseClick);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.registerToolStripMenuItem,
            this.linkToolStripMenuItem,
            this.aboutToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(194, 92);
            // 
            // registerToolStripMenuItem
            // 
            this.registerToolStripMenuItem.Name = "registerToolStripMenuItem";
            this.registerToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.registerToolStripMenuItem.Text = global::KeepRunning.Properties.Resources.ContextMenuRegister;
            this.registerToolStripMenuItem.Click += new System.EventHandler(this.RegisterToolStripMenuItem_Click);
            // 
            // linkToolStripMenuItem
            // 
            this.linkToolStripMenuItem.Name = "linkToolStripMenuItem";
            this.linkToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.linkToolStripMenuItem.Text = global::KeepRunning.Properties.Resources.DesktopLink;
            this.linkToolStripMenuItem.Click += new System.EventHandler(this.LinkToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Image = global::KeepRunning.Properties.Resources.help;
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.aboutToolStripMenuItem.Text = global::KeepRunning.Properties.Resources.HelpLabel;
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Image = global::KeepRunning.Properties.Resources.leave;
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.exitToolStripMenuItem.Text = global::KeepRunning.Properties.Resources.Exit;
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // ProcessName
            // 
            this.ProcessName.ForeColor = System.Drawing.SystemColors.GrayText;
            this.ProcessName.Location = new System.Drawing.Point(356, 46);
            this.ProcessName.Name = "ProcessName";
            this.ProcessName.Size = new System.Drawing.Size(131, 20);
            this.ProcessName.TabIndex = 2;
            this.ProcessName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ProcessName.Enter += new System.EventHandler(this.ProcessName_Enter);
            this.ProcessName.Leave += new System.EventHandler(this.ProcessName_Leave);
            // 
            // timerComboBox
            // 
            this.timerComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.timerComboBox.Enabled = false;
            this.timerComboBox.Items.AddRange(new object[] {
            global::KeepRunning.Properties.Resources.Hourly,
            global::KeepRunning.Properties.Resources.TwoTimesADay,
            global::KeepRunning.Properties.Resources.Daily});
            this.timerComboBox.Location = new System.Drawing.Point(357, 72);
            this.timerComboBox.Name = "timerComboBox";
            this.timerComboBox.Size = new System.Drawing.Size(129, 21);
            this.timerComboBox.TabIndex = 3;
            this.timerComboBox.DropDown += new System.EventHandler(this.timerComboBox_DropDown);
            this.timerComboBox.SelectedIndexChanged += new System.EventHandler(this.ComboBox1_SelectedIndexChanged);
            this.timerComboBox.DropDownClosed += new System.EventHandler(this.timerComboBox_DropDown);
            // 
            // statusPanel
            // 
            this.statusPanel.AutoSize = true;
            this.statusPanel.Location = new System.Drawing.Point(10, 130);
            this.statusPanel.Name = "statusPanel";
            this.statusPanel.Size = new System.Drawing.Size(62, 13);
            this.statusPanel.TabIndex = 6;
            this.statusPanel.Text = "statusPanel";
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(356, 98);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(131, 23);
            this.SaveButton.TabIndex = 4;
            this.SaveButton.Text = global::KeepRunning.Properties.Resources.SaveSettings;
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // HelpLabel
            // 
            this.HelpLabel.AutoSize = true;
            this.HelpLabel.BackColor = System.Drawing.Color.Transparent;
            this.HelpLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HelpLabel.Location = new System.Drawing.Point(465, 120);
            this.HelpLabel.Name = "HelpLabel";
            this.HelpLabel.Size = new System.Drawing.Size(34, 25);
            this.HelpLabel.TabIndex = 6;
            this.HelpLabel.Text = "❓";
            this.HelpLabel.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 147);
            this.Controls.Add(this.HelpLabel);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.statusPanel);
            this.Controls.Add(this.timerComboBox);
            this.Controls.Add(this.ProcessName);
            this.Controls.Add(this.FileChosen);
            this.Controls.Add(this.BrowseButton);
            this.Controls.Add(this.MonitorFileButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.ShowInTaskbar = false;
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.MonitorFileButton)).EndInit();
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private Label HelpLabel;
    }
}

