using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrojanWrapper
{
    public delegate void DelReadStdOutput(string result);
    public delegate void DelReadErrOutput(string result);
    public partial class Form1 : Form
    {
        public event DelReadStdOutput ReadStdOutput;
        public event DelReadErrOutput ReadErrOutput;

        
        public Form1()
        {
            InitializeComponent();
            ReadStdOutput += new DelReadStdOutput(ReadStdOutputAction);
            ReadErrOutput += new DelReadErrOutput(ReadErrOutputAction);
            this.Opacity = 0;
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
           // KillTrojan();
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Hide();
            RegistryKey reg = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");

            string exePath = Process.GetCurrentProcess().MainModule.FileName;
            string exeName = Process.GetCurrentProcess().MainModule.ModuleName;
            var value = reg.GetValue(exeName);
            if (value != null)
            {   
                autoStratupMenu.Checked = true;
                if (!value.ToString().Equals(exePath))
                {
                    reg.SetValue(exeName, exePath);
                }
            }

            System.IO.Directory.SetCurrentDirectory(Application.StartupPath);
            // MessageBox.Show(System.IO.Directory.GetCurrentDirectory());
            //  MessageBox.Show(Application.StartupPath);
            Process cmdProcess = new Process();
            cmdProcess.StartInfo.FileName = "trojan.exe";      // 命令

            cmdProcess.StartInfo.CreateNoWindow = true;         // 不创建新窗口
            cmdProcess.StartInfo.UseShellExecute = false;
            cmdProcess.StartInfo.RedirectStandardInput = true;  // 重定向输入
            cmdProcess.StartInfo.RedirectStandardOutput = true; // 重定向标准输出
            cmdProcess.StartInfo.RedirectStandardError = true;  // 重定向错误输出
            //CmdProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            cmdProcess.OutputDataReceived += new DataReceivedEventHandler(p_OutputDataReceived);
            cmdProcess.ErrorDataReceived += new DataReceivedEventHandler(p_ErrorDataReceived);

            cmdProcess.EnableRaisingEvents = true;                      // 启用Exited事件
            cmdProcess.Exited += new EventHandler(CmdProcess_Exited);   // 注册进程结束事件

            cmdProcess.Start();
            cmdProcess.BeginOutputReadLine();
            cmdProcess.BeginErrorReadLine();

            Process privoxyProcess = new Process
            {
                // Configure the process using the StartInfo properties.
                StartInfo =
                    {
                        FileName = "privoxy.exe",
                        Arguments = "config.txt",
                      
                        WindowStyle = ProcessWindowStyle.Hidden,
                        UseShellExecute = true,
                        CreateNoWindow = true
                    }
            };
            privoxyProcess.Start();

        }

        private void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                // 4. 异步调用，需要invoke
                this.Invoke(ReadStdOutput, new object[] { e.Data });
            }
        }

        private void p_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                this.Invoke(ReadErrOutput, new object[] { e.Data });
            }
        }

        private void ReadStdOutputAction(string result)
        {
           
        }

        private void ReadErrOutputAction(string result)
        {
             
        }

        private void CmdProcess_Exited(object sender, EventArgs e)
        {
            // 执行结束后触发
        }

        private void autoStratupMenu_Click(object sender, EventArgs e)
        {
            RegistryKey reg = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");

            string exePath = Process.GetCurrentProcess().MainModule.FileName;
            string exeName = Process.GetCurrentProcess().MainModule.ModuleName;
            if (autoStratupMenu.Checked)
            {
                reg.DeleteValue(exeName);
                autoStratupMenu.Checked = false;
               
            }
            else
            {
                reg.SetValue(exeName, exePath);
                autoStratupMenu.Checked = true;
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
           Hide();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Kill();
        }

        private void Kill()
        {
            
            string[] names = new string[] { "trojan", "privoxy" };
            
            var pro = Process.GetProcesses().Where(p=>names.Contains(p.ProcessName.ToLower()));

            foreach (Process p in pro)
            {
                p.Kill();
            }


        }
    }
}
