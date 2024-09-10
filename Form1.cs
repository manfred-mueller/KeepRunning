using IWshRuntimeLibrary;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Timers;
using System.Windows.Forms;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;

namespace KeepRunning
{
    public partial class Form1 : Form
    {
        // Timer to handle periodic checks
        public System.Timers.Timer hourlyTimer = new System.Timers.Timer();
        // Registry key for configuration
        RegistryKey regKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\NASS e.K.\KeepRunning", true);

        // Various configuration properties
        public string ProcName;
        public string FileName;
        public string FileCheck;
        public string CheckProc;
        public string CheckInterv;
        public double Interval;
        public bool checkRunning = false;

        // GUI components
        private Label statusPanel;
        private Button BrowseButton;
        private PictureBox MonitorFileButton;
        private Label FileChosen;
        private Button SaveButton;
        private NotifyIcon notifyIcon;
        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem registerToolStripMenuItem;
        private ToolStripMenuItem linkToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private TextBox ProcessName;
        private ComboBox timerComboBox;

        // Shortcut file properties
        public string linkName = Properties.Resources.AppName + ".lnk";
        public string linkPath = null;

        public Form1(string[] args)
        {
            InitializeComponent();

            // Check if command-line arguments were passed
            if (args.Length > 0)
            {
                CheckFile(args[0]);
            }
            else
            {
                Form1_Load(this, EventArgs.Empty); // Load form
            }
            // Subscribe to the FormClosing event
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
        }

        // Form Load event handler
        private void Form1_Load(object sender, EventArgs e)
        {
            // Initialize form properties
            CenterFormOnScreen();
            this.ShowInTaskbar = false;
            Version shortVersion = Assembly.GetExecutingAssembly().GetName().Version;
            this.Text = string.Format(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + $" {shortVersion.Major}.{shortVersion.Minor}.{shortVersion.Build}");
            this.ActiveControl = statusPanel;

            // Load configuration from registry or defaults
            if (regKey != null)
            {
                // Load values from registry
                FileChosen.Text = regKey.GetValue("CheckProc", null) as string;
                ProcessName.Text = regKey.GetValue("ProcName", Properties.Resources.ProcessName) as string;
                timerComboBox.SelectedItem = regKey.GetValue("CheckInterv", Properties.Resources.Daily) as string;
                statusPanel.Text = string.Format(Properties.Resources.Monitoring0, Path.GetFileName(FileChosen.Text), timerComboBox.SelectedItem);
            }
            else
            {
                // Use default values
                ProcessName.Text = Properties.Resources.ProcessName;
                timerComboBox.SelectedItem = Properties.Resources.Daily;
            }

            // Hide the Save button initially
            SaveButton.Visible = false;
            // Set the path for the shortcut on the desktop
            linkPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), linkName);
            // Check if context menu and shortcut exist
            registerToolStripMenuItem.Checked = Registry.CurrentUser.OpenSubKey("Software\\Classes\\*\\shell\\KeepRunning") != null;
            linkToolStripMenuItem.Checked = System.IO.File.Exists(linkPath);

