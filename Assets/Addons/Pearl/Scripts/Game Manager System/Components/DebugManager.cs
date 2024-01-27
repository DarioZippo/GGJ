﻿using Pearl.UI;
using UnityEngine;

namespace Pearl.Debugging
{
    public class DebugManager : PearlBehaviour, ISingleton
    {
        [SerializeField]
        private StringBoolDictionary debugViews = null;
        [SerializeField]
        private GameObject tunning = null;

        public const string debugFPS = "debugFPS";
        public const string debugInScreen = "debugScreen";
        public const string consoleInGameString = "consoleInGame";

        public static bool GetIstance(out DebugManager result)
        {
            return Singleton<DebugManager>.GetIstance(out result);
        }

        public static bool GetActiveDebug(in string debugString)
        {
            if (GetIstance(out var debugManager))
            {
                return debugManager != null && debugManager.GetActiveDebugPrivate(debugString);
            }
            return false;
        }

        // Start is called before the first frame update
        protected override void PearlStart()
        {
#if STOMPYROBOT
            if (tunning)
            {
                GameObjectExtend.CreateGameObject(tunning, out _, parent: transform, onlyInTheScene: true);
            }
#endif

            CreateDebugElements();
        }

        public void SetActiveDebug(in string debugString, bool value)
        {
            if (debugViews != null)
            {
                debugViews.Update(debugString, value);
            }
        }

        private bool GetActiveDebugPrivate(in string debugString)
        {
            debugViews.IsNotNullAndTryGetValue(debugString, out bool valueDebug);
            return valueDebug;
        }

        private void CreateDebugElements()
        {
            if (debugViews != null)
            {
            }
        }
    }
}
