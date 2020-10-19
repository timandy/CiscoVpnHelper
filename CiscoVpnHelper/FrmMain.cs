using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32;

namespace CiscoVpnHelper
{
    public partial class FrmMain : Form
    {
        private Label lblPwd;
        private TextBox txtPwd;
        private Button btnAutoLogin;
        private Timer timer;
        private NotifyIcon notifyIcon;
        private readonly ISet<IntPtr> handledPwdWnd = new HashSet<IntPtr>();

        public FrmMain()
        {
            InitializeComponent();
            this.InitUI();
            this.InitLogic();
        }

        private void InitLogic()
        {
            string filePath = "./password.txt";
            if (File.Exists(filePath))
                this.txtPwd.Text = File.ReadAllText(filePath, Encoding.ASCII);
            this.txtPwd.TextChanged += (sender, e) => { File.WriteAllText(filePath, this.txtPwd.Text, Encoding.ASCII); };

            //按钮
            this.btnAutoLogin.Text = this.timer.Enabled ? "已启用" : "已禁用";
            this.btnAutoLogin.Click += (sender, e) =>
            {
                this.timer.Enabled = !this.timer.Enabled;
                this.btnAutoLogin.Text = this.timer.Enabled ? "已启用" : "已禁用";
            };

            //定时器
            this.timer.Tick += (sender, e) =>
            {
                IntPtr hwnd = UnsafeNativeMethods.FindWindow("#32770", "Cisco AnyConnect | Haier-ChinaUnicom");
                if (hwnd == IntPtr.Zero)
                    return;
                IntPtr hwndLblPwd = UnsafeNativeMethods.FindWindowEx(hwnd, IntPtr.Zero, "Static", "Password:");
                if (hwndLblPwd == IntPtr.Zero)
                    return;
                IntPtr hwndTxtPwd = UnsafeNativeMethods.FindWindowEx(hwnd, hwndLblPwd, "Edit", null);
                if (hwndTxtPwd == IntPtr.Zero)
                    return;
                if (this.handledPwdWnd.Contains(hwndTxtPwd))
                    return;

                UnsafeNativeMethods.SetWindowText(hwndTxtPwd, string.Empty);
                foreach (char ch in this.txtPwd.Text)
                {
                    UnsafeNativeMethods.SendMessage(hwndTxtPwd, NativeMethods.WM_CHAR, (IntPtr) ch, null);
                }

                this.handledPwdWnd.Add(hwndTxtPwd);
            };


            this.notifyIcon.DoubleClick += (sender, e) => { this.Show(); };
        }


        private void InitUI()
        {
            this.SuspendLayout();
            //
            this.lblPwd = new Label
            {
                Text = "密码:",
                Location = new Point(20, 10),
                AutoSize = true
            };
            this.Controls.Add(this.lblPwd);
            //
            this.txtPwd = new TextBox
            {
                Location = new Point(60, 8),
                Size = new Size(150, 20)
            };
            this.Controls.Add(this.txtPwd);
            //
            this.btnAutoLogin = new Button();
            this.btnAutoLogin.Size = new Size(60, 25);
            this.btnAutoLogin.Location = new Point(60, 60);
            this.Controls.Add(this.btnAutoLogin);
            //
            this.timer = new Timer();
            this.timer.Interval = 1000;
            this.timer.Enabled = true;
            //
            this.notifyIcon = new NotifyIcon(this.components);
            this.notifyIcon.Icon = this.Icon;
            this.notifyIcon.Visible = true;
            //
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Size = new Size(300, 200);
            //
            this.ResumeLayout(true);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}