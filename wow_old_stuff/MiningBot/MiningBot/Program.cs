using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Windows.Forms;
using MouseKeyboardLibrary;
using System.Threading;

namespace ConsoleApplication1 {

    class Program {
        static int CTM_Base = 0x00CA11D8,                      // 3.3.5a 12340
            CTM_X = 0x8C,                               // 3.3.5a 12340
            CTM_Y = 0x90,                               // 3.3.5a 12340
            CTM_Z = 0x94,
            CTM_Action = 0x1C;

        public Program() {
            //WriteMemory(0x323550, Encoding.Unicode.GetBytes("Wow!" + "\n"), processHandle);

//            MouseSimulator.X += 1000;
            
//            MouseSimulator.Click(MouseButton.Right);
//            KeyboardSimulator.KeyPress(Keys.B);

           /* List<int> healths = new List<int>();

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\Franszöá\Desktop\akarmi.txt")) {
                for (int i = 0; i < 100000; ++i) {
                    file.WriteLine(readFloat(0xBD0A58 + 0x1 * i, processHandle));
                }
            }

            int[] healthsInt = healths.ToArray();


            */
            Position playerPos;
            Player character = new Player();

            //character.takeOff();
            character.flyTo(4797.0f, 3551.0f);
            character.flyTo(4795.0f, 3548.0f);

            while (true) {
                System.Console.Clear();
                playerPos = Zone.getRelativePosition(character.getX(), character.getY());
                System.Console.WriteLine("x: " + playerPos.x * 100 + ", y: " + playerPos.y * 100);
                System.Console.WriteLine(character.getX());
                System.Console.WriteLine(character.getY());
                Thread.Sleep(100);
            }

            System.Console.Read();       
        }
        [STAThread]
        static void Main(string[] args) {
            MemoryHandler.createReader();
            System.Console.WriteLine(MemoryHandler.readFloat( ObjectManager.sharedOM().getPlayerAdress() + 0x798));
            new Program();
        }
                


    }
}
