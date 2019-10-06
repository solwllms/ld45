using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ld45
{
    class s_engine
    {
        public const int DEF_WINDOW_WIDTH = 1280;
        public const int DEF_WINDOW_HEIGHT = 720;

        public const int DEF_SCREEN_WIDTH = 640;
        public const int DEF_SCREEN_HEIGHT = 360;

        public const string DATETIME_FORMAT = "MM/dd/yyyy h:mm tt";
        public static Color MAGIC_PINK = Color.FromArgb(255, 0, 255);

        public static Random random;
        private static Stopwatch _fclock;

        public static float ftime;
        public static float fps;
        public static int frame;

        public static void Init(string[] args)
        {
            filesystem.AddDirectory("");
            filesystem.AddDirectory("data");

            s_window.SetTitle("Population - Ludum Dare 45");
            s_window.SetIcon("icon.png");

            s_screen.Init(DEF_SCREEN_WIDTH, DEF_SCREEN_HEIGHT);
            s_gui.Init();
            s_input.Init();
            s_audio.Init();

            random = new Random();
            _fclock = new Stopwatch();

            g_game.Load();
        }

        public static void Shutdown()
        {
            s_audio.Shutdown();
        }

        public static void Tick()
        {
            /*
            s_input.Update();
            g_game.Update();
            g_game.Render();*/
        }

        public static void Render()
        {
            ftime = (float)_fclock.Elapsed.TotalSeconds;
            _fclock.Restart();

            frame++;
            if (frame > 60)
                frame = 1;

            if (frame % 10 == 0) fps = 1.0f / ftime;

            s_input.Update();
            g_game.Update();
            g_game.Render();

            // i know this game runs bad, but please.
            //s_gui.Write(fps, 0, 0);

            Point mouse = s_input.GetMousePos();
            //s_gui.DrawCursor((uint)mouse.X, (uint)mouse.Y, 0);
        }
    }
}
