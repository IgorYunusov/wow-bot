# AmeisenHook

This is the class that manages the hookig of WoW's D3D9 Endscene function. We will hook it to execute functions on WoW's main thread. This funtion is called after every frame, so if we're at a framerate of 60 we have 60 executions per second which is enough for what it is beeing used for at the moment.

* * *

## Preperation

### Getting the endscene address

First we need to know where to apply our memory-modification (where the endscene is located).

Example code with 3.3.5a offsets:

```C#
public readonly uint devicePtr1 = 0xC5DF88;
public readonly uint devicePtr2 = 0x397C;
public readonly uint endScene = 0xA8;

uint pDevice = BlackMagic.ReadUInt(Offsets.devicePtr1);
uint pEnd = BlackMagic.ReadUInt(pDevice + Offsets.devicePtr2);
uint pScene = BlackMagic.ReadUInt(pEnd);
uint endscene = BlackMagic.ReadUInt(pScene + Offsets.endScene);
```

* * *

### Using the found offset

Now that we know where our functions is located we need to search for the first thing with a size of 5 bytes. Why 5 bytes, thats because we're going to place a JMP instruction at this place and as we know a JMP plus Address to jump to are 1 + 4 bytes on a 32Bit system.

So lets do it, our memory at the endscene address should look like this:

```nasm
PUSH 18 // 2 bytes
MOV EAX, 64CAD751 // 5 bytes, this we're going to replace
CALL 676EBDB9 // here we will return after our stuff is done
MOV EDI, DWORD PTR SS:[EBP+8]
MOV EBX, EDI
...
```

As you might see there is something that's 5 bytes big, but it's offsetted by 0x2. That's no problem for us, we will just add that on top of our endscene

```C#
public const uint ENDSCENE_OFFSET = 0x2;

uint endscene += ENDSCENE_OFFSET;
```

So far so good, we know where to apply our hook but how do we do it, what are we going to do?

* Jump from endscene function to our own instructions
* Check if there is anything to execute
* If there is something, execute it otherwise do nothing
* After that return to the original like nothing happened

* * *

### Codecaves

Enough thinking, lets get down to business. First we need a place to store our Instrctions in the memory of WoW. 

```C#
uint codeCave = BlackMagic.AllocateMemory(64);
uint codeCaveForInjection = BlackMagic.AllocateMemory(256);
```

**codeCave**: will be the place where we check if there are instructions to run.

**codeCaveForInjection**: will be the place where we place our instructions that we want to execute

<br>

So far so good but how will we know that the code was executed? And what about return values? Don't worry its simple, just reserve two integers in WoW's memory.

```C#
uint codeToExecute = BlackMagic.AllocateMemory(4);                        
uint returnAdress = BlackMagic.AllocateMemory(4);

BlackMagic.WriteInt(codeToExecute, 0);
BlackMagic.WriteInt(returnAdress, 0);
```

**codeToExecute**: will hold 1 or 0 whether there is or isn't anything to execute

**returnAdress**: will be a pointer to the data returned by the function we called

We're finished with our preperational stuff.

* * *

## Assembly instructions

### Check for instructions to execute

By now we've got everything prepared for our hook, so let's crawl through some Assembly.

Step one, the code that checks for instructions to be executed, we don't want to change anything that WoW can continue to work after we've done our stuff. Thats a problem because we will change something, there's no way of not doing that. So what wre we going to do about that? *PUSHFD*, *POPFD*, *PUSHAD* & *POPAD*, that are the instructions we need.

**Simplified explanation**:

*PUSHFD* & *PUSHAD* will save our registers

*POPFD* & *POPAD* will restore our registers

```nasm
PUSHFD
PUSHAD

// our instructions will end up here

POPAD
POPFD
```

<br>

Now we're able to start the implementation of the "check for instructions" part.

**There are 9 steps to do that i've broke down into 3x3:**

1. Load the value of *codeToExecute* into EBX
2. Compare EBX (the value of *codeToExecute*) to 1
3. If there is no code jump to @out (we will declare it later)

**{(codeToExecute)}**: replace that with you *codeToExecute* variable

```nasm
MOV EBX, [{(codeToExecute)}]
TEST EBX, 1
JE @out
```

<br>

4. Load the instructions position into EDX
5. Execute our custom instructions
6. Set the value of our *returnAdress* to the value of EAX
    -> at this point EAX contains our return-value pointer and we want to save that for later

**{(codeCaveForInjection)}**: replace that with you *codeCaveForInjection* variable

