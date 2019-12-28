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

		public static string ReadString(uint offset)
		{
			return process.ReadString(new IntPtr(offset), Encoding.UTF8, false);
		}

		internal static T Read<T>(uint address)
		{
			return process[new IntPtr(address), false].Read<T>();
		}

		internal static void Write<T>(uint address, T value)
		{
			process.Write<T>(new IntPtr(address), value, false);
		}
	}
}
