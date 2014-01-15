using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Lemonade
{
    public class SakuraAPI
    {
        public const int BroadcastGhostchange = 1024;
        public const int HWND_BROADCAST = 0xffff;
        public const int WM_COPYDATA = 0x004a;

        private static int message = 0;
        public static int Message
        {
            get
            {
                if (message == 0)
                    message = RegisterWindowMessage("Sakura");
                return message;
            }
        }

        public static void SendBroadcastGhostchange()
        {
            SendNotifyMessage(new IntPtr(HWND_BROADCAST), Message, new IntPtr(BroadcastGhostchange), new IntPtr(Process.GetCurrentProcess().Id));
        }

        public static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == Message)
            {
                switch (wParam.ToInt32())
                {
                    case BroadcastGhostchange:
                        if (lParam.ToInt32() != Process.GetCurrentProcess().Id)
                            SakuraFMO.Reset();
                        break;
                }
                return IntPtr.Zero;
            }

            switch (msg)
            {
                case WM_COPYDATA:
                    COPYDATASTRUCT cds = (COPYDATASTRUCT)Marshal.PtrToStructure(lParam, typeof(COPYDATASTRUCT));
                    if (cds.dwData.ToInt32() == 9801)
                    {
                        byte[] data = new byte[cds.cbData];
                        Marshal.Copy(cds.lpData, data, 0, cds.cbData);
                        SSTP.Proc(data);
                    }
                    break;
            }

            return IntPtr.Zero;
        }

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int RegisterWindowMessage(string lpString);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool SendNotifyMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        private struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            public IntPtr lpData;
        }
    }
}
