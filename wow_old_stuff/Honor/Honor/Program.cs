using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace Honor {
    class Program {
        [DllImport("kernel32.dll")]
        public static extern int OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        public static Process process;
        private static int processHandle;

        public static void createReader() {
            Process[] processlist = Process.GetProcesses();

            foreach(Process theprocess in processlist){
                if (theprocess.ProcessName == "pandashan.dat") {
                    process = theprocess;
                }
            }
                

            uint DELETE = 0x00010000;
            uint READ_CONTROL = 0x00020000;
            uint WRITE_DAC = 0x00040000;
            uint WRITE_OWNER = 0x00080000;
            uint SYNCHRONIZE = 0x00100000;
            uint END = 0xFFF;
            uint PROCESS_ALL_ACCESS = (DELETE | READ_CONTROL | WRITE_DAC | WRITE_OWNER | SYNCHRONIZE | END);

            processHandle = OpenProcess(PROCESS_ALL_ACCESS, false, process.Id);
        }

        static void Main(string[] args) {
            Program.createReader();
            Robot r = new Robot();
            while (true) {
                r.hitKey(Robot.W, 10);
                r.Move(200, 300);
                Thread.Sleep(2000);
            }
        }
    }
}
