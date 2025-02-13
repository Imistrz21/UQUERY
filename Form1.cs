using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UQUERY
{
    public partial class Form1 : Form
    {
        private Timer timerApache;

        public Form1()
        {
            InitializeComponent();
            ExtractDataZip();
            button1.Text = "Start";
            InitializeServiceTimers();
            AppendServiceOutput("main", "Control Panel Ready", Color.Green);
        }

        #region Rozpakowywanie zip
        private void ExtractDataZip()
        {
            string startupPath = Application.StartupPath;
            string zipPath = Path.Combine(startupPath, "data.zip");
           // string extractPath = Path.Combine(startupPath, "data");
            string extractPath = startupPath;
            string checkfolder = Path.Combine(startupPath, "webdav");
            if (File.Exists(zipPath))
            {
                try
                {
                    if (Directory.Exists(checkfolder))
                    {
                        MessageBox.Show("Data loaded successfully.");
                        return;
                    }
                    MessageBox.Show("Extracting data, first run can take up to 5 min (the program will start again automaticlly)");
                    ZipFile.ExtractToDirectory(zipPath, extractPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error extracting data.zip: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("data.zip not found at: " + zipPath);
            }
        }
        #endregion

        #region Logi itp
        private void RunBat(string batPath, string serviceLabel, Color defaultColor)
        {
            if (File.Exists(batPath))
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c \"" + batPath + "\"", // Run the batch file and exit
                    WorkingDirectory = Path.GetDirectoryName(batPath),
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                Process proc = new Process { StartInfo = psi, EnableRaisingEvents = true };


                proc.OutputDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        AppendServiceOutput(serviceLabel, e.Data, defaultColor);
                    }
                };

                proc.ErrorDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        AppendServiceOutput(serviceLabel, e.Data, Color.Red);
                    }
                };

                try
                {
                    proc.Start();
                    proc.BeginOutputReadLine();
                    proc.BeginErrorReadLine();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error running " + serviceLabel + " bat file: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show(serviceLabel + " bat file not found: " + batPath);
            }
        }


        private void AppendServiceOutput(string service, string text, Color color)
        {
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.Invoke(new Action(() => AppendServiceOutput(service, text, color)));
            }
            else
            {
                string timeStamp = DateTime.Now.ToString("HH:mm:ss");
                string line = $"{timeStamp}  [{service}]   {text}{Environment.NewLine}";
                int start = richTextBox1.TextLength;
                richTextBox1.AppendText(line);
                int length = richTextBox1.TextLength - start;
                richTextBox1.Select(start, length);
                richTextBox1.SelectionColor = color;
                richTextBox1.SelectionLength = 0;
                richTextBox1.ScrollToCaret();
            }
        }
        #endregion

        #region Generalne metody
        private bool IsServiceRunning(string processName)
        {
            return Process.GetProcessesByName(processName).Length > 0;
        }


        private void StartService(string serviceLabel, string batFileName, Button button)
        {
            string projectPath = Application.StartupPath;
            string batPath = Path.Combine(projectPath, batFileName);
            RunBat(batPath, serviceLabel, Color.Blue);
            button.Text = "Stop";
        }


        private async Task StopServiceAsync(string serviceLabel, string batFileName, string processName, Button button)
        {
            string projectPath = Application.StartupPath;
            string batPath = Path.Combine(projectPath, batFileName);
            RunBat(batPath, serviceLabel, Color.Red);

            bool stopped = await WaitForProcessExitAsync(processName, TimeSpan.FromSeconds(10));
            if (!stopped)
            {
                Process[] procs = Process.GetProcessesByName(processName);
                foreach (var proc in procs)
                {
                    try
                    {
                        proc.Kill();
                        proc.WaitForExit();
                        AppendServiceOutput(serviceLabel, "Process killed forcibly after timeout.", Color.Red);
                    }
                    catch (Exception ex)
                    {
                        AppendServiceOutput(serviceLabel, "Error killing process: " + ex.Message, Color.Red);
                    }
                }
            }
            if (!IsServiceRunning(processName))
            {
                button.Text = "Start";
            }
        }
        //sprawdza czy process sie wywalil
        private async Task<bool> WaitForProcessExitAsync(string processName, TimeSpan timeout)
        {
            DateTime start = DateTime.Now;
            while (DateTime.Now - start < timeout)
            {
                if (Process.GetProcessesByName(processName).Length == 0)
                    return true;
                await Task.Delay(500);
            }
            return Process.GetProcessesByName(processName).Length == 0;
        }
        #endregion

        #region Apache
        private async void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Stop")
            {
                await StopServiceAsync("apache", "apache_stop.bat", "httpd", button1);
            }
            else
            {
                StartService("apache", "apache_start.bat", button1);
            }
        }

        private void InitializeServiceTimers()
        {
            // timer ze jak sie nie zamknie program po kliknieciu stop
            timerApache = new Timer();
            timerApache.Interval = 2000; 
            timerApache.Tick += TimerApache_Tick;
            timerApache.Start();
        }

        private void TimerApache_Tick(object sender, EventArgs e)
        {
            if (button1.Text == "Stop" && !IsServiceRunning("httpd"))
            {
                button1.Text = "Start";
                AppendServiceOutput("apache", "Service stopped unexpectedly.", Color.Red);
            }
        }
        #endregion

        #region MySQL
        private async void button2_Click(object sender, EventArgs e)
        {
            if (button2.Text == "Stop")
            {
                await StopServiceAsync("mysql", "mysql_stop.bat", "mysqld", button2);
            }
            else
            {
                StartService("mysql", "mysql_start.bat", button2);
            }
        }
        #endregion

        #region FileZilla
        private async void button3_Click(object sender, EventArgs e)
        {
            if (button3.Text == "Stop")
            {
                await StopServiceAsync("filezilla", "filezilla_stop.bat", "filezilla", button3);
            }
            else
            {
                StartService("filezilla", "filezilla_start.bat", button3);
            }
        }
        #endregion

        #region Mercury
        private async void button4_Click(object sender, EventArgs e)
        {
            if (button4.Text == "Stop")
            {
                await StopServiceAsync("mercury", "mercury_stop.bat", "mercury", button4);
            }
            else
            {
                StartService("mercury", "mercury_start.bat", button4);
            }
        }
        #endregion

        #region Catalina
        private async void button5_Click(object sender, EventArgs e)
        {
            if (button5.Text == "Stop")
            {
                await StopServiceAsync("catalina", "catalina_stop.bat", "catalina", button5);
            }
            else
            {
                StartService("catalina", "catalina_start.bat", button5);
            }
        }
        #endregion

        #region
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            //tutaj dodac powiadomienia
        }
        #endregion

        // Tutaj dodac info box
        private void label7_Click(object sender, EventArgs e) { }
        private void label8_Click(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }

        private void button24_Click(object sender, EventArgs e)
        {
            string startupPath = Application.StartupPath;
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = "/separate  ,\"" + startupPath + "\"", 
                WorkingDirectory = Path.GetDirectoryName(startupPath)
            };

            Process proc = new Process { StartInfo = psi, EnableRaisingEvents = true };
            proc.Start();
        }
    }
}
