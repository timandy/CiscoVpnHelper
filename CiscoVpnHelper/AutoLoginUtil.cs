using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using Microsoft.Win32;

namespace CiscoVpnHelper
{
    public class AutoLoginUtil
    {
        private readonly ISet<IntPtr> handledPwdWnd = new HashSet<IntPtr>();
        private const string configFilePath = "./password.txt";

        //从配置读取密码
        public string readPasswordFromConfig()
        {
            if (!File.Exists(configFilePath))
                return string.Empty;

            return File.ReadAllText(configFilePath, Encoding.ASCII);
        }

        //将密码写入配置
        public void savePasswordFromConfig(string password)
        {
            File.WriteAllText(configFilePath, password, Encoding.ASCII);
        }

        //关闭网络错误对话框
        public void closeNetworkErrorDialog()
        {
            IntPtr hwnd = UnsafeNativeMethods.FindWindow("#32770", "Cisco AnyConnect");
            if (hwnd == IntPtr.Zero)
                return;
            UnsafeNativeMethods.SendMessage(hwnd, NativeMethods.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            UnsafeNativeMethods.SendMessage(hwnd, NativeMethods.WM_DESTROY, IntPtr.Zero, IntPtr.Zero);
        }

        //单击 Connect
        public void clickConnectButton()
        {
            IntPtr hwnd = UnsafeNativeMethods.FindWindow("#32770", "Cisco AnyConnect Secure Mobility Client");
            if (hwnd == IntPtr.Zero)
                return;
            IntPtr hwndPanel = UnsafeNativeMethods.FindWindowEx(hwnd, IntPtr.Zero, "#32770", null);
            if (hwndPanel == IntPtr.Zero)
                return;
            IntPtr hwndConnect = UnsafeNativeMethods.FindWindowEx(hwndPanel, IntPtr.Zero, "Button", "Connect");
            if (hwndConnect == IntPtr.Zero)
                return;
            bool isConnectEnabled = Util.GetEnabled(hwndConnect);
            if (!isConnectEnabled)
                return;
            Util.SendMouseClick(hwndConnect, Point.Empty);
        }

        //输入密码
        public void inputPassword(string password)
        {
            IntPtr hwnd = UnsafeNativeMethods.FindWindow("#32770", "Cisco AnyConnect | Haier-ChinaUnicom");
            if (hwnd == IntPtr.Zero)
                return;
            try
            {
                //防止多次输入
                if (this.handledPwdWnd.Contains(hwnd))
                    return;
                this.handledPwdWnd.Add(hwnd);

                IntPtr hwndLblPwd = UnsafeNativeMethods.FindWindowEx(hwnd, IntPtr.Zero, "Static", "Password:");
                if (hwndLblPwd == IntPtr.Zero)
                    return;
                IntPtr hwndTxtPwd = UnsafeNativeMethods.FindWindowEx(hwnd, hwndLblPwd, "Edit", null);
                if (hwndTxtPwd == IntPtr.Zero)
                    return;

                //输入密码
                foreach (char ch in password)
                {
                    UnsafeNativeMethods.SendMessage(hwndTxtPwd, NativeMethods.WM_CHAR, (IntPtr) ch, null);
                }

                //单击OK
                IntPtr hwndOK = UnsafeNativeMethods.FindWindowEx(hwnd, IntPtr.Zero, "Button", "OK");
                if (hwndTxtPwd == IntPtr.Zero)
                    return;
                bool isOKEnabled = Util.GetEnabled(hwndOK);
                if (!isOKEnabled)
                    return;
                Util.SendMouseClick(hwndOK, Point.Empty);
            }
            finally
            {
                //防止点击按钮失效,卡在此界面
                UnsafeNativeMethods.SendMessage(hwnd, NativeMethods.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                UnsafeNativeMethods.SendMessage(hwnd, NativeMethods.WM_DESTROY, IntPtr.Zero, IntPtr.Zero);
            }
        }

        //单击 Accept
        public void clickAccept()
        {
            IntPtr hwnd = UnsafeNativeMethods.FindWindow("#32770", "Cisco AnyConnect");
            if (hwnd == IntPtr.Zero)
                return;
            IntPtr hwndAccept = UnsafeNativeMethods.FindWindowEx(hwnd, IntPtr.Zero, "Button", "Accept");
            if (hwndAccept == IntPtr.Zero)
                return;
            bool isAcceptEnabled = Util.GetEnabled(hwndAccept);
            if (!isAcceptEnabled)
                return;
            Util.SendMouseClick(hwndAccept, Point.Empty);
        }
    }
}