// CTM.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "header.h"

DWORD Pid;
uint BaseAddress;
HWND hwnd;
float mx,my,mz;

using namespace std;

int main(int argc, char* argv[])
{
	bool NavMode=false;
	int ClickTimer=0;
	int NextCoord=0;
	LoadSetting();
	vector<NAV> StoreCoords;
	while(true)
	{
		if(GetAsyncKeyState(VK_F1))
		{
			NAV coords(mx,my,mz);
			StoreCoords.push_back(coords);
			cout << "Storing: " << " x: " << coords.getx() << " y: " << coords.gety() << " z: " << coords.getz() << endl;
			Sleep(1000);
		}
		if(GetAsyncKeyState(VK_F2))
		{
			if(!NavMode)
			{
				NavMode=true;
			}
			else
			{
				NavMode=false;
			}
			Sleep(500);
		}
		if(NavMode)
		{
			if(ClickTimer==5000)
			{
				ClickToMove(StoreCoords[NextCoord].getx(),StoreCoords[NextCoord].gety(),StoreCoords[NextCoord].getz());
				ClickTimer=0;
			}
			if(distance(StoreCoords[NextCoord].getx(),StoreCoords[NextCoord].gety(),StoreCoords[NextCoord].getz(),mx,my,mz)<=5.0f)
			{
				cout << "Going To: " << " x: " << StoreCoords[NextCoord].getx() << " y: " << StoreCoords[NextCoord].gety() << " z: " << StoreCoords[NextCoord].getz() << endl;
				NextCoord+=1;
			}
			ClickTimer+=1;
			if(NextCoord>=StoreCoords.size())
				NextCoord=0;
		}
	}
	system("pause");
	return 0;
}

void LoadSetting()
{
	Pid = GetPid("Wow.exe");
	BaseAddress = GetBase("Wow.exe",Pid);
	//hwnd = ::FindWindow(NULL, "World of Warcraft");
	CreateThread(NULL,0,(LPTHREAD_START_ROUTINE)ReadCoords,NULL, 0, NULL);
}

UINT ReadCoords()
{
	while(true)
	{
	    uint ObjectPointer;
	    ReadProcMem((LPVOID)(BaseAddress + CurMgrPointer),&ObjectPointer,4);
	    ReadProcMem((LPVOID)(ObjectPointer + CurMgrOffset),&ObjectPointer,4);
	    UINT64 me;
	    ReadProcMem((LPVOID)(ObjectPointer + LocalGUID),&me, 8);
	    ReadProcMem((LPVOID)(ObjectPointer + FirstObject),&ObjectPointer,4);
	    while (ObjectPointer != 0 && ObjectPointer % 2 == 0)
	    {
		    UINT64 cobj;
		    ReadProcMem((LPVOID)(ObjectPointer + 0x30),&cobj, 8);
		    if(me == cobj)
		    {
			    ReadProcMem((LPVOID)(ObjectPointer + X),&mx, 4);
			    ReadProcMem((LPVOID)(ObjectPointer + Y),&my, 4);
			    ReadProcMem((LPVOID)(ObjectPointer + Z),&mz, 4);
		    }
		    ReadProcMem((LPVOID)(ObjectPointer + NextObject),&ObjectPointer,4);
	    }
	}
}

float distance(float x, float y, float z, float dx, float dy, float dz)
{
	float dist_x = x-dx;
	float dist_y = y-dy;
	float dist_z = z-dz;
    float dist = sqrt(dist_x * dist_x + dist_y * dist_y + dist_z * dist_z);
    return dist;
}

void ClickToMove(float x,float y,float z)
{
	uint Action = 0x04;
	float Dist = 0.0f;
	WriteProcMem((LPVOID)(BaseAddress + CTM_Base + CTM_X),&x, 4);
	WriteProcMem((LPVOID)(BaseAddress + CTM_Base + CTM_X + CTM_Y),&y, 4);
	WriteProcMem((LPVOID)(BaseAddress + CTM_Base + CTM_X + CTM_Z),&z, 4);
	WriteProcMem((LPVOID)(BaseAddress + CTM_Base + CTM_Push),&Action, 4);
	WriteProcMem((LPVOID)(BaseAddress + CTM_Base + CTM_Distance),&Dist, 4);
}

void ReadProcMem(void *pAddress, void *pMem, int iSize)
{
	HANDLE hProc = OpenProcess(PROCESS_VM_OPERATION|PROCESS_VM_WRITE|PROCESS_VM_READ, FALSE, Pid);
	DWORD dwOld;
	VirtualProtectEx(hProc, pAddress, iSize, PAGE_EXECUTE_READWRITE, &dwOld);
	ReadProcessMemory(hProc, pAddress, pMem, iSize, 0);
	CloseHandle(hProc);
}

void WriteProcMem(void *pAddress, void *pMem, int iSize)
{
	HANDLE hProc = OpenProcess(PROCESS_ALL_ACCESS, FALSE, Pid);
	DWORD dwOld;
	VirtualProtectEx(hProc, pAddress, iSize, PAGE_EXECUTE_READWRITE, &dwOld);
	WriteProcessMemory(hProc, pAddress, pMem, iSize, 0);
	CloseHandle(hProc);
}

unsigned long GetPid(char *procName)
{
   PROCESSENTRY32 pe;
   HANDLE thSnapshot;
   BOOL retval, ProcFound = false;
   thSnapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
   if(thSnapshot == INVALID_HANDLE_VALUE)
   {
      //MessageBox(NULL, "Error: unable to create toolhelp snapshot","Error", NULL);
      return false;
   }
   pe.dwSize = sizeof(PROCESSENTRY32);
   retval = Process32First(thSnapshot, &pe);
   while(retval)
   {
      if(strcmp(pe.szExeFile, procName) == 0)
      {
         ProcFound = true;
         break;
      }
      retval    = Process32Next(thSnapshot,&pe);
      pe.dwSize = sizeof(PROCESSENTRY32);
   }
   if (!ProcFound) return 0;
   return pe.th32ProcessID;
}

DWORD GetBase(char* DllName, DWORD tPid)
{
    HANDLE snapMod;
    MODULEENTRY32 me32;
    if (tPid == 0) return 0;
    snapMod = CreateToolhelp32Snapshot(TH32CS_SNAPMODULE, tPid);
    me32.dwSize = sizeof(MODULEENTRY32);
    if (Module32First(snapMod, &me32))
	{
        do{
              if (strcmp(DllName,me32.szModule) == 0)
			  {
                  CloseHandle(snapMod);
                  return (DWORD) me32.modBaseAddr;
			  }
		}while(Module32Next(snapMod,&me32));
    }
    CloseHandle(snapMod);
	return 0;
}

NAV::NAV()
{
	x=0;
	y=0;
	z=0;
}

NAV::NAV(float ix,float iy,float iz)
{
	x=ix;
	y=iy;
	z=iz;
}

NAV::~NAV()
{
}

float NAV::getx()const
{
	return x;
}

float NAV::gety()const
{
	return y;
}

float NAV::getz()const
{
	return z;
}