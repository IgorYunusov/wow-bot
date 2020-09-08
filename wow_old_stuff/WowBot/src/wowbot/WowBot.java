package wowbot;

import java.lang.reflect.Field;
import java.util.logging.Level;
import java.util.logging.Logger;
import org.xvolks.jnative.*;
import org.xvolks.jnative.exceptions.NativeException;
import org.xvolks.jnative.misc.basicStructures.HANDLE;
import org.xvolks.jnative.pointers.Pointer;
import org.xvolks.jnative.pointers.memory.MemoryBlockFactory;
import org.xvolks.jnative.util.Kernel32;
import sun.misc.Unsafe;

public class WowBot {

    public static void main(String[] args) {
            /*
        Unsafe unsafe = null;
 
        try {
            Field field = sun.misc.Unsafe.class.getDeclaredField("theUnsafe");
            field.setAccessible(true);
            unsafe = (sun.misc.Unsafe) field.get(null);
        } catch (Exception e) {
            throw new AssertionError(e);
        }
 
        long value = 12345;
        byte size = 1;
        long allocateMemory = unsafe.allocateMemory(size);
        unsafe.putAddress(allocateMemory, value);
        long readValue = unsafe.getAddress(allocateMemory);
        System.out.println(allocateMemory);
        System.out.println("read value : " + readValue);*/
        
        try {
            HANDLE processHandle = Kernel32.OpenProcess(Kernel32.PROCESS_QUERY_INFORMATION, false, 3520);
            JNative readProcessMemory = new JNative("Kernel32.dll", "ReadProcessMemory");
            
            int b = 128; 
            Pointer p = new Pointer(MemoryBlockFactory.createMemoryBlock(b));
            int base = 16777216;
            
            readProcessMemory.setRetVal(Type.INT);
            readProcessMemory.setParameter(0,processHandle.getValue().intValue());
            readProcessMemory.setParameter(1, base);
            readProcessMemory.setParameter(2, p);
            readProcessMemory.setParameter(3, b);

            for (int x = 0; x < 100; x++) { //loop thru the first 128*100 bytes of app memory, starting at base address
                readProcessMemory.setParameter(1, base + x * b); // move the pointer to the start of the next 128 byte chunk
                readProcessMemory.invoke();
                if (readProcessMemory.getRetValAsInt() == 0) { // 0: Good, 1: read failed, most likely memnory access rights
                    System.out.println("Read fail - check access rights: " + x * b);
                    //break;
                }
            }
            
        } catch (NativeException ex) {
            Logger.getLogger(WowBot.class.getName()).log(Level.SEVERE, null, ex);
        } catch (IllegalAccessException ex) {
            Logger.getLogger(WowBot.class.getName()).log(Level.SEVERE, null, ex);
        }
    }
}

