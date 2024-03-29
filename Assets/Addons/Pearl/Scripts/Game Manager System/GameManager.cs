﻿using Pearl.Audio;
using Pearl.Events;
using Pearl.Storage;
using Pearl.UI;
using System;
using System.Runtime.InteropServices;
using TypeReferences;
using UnityEngine;

namespace Pearl
{
    [DisallowMultipleComponent]
    /// <summary>
    /// The general Game manager. It will be the father of every game-specific game manager
    /// </summary>
    public class GameManager : PearlBehaviour, IStoragePlayerPrefs, ISingleton
    {
        #region Static
        public static event Action OnUpate;
        public static event Action OnFixedUpdate;
        public static event Action OnLateUpdate;

        [DllImport(dllName: "__Internal")]
        private static extern bool IsMobileWeb();

        public static bool IsMobile()
        {
            if (IsWebGL())
            {
                return IsMobileWeb();
            }
            else
            {
                return Application.isMobilePlatform;
            }
        }

        public static bool IsWebGL()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return true;
#endif
            return false;
        }

        public static bool IsLandscape()
        {
            if (Singleton<GameManager>.GetIstance(out var gameManager))
            {
                return gameManager.isLandascape;
            }

            return false;
        }

        public static void ChangeUpdateAction(ActionEvent actionEvent, UpdateModes updateMode, Action action)
        {
            if (updateMode == UpdateModes.Update)
            {
                if (actionEvent == ActionEvent.Add)
                {
                    OnUpate += action;
                }
                else
                {
                    OnUpate -= action;
                }
            }
            else if (updateMode == UpdateModes.FixedUpdate)
            {
                if (actionEvent == ActionEvent.Add)
                {
                    OnFixedUpdate += action;
                }
                else
                {
                    OnFixedUpdate -= action;
                }
            }
            else
            {
                if (actionEvent == ActionEvent.Add)
                {
                    OnLateUpdate += action;
                }
                else
                {
                    OnLateUpdate -= action;
                }
            }
        }
        #endregion

        #region Inspector Fields
        [Header("General setting")]

        [ReadOnly, SerializeField, Tooltip("Game Name")]
        private string gameName = "Game";

        [ReadOnly, SerializeField, Tooltip("If the game ran on mobile, is it landscape?")]
        private bool isLandascape = true;

        [SerializeField]
        private int limitFrameRate = 60;

        [SerializeField, ClassImplements(typeof(GameVersionManager))]
        public ClassTypeReference versionType;

        [ReadOnly, SerializeField]
        private string gameVersionString;

        [SerializeField]
        private bool thereIsOnline = true;

        [SerializeField]
        private bool updateWindowSize = true;

        [Header("Components")]

        [SerializeField, InterfaceType(typeof(IFSM))]
        protected UnityEngine.Object FSMObject = null;

        [Header("Prefabs")]

        [SerializeField]
        private GameObject canvasPrefab = null;

        [SerializeField]
        private GameObject blackPagePrefab = null;
        #endregion

        #region Private Fields
        private BlackPageManager _blackPageManager;
        private bool _isInternetReachability = false;
        private IFSM _FSM;
        #endregion

        #region Propieties
        public static string NameGame
        {
            get
            {
                if (Singleton<GameManager>.GetIstance(out var gameManager))
                {
                    return gameManager.gameName;
                }
                return null;
            }
        }

        public static bool ThereIsOnline
        {
            get
            {
                if (Singleton<GameManager>.GetIstance(out var gameManager))
                {
                    return gameManager.thereIsOnline;
                }
                return false;
            }
        }

        /// <summary>
        /// The current scene in the game
        /// </summary>
        public string GameVersion
        {
            get { return gameVersionString; }
        }

        #endregion

