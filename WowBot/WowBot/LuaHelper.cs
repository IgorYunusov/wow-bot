﻿using Magic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WowBot
{
	class LuaHelper
	{
		static Hook MyHook;

		public static void DoString(string command)
		{ // Allocate memory
			var proc = Process.GetProcessesByName("wow")[0];
			var a = MagicHandler.BMWow.OpenProcessAndThread(SProcess.GetProcessFromWindowTitle("World of Warcraft")); //This Opens "World of Warcraft" window

			MyHook = new Hook(proc.Id);

			uint DoStringArg_Codecave = MyHook.Memory.AllocateMemory(Encoding.UTF8.GetBytes(command).Length + 1); // offset:
			uint FrameScript__Execute = 0x819210; // Write value:
			MyHook.Memory.WriteBytes(DoStringArg_Codecave, Encoding.UTF8.GetBytes(command)); // Write the asm stuff for Lua_DoString
			string[] asm = new string[] {
				"mov eax, " + DoStringArg_Codecave,
				"push 0",
				"push eax",
				"push eax",
				"mov eax, " + (uint) FrameScript__Execute, // Lua_DoString
				"call eax",
				"add esp, 0xC",
				"retn",
			};
			// Inject
			MyHook.InjectAndExecute(asm); // Free memory allocated
			MyHook.Memory.FreeMemory(DoStringArg_Codecave);
		}

		public static string GetLocalizedText(string Commandline)
		{ // Command to send using LUA 
			string Command = Commandline; // Allocate memory for command 
			uint Lua_GetLocalizedText_Space = MyHook.Memory.AllocateMemory(Encoding.UTF8.GetBytes(Command).Length + 1); // offset:
			uint ClntObjMgrGetActivePlayerObj = 0x4038F0;
			uint FrameScript__GetLocalizedText = 0x7225E0; // Write command in the allocated memory 
			MyHook.Memory.WriteBytes(Lua_GetLocalizedText_Space, Encoding.UTF8.GetBytes(Command));
			string[] asm = new string[] {
				"call " + (uint) ClntObjMgrGetActivePlayerObj,
				"mov ecx, eax",
				"push -1",
				"mov edx, " + Lua_GetLocalizedText_Space + "",
				"push edx", "call " + (uint) FrameScript__GetLocalizedText,
				"retn",
			};
			// Inject the shit 
			string sResult = Encoding.ASCII.GetString(MyHook.InjectAndExecute(asm)); // Free memory allocated for command 
			MyHook.Memory.FreeMemory(Lua_GetLocalizedText_Space); // Uninstall the hook 
			return sResult;
		}

		public static int GetContainerNumFreeSlots()
		{
			DoString("freeslots = GetContainerNumFreeSlots(0) + GetContainerNumFreeSlots(1) + GetContainerNumFreeSlots(2) + GetContainerNumFreeSlots(3) + GetContainerNumFreeSlots(4)");
			return Convert.ToInt32(GetLocalizedText("freeslots"));
		}
	}
}
