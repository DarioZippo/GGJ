using Pearl.Input;
using UnityEngine;
using Pearl.Events;
using System;
using TypeReferences;
using Pearl.Debugging.ScreenVars;
using ParadoxNotion;

namespace Pearl
{
    #region Enum
    public enum StateLevelEnum { Pause, InGame, GameOver, }
    #endregion

    [DisallowMultipleComponent]
    public abstract class LevelManager : PearlBehaviour, ISingleton
    {
        #region Inspector Fields
        [SerializeField]
        private bool isOnApplicationPause = true;
        [SerializeField]
        protected string pauseInputMap = "UI";
        [SerializeField]
        protected string gameplayInputMap = "Gameplay";
        [SerializeField, InterfaceType(typeof(IFSM))]
        protected UnityEngine.Object FSMObject = null;
        [ReadOnly, SerializeField]
        protected StateLevelEnum _stateLevel = StateLevelEnum.InGame;
        [SerializeField, ClassImplements(typeof(DebugScreenVarsNative))]
        private ClassTypeReference[] pagesForScreenTesting;
        #endregion

        #region Private Fields
        private IFSM _FSM;
        #endregion

        #region Static
        public static bool GetIstance(out LevelManager result)
        {
            return Singleton<LevelManager>.GetIstance(out result);
        }

        public static LevelManager GetIstance()
        {
            Singleton<LevelManager>.GetIstance(out LevelManager result);
            return result;
        }

        public static StateLevelEnum StateLevel
        {
            get
            {
                if (GetIstance(out var manager))
                {
                    return manager._stateLevel;
                }
                else return StateLevelEnum.InGame;
            }
            private set
            {
                if (GetIstance(out var manager))
                {
                    manager._stateLevel = value;
                }
            }
        }

        public static bool IsPause { get { return StateLevel == StateLevelEnum.Pause; } }

        public static void ResetGame()
        {
            if (GetIstance(out var manager))
            {
                PearlEventsManager.CallEvent(ConstantStrings.Reset);
                manager.ResetGamePrivate();
            }
        }

        public static void GameOver()
        {
            if (GetIstance(out var manager))
            {
                manager._stateLevel = StateLevelEnum.InGame;
                PearlEventsManager.CallEvent(ConstantStrings.Gameover);
                manager.GameOverPrivate();
            }
        }

        public static void CallPause(bool pause)
        {
            if (GetIstance(out var manager))
            {
#if INK
                DialogsManager.Pause(pause);
#endif
                if (pause && StateLevel == StateLevelEnum.InGame)
                {
                    StateLevel = StateLevelEnum.Pause;
                    PearlEventsManager.CallEvent(ConstantStrings.Pause, pause);
                    manager.PauseInternal();
                }
                else if (!pause && StateLevel == StateLevelEnum.Pause)
                {
                    StateLevel = StateLevelEnum.InGame;
                    PearlEventsManager.CallEvent(ConstantStrings.Pause, pause);
                    manager.UnpauseInternal();
                }
            }
        }
        #endregion

        #region Unity Callbacks
        protected virtual void Reset()
        {
            if (FSMObject != null)
            {
                _FSM = (IFSM)FSMObject;
            }
        }

        protected override void OnApplicationInterrupt(bool interrupt)
        {
            if (isOnApplicationPause && interrupt && _stateLevel == StateLevelEnum.InGame)
            {
                CallPause(true);
            }
        }

        protected override void PearlAwake()
        {
            base.PearlAwake();
        }

        protected override void PearlStart()
        {
            base.PearlStart();
            AddPage();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            RemovePage();
        }
        #endregion

        public void AddPage()
        {
            if (pagesForScreenTesting != null)
            {
                foreach (var page in pagesForScreenTesting)
                {
                    DebugScreenManager.AddPage(page);
                }
            }
        }

        public void RemovePage()
        {
            if (pagesForScreenTesting != null)
            {
                foreach (var page in pagesForScreenTesting)
                {
                    DebugScreenManager.RemovePage(page);
                }
            }
        }

        #region Abstract
        protected abstract void PauseInternal();

        protected abstract void UnpauseInternal();

        protected abstract void ResetGamePrivate();

        protected abstract void GameOverPrivate();
        #endregion
    }
}