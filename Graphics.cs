using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

class Graphics : Form
{
    private Memory memory;
    private PictureBox display;
    private Bitmap screen;

    private const int SCREEN_WIDTH = 240;
    private const int SCREEN_HEIGHT = 160;

    private int scale = 2; // Window scaling factor
    private Stopwatch fpsTimer = new Stopwatch();
    private const int FRAME_MS = 16; // ~60 FPS

    public Graphics(Memory mem, int scaleFactor = 2)
    {
        this.memory = mem;
        this.scale = scaleFactor;

        // Window setup
        this.Text = "GBA Emulator";
        this.ClientSize = new Size(SCREEN_WIDTH * scale, SCREEN_HEIGHT * scale);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;

        // PictureBox for rendering
        display = new PictureBox();
        display.Size = new Size(SCREEN_WIDTH * scale, SCREEN_HEIGHT * scale);
        display.Location = new Point(0, 0);
        this.Controls.Add(display);

        // Bitmap buffer
        screen = new Bitmap(SCREEN_WIDTH, SCREEN_HEIGHT);

        fpsTimer.Start();
    }

    /// <summary>
    /// Render the current frame from VRAM & Palette
    /// Mode 3: direct 16-bit RGB
    /// Mode 4: optional palette indexed colors (first 256 entries)
    /// </summary>
    public void RenderFrame(bool usePalette = false)
    {
        for (int y = 0; y < SCREEN_HEIGHT; y++)
        {
            for (int x = 0; x < SCREEN_WIDTH; x++)
            {
                ushort color = 0;

                if (usePalette)
                {
                    // Mode 4: indexed 8-bit from VRAM
                    uint addr = 0x06000000 + (uint)(y * SCREEN_WIDTH + x);
                    byte index = memory.Read8(addr);
                    color = memory.Read16(0x05000000 + (uint)(index * 2));
                }
                else
                {
                    // Mode 3: direct 16-bit color
                    uint addr = 0x06000000 + (uint)((y * SCREEN_WIDTH + x) * 2);
                    color = memory.Read16(addr);
                }

                // Convert 15-bit GBA color to 24-bit
                int r = (color & 0x1F) << 3;
                int g = ((color >> 5) & 0x1F) << 3;
                int b = ((color >> 10) & 0x1F) << 3;

                screen.SetPixel(x, y, Color.FromArgb(r, g, b));
            }
        }

        // Draw scaled bitmap
        display.Image = new Bitmap(screen, display.Size);

        // Maintain roughly 60 FPS
        long elapsed = fpsTimer.ElapsedMilliseconds;
        if (elapsed < FRAME_MS)
            System.Threading.Thread.Sleep((int)(FRAME_MS - elapsed));
        fpsTimer.Restart();

        Application.DoEvents();
    }
}