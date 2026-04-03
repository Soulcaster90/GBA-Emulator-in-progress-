using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("GBA Emulator Starting...");

        // Load ROM file
        byte[] rom = File.ReadAllBytes("game.gba");

        Console.WriteLine($"ROM Loaded: {rom.Length} bytes");

        // Create CPU
        CPU cpu = new CPU(rom);

        // Run emulator loop
        while (true)
        {
            cpu.Step();
        }
    }
}

class CPU
{
    private byte[] rom;

    // R0-R15 registers
    public uint[] Registers = new uint[16];

    public CPU(byte[] romData)
    {
        rom = romData;

        // Program Counter starts at ROM
        Registers[15] = 0x08000000;
    }

    public void Step()
    {
        uint instruction = Fetch();
        Decode(instruction);
    }

    private uint Fetch()
    {
        uint pc = Registers[15];

        // Convert GBA ROM address to array index
        uint index = pc - 0x08000000;

        if (index + 3 >= rom.Length)
            return 0;

        uint instruction =
            (uint)(rom[index] |
            (rom[index + 1] << 8) |
            (rom[index + 2] << 16) |
            (rom[index + 3] << 24));

        Registers[15] += 4; // move to next instruction

        return instruction;
    }

    private void Decode(uint instruction)
    {
        uint opcode = instruction >> 24;

        Console.WriteLine($"Instruction: 0x{instruction:X8} Opcode: 0x{opcode:X2}");
    }
}