        #region Unity Callbacks
        protected override void PearlAwake()
        {
            SettingLimitFrameRate();

            if (FSMObject != null)
            {
                _FSM = (IFSM)FSMObject;
            }

            gameName = Application.productName;

            if (canvasPrefab)
            {
                GameObjectExtend.CreateGameObject(canvasPrefab, out _, onlyInTheScene: true);
            }

            if (blackPagePrefab)
            {
                GameObjectExtend.CreateUIlement<BlackPageManager>(blackPagePrefab, out _blackPageManager, canvasTipology: CanvasTipology.UI, onlyInTheScene: true); ;
                if (_blackPageManager)
                {
                    _blackPageManager.Disappear();
                }
            }
        }

        protected override void PearlStart()
        {
            if (versionType != null)
            {
                ReflectionExtend.Invoke<String>(versionType.Type, "ControlVersion", out gameVersionString, versionType.Type);
            }

            TextManager.SetVariableString("gameName", gameName);
        }

        protected virtual void Update()
        {
            OnUpate?.Invoke();

            ControlIsOnline();

            if (updateWindowSize)
            {
                WindowManager.OnUpdate();
            }
        }

        protected virtual void LateUpdate()
        {
            OnLateUpdate?.Invoke();
        }

        protected virtual void FixedUpdate()
        {
            OnFixedUpdate?.Invoke();
        }
        #endregion

        #region Init Methods
        private void SettingLimitFrameRate()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = limitFrameRate;
        }

        private void ControlIsOnline()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                if (_isInternetReachability)
                {
                    _isInternetReachability = false;
                    PearlEventsManager.CallEvent(ConstantStrings.InternetReachability, PearlEventType.Normal, _isInternetReachability);
                }
            }
            else if (!_isInternetReachability)
            {
                _isInternetReachability = true;
                PearlEventsManager.CallEvent(ConstantStrings.InternetReachability, PearlEventType.Normal, _isInternetReachability);
            }
        }
        #endregion

        #region Singleton Methods
        public static void Quit()
        {
#if UNITY_STANDALONE
            Application.Quit();
#endif

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        public static void SaveOption()
        {
            AudioManager.SaveAudio();
#if LOCALIZATION
            LocalizationManager.SaveLanguage();
#endif
        }

        #region FSM
        public static void CheckTransitions(bool finishState)
        {
            if (Singleton<GameManager>.GetIstance(out var gameManager) && gameManager._FSM != null)
            {
                gameManager._FSM.CheckTransitions(finishState);
            }
        }

        public static void EndAction()
        {
            if (Singleton<GameManager>.GetIstance(out var gameManager) && gameManager._FSM != null)
            {
                gameManager._FSM.EndAction();
            }
        }


        public static void CheckTransitionsAfterChangeLabel(string newLabel, bool forceFinishState = true)
        {
            if (Singleton<GameManager>.GetIstance(out var gameManager) && newLabel != null && gameManager._FSM != null)
            {
                gameManager._FSM.ChangeLabel(newLabel);
                gameManager._FSM.CheckTransitions(forceFinishState);
            }
        }

        public static void ChangeLabel(string newLabel)
        {
            if (Singleton<GameManager>.GetIstance(out var gameManager) && newLabel != null && gameManager._FSM != null)
            {
                gameManager._FSM.ChangeLabel(newLabel); ;
            }
        }

        public static void UpdateVariableFSM<T>(string nameVar, T content)
        {
            if (Singleton<GameManager>.GetIstance(out var gameManager) && nameVar != null && gameManager._FSM != null)
            {
                gameManager._FSM.UpdateVariable<T>(nameVar, content);
            }
        }

        public static T GetVariableFSM<T>(string nameVar)
        {
            if (Singleton<GameManager>.GetIstance(out var gameManager) && gameManager._FSM != null)
            {
                return gameManager._FSM.GetVariable<T>(nameVar);
            }
            return default;
        }

        public static void RemoveVariableFSM<T>(string nameVar)
        {
            if (Singleton<GameManager>.GetIstance(out var gameManager) && gameManager._FSM != null)
            {
                gameManager._FSM.RemoveVariable<T>(nameVar);
            }
        }
        #endregion


        #endregion
    }
}
