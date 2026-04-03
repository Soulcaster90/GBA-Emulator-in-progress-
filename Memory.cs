// Memory.cs
using System;

class Memory
{
    // WRAM = 256 KB
    private byte[] wram = new byte[0x40000];

    // ROM (loaded from file)
    private byte[] rom;

    // VRAM = 96 KB
    private byte[] vram = new byte[0x18000];

    // Palette memory = 1 KB
    private byte[] palette = new byte[0x400];

    // I/O Registers = 1 KB
    private byte[] ioRegisters = new byte[0x400];

    // OAM (Sprite Attribute Memory) = 1 KB
    private byte[] oam = new byte[0x400];

    public Memory(byte[] romData)
    {
        rom = romData;
    }

    // ----------------- 32-bit access -----------------
    public uint Read32(uint address)
    {
        if (address >= 0x08000000 && address < 0x0A000000) // ROM
            return ReadArray32(rom, address - 0x08000000);
        if (address >= 0x02000000 && address <= 0x0203FFFF) // WRAM
            return ReadArray32(wram, address - 0x02000000);
        if (address >= 0x06000000 && address <= 0x06017FFF) // VRAM
            return ReadArray32(vram, address - 0x06000000);
        if (address >= 0x05000000 && address <= 0x050003FF) // Palette
            return ReadArray32(palette, address - 0x05000000);
        if (address >= 0x04000000 && address <= 0x040003FF) // I/O
            return ReadArray32(ioRegisters, address - 0x04000000);
        if (address >= 0x07000000 && address <= 0x070003FF) // OAM
            return ReadArray32(oam, address - 0x07000000);

        Console.WriteLine($"Read32 from unmapped address: 0x{address:X8}");
        return 0;
    }

    public void Write32(uint address, uint value)
    {
        if (address >= 0x02000000 && address <= 0x0203FFFF) // WRAM
            WriteArray32(wram, address - 0x02000000, value);
        else if (address >= 0x06000000 && address <= 0x06017FFF) // VRAM
            WriteArray32(vram, address - 0x06000000, value);
        else if (address >= 0x05000000 && address <= 0x050003FF) // Palette
            WriteArray32(palette, address - 0x05000000, value);
        else if (address >= 0x04000000 && address <= 0x040003FF) // I/O
            WriteArray32(ioRegisters, address - 0x04000000, value);
        else if (address >= 0x07000000 && address <= 0x070003FF) // OAM
            WriteArray32(oam, address - 0x07000000, value);
        else
            Console.WriteLine($"Write32 to unmapped address: 0x{address:X8} ignored");
    }

    // ----------------- 16-bit access -----------------
    public ushort Read16(uint address)
    {
        return (ushort)(Read8(address) | (Read8(address + 1) << 8));
    }

    public void Write16(uint address, ushort value)
    {
        Write8(address, (byte)(value & 0xFF));
        Write8(address + 1, (byte)((value >> 8) & 0xFF));
    }

    // ----------------- 8-bit access -----------------
    public byte Read8(uint address)
    {
        if (address >= 0x08000000 && address < 0x0A000000)
            return rom[address - 0x08000000];
        if (address >= 0x02000000 && address <= 0x0203FFFF)
            return wram[address - 0x02000000];
        if (address >= 0x06000000 && address <= 0x06017FFF)
            return vram[address - 0x06000000];
        if (address >= 0x05000000 && address <= 0x050003FF)
            return palette[address - 0x05000000];
        if (address >= 0x04000000 && address <= 0x040003FF)
            return ioRegisters[address - 0x04000000];
        if (address >= 0x07000000 && address <= 0x070003FF)
            return oam[address - 0x07000000];

        Console.WriteLine($"Read8 from unmapped address: 0x{address:X8}");
        return 0;
    }

    public void Write8(uint address, byte value)
    {
        if (address >= 0x02000000 && address <= 0x0203FFFF)
            wram[address - 0x02000000] = value;
        else if (address >= 0x06000000 && address <= 0x06017FFF)
            vram[address - 0x06000000] = value;
        else if (address >= 0x05000000 && address <= 0x050003FF)
            palette[address - 0x05000000] = value;
        else if (address >= 0x04000000 && address <= 0x040003FF)
            ioRegisters[address - 0x04000000] = value;
        else if (address >= 0x07000000 && address <= 0x070003FF)
            oam[address - 0x07000000] = value;
        else
            Console.WriteLine($"Write8 to unmapped address: 0x{address:X8} ignored");
    }

    // ----------------- Helpers -----------------
    private uint ReadArray32(byte[] array, uint index)
    {
        if (index + 3 >= array.Length) return 0;
        return (uint)(array[index] |
                     (array[index + 1] << 8) |
                     (array[index + 2] << 16) |
                     (array[index + 3] << 24));
    }

    private void WriteArray32(byte[] array, uint index, uint value)
    {
        if (index + 3 >= array.Length) return;
        array[index] = (byte)(value & 0xFF);
        array[index + 1] = (byte)((value >> 8) & 0xFF);
        array[index + 2] = (byte)((value >> 16) & 0xFF);
        array[index + 3] = (byte)((value >> 24) & 0xFF);
    }
}