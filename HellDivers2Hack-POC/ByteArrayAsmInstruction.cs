﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellDivers2Hack_POC
{
    public class ByteArrayAsmInstruction
    {
        public static byte[] Invulnerable = new byte[]
        {
            0x90, 0x49, 0x8B, 0x84, 0xDE, 0x28, 0x04, 0x00, 0x00, // mov rax, [r12+rbx*8+00000428]
            0x8B, 0x48, 0x10,             // mov ecx, [rax+10]
            0xFF, 0x25, 0x00, 0x00, 0x00, 0x00,  // JMP [rip+6]
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Placeholder for the target address
        };

        public static byte[] MaxResources = new byte[]
        {
            0x41, 0x81, 0x84, 0x8A, 0xEC, 0x17, 0x00, 0x00, 0xF4, 0x01, 0x00, 0x00,
            0x41, 0x81, 0x84, 0x8A, 0xF0, 0x17, 0x00, 0x00, 0xF4, 0x01, 0x00, 0x00,
            0x41, 0x81, 0x84, 0x8A, 0xF4, 0x17, 0x00, 0x00, 0xF4, 0x01, 0x00, 0x00,
            0xFF, 0x25, 0x00, 0x00, 0x00, 0x00,  // JMP [rip+6]
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Placeholder for the target address
        };

        public static byte[] NoReload = new byte[]
        {
            0x90,
            0x49, 0x8B, 0x84, 0xCA, 0x28, 0x20, 0x00, 0x00,
            0x8B, 0x48, 0x10,
            0xFF, 0x25, 0x00, 0x00, 0x00, 0x00,  // JMP [rip+6]
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Placeholder for the target address
        };

        public static byte[] InfHealth = new byte[]
        {
            0xB8, 0x0F, 0x27, 0x00, 0x00,
            0x41, 0x89, 0x84, 0x8B, 0x28, 0x4C, 0x00, 0x00,
            0x48, 0x8B, 0x5C, 0x24, 0x20,
            0x48, 0x8B, 0x74, 0x24, 0x28,
            0xFF, 0x25, 0x00, 0x00, 0x00, 0x00,  // JMP [rip+6]
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Placeholder for the target address
        };
    }
}
