using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ConsoleApplication1{
    class ChatWriter {
            [DllImport("user32.dll")]
            private static extern int SendMessage(IntPtr thWnd, int msg, int wParam, IntPtr lParam);
 
            private const int VK_CONTROL = 0xA2;
            private const int WM_KEYDOWN = 0x100;
            private const int WM_KEYUP = 0x101;
            private const int VK_RETURN = 0x0D;
 
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
        }
    
}
