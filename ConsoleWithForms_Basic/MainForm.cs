using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleWithForms_Basic
{
    public partial class MainForm : Form
    {
        public const int WM_QUERYENDSESSION = 0x0011;
        public const int WM_ENDSESSION = 0x0016;
        public const uint SHUTDOWN_NORETRY = 0x00000001;

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool ShutdownBlockReasonCreate(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)] string reason);
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool ShutdownBlockReasonDestroy(IntPtr hWnd);
        [DllImport("kernel32.dll")]
        static extern bool SetProcessShutdownParameters(uint dwLevel, uint dwFlags);

        public MainForm()
        {
            InitializeComponent();
            // Define the priority of the application (0x3FF = The higher priority)
            SetProcessShutdownParameters(0x3FF, SHUTDOWN_NORETRY);
        }

        private void InitializeComponent()
        {
            Text = "ShutDownBlockerForm";
            Width = 0;
            Height = 0;
            ShowIcon = false;
            ShowInTaskbar = false;
            Opacity = 0.0;
            Visible = false;
            ControlBox = false;
            MaximizeBox = false;
            MinimizeBox = false;
            FormBorderStyle = FormBorderStyle.None;
        }

        protected override void WndProc(ref Message m)
        {
            Console.WriteLine($"Window message received {m.Msg} {m.LParam} {m.WParam}");

            if (m.Msg == WM_QUERYENDSESSION || m.Msg == WM_ENDSESSION)
            {
                // Prevent windows shutdown
                ShutdownBlockReasonCreate(this.Handle, "I want to live!");
                ThreadPool.QueueUserWorkItem(o =>
                {
                    // Simulate some work
                    Thread.Sleep(5000);
                    this.BeginInvoke((Action)(() =>
                    {
                        // This method must be called on the same thread as the one that have create the Handle, so use BeginInvoke
                        ShutdownBlockReasonCreate(this.Handle, "Now I must die!");

                        // Allow Windows to shutdown
                        ShutdownBlockReasonDestroy(this.Handle);
                    }));
                });

                return;
            }

            base.WndProc(ref m);
        }

    }
}
