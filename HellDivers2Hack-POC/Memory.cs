using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellDivers2Hack_POC
{
    public class Memory
    {
        // Function to write the assembly instructions to the allocated memory 
        public static void WriteAssemblyInstructions(Process process, IntPtr allocatedMemory, IntPtr offset, byte[] assemblyBytes, int byteReplaced)
        {
            // Convert the jump target to bytes and write it to the jmp instruction
            byte[] jumpTargetBytes = BitConverter.GetBytes(offset.ToInt64() + byteReplaced);
            byte[] patternToFind = { 0xFF, 0x25, 0x00, 0x00, 0x00, 0x00 };
            int index = FindPatternIndex(assemblyBytes, patternToFind) + patternToFind.Length;
            Array.Copy(jumpTargetBytes, 0, assemblyBytes, index, 8);

            IntPtr bytesWritten;
            NativeMethods.WriteProcessMemory(process.Handle, allocatedMemory, assemblyBytes, (IntPtr)assemblyBytes.Length, out bytesWritten);
        }

        private static int FindPatternIndex(byte[] byteArray, byte[] pattern)
        {
            for (int i = 0; i < byteArray.Length - pattern.Length + 1; i++)
            {
                bool patternFound = true;

                for (int j = 0; j < pattern.Length; j++)
                {
                    if (byteArray[i + j] != pattern[j])
                    {
                        patternFound = false;
                        break;
                    }
                }

                if (patternFound)
                {
                    return i;
                }
            }

            return -1; // Pattern not found
        }

        // Function to get a process by name
        public static Process GetProcessByName(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            return processes.Length > 0 ? processes[0] : null;
        }

        // Function to get a module by name within a process
        public static ProcessModule GetModuleByName(Process process, string moduleName)
        {
            foreach (ProcessModule module in process.Modules)
            {
                if (module.ModuleName.Equals(moduleName, StringComparison.OrdinalIgnoreCase))
                {
                    return module;
                }
            }
            return null;
        }

        // Function to find the offset of a pattern of bytes in the module's memory
        public static IntPtr FindPatternOffset(Process process, IntPtr baseAddress, int moduleSize, string pattern)
        {
            byte[] patternBytes = ParsePattern(pattern);
            byte[] moduleBytes = new byte[moduleSize];
            IntPtr bytesRead;

            // Read the module's memory into the byte array
            if (NativeMethods.ReadProcessMemory(process.Handle, baseAddress, moduleBytes, (IntPtr)moduleSize, out bytesRead))
            {
                for (int i = 0; i < moduleSize - patternBytes.Length; i++)
                {
                    bool patternFound = true;

                    for (int j = 0; j < patternBytes.Length; j++)
                    {
                        if (patternBytes[j] != 0x00 && moduleBytes[i + j] != patternBytes[j])
                        {
                            patternFound = false;
                            break;
                        }
                    }

                    if (patternFound)
                    {
                        // Calculate the offset from the base address
                        IntPtr offset = baseAddress + i;
                        return offset;
                    }
                }
            }

            return IntPtr.Zero;
        }

        static byte[] ParsePattern(string pattern)
        {
            string[] bytes = pattern.Split(' ');

            List<byte> patternBytes = new List<byte>();

            foreach (string byteString in bytes)
            {
                if (byteString == "?")
                {
                    // Wildcard, represented as 0x00 in the pattern
                    patternBytes.Add(0x00);
                }
                else
                {
                    // Parse the hexadecimal string to byte
                    byte byteValue = byte.Parse(byteString, System.Globalization.NumberStyles.HexNumber);
                    patternBytes.Add(byteValue);
                }
            }

            return patternBytes.ToArray();
        }

        // Function to allocate memory near the specified offset
        public static IntPtr AllocateMemoryNearOffset(Process process, IntPtr baseAddress, IntPtr offset, int allocationSize)
        {
            IntPtr allocatedMemory = IntPtr.Zero;

            // Calculate the target address for memory allocation
            IntPtr targetAddress = (IntPtr)(baseAddress.ToInt64() + offset.ToInt64());

            // Round down the target address to the nearest page boundary
            targetAddress = new IntPtr((long)targetAddress & ~(0xFFF));

            // Allocate memory near the target address
            allocatedMemory = NativeMethods.VirtualAllocEx(process.Handle, targetAddress, (IntPtr)allocationSize, NativeMethods.MEM_COMMIT | NativeMethods.MEM_RESERVE, NativeMethods.PAGE_EXECUTE_READWRITE);

            return allocatedMemory;
        }

        // Function to create a trampoline from baseAddress + offset to the new allocated memory
        public static void CreateTrampoline(Process process, IntPtr baseAddress, IntPtr offset, IntPtr trampolineAddress)
        {
            byte[] trampolineBytes = new byte[]
            {
                0xFF, 0x25, 0x00, 0x00, 0x00, 0x00,  // JMP [rip+6]
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Placeholder for the target address
            };

            // Calculate the relative offset to the target address
            long relativeOffset = trampolineAddress.ToInt64();
            BitConverter.GetBytes(relativeOffset).CopyTo(trampolineBytes, 6);

            // Write the jmp instruction to allocatedMemory
            IntPtr bytesWritten;
            NativeMethods.WriteProcessMemory(process.Handle, offset, trampolineBytes, (IntPtr)trampolineBytes.Length, out bytesWritten);
        }

        // Function to NOP the specified address
        public static void NopAddress(Process process, IntPtr targetAddress, int numberOfNops)
        {
            // Create an array of NOP instructions (0x90) based on the specified count
            byte[] nopInstructions = new byte[numberOfNops];
            for (int i = 0; i < numberOfNops; i++)
            {
                nopInstructions[i] = 0x90;
            }

            // Write the NOP instructions to the specified address
            IntPtr bytesWritten;
            NativeMethods.WriteProcessMemory(process.Handle, targetAddress, nopInstructions, (IntPtr)nopInstructions.Length, out bytesWritten);
        }

        // Function to patch the specified address with the byte string
        public static void PatchAddress(Process process, IntPtr targetAddress, string byteString)
        {
            // Convert the hex string to a byte array
            byte[] bytesToPatch = StringToByteArray(byteString);

            // Write the byte array to the specified address
            IntPtr bytesWritten;
            NativeMethods.WriteProcessMemory(process.Handle, targetAddress, bytesToPatch, (IntPtr)bytesToPatch.Length, out bytesWritten);
        }

        // Function to convert a hex string to a byte array
        public static byte[] StringToByteArray(string hex)
        {
            int numberChars = hex.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }
    }
}