**{(returnAdress)}**: replace that with you *returnAdress* variable

```nasm
MOV EDX, {(codeCaveForInjection)}
CALL EDX
MOV [{(returnAdress)}], EAX
```

<br>

7. Declare our @out anker that we will jump to if there is nothing to execute
8. Set EDX to 0
9. set the value of *codeToExecute* to EDX (0) to signal that we are done

**{(codeToExecute)}**: replace that with you *codeToExecute* variable

```nasm
@out:
MOV EDX, 0
MOV [{(codeToExecute)}], EDX
```

<br>

Your finished code should now look something like this:

```nasm
PUSHFD
PUSHAD

MOV EBX, [{(codeToExecute)}]
TEST EBX, 1
JE @out
MOV EDX, {(codeCaveForInjection)}
CALL EDX
MOV [{(returnAdress)}], EAX
@out:
MOV EDX, 0
MOV [{(codeToExecute)}], EDX

POPAD
POPFD
```

<br>

These Instructions will be placed in the first *codeCave* that we've allocated, you can inject it using Blackmagic like this:

```C#
BlackMagic.Asm.Clear();

foreach (string s in asm)
{
    BlackMagic.Asm.AddLine(s);
}

int asmLenght = BlackMagic.Asm.Assemble().Length;
BlackMagic.Asm.Inject(codeCave);
```

<br>

If you've done all of this there're only two things left to be done:

1. Execute the original endscene stuff
2. Return to the original function

```C#
BlackMagic.WriteBytes(codeCave + (uint)asmLenght, originalEndscene);

BlackMagic.Asm.Clear();
BlackMagic.Asm.AddLine($"JMP {(endsceneReturnAddress)}");
BlackMagic.Asm.Inject((codeCave + (uint)asmLenght) + 5);
```

<br>

First we are going to place the original EndScene instructions below our injected ones. You should  be able to read the original bytes like this:

```C#
byte[] originalEndscene = BlackMagic.ReadBytes(endscene, 5);
```

or use mine (they might not work for you, first method is safer)

```C#
byte[] originalEndscene = new byte[] { 0xB8, 0x51, 0xD7, 0xCA, 0x64 };
```

<br>

After this we want to return to the place where we came from:

```nasm
JMP {(endsceneReturnAddress)}
```
```C#
uint endsceneReturnAddress = endscene + 5;
```

Now we will return after the instruction that we will replace in a moment to finally hook WoW's EndScene. 

<br>

Let's replace the original instructions with the one that jumps to our codeCave.

```nasm
JMP {(codeCave)}
```
```C#
BlackMagic.Asm.Clear();
BlackMagic.Asm.AddLine($"JMP {(codeCave)}");
BlackMagic.Asm.Inject(endscene);
```

Congratulations, you hooked WoW's EndScene and it doesn't do anything. But don't worry the hardest part is done, we just need to inject the code we wan't to execute to the *codeCaveForInjection* and set *codeToExecute* to 1.

* * *

### Executing your stuff

To execute your stuff just inject it to the *codeCaveForInjection* and set *codeToExecute* to 1. Remember that you should not inject new code while there is code beeing executed.

```C#
while (BlackMagic.ReadInt(codeToExecute) > 0)
{
    Thread.Sleep(1);
}

BlackMagic.Asm.Clear();
foreach (string s in asm)
{
    BlackMagic.Asm.AddLine(s);
}

BlackMagic.Asm.Inject(codeCaveForInjection);
BlackMagic.WriteInt(codeToExecute, 1);
```

<br>

After you've done that you could for example wait for the stuff-execution to finish and then read the returnAddress for a result.

```C#
while (BlackMagic.ReadInt(codeToExecute) > 0)
{
    Thread.Sleep(1);
}
```

* * *

### Reading the returnAddress

Reading the returnAddress content is pretty easy.

```C#
List<byte> returnBytes = new List<byte>();
byte buffer = new byte();

try
{
    uint dwAddress = BlackMagic.ReadUInt(returnAdress);
    buffer = BlackMagic.ReadByte(dwAddress);

    while (buffer != 0)
    {
        returnBytes.Add(buffer);
        dwAddress = dwAddress + 1;
        buffer = BlackMagic.ReadByte(dwAddress);
    }
}
catch (Exception e)
{
    AmeisenLogger.Instance.Log(
        LogLevel.ERROR,
        $"Crash at reading returnAddress: {e.ToString()}",
        this
    );
}
```

*to be continued...*