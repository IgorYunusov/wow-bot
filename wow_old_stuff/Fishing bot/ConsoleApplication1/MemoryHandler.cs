using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;


namespace FishingBot {
    class MemoryHandler {
        [DllImport("kernel32.dll")]
        public static extern int OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess, int lpBaseAdress, byte[] buffer, int size, int lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory(int hProcess, int lpBaseAdress, byte[] buffer, int size, int lpNumberOfBytesWritten);

        private static int processHandle;

        //ez azért public static mert máshonnan is el akarom érni
        public static Process process;

        public static void createReader() {
            Process[] p = Process.GetProcessesByName("wow");
            process = p[0];

            uint DELETE = 0x00010000;
            uint READ_CONTROL = 0x00020000;
            uint WRITE_DAC = 0x00040000;
            uint WRITE_OWNER = 0x00080000;
            uint SYNCHRONIZE = 0x00100000;
            uint END = 0xFFF;
            uint PROCESS_ALL_ACCESS = (DELETE | READ_CONTROL | WRITE_DAC | WRITE_OWNER | SYNCHRONIZE | END);

            processHandle = OpenProcess(PROCESS_ALL_ACCESS, false, process.Id);
        }

        public static String readHexAsString(int address) {
            byte[] bytesRead = ReadMemory(address, 8, processHandle);
            Int64 result = BitConverter.ToInt64(bytesRead, 0);
            return result.ToString("X");
        }

        public static String readString(int address, int length) {
            byte[] bytesRead = ReadMemory(address, length, processHandle);
            string result = Encoding.Unicode.GetString(bytesRead);
            /*while (true) {
                System.Console.Clear();
                System.Console.WriteLine(result);
                Thread.Sleep(200);
            }*/
            return result;
        }

        public static uint readUint(int address) {
            byte[] bytesRead = ReadMemory(address, 4, processHandle);
            return BitConverter.ToUInt32(bytesRead, 0);
        }

        public static Int64 readHexAsInt64(int address) {
            byte[] bytesRead = ReadMemory(address, 8, processHandle);
            return BitConverter.ToInt64(bytesRead, 0);
        }

        public static float readFloat(int address) {
            byte[] bytesRead = ReadMemory(address, 4, processHandle);
            return BitConverter.ToSingle(bytesRead, 0);
        }

        public static int readInt(int address) {
            return BitConverter.ToInt32(ReadMemory(address, 4, processHandle), 0);
        }

        public static byte readByte(int address) {
            return ReadMemory(address, 2, processHandle)[0];
        }

        public static Int16 readInt16(int address) {
            return BitConverter.ToInt16(ReadMemory(address, 2, processHandle), 0);
        }

        public static Int64 readInt64(int address) {
            return BitConverter.ToInt64(ReadMemory(address, 8, processHandle), 0);
        }

        public static double readDouble(int address) {
            return BitConverter.ToDouble(ReadMemory(address, 8, processHandle), 0);
        }

        public static void writeInt16(int address, Int16 value) {
            WriteMemory(address, BitConverter.GetBytes(value), processHandle);
        }

        public static void writeInt(int address, int value) {
            WriteMemory(address, BitConverter.GetBytes(value), processHandle);
        }

        public static void writeInt64(int address, Int64 value) {
            WriteMemory(address, BitConverter.GetBytes(value), processHandle);
        }

        public static void writeFloat(int address, float value) {
            WriteMemory(address, BitConverter.GetBytes(value), processHandle);
        }

        public static byte[] ReadMemory(int address, int processSize, int processHandle) {
            byte[] buffer = new byte[processSize];
            ReadProcessMemory(processHandle, address, buffer, processSize, 0);
            return buffer;
        }

        public static void WriteMemory(int adress, byte[] processBytes, int processHandle) {
            WriteProcessMemory(processHandle, adress, processBytes, processBytes.Length, 0);
        }

        /*
        public static int GetObjectSize(object TestObject) {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            byte[] Array;
            bf.Serialize(ms, TestObject);
            Array = ms.ToArray();
            return Array.Length;
        }*/
    }
}
