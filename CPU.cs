using System;

class CPU
{
    private Memory memory;

    // R0-R15 registers
    public uint[] Registers = new uint[16];

    // Flags: Negative, Zero, Carry, Overflow
    public bool N, Z, C, V;

    public CPU(byte[] romData)
    {
        memory = new Memory(romData);
        Registers[15] = 0x08000000; // Program Counter start
    }

    public void Step()
    {
        uint instruction = Fetch();
        Decode(instruction);
        PrintRegisters();
    }

    private uint Fetch()
    {
        uint pc = Registers[15];
        uint instruction = memory.Read32(pc);
        Registers[15] += 4; // move to next instruction
        return instruction;
    }

    private void Decode(uint instruction)
    {
        uint opcode = (instruction >> 24) & 0xFF; // top 8 bits

        if ((opcode & 0xF0) == 0xE3) // MOV immediate
        {
            uint rd = (instruction >> 12) & 0xF;
            uint imm = instruction & 0xFF;
            ExecuteMOV(rd, imm);
        }
        else if ((opcode & 0xF0) == 0xE0) // ADD
        {
            uint rd = (instruction >> 12) & 0xF;
            uint rn = (instruction >> 16) & 0xF;
            uint rm = instruction & 0xF; // simplified: rm = lower 4 bits
            ExecuteADD(rd, rn, rm);
        }
        else if ((opcode & 0xF0) == 0xE1) // SUB
        {
            uint rd = (instruction >> 12) & 0xF;
            uint rn = (instruction >> 16) & 0xF;
            uint rm = instruction & 0xF;
            ExecuteSUB(rd, rn, rm);
        }
        else if ((opcode & 0xF0) == 0xE5) // LDR
        {
            uint rd = (instruction >> 12) & 0xF;
            uint rn = (instruction >> 16) & 0xF;
            uint offset = instruction & 0xFFF;
            ExecuteLDR(rd, rn, offset);
        }
        else if ((opcode & 0xF0) == 0xE4) // STR
        {
            uint rd = (instruction >> 12) & 0xF;
            uint rn = (instruction >> 16) & 0xF;
            uint offset = instruction & 0xFFF;
            ExecuteSTR(rd, rn, offset);
        }
        else if ((opcode & 0xF0) == 0xEA) // B (branch)
        {
            int offset = (int)(instruction & 0x00FFFFFF); // 24-bit offset
            offset = offset << 2; // multiply by 4
            ExecuteB(offset);
        }
        else if ((opcode & 0xF0) == 0xEF) // SWI
        {
            uint swiNumber = instruction & 0x00FFFFFF;
            ExecuteSWI(swiNumber);
        }
        else
        {
            Console.WriteLine($"Unknown instruction: 0x{instruction:X8}");
        }
    }

    private void ExecuteMOV(uint rd, uint value)
    {
        Registers[rd] = value;
        UpdateFlags(value);
        Console.WriteLine($"Executed MOV: R{rd} = 0x{value:X2}");
    }

    private void ExecuteADD(uint rd, uint rn, uint rm)
    {
        uint result = Registers[rn] + Registers[rm];
        Registers[rd] = result;
        UpdateFlags(result);
        Console.WriteLine($"Executed ADD: R{rd} = R{rn} + R{rm}");
    }

    private void ExecuteSUB(uint rd, uint rn, uint rm)
    {
        uint result = Registers[rn] - Registers[rm];
        Registers[rd] = result;
        UpdateFlags(result);
        Console.WriteLine($"Executed SUB: R{rd} = R{rn} - R{rm}");
    }

    private void ExecuteLDR(uint rd, uint rn, uint offset)
    {
        uint address = Registers[rn] + offset;
        Registers[rd] = memory.Read32(address);
        Console.WriteLine($"Executed LDR: R{rd} = MEM[0x{address:X8}]");
    }

    private void ExecuteSTR(uint rd, uint rn, uint offset)
    {
        uint address = Registers[rn] + offset;
        memory.Write32(address, Registers[rd]);
        Console.WriteLine($"Executed STR: MEM[0x{address:X8}] = R{rd}");
    }

    private void ExecuteB(int offset)
    {
        Registers[15] += (uint)offset;
        Console.WriteLine($"Executed B: PC += 0x{offset:X}");
    }

    private void ExecuteSWI(uint swiNumber)
    {
        Console.WriteLine($"SWI called: {swiNumber}");
    }

    private void UpdateFlags(uint value)
    {
        Z = (value == 0);
        N = (value >> 31) != 0;
        // Carry and Overflow are ignored for now
    }

    private void PrintRegisters()
    {
        for (int i = 0; i < 16; i++)
        {
            Console.WriteLine($"R{i}: 0x{Registers[i]:X8}");
        }
        Console.WriteLine($"Flags - N:{N} Z:{Z} C:{C} V:{V}");
        Console.WriteLine("---------------------------");
    }
}