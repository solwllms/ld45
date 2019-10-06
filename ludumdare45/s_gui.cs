using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ld45
{
    public class s_gui
    {
        public const int CHARSACROSS = 26;
        public const int WIDELINE = 3;
        public static texture font;

        public static Color lighter = Color.FromArgb(174, 234, 0);
        public static Color darker = Color.FromArgb(36, 196, 50);
        public static Color back = Color.FromArgb(48, 48, 48);

        public static char[] chars =
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U',
            'V', 'W', 'X', 'Y', 'Z',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '!', '?', '\'', ',', '.', '-', ':', ';', '"', '(', ')',
            '[', ']', '<', '>', '∙',
            '_', '/', '\\', '+', '=', '$', 'É', 'Í', 'Á', 'Ú', 'Ñ', 'Ü', 'Ö', 'Ä', 'Ê', 'Í', 'Ô', 'À', 'Ç', '{'
        };

        public static char[] wideChars =
        {
            'I', 'M', 'T', 'V', 'W', 'X', '£', '&', '^' , '$'
        };

        public static void Init()
        {
            font = new texture("gui/font");
        }

        public static void Draw(uint x, uint y, uint rw, uint ry, Color col)
        {
            for (uint tx = 0; tx < rw; tx++)
            for (uint ty = 0; ty < ry; ty++)
                s_screen.SetPixel(x + tx, y + ty, col);
        }

        public static void DrawCursor(uint x, uint y, uint state)
        {
            cache.GetTexture("gui/cursor").Draw(x, y, state * 6, 0, 6, 8);
        }

        // gameplay ui
        public static void Prompt(string s, int a = -1)
        {
            WriteCentre(s, s_screen.height / 2 - 3, a);
        }

        public static void PromptBottom(string s)
        {
            WriteCentre(s, 70);
        }

        public static void WriteCentered(string s, uint y, Color c)
        {
            Write(s, (uint)(s_screen.width / 2 - s.Length * 5 / 2), y, c);
        }

        public static void WritePixels(uint x, uint y, uint width, uint height, uint col)
        {
            for (uint fx = 0; fx < width; fx++)
                for (uint fy = 0; fy < height; fy++)
                    WritePixel(x + fx, y + fy, col);
        }

        public static void WritePixel(uint x, uint y, uint col)
        {
            s_screen.SetPixel(x, y, col);
        }

        public static void WriteCentre(string t, uint y, int a = -1)
        {
            Write(t, (uint)((s_screen.width - GetStringWidth(t)) / 2) - 2, y, a);
        }

        public static void Write(object obj, uint x, uint y, int a = -1)
        {
            Write(obj, x, y, Color.White, a);
        }

        public static void Write(object obj, uint x, uint y, Color color, int a = -1)
        {
            var str = obj.ToString().ToUpper();
            var cx = x;
            for (var i = 0; i < str.Length; i++) WriteChar(ref cx, y, str[i], color, a);
        }

        public static void WriteChar(ref uint x, uint y, char c, Color col, int alpha = -1)
        {
            int index;
            int cy, cx;

            var unknown = !chars.Contains(c) && !wideChars.Contains(c);
            var wide = wideChars.Contains(c);
            if (wide)
            {
                index = Array.IndexOf(wideChars, c);
                cx = index % CHARSACROSS;
                cy = WIDELINE;
            }
            else
            {
                index = Array.IndexOf(chars, c);
                cx = index % CHARSACROSS;
                cy = index / CHARSACROSS;
            }

            var w = wide ? 6 : 5;

            if (cx < 0)
                unknown = true;

            if (c != ' ')
            {
                for (var fx = 0; fx < w; fx++)
                for (var fy = 0; fy < 6; fy++)
                {
                    var color = unknown
                        ? ((fx + fy) % 2 == 0 ? Color.White : Color.Black)
                        : font.GetPixel((uint)(cx * w + fx), (uint)(cy * 6 + fy));

                    if (color.ToArgb() != Color.Black.ToArgb() && y + fy > 0 && IsOnScreen((int)x + fx, (int)y + fy))
                    {
                        if (alpha == -1)
                        {
                            s_screen.SetPixel((uint)(x + fx), (uint)(y + fy), col);
                        }
                        else
                        {
                            var a = s_screen.GetPixel((uint)(x + fx), (uint)(y + fy));
                            var b = Color.FromArgb(255, 255, 255);

                            s_screen.SetPixel((uint)(x + fx), (uint)(y + fy), s_screen.MixColor(a, b, alpha));
                        }
                    }
                }
            }

            x += (uint)w;
        }
        public static string[] GetTruncatedLines(string msg, int charLen)
        {
            string[] words = msg.Split(' ');
            List<String> old = new List<string>();

            string cur = "";
            for (int i = 0; i < words.Length; i++)
            {
                if (cur.Length >= charLen)
                {
                    old.Add(cur);
                    cur = "";
                }

                cur += words[i] + " ";
            }
            old.Add(cur);
            return old.ToArray();
        }
        public static int GetStringWidth(string text)
        {
            var c = 0;
            foreach (var t in text) c += wideChars.Contains(t) ? 6 : 5;
            return c;
        }
        private static bool IsOnScreen(int x, int y)
        {
            return x < s_screen.width && y < s_screen.height && x >= 0 && y >= 0;
        }
    }
}