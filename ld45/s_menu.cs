using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ld45
{
    class s_menu
    {
        static int hovered;

        public static void OpenMenu(bool mainMenu)
        {
            RenderBG();
            g_game.inMenu = true;
            g_game.mainMenu = mainMenu;

            if (mainMenu)
            {
                AL.Source(s_audio.source, ALSourceb.Looping, true);
                s_audio.musicPlayer.PlaySFX("music/theme");
            }
        }

        public static void CloseMenu()
        {
            g_game.inMenu = false;
            g_game.mainMenu = false;

            s_audio.musicPlayer.Stop();
            AL.Source(s_audio.source, ALSourceb.Looping, false);
        }

        public static void Render()
        {
            //cache.GetTexture("gui/logo").Draw(130, 64);
            s_gui.WriteCentered("a people-homing city-builder", 167, Color.Orange);
            s_gui.WriteCentered("created for LUDUM DARE 45 by sol williams", 175, Color.Orange);

            hovered = -1;
            RenderButton("resume game", 215, 4, g_game.mainMenu);
            RenderButton("start new city", 235, 0, !g_game.mainMenu);
            RenderButton("load city", 255, 1);
            RenderButton("save city", 275, 2, g_game.mainMenu);
            RenderButton("exit game", 295, 3);
        }

        public static void Update()
        {
            if (s_input.IsMouseBtnDown(OpenTK.Input.MouseButton.Left))
            {
                if (hovered == 4) CloseMenu();
                else if (hovered == 0) { g_map.GenerateFromImage(cache.GetTexture("level")); CloseMenu(); }
                else if (hovered == 1) s_save.LoadGame();
                else if (hovered == 2) s_save.SaveGame();
                else Exit();

                RenderBG();
            }
        }

        public static void RenderBG()
        {
            g_map.RenderMap();
            cache.GetTexture("gui/logo").Draw(130, 64);
        }

        public static void Exit() { }

        public static void RenderButton(string msg, uint y, int id, bool disabled = false)
        {
            bool hover = new Rectangle(270, (int)y, 100, 12).Contains(s_input.GetMousePos());
            s_gui.WriteCentered(msg, y+1, Color.Black);
            s_gui.WriteCentered(msg, y, disabled ? Color.Gray : hover ? Color.Orange : Color.White);

            if (hover && !disabled) hovered = id;
        }
    }
}
