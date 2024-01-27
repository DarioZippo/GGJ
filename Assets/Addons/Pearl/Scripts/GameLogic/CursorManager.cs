using Pearl.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pearl
{
    public static class CursorManager
    {
        private static bool _visible;

        public static bool Visible
        {
            get { return _visible; }
            set
            {
                _visible = value;
                Cursor.visible = value;
                if (value)
                {
                    PearlVirtualMouse.Abilitate();
                    PearlVirtualMouse.Disabilitate();
                }
                else
                {
                    PearlVirtualMouse.Disabilitate();
                }
            }
        }
    }
}
