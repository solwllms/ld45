using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ludumdare45
{
    class g_map
    {
        public static int frustum_width;
        public static int frustum_height;

        public static int size;

        public static int[,] terrain;
        public static bool[,] water;
        public static int[,] buildings;

        public static void Generate(int size)
        {
            g_map.size = size;
            g_game.datetime = new DateTime(1990, 1, 1, 7, 27, 0, 0);
            g_game.money = g_game.MONEY_START;
            Init();

            for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
            {
                terrain[x, y] = 0;
                water[x, y] = false;
                buildings[x, y] = -1;
            }
        }

        public static void GenerateFromImage(texture t)
        {
            if(t.Height != t.Width) log.ThrowFatal("image not 1:1!");

            size = (int)t.Height;
            g_game.datetime = new DateTime(1990, 1, 1, 7, 27, 0, 0);
            g_game.money = g_game.MONEY_START;
            Init();

            for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
            {
                Color c = t.GetPixel((uint)x, (uint)y);

                terrain[x, y] = 0;
                if (c == Color.FromArgb(0, 0, 255)) water[x, y] = true;
                else water[x, y] = false;

                if (c == Color.FromArgb(0, 255, 255)) { g_game.cameraX = x; g_game.cameraY = y; }

                if (c == Color.FromArgb(255, 0, 0)) buildings[x, y] = 2;
                else if (c == Color.FromArgb(255, 255, 0)) { buildings[x, y] = 2; water[x, y] = true; }
                else if (c == Color.FromArgb(0, 0, 0)) buildings[x, y] = 0;
                else if (c == Color.FromArgb(0, 255, 0)) buildings[x, y] = 4;
                else buildings[x, y] = -1;
            }
        }

        public static void Init()
        {
            frustum_width = s_engine.DEF_SCREEN_WIDTH / 16;
            frustum_height = s_engine.DEF_SCREEN_HEIGHT / 16;

            terrain = new int[size, size];
            water = new bool[size, size];            
            buildings = new int[size, size];

            g_game.cameraX = 0;
            g_game.cameraY = 0;
        }

        public static void RenderMap()
        {
            for (int x = -1; x < frustum_width + 1; x++)
                for (int y = -1; y < frustum_height + 1; y++)
                    RenderTile(g_game.cameraX + x, g_game.cameraY + y);
        }

        static void RenderTile(int x, int y)
        {
            if (x < 0 || y < 0 || y >= size || x >= size) return;

            uint sx = (uint)(x - g_game.cameraX);
            uint sy = (uint)(y - g_game.cameraY);

            int t = terrain[x, y];
            if (water[x, y])
            {
                int te = CalcWater(x, y, true, compat: new int[] { -2 });

                if(te != 11) DrawTilemapTile("terrain", sx * 16, sy * 16, GetTileTexID(t), 16);
                DrawTilemapTile("water", sx * 16, sy * 16, te, 16);
            }
            else DrawTilemapTile("terrain", sx * 16, sy * 16, GetTileTexID(t), 16);

            int b = buildings[x, y];
            if (b != -1) g_game.tileTypes[b].Render(x, y, sx, sy);
        }

        public static int GetTileTexID(int id)
        {
            return id;
        }
        public static int GetBuildingAt(int x, int y)
        {
            if (x < 0 || y < 0 || x >= size || y >= size) return -2;
            return buildings[x, y];
        }
        public static bool GetWaterAt(int x, int y)
        {
            if (x < 0 || y < 0 || x >= size || y >= size) return false;
            return water[x, y];
        }
        public static int GetTerrainAt(int x, int y)
        {
            if (x < 0 || y < 0 || x >= size || y >= size) return -2;
            return terrain[x, y];
        }
        public static void SetBuildingAt(int x, int y, int b)
        {
            if (x >= 0 && y >= 0 && x < size && y < size) 
                buildings[x, y] = b;
        }

        public static Point[] GetLocationsOf(int[] search)
        {
            List<Point> locs = new List<Point>();
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    if (search.Contains(GetBuildingAt(x, y))) locs.Add(new Point(x, y));
                }
            }

            return locs.ToArray();
        }

        public static bool TryCreateMultiBuilding(int x, int y, int id, int size = 2)
        {
            for (int sx = 0; sx < size; sx++)
            {
                for (int sy = 0; sy < size; sy++)
                {
                    if (GetBuildingAt(x + sx, y - sy) != -1) return false;
                }
            }

            int rid = 0;
            for (int sy = 0; sy < size; sy++)
            {
                for (int sx = 0; sx < size; sx++)
                {
                        SetBuildingAt(x + sx, y - size + sy + 1, id + rid);
                    rid++;
                }
            }
            return true;
        }

        public static bool IsTileMultiPart(int x, int y)
        {
            return g_game.tileTypes[GetBuildingAt(x, y)].isPartOfMulti;
        }
        public static int GetTileMultiPart(int x, int y)
        {
            return g_game.tileTypes[GetBuildingAt(x, y)].multiPart;
        }
        public static int GetTileMultiSize(int x, int y)
        {
            return g_game.tileTypes[GetBuildingAt(x, y)].multiSize;
        }
        public static void DestroyMultiBuilding(int x, int y)
        {
            if (GetBuildingAt(x, y) < 0) return;
            if (!IsTileMultiPart(x, y)) return;
            int id = GetTileMultiPart(x, y) - 1;
            int size = GetTileMultiSize(x, y);

            int offx = id % size;
            int offy = id / size;
            for (int sx = 0; sx < size; sx++)
            {
                for (int sy = 0; sy < size; sy++)
                {
                    SetBuildingAt(x + sx - offx, y - sy - offy + 1, -1);
                }
            }
        }
        public static void DrawRoad(string tileset, int id, uint sx, uint sy, int x, int y, bool allowFull = false, int[] compat = null)
        {
            int t = 0;
            bool left = GetBuildingAt(x - 1, y) == id || (compat != null && compat.Contains(GetBuildingAt(x - 1, y)));
            bool right = GetBuildingAt(x + 1, y) == id || (compat != null && compat.Contains(GetBuildingAt(x + 1, y)));
            bool down = GetBuildingAt(x, y + 1) == id || (compat != null && compat.Contains(GetBuildingAt(x, y + 1)));
            bool up = GetBuildingAt(x, y - 1) == id || (compat != null && compat.Contains(GetBuildingAt(x, y - 1)));


            if (!left && !right && (up || down)) t = 1;
            else if ((left || right) && !down && !up) t = 0;
            else if (left && right && down && !up) t = 2;
            else if (left && right && !down && up) t = 3;
            else if (left && right && down && up) t = 8;
            else if (down && right && !left && !up) t = 4;
            else if (down && left && !right && !up) t = 5;
            else if (up && right && !left && !down) t = 6;
            else if (up && left && !right && !down) t = 7;
            else if (up && left && !right && down) t = 9;
            else if (up && !left && right && down) t = 10;

            if (allowFull && up & right && left && down) t = 11;

            DrawTilemapTile(tileset, sx * 16, sy * 16, t, 16);
        }

        public static int CalcWater(int x, int y, bool allowFull = false, int[] compat = null)
        {
            int t = 0;
            bool left = GetWaterAt(x - 1, y) || (compat != null && compat.Contains(GetBuildingAt(x - 1, y)));
            bool right = GetWaterAt(x + 1, y) || (compat != null && compat.Contains(GetBuildingAt(x + 1, y)));
            bool down = GetWaterAt(x, y + 1) || (compat != null && compat.Contains(GetBuildingAt(x, y + 1)));
            bool up = GetWaterAt(x, y - 1) || (compat != null && compat.Contains(GetBuildingAt(x, y - 1)));


            if (!left && !right && (up || down)) t = 1;
            else if ((left || right) && !down && !up) t = 0;
            else if (left && right && down && !up) t = 2;
            else if (left && right && !down && up) t = 3;
            else if (left && right && down && up) t = 8;
            else if (down && right && !left && !up) t = 4;
            else if (down && left && !right && !up) t = 5;
            else if (up && right && !left && !down) t = 6;
            else if (up && left && !right && !down) t = 7;
            else if (up && left && !right && down) t = 9;
            else if (up && !left && right && down) t = 10;

            if (allowFull && up & right && left && down) t = 11;
            return t;
        }

        public static void DrawTilemapTile(string tex, uint sx, uint sy, int id, uint s = 16)
        {
            if (id < 0) return;
            uint i = (uint)id;
            texture t = cache.GetTexture(tex);
            t.Draw(sx, sy, (i % (t.Width / s)) * s, (i / (t.Width / s)) * s, s, s);
        }
    }
}
