using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK.Input;

namespace ld45
{
    public partial class s_input
    {
        private static List<Key> _keysDown;
        private static List<Key> _wasDown;

        public static string inputstring;
        public static vector mousepos;

        public static bool mouselock = false;
        protected static bool mouselocked = true;

        private static MouseState _mouseState;
        private static KeyboardState _keyboardState;

        internal static void Init()
        {
            _keysDown = new List<Key>();
            _wasDown = new List<Key>();

            inputstring = "";
            mousepos = new vector(0, 0);
        }

        internal static void Update()
        {
            if (!s_window.HasFocus()) return;

            _wasDown.Clear();
            _keysDown.ForEach((item) =>
            {
                _wasDown.Add(item);
            });
            _keysDown.Clear();

            _mouseState = Mouse.GetState();
            _keyboardState = Keyboard.GetState();

            inputstring = "";
            foreach (Key key in Enum.GetValues(typeof(Key)))
            {
                if (_keyboardState.IsKeyDown(key) && s_window.HasFocus())
                {
                    _keysDown.Add(key);

                    if (_wasDown.Contains(key)) continue;

                    if (key == Key.Space) inputstring += " ";
                    else if (key == Key.Quote) inputstring += '"';
                    else if (key == Key.LBracket) inputstring += '(';
                    else if (key == Key.RBracket) inputstring += ')';
                    else if (key == Key.Minus) inputstring += '_';
                    else if (key == Key.Plus) inputstring += '+';
                    else if (key == Key.Period) inputstring += '.';

                    else if (key == Key.Number0) inputstring += "0";
                    else if (key == Key.Number1) inputstring += "1";
                    else if (key == Key.Number2) inputstring += "2";
                    else if (key == Key.Number3) inputstring += "3";
                    else if (key == Key.Number4) inputstring += "4";
                    else if (key == Key.Number5) inputstring += "5";
                    else if (key == Key.Number6) inputstring += "6";
                    else if (key == Key.Number7) inputstring += "7";
                    else if (key == Key.Number8) inputstring += "8";
                    else if (key == Key.Number9) inputstring += "9";

                    else
                    {
                        string c = key.ToString().ToLower();
                        if (c.Length == 1) inputstring += c;
                    }
                }
            }

            var mp = GetMousePos();
            mousepos = new vector((int)s_engine.DEF_WINDOW_WIDTH / 2 - mp.X, (int)s_engine.DEF_WINDOW_HEIGHT / 2 - mp.Y);

            if (mouselock)
            {
                if (mouselocked && !mouselock) mouselocked = false;
                if (!mouselocked && mouselock && s_window.HasFocus()) mouselocked = mouselock;

                if (mouselocked) SetMousePos(new vector((int)s_engine.DEF_WINDOW_WIDTH / 2, (int)s_engine.DEF_WINDOW_HEIGHT / 2));
                SetMouseVisible(!mouselocked);
            }
            else
            {
                SetMouseVisible(true);
            }
        }

        public static void MouseReturn()
        {
            mouselocked = mouselock;
        }

        public static void MouseLost()
        {
            mouselocked = false;
        }

        public static bool MouseLocked()
        {
            return mouselocked;
        }

        public static bool IsKeyPressed(Key key)
        {
            return !_wasDown.Contains(key) && _keyboardState.IsKeyDown(key);
        }
        public static bool IsKey(Key key)
        {
            return _keyboardState.IsKeyDown(key);
        }
        public static bool IsKeyReleased(Key key)
        {
            return _wasDown.Contains(key) && !_keyboardState.IsKeyDown(key);
        }
        public static bool AnyKey()
        {
            return _keyboardState.IsAnyKeyDown;
        }

        public static string GetKeyName(Key key)
        {
            return key.ToString();
        }

        public static bool FindKey(string s, out Key k)
        {
            try
            {
                k = (Key)Enum.Parse(typeof(Key),
                    Enum.GetNames(typeof(Key)).First(x => x.ToLower() == s.ToLower()));
                return true;
            }
            catch
            {
                k = Key.Unknown;
                return false;
            }
        }

        public static void SetMouseVisible(bool v)
        {
            s_window.SetMouseVisible(v);
        }
        public static void SetMousePos(vector vector)
        {
            Mouse.SetPosition((int)vector.x, (int)vector.y);
        }
        public static Point GetMousePos()
        {
            float xs = (float)s_window._instance.Width/ s_engine.DEF_SCREEN_WIDTH;
            float ys = (float)s_window._instance.Height / s_engine.DEF_SCREEN_HEIGHT;
            Point p = s_window.mousePos;
            p.X = (int)(p.X / xs);// (int)((p.X + (s_engine.DEF_SCREEN_WIDTH)) / xs);
            p.Y = (int)(p.Y / ys); //(int)((p.Y + (s_engine.DEF_SCREEN_HEIGHT)) / ys);
            return p;
        }
        public static bool IsMouseBtnDown(MouseButton button)
        {
            return _mouseState.IsButtonDown(button);
        }
    }
}