            // Add tooltips for UI elements
            ToolTip t = new ToolTip();
            t.Active = true;
            t.SetToolTip(ProcessName, Properties.Resources.ProcessNameToCheck);
            t.SetToolTip(BrowseButton, Properties.Resources.ClickToChoose);
            t.SetToolTip(MonitorFileButton, Properties.Resources.StartStopMonitoring);
            t.SetToolTip(timerComboBox, Properties.Resources.ChooseCheckInterval);
        }

        // Event handler for Browse button click
        private void BrowseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                // File dialog settings
                InitialDirectory = @"C:\",
                Title = Properties.Resources.ChooseFile,
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "exe",
                Filter = Properties.Resources.ExecutableFilesExe,
                FilterIndex = 2,
                RestoreDirectory = true,
                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (IsValidExecutableFile(openFileDialog1.FileName))
                {
                    // Update UI elements after choosing a valid file
                    FileChosen.Text = openFileDialog1.FileName;
                    ProcessName.Text = Properties.Resources.ProcessName;
                    timerComboBox.Enabled = true;
                    this.SaveButton.Visible = true;
                    statusPanel.Text = Properties.Resources.SettingsHaveChangedPleaseClickSaveButton;
                    notifyIcon.Text = Properties.Resources.SettingsHaveChangedPleaseClickSaveButton;
                }
                else
                {
                    statusPanel.Text = string.Format(Properties.Resources.FileNotValid0, openFileDialog1.FileName);
                }
            }
        }

        // Center the form on the screen
        private void CenterFormOnScreen()
        {
            int centerX = Screen.PrimaryScreen.WorkingArea.Width / 2 - 240;
            int centerY = Screen.PrimaryScreen.WorkingArea.Height / 2 - 75;

            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(centerX, centerY);
        }

        // Event handler for NotifyIcon click
        private void NotifyIcon_MouseClick(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            if (me.Button == MouseButtons.Left)
            {
                // Show/Hide the form
                if (WindowState == FormWindowState.Minimized)
                {
                    this.Show();
                    this.WindowState = FormWindowState.Normal;
                }
                else
                {
                    this.Hide();
                    this.WindowState = FormWindowState.Minimized;
                }
            }
        }

        // Check if a file is a valid executable
        private bool IsValidExecutableFile(string filePath)
        {
            string extension = Path.GetExtension(filePath);
            return System.IO.File.Exists(filePath) && (extension.Equals(".exe", StringComparison.OrdinalIgnoreCase));
        }

        // Check and handle the target program's status
        private void CheckProgram(string Program)
        {
            if (IsValidExecutableFile(FileChosen.Text))
            {
                if (ProgramIsRunning(Program))
                {
                    notifyIcon.ShowBalloonTip(2000, Properties.Resources.AppName, string.Format(Properties.Resources.ProcessIsRunning, Path.GetFileName(FileChosen.Text)), ToolTipIcon.Info);
                    this.MonitorFileButton.Image = Properties.Resources.rechteck_40x40;
                }
                else
                {
                    notifyIcon.ShowBalloonTip(2000, Properties.Resources.AppName, string.Format(Properties.Resources.ProcessIsNotRunningAndWillBeStarted, Path.GetFileName(FileChosen.Text)), ToolTipIcon.Warning);
                    Process.Start(Program);
                    this.MonitorFileButton.Image = Properties.Resources.rechteck_40x40;
                }
            }
            else
            {
                ShowMessageBox(Properties.Resources.NoValidExecutableExeFile, MessageBoxIcon.Error);
            }
        }

        // Check if the specified program is running
        private bool ProgramIsRunning(string FullPath)
        {
            FileName = Path.GetFileNameWithoutExtension(FullPath).ToLower();
            ProcName = ProcessName.Text;

            if (ProcName.Equals(Properties.Resources.ProcessName))
            {
                FileCheck = FileName;
            }
            else
            {
                FileCheck = ProcName;
            }

            try
            {
                foreach (Process p in Process.GetProcesses())
                {
                    if (p.ProcessName.ToString() == FileCheck) return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Event handler for Register context menu item
        private void RegisterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.registerToolStripMenuItem.Checked == false)
            {
                RegisterContextMenu();
                this.registerToolStripMenuItem.Checked = true;
            }
            else
            {
                UnregisterContextMenu();
                this.registerToolStripMenuItem.Checked = false;
            }
        }

        // Event handler for Link context menu item
        private void LinkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.linkToolStripMenuItem.Checked == false)
            {
                // Create a shortcut
                var WshShell = new WshShell();
                IWshShortcut MyShortcut;

                MyShortcut = (IWshShortcut)WshShell.CreateShortcut(linkPath);
                MyShortcut.TargetPath = Application.ExecutablePath;
                MyShortcut.WorkingDirectory = Environment.CurrentDirectory;
                MyShortcut.Description = Application.ProductName;
                MyShortcut.Save();
                this.linkToolStripMenuItem.Checked = true;
            }
            else
            {
                if (System.IO.File.Exists(linkPath))
                {
                    // Delete the existing shortcut
                    System.IO.File.Delete(linkPath);
                    this.linkToolStripMenuItem.Checked = false;
                }
            }
        }

        // Event handler for Exit context menu item
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Close the application
            notifyIcon.Visible = false;
            Application.Exit();
        }

        // Register the context menu in the registry
        private void RegisterContextMenu()
        {
            string programPath = Application.ExecutablePath;
            RegisterContextEntry(programPath);
        }

        // Create a registry entry for the context menu
        private void RegisterContextEntry(string programPath)
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey($"Software\\Classes\\*\\shell\\KeepRunning"))
            {
                key.SetValue("", "Keep running");
                key.SetValue("NoWorkingDirectory", "");
                key.SetValue("Position", "bottom");
                key.SetValue("Icon", programPath + ",0");
            }

            using (RegistryKey key = Registry.CurrentUser.CreateSubKey($"Software\\Classes\\*\\shell\\KeepRunning\\command"))
            {
                key.SetValue("", $"\"{programPath}\" \"%1\"");
            }
        }

        // Unregister the context menu from the registry
        private void UnregisterContextMenu()
        {
            Registry.CurrentUser.DeleteSubKeyTree($"Software\\Classes\\*\\shell\\KeepRunning", false);
        }

        // Display a message box with the specified message and icon
        private void ShowMessageBox(string message, MessageBoxIcon icon)
        {
            MessageBox.Show(message, Properties.Resources.AppName, MessageBoxButtons.OK, icon);
        }

        // Switch the timer based on the given switch value
        public void TimerSwitch(int Switch)
        {
            if (Switch == 0)
            {
                hourlyTimer?.Dispose();
                checkRunning = false;
                this.MonitorFileButton.Image = Properties.Resources.timer;
            }
            else
            {
                Interval = GetComboBoxValue();
                hourlyTimer = new System.Timers.Timer(Interval);
                hourlyTimer.Elapsed += HourlyTimerElapsed;
                hourlyTimer.AutoReset = true;
                hourlyTimer.Start();
                checkRunning = true;
                this.MonitorFileButton.Image = Properties.Resources.rechteck_40x40;
                CheckProgram(FileChosen.Text);
            }
        }

        // Event handler for MonitorFileButton click
        private void MonitorFileButton_Click(object sender, EventArgs e)
        {
            if (checkRunning == true)
            {
                TimerSwitch(0);
                UpdateStatusPanel(string.Format(Properties.Resources.Monitoring0Stopped, Path.GetFileName(FileChosen.Text), timerComboBox.SelectedItem));
            }
            else
            {
                TimerSwitch(1);
                UpdateStatusPanel(string.Format(Properties.Resources.Monitoring0, Path.GetFileName(FileChosen.Text), timerComboBox.SelectedItem));
            }
        }

        // Event handler for hourly timer elapsed
        private void HourlyTimerElapsed(object sender, ElapsedEventArgs e)
        {
            // This method will be called every hour
            CheckProgram(FileChosen.Text);
        }

        // Event handler for ProcessName leave
        private void ProcessName_Leave(object sender, EventArgs e)
        {
            if (FileChosen.Text.Length == 0 || ProcessName.Text.Length == 0)
            {
                ProcessName.Text = Properties.Resources.ProcessName;
                ProcessName.ForeColor = SystemColors.GrayText;
            }
            if (!ProcessName.Text.Equals(Properties.Resources.ProcessName))
            {
                if (checkRunning == true)
                {
                    TimerSwitch(0);
                }
                this.SaveButton.Visible = true;
                UpdateStatusPanel(Properties.Resources.SettingsHaveChangedPleaseClickSaveButton);
            }
        }

        // Event handler for ProcessName enter
        private void ProcessName_Enter(object sender, EventArgs e)
        {
            ProcessName.Text = "";
            ProcessName.ForeColor = SystemColors.WindowText;
        }

        // Get the time interval based on the selected combo box index
        private double GetComboBoxValue()
        {
            int index = timerComboBox.SelectedIndex;

            switch (index)
            {
                case 2:
                    return 86400000;
                case 1:
                    return 43200000;
                case 0:
                    return 3200000;
                default:
                    return 0;
            }
        }

        // Event handler for ComboBox1 selected index change
        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkRunning == true)
            {
                TimerSwitch(0);
            }
            this.SaveButton.Visible = true;
            UpdateStatusPanel(Properties.Resources.SettingsHaveChangedPleaseClickSaveButton);
        }

        // Event handler for FileChosen text change
        protected void FileChosen_TextChanged(object sender, EventArgs e)
        {
            if (checkRunning == true)
            {
                TimerSwitch(0);
            }
        }

        // Event handler for SaveButton click
        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (regKey == null)
            {
                regKey = Registry.CurrentUser.CreateSubKey("SOFTWARE\\NASS e.K.\\KeepRunning");
            }
            regKey.SetValue("CheckProc", FileChosen.Text);
            string procNameValue = ProcessName.Text;
            if (string.IsNullOrWhiteSpace(procNameValue))
            {
                procNameValue = Properties.Resources.ProcessName;
            }
            regKey.SetValue("ProcName", procNameValue);
            regKey.SetValue("CheckInterv", (string)timerComboBox.SelectedItem);
            UpdateStatusPanel(string.Format(Properties.Resources.Monitoring0, Path.GetFileName(FileChosen.Text), timerComboBox.SelectedItem));
            this.SaveButton.Visible = false;
        }

        // Check the chosen file and perform necessary actions
        public void CheckFile(string file)
        {
            if (checkRunning)
            {
                if (WindowState == FormWindowState.Minimized)
                {
                    Show();
                    WindowState = FormWindowState.Normal;
                }
                else
                {
                    Hide();
                    WindowState = FormWindowState.Minimized;
                }

                ShowMessageBox(string.Format(Properties.Resources.Monitoring0, Path.GetFileName(FileChosen.Text), timerComboBox.SelectedItem), MessageBoxIcon.Error);
            }
            else
            {
                if (IsValidExecutableFile(file))
                {
                    Form1_Load(this, EventArgs.Empty);
                    timerComboBox.Enabled = true;
                    timerComboBox.SelectedItem = Properties.Resources.Daily;
                    FileChosen.Text = file;
                    ProcessName.Text = Properties.Resources.ProcessName;
                }
                else
                {
                    Show();
                    WindowState = FormWindowState.Normal;
                    BringToFront();
                    ShowMessageBox(Properties.Resources.NoValidExecutableExeFile, MessageBoxIcon.Error);
                }
            }
        }

        // Event handler for ComboBox1 dropdown
        void timerComboBox_DropDown(object sender, EventArgs e)
        {
            timerComboBox.BackColor = Color.White;
        }

        // Event handler for About context menu item
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 aboutBox = new AboutBox1();
            aboutBox.ShowDialog();
        }

        // Update the status panel with the given message
        private void UpdateStatusPanel(string message)
        {
            statusPanel.Text = message;
            notifyIcon.Text = message;
        }
        // Event handler for the FormClosing event
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Check if checkRunning is true
            if (checkRunning)
            {
                // Display a confirmation message box
                DialogResult result = MessageBox.Show(
                    String.Format(KeepRunning.Properties.Resources._0IsStillBeingMonitored + Environment.NewLine + KeepRunning.Properties.Resources.ReallyCloseTheApplication, Path.GetFileName(FileChosen.Text)),
                    KeepRunning.Properties.Resources.ConfirmExit,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2 // Default to No
                );

                // If the user chooses 'No', cancel the close event
                if (result == DialogResult.No)
                {
                    this.Hide();
                    this.WindowState = FormWindowState.Minimized;
                    e.Cancel = true;
                }
            }
        }
    }
}
