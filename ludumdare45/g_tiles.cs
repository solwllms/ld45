using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ludumdare45
{

    class tile
    {
        public int cost;

        public bool isPartOfMulti = false;
        public int multiPart = -1;
        public int multiSize = -1;

        public tile(int cost) {
            this.cost = cost;
        }
        public tile(int cost, int part, int size)
        {
            this.cost = cost;
            isPartOfMulti = true;
            multiPart = part;
            multiSize = size;
        }
        public virtual void Render(int x, int y, uint sx, uint sy) { }
    }
    class tile_house : tile
    {
        public tile_house() : base(200) { }
        public override void Render(int x, int y, uint sx, uint sy)
        {
            bool onRoadNetwork = s_pathfind.DoesPathExist(new Point(x, y), new Point(49, 99), new int[] { 0, 2, 6 }) ||
                s_pathfind.DoesPathExist(new Point(x, y), new Point(0, 25), new int[] { 0, 2, 6 });

            bool onPowerNetwork = false;
            if (!g_game.powerOut)
            {
                for (int i = 0; i < g_game.powerGens.Length; i++)
                {
                    if (s_pathfind.DoesPathExist(new Point(x, y), g_game.powerGens[i], new int[] { 6, 5, 7, 8, 9, 10 }))
                    {
                        onPowerNetwork = true;
                        break;
                    }
                }
            }

            int id = onRoadNetwork && onPowerNetwork ? 1 : 0;
            g_map.DrawTilemapTile("building", sx * 16, sy * 16, id);

            if (onRoadNetwork && onPowerNetwork && g_game.ticks % g_game.TICKS_TAX == 0) g_game.money += g_game.houseTax;
        }
    }

    class tile_road : tile
    {
        public tile_road() : base(75) { }
        public override void Render(int x, int y, uint sx, uint sy)
        {
            g_map.DrawRoad("road", 0, sx, sy, x, y, compat: new int[] { -2, 6 , 2 });

            // check for powerlines
            bool left = g_map.GetBuildingAt(x - 1, y) == 5;
            bool right = g_map.GetBuildingAt(x + 1, y) == 5;
            bool down = g_map.GetBuildingAt(x, y + 1) == 5;
            bool up = g_map.GetBuildingAt(x, y - 1) == 5;
            if ((down && up) || (left && right)) g_map.SetBuildingAt(x, y, 6);

        }
    }
    class tile_road_powerline : tile_road
    {
        public override void Render(int x, int y, uint sx, uint sy)
        {
            g_map.DrawRoad("road", 0, sx, sy, x, y, compat: new int[] { -2, 0, 2});

            // check for powerlines
            bool left = g_map.GetBuildingAt(x - 1, y) == 5;
            bool right = g_map.GetBuildingAt(x + 1, y) == 5;
            bool down = g_map.GetBuildingAt(x, y + 1) == 5;
            bool up = g_map.GetBuildingAt(x, y - 1) == 5;
            if (down && up) g_map.DrawTilemapTile("powerlines", sx * 16, sy * 16, 11, 16);
            else if (left && right) g_map.DrawTilemapTile("powerlines", sx * 16, sy * 16, 12, 16);
            else g_map.SetBuildingAt(x, y, 0);
        }
    }
    class tile_bridge : tile
    {
        public tile_bridge() : base(300) { }
        public override void Render(int x, int y, uint sx, uint sy)
        {
            g_map.DrawRoad("bridge", 2, sx, sy, x, y, compat: new int[] { -2, 0, 6 });
        }
    }
    class tile_tree : tile
    {
        public tile_tree() : base(250) { }
        public override void Render(int x, int y, uint sx, uint sy)
        {
            g_map.DrawTilemapTile("tree", sx * 16, sy * 16, g_map.GetTileTexID(4), 16);
        }
    }
    class tile_powerlines : tile
    {
        public tile_powerlines() : base(150) { }
        public override void Render(int x, int y, uint sx, uint sy)
        {
            g_map.DrawRoad("powerlines", 5, sx, sy, x, y, compat: new int[] { 6, 7, 8, 9, 10, 3 });
        }
    }

    class tile_solar1 : tile
    {
        public tile_solar1() : base(600, 1, 2) { }
        public override void Render(int x, int y, uint sx, uint sy)
        {
            g_map.DrawTilemapTile("solar", sx * 16, sy * 16, 0, 16);
        }
    }
    class tile_solar2 : tile
    {
        public tile_solar2() : base(600, 2, 2) { }
        public override void Render(int x, int y, uint sx, uint sy)
        {
            g_map.DrawTilemapTile("solar", sx * 16, sy * 16, 1, 16);
        }
    }
    class tile_solar3 : tile
    {
        public tile_solar3() : base(600, 3, 2) { }
        public override void Render(int x, int y, uint sx, uint sy)
        {
            g_map.DrawTilemapTile("solar", sx * 16, sy * 16, 2, 16);
        }
    }
    class tile_solar4 : tile
    {
        public tile_solar4() : base(600, 4, 2) { }
        public override void Render(int x, int y, uint sx, uint sy)
        {
            g_map.DrawTilemapTile("solar", sx * 16, sy * 16, 3, 16);
        }
    }
}
