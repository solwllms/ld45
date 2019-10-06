using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ld45
{
    class g_game
    {
        public const int MONEY_START = 2500;
        public const int SOLAR_CAPACITY = 15;
        public const int MAX_TILE_TYPES = 64;

        public static bool inMenu = false;
        public static bool mainMenu = false;

        public static DateTime datetime;
        public static int ticks = 0;
        public const int TICKS_TAX = 300;

        public static int cameraX;
        public static int cameraY;
        public static Point worldMouse;

        public static int money = MONEY_START;
        public static tile[] tileTypes;

        public static bool powerOut = false;
        public static int powerProd;
        public static Point[] powerGens;

        public static int houseTax = 100;

        public static void Load()
        {
            tileTypes = new tile[MAX_TILE_TYPES];
            tileTypes[0] = new tile_road();
            tileTypes[1] = null;
            tileTypes[2] = new tile_bridge();
            tileTypes[3] = new tile_house();
            tileTypes[4] = new tile_tree();
            tileTypes[5] = new tile_powerlines();
            tileTypes[6] = new tile_road_powerline();
            tileTypes[7] = new tile_solar1();
            tileTypes[8] = new tile_solar2();
            tileTypes[9] = new tile_solar3();
            tileTypes[10] = new tile_solar4();

            g_map.GenerateFromImage(cache.GetTexture("level"));

            s_menu.OpenMenu(true);
            //g_map.Generate(50);
        }

        static string inputStr;
        static bool wasMouseDown = false;
        static int lastMoney = -1;
        public static void Update()
        {
            if (s_input.IsKeyPressed(OpenTK.Input.Key.F11)) s_screen.WriteScreenshot();

            Point mouse = s_input.GetMousePos();
            if (!inMenu)
            {
                inputStr += s_input.inputstring;
                if (inputStr.Contains("bigfun")) { inputStr = ""; money = 900000; }

                if (s_input.IsKeyPressed(OpenTK.Input.Key.Escape)) s_menu.OpenMenu(false);

                if (s_input.IsKeyPressed(OpenTK.Input.Key.Left) || mouse.X < 10) cameraX = (cameraX - 1).Clamp(0, g_map.size - g_map.frustum_width);
                if (s_input.IsKeyPressed(OpenTK.Input.Key.Right) || mouse.X > s_screen.width - 10) cameraX = (cameraX + 1).Clamp(0, g_map.size - g_map.frustum_width);
                if (s_input.IsKeyPressed(OpenTK.Input.Key.Up) || mouse.Y < 10) cameraY = (cameraY - 1).Clamp(0, g_map.size - g_map.frustum_height - 1);
                if (s_input.IsKeyPressed(OpenTK.Input.Key.Down) || mouse.Y > s_screen.height - 10) cameraY = (cameraY + 1).Clamp(0, g_map.size - g_map.frustum_height - 1);

                if (!new Rectangle(55, 299, 135, 61).Contains(mouse) && !wasMouseDown)
                {
                    if (s_input.IsMouseBtnDown(OpenTK.Input.MouseButton.Left))
                    {
                        if (selected == 0)
                        {
                            Demolish();
                        }
                        else if (selected == 1)
                        {
                            TryBuild(0);
                        }
                        else if (selected == 2)
                        {
                            TryBuild(5);
                        }
                        else if (selected == 3)
                        {
                            TryBuild(2, true);
                        }
                        else if (selected == 4)
                        {
                            TryBuildMulti(7);
                        }
                        else if (selected == 5)
                        {
                            TryBuild(3);
                        }
                        else if (selected == 6)
                        {
                            TryBuild(4);
                        }
                    }
                }

                if(money > lastMoney && lastMoney != -1) s_audio.sfxPlayer.PlaySFX("sfx/income");

                if (hovered != -1 && s_input.IsMouseBtnDown(OpenTK.Input.MouseButton.Left) && !wasMouseDown)
                {
                    selected = hovered;
                }

                wasMouseDown = s_input.IsMouseBtnDown(OpenTK.Input.MouseButton.Left);

                if (ticks % 30 == 0) datetime = datetime.AddMinutes(1);
                if (mTime > 0) mTime--;
                if (aTime > 0) aTime--;
                ticks++;

                powerGens = g_map.GetLocationsOf(new int[] { 7, 8, 9, 10 });
                powerProd = (powerGens.Length / 4) * SOLAR_CAPACITY;
                lastMoney = money;
            }
            else s_menu.Update();
        }
        public static void Demolish()
        {
            if (g_map.GetBuildingAt(worldMouse.X, worldMouse.Y) != -1)
            {
                if (TryChargeMoney(50)) {
                    if (g_map.IsTileMultiPart(worldMouse.X, worldMouse.Y)) g_map.DestroyMultiBuilding(worldMouse.X, worldMouse.Y);
                    else g_map.SetBuildingAt(worldMouse.X, worldMouse.Y, -1);

                    s_audio.sfxPlayer.PlaySFX("sfx/demolish");
                }
                else PrintMessagePrompt("insufficient funds!");
            }
        }
        public static void TryBuild(int id, bool overWater = false)
        {
            if (g_map.GetBuildingAt(worldMouse.X, worldMouse.Y) == -1 && (!g_map.GetWaterAt(worldMouse.X, worldMouse.Y) || overWater))
            {
                if (TryChargeMoney(tileTypes[id].cost))
                {
                    s_audio.sfxPlayer.PlaySFX("sfx/build");
                    g_map.SetBuildingAt(worldMouse.X, worldMouse.Y, id);
                }
                else PrintMessagePrompt("insufficient funds!");
            }
            else PrintMessagePrompt("spot not empty!");
        }

        public static void TryBuildMulti(int id)
        {
            if (TryChargeMoney(tileTypes[id].cost, false))
            {
                s_audio.sfxPlayer.PlaySFX("sfx/build");
                if (!g_map.TryCreateMultiBuilding(worldMouse.X, worldMouse.Y, id))
                {
                    PrintMessagePrompt("spot not empty");
                }
            }
            else PrintMessagePrompt("insufficient funds!");
        }

        public static bool TryChargeMoney(int c, bool charge = true)
        {
            if (money >= c)
            {
                if (charge)
                {
                    money -= c;
                    if (mTime > 0) mPrompt += c;
                    else mPrompt = c;
                    mTime = 60;
                    mPos = s_input.GetMousePos();
                    mPos.Y -= 5;
                }
                return true;
            }
            return false;
        }

        static void PrintMessagePrompt(string msg, int time = 60)
        {
            s_audio.sfxPlayer.PlaySFX("sfx/alert3");
            aPrompt = msg;
            aTime = time;
        }

        static int mPrompt;
        static Point mPos;
        static int mTime;

        static string aPrompt;
        static int aTime;

        static int selected = -1;
        static int hovered = -1;

        static int[] powerCons = new int[] { 3};
        public static void Render()
        {
            int powerConsCount = 0;
            for (int x = 0; x < g_map.size; x++)
            {
                for (int y = 0; y < g_map.size; y++)
                {
                    if (powerCons.Contains(g_map.GetBuildingAt(x, y))) powerConsCount++; ;
                }
            }
            powerOut = powerConsCount > powerProd;

            if (inMenu) {
                s_menu.Render();
                return;
            }
            else g_map.RenderMap();

            Point mouse = s_input.GetMousePos();
            mouse.X /= 16; mouse.Y /= 16;
            //mouse.X += cameraX; mouse.Y += cameraY;
            worldMouse = mouse;
            worldMouse.X += cameraX; worldMouse.Y += cameraY;
            cache.GetTexture("gui/world_cursor").Draw((uint)mouse.X * 16, (uint)mouse.Y * 16);

            cache.GetTexture("gui/time").Draw(467, 344);
            string time = datetime.ToString("MMM yyyy hh:mmtt");
            s_gui.Write(time, 473, 348, Color.FromArgb(32, 32, 56));
            s_gui.Write(time, 473, 347);

            hovered = -1;
            cache.GetTexture("gui/tools").Draw(55, 299);
            DrawButton(6, 17, 0);
            DrawButton(24, 17, 1);
            DrawButton(42, 17, 2);
            DrawButton(60, 17, 3);
            DrawButton(78, 17, 4);
            DrawButton(96, 17, 5);
            DrawButton(114, 17, 6);

            string hoverMsg = "";
            if (hovered == 0) hoverMsg = String.Format("Demolish ${0}", 50);
            else if (hovered == 1) hoverMsg = String.Format("Build roads ${0}", tileTypes[0].cost);
            else if (hovered == 2) hoverMsg = String.Format("Build powerlines ${0}", tileTypes[5].cost);
            else if (hovered == 3) hoverMsg = String.Format("Build bridges ${0}", tileTypes[2].cost);
            else if (hovered == 4) hoverMsg = String.Format("Build solar farms ${0}", tileTypes[7].cost);
            else if (hovered == 5) hoverMsg = String.Format("Build homes ${0}", tileTypes[3].cost);
            else if (hovered == 6) hoverMsg = String.Format("Plant tree ${0}", tileTypes[3].cost);

            if (hoverMsg != "")
            {
                s_gui.Write(hoverMsg, 59, 348, Color.FromArgb(32, 32, 56));
                s_gui.Write(hoverMsg, 59, 347);
            }
            else
            {
                s_gui.Write("$"+money, 59, 348, Color.FromArgb(32, 32, 56));
                s_gui.Write("$"+money, 59, 347, Color.LightGreen);
            }

            if(mTime > 0)
                s_gui.Write("-$" + mPrompt, (uint)mPos.X + (uint)(Math.Sin(((float)ticks / 90) * 2* Math.PI) * 3), (uint)mPos.Y--, Color.FromArgb(216, 68, 0));
            if (aTime > 0)
            {
                uint y = (s_screen.height / 2) - 3;
                s_gui.WriteCentered(aPrompt, y, Color.FromArgb(32, 32, 56));
                s_gui.WriteCentered(aPrompt, y-1, Color.White);
            }
        }

        static void DrawButton(uint x, uint y, uint n)
        {
            bool hover = new Rectangle((int)(55 + x), (int)(299 + y), 16, 18).Contains(s_input.GetMousePos());
            cache.GetTexture("gui/button").Draw(55 + x, 299 + y, selected == n ? (uint)16 : (uint)0, 0, 16, 18);

            texture t = cache.GetTexture("gui/tool_icons");
            t.Draw(55 + x, 299 + y + (selected == n ? (uint)3 : (uint)0), (n % (t.Width / 16)) * 16, (n / (t.Width / 16)) * 16, 16, 16);

            if (hover) hovered = (int)n;
        }
    }
}
