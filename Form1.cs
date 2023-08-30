using IWshRuntimeLibrary;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Timers;
using System.Windows.Forms;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;

namespace KeepRunning
{
    public partial class Form1 : Form
    {
        public System.Timers.Timer hourlyTimer = new System.Timers.Timer();
        RegistryKey regKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\NASS e.K.\KeepRunning", true);
        public string ProcName;
        public string CheckProc;
        public string CheckInterv;
        public double Interval;
        public bool checkRunning = false;
        private Label statusPanel;
        private LinkLabel copyRight;
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
        private ComboBox comboBox1;
        public string linkName = Properties.Resources.AppName + ".lnk";
        public string linkPath = null;


        public Form1(string[] args)
        {
            InitializeComponent();

            if (args.Length > 0)
            {
                checkFile(args[0]);
            }
            else
            {
                Form1_Load(this, EventArgs.Empty); // Load form
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CenterFormOnScreen();
            this.ShowInTaskbar = false;
            this.Text = Properties.Resources.AppName;
            this.ActiveControl = statusPanel;
            if (regKey != null)
            {
                FileChosen.Text = (string)regKey.GetValue("CheckProc", null);
                ProcessName.Text = (string)regKey.GetValue("ProcName", Properties.Resources.ProcessName);
                comboBox1.SelectedItem = (string)regKey.GetValue("CheckInterv", Properties.Resources.Daily);
                statusPanel.Text = string.Format(Properties.Resources.Monitoring0, Path.GetFileName(FileChosen.Text), comboBox1.SelectedItem);
            }
            else
            {
                ProcessName.Text = Properties.Resources.ProcessName;
                comboBox1.SelectedItem = Properties.Resources.Daily;
            }
            this.SaveButton.Visible = false;
            this.linkPath = (@Environment.GetEnvironmentVariable("USERPROFILE") + "\\Desktop\\" + linkName);
            if (Registry.CurrentUser.OpenSubKey($"Software\\Classes\\*\\shell\\KeepRunning") != null)
            {
                this.registerToolStripMenuItem.Checked = true;
            }
            if (System.IO.File.Exists(linkPath))
            {
                this.linkToolStripMenuItem.Checked = true;
            }

            ToolTip t = new ToolTip();
            t.Active = true;
            t.SetToolTip(ProcessName, Properties.Resources.ProcessNameToCheck);
            t.SetToolTip(BrowseButton, Properties.Resources.ClickToChoose);
            t.SetToolTip(MonitorFileButton, Properties.Resources.StartStopMonitoring);
            t.SetToolTip(comboBox1, Properties.Resources.ChooseCheckInterval);
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
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
                    FileChosen.Text = openFileDialog1.FileName;
                    ProcessName.Text = Properties.Resources.ProcessName;
                    comboBox1.Enabled = true;
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

        private void CenterFormOnScreen()
        {
            // Calculate the center of the screen
            int centerX = Screen.PrimaryScreen.WorkingArea.Width / 2 - 230;
            int centerY = Screen.PrimaryScreen.WorkingArea.Height / 2 - 75;

            // Set the form's location
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(centerX, centerY);
        }
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

        private bool IsValidExecutableFile(string filePath)
        {
            string extension = Path.GetExtension(filePath);
            return System.IO.File.Exists(filePath) && (extension.Equals(".exe", StringComparison.OrdinalIgnoreCase));
        }

        private void CheckProgram(string Program)
        {
            if (IsValidExecutableFile(FileChosen.Text)) {
                if (ProgramIsRunning(Program))
                {
                    notifyIcon.ShowBalloonTip(2000, Properties.Resources.AppName, string.Format(Properties.Resources.ProcessIsRunning , Path.GetFileName(FileChosen.Text)), ToolTipIcon.Info);
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

        private bool ProgramIsRunning(string FullPath)
        {
            string FileName = Path.GetFileNameWithoutExtension(FullPath).ToLower();
            string procName = ProcessName.Text;
            string checkFile;

            if (procName.Equals(Properties.Resources.ProcessName))
            {
                checkFile = FileName;
            }
            else
            {
                checkFile = procName;
            }

            try
            {
                foreach (Process p in Process.GetProcesses())
                {
                    if (p.ProcessName.ToString() == checkFile) return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

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

        private void LinkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.linkToolStripMenuItem.Checked == false)
            {
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
                    System.IO.File.Delete(linkPath);
                    this.linkToolStripMenuItem.Checked = false;
                }
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Clean up resources and exit the application
            notifyIcon.Visible = false;
            ExitApplication();
        }

        private void RegisterContextMenu()
        {
            string programPath = Application.ExecutablePath;
            RegisterContextEntry(programPath);
        }

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

        private void UnregisterContextMenu()
        {
            UnregisterContextEntry();
        }

        private void UnregisterContextEntry()
        {
            Registry.CurrentUser.DeleteSubKeyTree($"Software\\Classes\\*\\shell\\KeepRunning", false);
        }

        private void ExitApplication()
        {
            Application.Exit();
        }
        private void ShowMessageBox(string message, MessageBoxIcon icon)
        {
            MessageBox.Show(message, Properties.Resources.AppName, MessageBoxButtons.OK, icon);
        }

        public void TimerSwitch(string Switch)
        {
            if (Switch.Equals("Stop"))
            {
                hourlyTimer.Stop();
                this.MonitorFileButton.Image = Properties.Resources.timer;
            }
            else if (Switch.Equals("Start"))
            {
                Interval = GetComboBoxValue();
                hourlyTimer = new System.Timers.Timer(Interval);
                hourlyTimer.Elapsed += HourlyTimerElapsed;
                hourlyTimer.AutoReset = true;
                hourlyTimer.Start();
                this.MonitorFileButton.Image = Properties.Resources.rechteck_40x40;
                statusPanel.Text = string.Format(Properties.Resources.Monitoring0, FileChosen.Text, comboBox1.SelectedItem);
                notifyIcon.Text = string.Format(Properties.Resources.Monitoring0, FileChosen.Text, comboBox1.SelectedItem);
                CheckProgram(FileChosen.Text);
            }
        }

        private void MonitorFileButton_Click(object sender, EventArgs e)
        {
            if (checkRunning == true)
            {
                TimerSwitch("Stop");
                checkRunning = false;
                statusPanel.Text = string.Format(Properties.Resources.Monitoring0Stopped, Path.GetFileName(FileChosen.Text), comboBox1.SelectedItem);
            }
            else
            {
                TimerSwitch("Start");
                checkRunning = true;
                statusPanel.Text = string.Format(Properties.Resources.Monitoring0, Path.GetFileName(FileChosen.Text), comboBox1.SelectedItem);
                notifyIcon.Text = string.Format(Properties.Resources.Monitoring0, Path.GetFileName(FileChosen.Text), comboBox1.SelectedItem);
            }
        }
        private void HourlyTimerElapsed(object sender, ElapsedEventArgs e)
        {
            // This method will be called every hour
            CheckProgram(FileChosen.Text);
        }
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
                    TimerSwitch("Stop");
                }
                this.SaveButton.Visible = true;
                statusPanel.Text = Properties.Resources.SettingsHaveChangedPleaseClickSaveButton;
                notifyIcon.Text = Properties.Resources.SettingsHaveChangedPleaseClickSaveButton;
            }
        }

        private void ProcessName_Enter(object sender, EventArgs e)
        {
            ProcessName.Text = "";
            ProcessName.ForeColor = SystemColors.WindowText;
        }

        private double GetComboBoxValue()
        {
            int index = comboBox1.SelectedIndex;
            if (index == 2)
            {
                return 86400000;
            }
            else if (index == 1)
            {
                return 43200000;
            }
            else if (index == 0)
            {
                return 3200000;
            }
                return 0;
        }
        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkRunning == true)
            {
                TimerSwitch("Stop");
            }
            this.SaveButton.Visible = true;
            statusPanel.Text = Properties.Resources.SettingsHaveChangedPleaseClickSaveButton;
            notifyIcon.Text = Properties.Resources.SettingsHaveChangedPleaseClickSaveButton;
        }
        protected void FileChosen_TextChanged(object sender, EventArgs e)
        {
            if (checkRunning == true)
            {
                TimerSwitch("Stop");
            }
        }

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
            regKey.SetValue("CheckInterv", (string)comboBox1.SelectedItem);
            statusPanel.Text = string.Format(Properties.Resources.Monitoring0, Path.GetFileName(FileChosen.Text), comboBox1.SelectedItem);
            notifyIcon.Text = string.Format(Properties.Resources.Monitoring0, Path.GetFileName(FileChosen.Text), comboBox1.SelectedItem);
            this.SaveButton.Visible = false;
        }

