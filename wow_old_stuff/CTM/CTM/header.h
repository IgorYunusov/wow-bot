#include <windows.h>
#include <iostream>
#include <vector>
#include <math.h>
#include <tlhelp32.h>

typedef unsigned int uint;

enum ObjectManager
{
    CurMgrPointer = 0x879CE0,
    CurMgrOffset = 0x2ED0,
    NextObject = 0x3C,
    FirstObject = 0xAC,
    LocalGUID = 0xC0
};

enum WowObject
{
    X = 0x798,
    Y = X + 0x4,
    Z = X + 0x8,
};

enum CTM
{
	CTM_Base = 0x8A11D8,
	CTM_Push = 0x1C,
	CTM_X = 0x8C,
	CTM_Y = 0x4,
	CTM_Z = 0x8,
	CTM_Distance = 0xC,
};

class NAV
{
public:
	NAV();
	NAV(float,float,float);
	~NAV();
	float getx()const;
	float gety()const;
	float getz()const;
private:
	float x;
	float y;
	float z;
};

void ReadProcMem(void *pAddress, void *pMem, int iSize);
void WriteProcMem(void *pAddress, void *pMem, int iSize);
void ClickToMove(float x,float y,float z);
void LoadSetting();
unsigned long GetPid(char *procName);
DWORD GetBase(char* DllName, DWORD tPid);
UINT ReadCoords();
float distance(float x, float y, float z, float dx, float dy, float dz);