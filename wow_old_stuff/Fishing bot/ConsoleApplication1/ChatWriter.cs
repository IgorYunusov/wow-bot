using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FishingBot {
    class ChatWriter {
            [DllImport("user32.dll")]
            public static extern int SendMessage(IntPtr hWnd, uint msg, int wParam, IntPtr lParam);

            private const int VK_CONTROL = 0xA2;
            private const int WM_KEYDOWN = 0x100;
            private const int WM_KEYUP = 0x101;
            private const int VK_RETURN = 0x0D;

            public const int B = 0x42;
 
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
                Thread.Sleep(210);
                SendMessage(hWnd, WM_KEYUP, key, IntPtr.Zero);
            }
        }
    
}
