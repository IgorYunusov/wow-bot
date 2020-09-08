using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MiningBot {
    class ChatWriter {
            [DllImport("user32.dll")]
            public static extern int SendMessage(IntPtr hWnd, uint msg, int wParam, IntPtr lParam);

            private const int VK_CONTROL = 0xA2;
            private const int WM_KEYDOWN = 0x100;
            private const int WM_KEYUP = 0x101;
            private const int VK_RETURN = 0x0D;

            public const int ZERO   = 48;
            public const int ONE    = 49;
            public const int TWO    = 50;
            public const int THREE  = 51;
            public const int FOUR   = 52;
            public const int FIVE   = 53;
            public const int SIX    = 54;
            public const int SEVEN  = 55;
            public const int EIGTH  = 56;
            public const int NINE   = 57;

            public const int V = 86;
            public const int W = 87;
            public const int I = 73;
            public const int J = 74;
            public const int K = 75;
            public const int B = 66;
            public const int F = 70;
            public const int G = 71;
            public const int S = 83;
            public const int U = 85;
            public const int Z = 90;
 
            public static void send(IntPtr hWnd, string slashCommand){
                Object savedClipboard = Clipboard.GetDataObject();
                Clipboard.SetText(slashCommand);
 
                SendMessage(hWnd, WM_KEYDOWN, VK_RETURN, IntPtr.Zero);
                SendMessage(hWnd, WM_KEYUP, VK_RETURN, IntPtr.Zero);

                SendMessage(hWnd, WM_KEYDOWN, VK_CONTROL, IntPtr.Zero);
                SendMessage(hWnd, WM_KEYDOWN, 0x56, IntPtr.Zero);
                SendMessage(hWnd, WM_KEYUP, 0x56, IntPtr.Zero);
                SendMessage(hWnd, WM_KEYUP, VK_CONTROL, IntPtr.Zero);

                SendMessage(hWnd, WM_KEYDOWN, VK_RETURN, IntPtr.Zero);
                SendMessage(hWnd, WM_KEYUP, VK_RETURN, IntPtr.Zero);
 
                Clipboard.SetDataObject(savedClipboard);
            }

            public static void hitKey(int key) {
                IntPtr hWnd = MemoryHandler.process.MainWindowHandle;
                SendMessage(hWnd, WM_KEYDOWN, key, IntPtr.Zero);
                //Thread.Sleep(210);
                SendMessage(hWnd, WM_KEYUP, key, IntPtr.Zero);
            }
        }
    
}
