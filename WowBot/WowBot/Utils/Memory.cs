using Binarysharp.MemoryManagement;
using Binarysharp.MemoryManagement.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WowBot
{
	static class Memory
	{
		static MemorySharp process;

		static Memory()
		{
			process = new MemorySharp(ApplicationFinder.FromProcessName("Wow").First());
		}

		public static string ReadString(int offset)
		{
			return process.ReadString(new IntPtr(offset), Encoding.UTF8, false);
		}

		public static float ReadFloat(int offset)
		{
			return process[new IntPtr(offset), false].Read<float>();
		}

		public static int ReadByte(int offset)
		{
			return process[new IntPtr(offset), false].Read<byte>();
		}

		public static int ReadShort(int offset)
		{
			return process[new IntPtr(offset), false].Read<short>();
		}

		public static int ReadInt(int offset)
		{
			return process[new IntPtr(offset), false].Read<int>();
		}

		public static long ReadLong(int offset)
		{
			return process[new IntPtr(offset), false].Read<long>();
		}

		internal static string ReadHexAsString(int offset)
		{
			return ReadLong(offset).ToString("X");
		}

		internal static void Write<T>(int offset, T value)
		{
			process.Write<T>(new IntPtr(offset), value, false);
		}
	}
}
