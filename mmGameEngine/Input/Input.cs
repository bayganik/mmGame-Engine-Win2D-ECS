using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace mmGameEngine
{
    public static class Input
    {
        public static Vector2 MousePosition { get; set; } = Vector2.One;
        public static bool LeftMousePressed { get; set; } = false;
        public static bool LeftMouseReleased { get; set; } = false;
        public static bool RightMousePressed { get; set; } = false;
        public static bool RightMouseReleased { get; set; } = false;
        public static bool KeyPressed { get; set; } = false;
        public static bool KeyReleased { get; set; } = false;
        public static string KeyValue { get; set; }
        public static bool IsLeftMousePressed()
        {
            if (LeftMousePressed)
            {
                LeftMousePressed = false;
                return true;
            }
            return false;
        }
        public static bool IsRightMousePressed()
        {
            if (RightMousePressed)
            {
                RightMousePressed = false;
                return true;
            }
            return false;
        }
        public static bool IsLeftMouseReleased()
        {
            if (LeftMouseReleased)
            {
                LeftMouseReleased = false;
                return true;
            }
            return false;
        }
        public static bool IsRightMouseReleased()
        {
            if (RightMouseReleased)
            {
                RightMouseReleased = false;
                return true;
            }
            return false;
        }
        public static bool IsKeyPressed(string _keyvalue)
        {
            if (!KeyPressed) return false;
            //KeyPressed = false;
            //
            // we have a keypressed event
            //
            if (_keyvalue.ToLower() == KeyValue.ToLower())
            {
                Input.KeyPressed = false;
                return true;
            }
            return false;
        }
        public static bool IsKeyReleased(string _keyvalue)
        {
            //if (KeyValue.ToLower() != _keyvalue) return true;

            if (!KeyReleased) return false;
            //
            // we have a keyrelease event
            //
            if (_keyvalue.ToLower() == KeyValue.ToLower()) return true;
            return KeyReleased;
        }
    }
}
