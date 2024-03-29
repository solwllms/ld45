﻿using System.Drawing;

namespace ld45
{
    public class texture
    {
        public uint Width => (uint)_data.GetLength(0);
        public uint Height => (uint)_data.GetLength(1);

        private Color[,] _data;

        public texture(string name)
        {
            Bitmap myBitmap;
            if (!filesystem.Exists(name + ".png")) {
                log.WriteLine("missing texture: " + name);
                myBitmap = new Bitmap(1, 1);
            }
            else myBitmap = new Bitmap(filesystem.Open(name + ".png"));
            _data = new Color[myBitmap.Width, myBitmap.Height];

            for (int x = 0; x < myBitmap.Width; x++)
            for (int y = 0; y < myBitmap.Height; y++)
                _data[x, y] = myBitmap.GetPixel(x, y);
        }
        public texture(uint width, uint height)
        {
            _data = new Color[width, height];
        }

        public Color GetPixel(uint x, uint y)
        {
            return _data[x % Width, y % Height];
        }
        public void SetPixel(uint x, uint y, Color c)
        {
            _data[x % Width, y % Height] = c;
        }

        public void Draw(uint x, uint y, uint sx, uint sy, uint rw, uint ry, Color tint)
        {
            for (uint tx = 0; tx < rw; tx++)
            for (uint ty = 0; ty < ry; ty++)
            {
                var c = GetPixel(sx + tx, sy + ty);
                if (c != s_engine.MAGIC_PINK && x + tx < s_screen.width && y + ty < s_screen.height)
                    s_screen.SetPixel(x + tx, y + ty, s_screen.Additive(c, tint));
            }
        }

        public void Draw(uint x, uint y, uint sx, uint sy, uint rw, uint ry)
        {
            for (uint tx = 0; tx < rw; tx++)
            for (uint ty = 0; ty < ry; ty++)
            {
                var c = GetPixel(sx + tx, sy + ty);
                if (c != s_engine.MAGIC_PINK && x + tx < s_screen.width && y + ty < s_screen.height)
                    s_screen.SetPixel(x + tx, y + ty, c);
            }
        }

        public void DrawStencil(uint x, uint y, uint sx, uint sy, uint rw, uint ry, Color c)
        {
            for (uint tx = 0; tx < rw; tx++)
            for (uint ty = 0; ty < ry; ty++)
                if (GetPixel(sx + tx, sy + ty) != s_engine.MAGIC_PINK)
                    s_screen.SetPixel(x + tx, y + ty, c);
        }

        public void Draw(uint x, uint y)
        {
            Draw(x, y, 0, 0, Width, Height);
        }
    }
}