        public void checkFile(string file)
        {
            if (checkRunning == true)
            {
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
                ShowMessageBox(string.Format(Properties.Resources.Monitoring0, Path.GetFileName(FileChosen.Text), comboBox1.SelectedItem), MessageBoxIcon.Error); 
            } else
            {
                if (IsValidExecutableFile(file))
                {
                    Form1_Load(this, EventArgs.Empty);
                    comboBox1.Enabled = true;
                    comboBox1.SelectedItem = Properties.Resources.Daily;
                    FileChosen.Text = file;
                    ProcessName.Text = Properties.Resources.ProcessName;
                }
                else
                {
                    this.Show();
                    this.WindowState = FormWindowState.Normal;
                    this.BringToFront();
                    ShowMessageBox(Properties.Resources.NoValidExecutableExeFile, MessageBoxIcon.Error);
                }

            }
        }
        private void CopyrightLabelClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string target = e.Link.LinkData as string;
            Process.Start(target);
        }
        void comboBox1_DropDown(object sender, EventArgs e)
        {
            comboBox1.BackColor = Color.White;
        }

        void comboBox1_DropDownClosed(object sender, EventArgs e)
        {
            comboBox1.BackColor = Color.White;
        }
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 aboutBox = new AboutBox1();

            aboutBox.ShowDialog();
        }
    }
}
