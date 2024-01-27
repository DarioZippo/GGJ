#if PEARL_2DSIDE

using Pearl.ClockManager;
using Pearl.Debugging.ScreenVars;
using Pearl.Debugging;
using Pearl.Tweens;
using Pearl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Pearl.Side2D
{
    public class Side2DScreenVars : DebugScreenVarsNative
    {
        [DebugScreen("Side2D", "StatePlayer")]
        public MemberComplexInfo GetCurrentStatePlayer()
        {
            return ReflectionExtend.GetValueInfo<SpecificCharacterManager>("_currentState");
        }
    }
}

#endif
