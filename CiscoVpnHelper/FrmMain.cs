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
        private readonly AutoLoginUtil util = new AutoLoginUtil();
        private Label lblPwd;
        private TextBox txtPwd;
        private Button btnAutoLogin;
        private Timer timer;
        private NotifyIcon notifyIcon;

        public FrmMain()
        {
            InitializeComponent();
            this.InitUI();
            this.InitLogic();
        }

        private void InitLogic()
        {
            this.txtPwd.Text = this.util.readPasswordFromConfig();
            this.txtPwd.TextChanged += (sender, e) => { this.util.savePasswordFromConfig(this.txtPwd.Text); };

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
                this.util.clickConnectButton();
                this.util.inputPassword(this.txtPwd.Text);
                this.util.clickAccept();
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