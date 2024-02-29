using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HellDivers2Hack_POC
{
    internal class Program
    {
        

        static void Main(string[] args)
        {
            Console.WriteLine("#Helldivers 2 Hack PoC ( Converting Auto Assembly CE into C# )\n\n");

            string targetProcessName = "helldivers2";
            string targetModuleName = "game.dll";

            Process targetProcess;

            Console.WriteLine("Find HellDivers2 Process..");
            while (true)
            {
                targetProcess = Memory.GetProcessByName(targetProcessName);

                // Check if the process is running
                if (targetProcess != null)
                    break;

                // Sleep for a while before checking again
                Thread.Sleep(1000);
            }
            Console.WriteLine("HellDivers2 Process Found.");

            while (true)
            {
                if (targetProcess != null)
                {
                    // Process found, now check for the module
                    ProcessModule targetModule = Memory.GetModuleByName(targetProcess, targetModuleName);
                    Console.WriteLine("Find game.dll Module..");
                    if (targetModule == null)
                    {
                        targetProcess = Memory.GetProcessByName(targetProcessName);
                    }
                    if (targetModule != null)
                    {
                        Console.WriteLine("Module game.dll Found.");
                        Console.WriteLine($"Process {targetProcessName} found with module {targetModuleName}.");

                        // Get base address and size of the module
                        IntPtr baseAddress = targetModule.BaseAddress;
                        int moduleSize = targetModule.ModuleMemorySize;

                        Console.WriteLine($"Module Base Address: 0x{baseAddress.ToInt64():X}");
                        Console.WriteLine($"Module Size: {moduleSize} bytes");
                        Console.WriteLine("Start Hack Process..");

                    //Invulnerable
                    RetryScanPatern: // ( Label Needed to wait bytes is ready )
                        IntPtr offset = Memory.FindPatternOffset(targetProcess, baseAddress, moduleSize, "45 89 38 49 8B 84 DE 28 04 00 00 8B 48 10");
                        if (offset != IntPtr.Zero)
                        {
                            IntPtr allocatedMemory = Memory.AllocateMemoryNearOffset(targetProcess, baseAddress, offset, 0x1000); // Allocate memory near the offset | Adjust the size as needed
                            if (allocatedMemory != IntPtr.Zero)
                            {
                                Memory.CreateTrampoline(targetProcess, baseAddress, offset, allocatedMemory);
                                Memory.WriteAssemblyInstructions(targetProcess, allocatedMemory, offset, ByteArrayAsmInstruction.Invulnerable, 14);
                                Console.WriteLine("Invulnerable Active.");
                            }
                            else
                            {
                                Console.WriteLine("Failed to allocate memory.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Pattern Invulnerable not found in the module.");
                            goto RetryScanPatern;
                        }

                        // Max Resources
                        offset = Memory.FindPatternOffset(targetProcess, baseAddress, moduleSize, "45 01 B4 8A EC 17 00 00");
                        if (offset != IntPtr.Zero)
                        {
                            IntPtr allocatedMemory = Memory.AllocateMemoryNearOffset(targetProcess, baseAddress, offset, 0x1000); // Adjust the size as needed | Allocate memory near the offset

                            if (allocatedMemory != IntPtr.Zero)
                            {
                                Memory.CreateTrampoline(targetProcess, baseAddress, offset, allocatedMemory);
                                Memory.WriteAssemblyInstructions(targetProcess, allocatedMemory, offset, ByteArrayAsmInstruction.MaxResources, 17);
                                Console.WriteLine("Max Resources Active.");
                            }
                            else
                            {
                                Console.WriteLine("Failed to allocate memory.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Pattern Max Resources not found in the module.");
                        }

                        //No Reload
                        offset = Memory.FindPatternOffset(targetProcess, baseAddress, moduleSize, "41 89 28 49 8B 84 CA 28 20 00 00 8B 48 10");
                        if (offset != IntPtr.Zero)
                        {
                            IntPtr allocatedMemory = Memory.AllocateMemoryNearOffset(targetProcess, baseAddress, offset, 0x1000); // Adjust the size as needed | Allocate memory near the offset

                            if (allocatedMemory != IntPtr.Zero)
                            {
                                Memory.CreateTrampoline(targetProcess, baseAddress, offset, allocatedMemory);
                                Memory.WriteAssemblyInstructions(targetProcess, allocatedMemory, offset, ByteArrayAsmInstruction.NoReload, 14);
                                Console.WriteLine("No Reload Active.");
                            }
                            else
                            {
                                Console.WriteLine("Failed to allocate memory.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Pattern No Reload not found in the module.");
                        }

                        // Inf Health
                        offset = Memory.FindPatternOffset(targetProcess, baseAddress, moduleSize, "41 8B 84 8B 28 4C 00 00 48 8B 5C 24 20 48 8B 74 24 28");
                        if (offset != IntPtr.Zero)
                        {
                            IntPtr allocatedMemory = Memory.AllocateMemoryNearOffset(targetProcess, baseAddress, offset, 0x1000); // Adjust the size as needed || Allocate memory near the offset

                            if (allocatedMemory != IntPtr.Zero)
                            {
                                Memory.CreateTrampoline(targetProcess, baseAddress, offset, allocatedMemory);
                                Memory.WriteAssemblyInstructions(targetProcess, allocatedMemory, offset, ByteArrayAsmInstruction.InfHealth, 18);
                                Console.WriteLine("Infinite Health Active.");
                            }
                            else
                            {
                                Console.WriteLine("Failed to allocate memory.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Pattern Inf Health not found in the module.");
                        }

                        //Inf Granades
                        offset = Memory.FindPatternOffset(targetProcess, baseAddress, moduleSize, "41 FF 08 4A 8B 84 ED");

                        if (offset != IntPtr.Zero)
                        {
                            Memory.NopAddress(targetProcess, offset, 3);
                            Console.WriteLine("Granades Active.");
                        }
                        else
                        {
                            Console.WriteLine("Pattern Granades not found in the module.");
                        }

                        //Inf Ammo
                        offset = Memory.FindPatternOffset(targetProcess, baseAddress, moduleSize, "41 83 2C C2 01 4D 8D 04 C2 49 8B 84 CA");

                        if (offset != IntPtr.Zero)
                        {
                            Memory.PatchAddress(targetProcess, IntPtr.Add(offset, 4), "00");
                            Console.WriteLine("Ammo Active.");
                        }
                        else
                        {
                            Console.WriteLine("Pattern Ammo not found in the module.");
                        }

                        //Inf Syringes
                        offset = Memory.FindPatternOffset(targetProcess, baseAddress, moduleSize, "41 FF CF 3B C2 74 61");

                        if (offset != IntPtr.Zero)
                        {
                            Memory.NopAddress(targetProcess, offset, 3);
                            Console.WriteLine("Syringes Active.");
                        }
                        else
                        {
                            Console.WriteLine("Pattern Syringes not found in the module.");
                        }

                        //Inf Stamina
                        offset = Memory.FindPatternOffset(targetProcess, baseAddress, moduleSize, "F3 0F 5F C8 F3 41 0F 11 08");

                        if (offset != IntPtr.Zero)
                        {
                            Memory.NopAddress(targetProcess, IntPtr.Add(offset, 4), 5);
                            Console.WriteLine("Stamina Active.");
                        }
                        else
                        {
                            Console.WriteLine("Pattern Stamina not found in the module.");
                        }

                        //Inf Strategems
                        offset = Memory.FindPatternOffset(targetProcess, baseAddress, moduleSize, "C0 F3 48 0F 2C C8 48 03 48 18 48 89 8C 37 40 02 00 00");

                        if (offset != IntPtr.Zero)
                        {
                            Memory.NopAddress(targetProcess, IntPtr.Add(offset, 6), 4);
                            Console.WriteLine("Strategems Active.");
                        }
                        else
                        {
                            Console.WriteLine("Pattern Strategems not found in the module.");
                        }

                        //Inf Time Mission
                        offset = Memory.FindPatternOffset(targetProcess, baseAddress, moduleSize, "F3 43 0F 11 84 F4 88 64 03 00");

                        if (offset != IntPtr.Zero)
                        {
                            Memory.NopAddress(targetProcess, offset, 10);
                            Console.WriteLine("Time Mission Active.");
                        }
                        else
                        {
                            Console.WriteLine("Pattern Time Mission not found in the module.");
                        }

                        Console.WriteLine("Happy Cheating.......");
                        Console.ReadLine();


                    }
                }
            }
        }
    }
}
