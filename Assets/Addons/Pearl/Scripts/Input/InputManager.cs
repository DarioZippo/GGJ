﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace Pearl.Input
{
    [Serializable]
    public enum InputDeviceEnum
    {
        Null,
        Keyboard,
        Xbox,
        Playstation,
        Switch,
        Other
    }

    public enum InputType { Button, Float, Vector2 }

    [DisallowMultipleComponent]
    public class InputManager : PearlBehaviour, ISingleton
    {
        #region Inspector fields
        [SerializeField]
        private PlayerInputManager manager = null;

        [SerializeField]
        private GameObject inputPrefab = null;

        [SerializeField]
        private GameObject eventSystemPrefab = null;

        [SerializeField]
        private int playersAtStart = 1;

        [SerializeField]
        private string initMapInput = "UI";

        [SerializeField]
        private InputSystemUIInputModule inputSystemUI = null;

        [ReadOnly, SerializeField]
        private bool _enableInput = true;
        #endregion

        #region Private fields
        private static readonly List<InputInterface> _players = new();
        #endregion

        #region Static

        #region Direct 1 player

        #region Axis
        public static PlayerInput GetPlayerInput()
        {
            var inputInterface = Get(0);
            if (inputInterface != null)
            {
                return inputInterface.PlayerInput;
            }
            return default;
        }

        public static Vector2 GetVectorAxis(in string actionString, in bool raw = false, in string mapString = "", in bool ignoreBlock = false, Func<Vector2, Vector2> filter = null)
        {
            var inputInterface = Get(0);
            if (inputInterface != null)
            {
                return inputInterface.GetVectorAxis(actionString, raw, mapString, ignoreBlock, filter);
            }
            return default;
        }

        public void ClearAixsValue()
        {
            var inputInterface = Get(0);
            if (inputInterface != null)
            {
                inputInterface.ClearAixsValue();
            }
        }

        public static float GetAxis(in string actionString, in bool raw = false, in string mapString = "", in bool ignoreBlock = false, Func<float, float> filter = null)
        {
            var inputInterface = Get(0);
            if (inputInterface != null)
            {
                return inputInterface.GetAxis(actionString, raw, mapString, ignoreBlock, filter);
            }
            return default;
        }

        public static float GetAxis(in string actionString, StateButton stateButton, in bool raw = false, in string mapString = "", bool ignoreBlock = false, Func<float, float> filter = null)
        {
            var inputInterface = Get(0);
            if (inputInterface != null)
            {
                return inputInterface.GetAxis(actionString, stateButton, raw, mapString, ignoreBlock, filter);
            }
            return default;
        }

        public static void AddVirtualAxis(in string actionString, float value, in string mapString = "")
        {
            var inputInterface = Get(0);
            if (inputInterface != null)
            {
                inputInterface.AddVirtualAxis(actionString, value, mapString);
            }
        }

        public static void AddVirtualVector(in string actionString, Vector2 value, in string mapString = "")
        {
            var inputInterface = Get(0);
            if (inputInterface != null)
            {
                inputInterface.AddVirtualVector(actionString, value, mapString);
            }
        }

        public static void RemoveVirtualAxis(in string actionString, in string mapString = "")
        {
            var inputInterface = Get(0);
            if (inputInterface != null)
            {
                inputInterface.RemoveVirtualAxis(actionString, mapString);
            }
        }

        public static void RemoveVirtualVector(in string actionString, in string mapString = "")
        {
            var inputInterface = Get(0);
            if (inputInterface != null)
            {
                inputInterface.RemoveVirtualAxis(actionString, mapString);
            }
        }
        #endregion

        #region Button
        public static void InvokeVirtualAction(string inputActonString, string mapAction, StateButton stateButton)
        {
            var inputInterface = Get(0);
            if (inputInterface != null)
            {
                inputInterface.InvokeVirtualAction(inputActonString, mapAction, stateButton);
            }
        }

        public static void PerformedHandle(in InputInfo inputInfo, in Action actionDown, in Action actionUp, in ActionEvent actionEvent, in int order = 0, in bool interrupt = false)
        {
            var inputInterface = Get(0);
            if (inputInterface != null)
            {
                inputInterface.PerformedHandle(inputInfo, actionDown, actionUp, actionEvent, order, interrupt);
            }
        }

        public static void ChangeInterrupt(bool interrunpt, in string actionString, in StateButton stateButton, in string mapString = null)
        {
            var inputInterface = Get(0);
            if (inputInterface != null)
            {
                inputInterface.ChangeInterrupt(interrunpt, actionString, stateButton, mapString);
            }
        }

        public static void ChangeInterrupt(bool interrupt)
        {
            var inputInterface = Get(0);
            if (inputInterface != null)
            {
                inputInterface.ChangeInterrupt(interrupt);
            }
        }

        public static void ChangeOrder(int order, in string actionString, in StateButton stateButton, in string mapString = null)
        {
            var inputInterface = Get(0);
            if (inputInterface != null)
            {
                inputInterface.ChangeOrder(order, actionString, stateButton, mapString);
            }
        }

        public static void PerformedHandle(in string actionString, in Action actionDown, in Action actionUp, in ActionEvent actionEvent, in string mapString = null, in int order = 0, in bool interrupt = false)
        {
            var inputInterface = Get(0);
            if (inputInterface != null)
            {
                inputInterface.PerformedHandle(actionString, actionDown, actionUp, actionEvent, mapString, order, interrupt);
            }
        }

        public static void PerformedHandle(in string actionString, in Action action, in ActionEvent actionEvent, StateButton stateButton = StateButton.Down, in string mapString = null, in int order = 0, in bool interrupt = false)
        {
            var inputInterface = Get(0);
            if (inputInterface != null)
            {
                inputInterface.PerformedHandle(actionString, action, actionEvent, stateButton, mapString, order, interrupt);
            }
        }

        public static void PerformedHandle(in InputInfo inputInfo, in Action action, in ActionEvent actionEvent, StateButton stateButton = StateButton.Down, in int order = 0, in bool interrupt = false)
        {
            var inputInterface = Get(0);
            if (inputInterface != null)
            {
                inputInterface.PerformedHandle(inputInfo, action, actionEvent, stateButton, order, interrupt);
            }
        }
        #endregion

        #region Map
        public static void SetSwitchMap(in string newMapName, bool UIEnable = true, List<string> mapForAbilitate = null, List<string> mapForDisabilitate = null)
        {
            var inputInterface = Get(0);
            if (inputInterface != null)
            {
                inputInterface.SetSwitchMap(newMapName, UIEnable, mapForAbilitate, mapForDisabilitate);
            }
        }

        public static void BlockMap(in string map, LockEnum lockState, params string[] actionsException)
        {
            var inputInterface = Get(0);
            if (inputInterface != null)
            {
                inputInterface.BlockMap(map, lockState, actionsException);
            }
        }
        #endregion

        #endregion

        #region Propierties
        public static int NumberPlayers { get { return _players.Count; } }

        public static List<InputInterface> Players { get { return _players; } }

        public static InputSystemUIInputModule InputSystemUI
        {
            get
            {
                if (GetIstance(out var inputManager))
                {
                    if (inputManager.inputSystemUI == null)
                    {
                        inputManager.inputSystemUI = GameObject.FindFirstObjectByType<InputSystemUIInputModule>();
                    }

                    return inputManager.inputSystemUI;
                }
                return null;
            }
        }

        public static InputInterface Input { get { return Get(); } }

        public static GameObject InputPrefab
        {
            get
            {
                if (Singleton<InputManager>.GetIstance(out var inputManager))
                {
                    return inputManager.inputPrefab;
                }
                return null;
            }

            set
            {
                if (Singleton<InputManager>.GetIstance(out var inputManager) && inputManager.manager != null && value != null)
                {
                    inputManager.inputPrefab = value;
                    inputManager.manager.playerPrefab = value;
                }
            }
        }

        public static bool EnableInput
        {
            get
            {
                if (GetIstance(out var inputManager))
                {
                    return inputManager._enableInput;
                }
                return false;
            }
            private set
            {
                if (GetIstance(out var inputManager))
                {
                    inputManager._enableInput = value;
                }
            }
        }

        #endregion

        #region Public Methods
        public static InputActionPhase GetMovementState()
        {
            if (GetIstance(out var inputManager))
            {
                return inputManager.inputSystemUI.move.action.phase;
            }
            return InputActionPhase.Disabled;
        }

        public static void ActiveAllInput(bool active)
        {
            if (_players != null)
            {
                for (int i = 0; i < _players.Count; i++)
                {
                    ActivePlayer(active, i);
                }
            }
        }

        public static void ActivePlayer(in bool active, in int indexPlayer = 0)
        {
            InputInterface manager = Get(indexPlayer);
            if (manager != null)
            {
                ReflectionExtend.Invoke(manager, "ActivePlayerInput", active);
            }

            if (indexPlayer == 0)
            {
                EnableInput = active;
            }
        }

        public static void RemoveAllPlayer()
        {
            if (_players != null)
            {
                for (int i = _players.Count - 1; i >= 0; i--)
                {
                    RemovePlayer(i);
                }
            }
        }

        public static void RemovePlayer(int indexPlayer)
        {
            if (_players.IsAlmostSpecificCount(indexPlayer))
            {
                var inputInterface = _players[indexPlayer];
                _players.Remove(inputInterface);
                GameObject.Destroy(_players[indexPlayer].gameObject);
            }
        }

        public static void AddPlayer(string controlScheme = null, InputDevice device = null)
        {
            if (GetIstance(out var inputManager))
            {
                inputManager.AddPlayerPrivate(controlScheme, device);
            }
        }

        public static InputInterface Get(int indexPlayer = 0)
        {
            if (_players.IsAlmostSpecificCount(indexPlayer))
            {
                return _players[indexPlayer];
            }
            else if (indexPlayer == 0)
            {
                if (GetIstance(out _))
                {
                    if (_players.IsAlmostSpecificCount(indexPlayer))
                    {
                        return _players[0];
                    }
                }
            }
            else
            {
                Debugging.LogManager.LogWarning("The player isn't exist", "Input");
            }
            return null;
        }
        #endregion

        #region Private methods
        private static bool GetIstance(out InputManager inputManager)
        {
            var result = Singleton<InputManager>.GetIstance(out inputManager, "PearlInputManager", true);
            if (inputManager != null)
            {
                inputManager.ForceAwake();
            }
            return result;
        }
        #endregion

        #endregion

        #region Unity CallBacks
        protected override void PearlAwake()
        {
            _enableInput = true;
            InputPrefab = inputPrefab;

            if (eventSystemPrefab && !GameObject.FindFirstObjectByType<InputSystemUIInputModule>())
            {
                GameObjectExtend.CreateGameObject(eventSystemPrefab, out _, onlyInTheScene: true);
            }

            for (int i = 0; i < playersAtStart; i++)
            {
                AddPlayer();
            }
        }

        protected void Reset()
        {
            manager = GetComponent<PlayerInputManager>();
        }
        #endregion

        #region Private Methods
        private void AddPlayerPrivate(string controlScheme = null, InputDevice device = null)
        {
            if (manager == null)
            {
                return;
            }

            PlayerInput playerInput = manager.JoinPlayer(_players.Count, -1, controlScheme, device);

            if (playerInput == null)
            {
                return;
            }

            playerInput.transform.SetParent(transform);
            InputInterface inputInterface = playerInput.GetComponent<InputInterface>();

            if (inputInterface == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(initMapInput))
            {
                inputInterface.SetSwitchMap(initMapInput);
            }

            _players.Add(inputInterface);
            ReflectionExtend.Invoke(inputInterface, "SetNumberPlayer", _players.Count);
        }
        #endregion
    }
}
