using Ink.Runtime;
using NodeCanvas.BehaviourTrees;
using Pearl.AI;
using Pearl.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using static UnityEngine.InputSystem.InputAction;
using static UnityEngine.InputSystem.PlayerInput;

namespace Pearl.Input
{
    [Serializable]
    public struct InputInfo
    {
        public string nameInput;
        public string nameMap;

        public InputInfo(string nameInput, string nameMap)
        {
            this.nameInput = nameInput;
            this.nameMap = nameMap;
        }

        public override bool Equals(object obj)
        {
            if (obj is InputInfo info)
            {
                return nameInput == info.nameInput &&
                    nameMap == info.nameMap;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(nameInput, nameMap);
        }
    }

    [Serializable]
    public struct InfoInputButton
    {
        public string nameInput;
        public StateButton stateButton;

        public InfoInputButton(string nameInput, StateButton stateButton)
        {
            this.nameInput = nameInput;
            this.stateButton = stateButton;
        }

        public override bool Equals(object obj)
        {
            if (obj is InfoInputButton infoButton)
            {
                return infoButton.nameInput == nameInput && infoButton.stateButton == stateButton;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(nameInput, stateButton);
        }
    }

    public class InputInterface : PearlBehaviour
    {
        private class InputState
        {
            #region Structs
            public class ActionForDelegateInfo
            {
                public Action action;
                public bool interrunptInput = false;
                public int order = 0;

                public void AddAction(Action newAction)
                {
                    action += newAction;
                }

                public void RemoveAction(Action oldAction)
                {
                    action -= oldAction;
                }

                public bool IsNull()
                {
                    return action == null;
                }
            }

            public struct DelegateInfo
            {
                public string inputActonString;
                public StateButton stateButton;

                public DelegateInfo(string inputName, StateButton stateButton)
                {
                    this.inputActonString = inputName;
                    this.stateButton = stateButton;
                }

                public override bool Equals(object obj)
                {
                    if (obj is DelegateInfo delegateInfo)
                    {
                        return delegateInfo.inputActonString == inputActonString && delegateInfo.stateButton == stateButton;
                    }
                    return false;
                }

                public override int GetHashCode()
                {
                    return HashCode.Combine(inputActonString, stateButton);
                }
            }
            #endregion

            #region Private Fields
            private readonly InputActionAsset _actions;
            private readonly Dictionary<DelegateInfo, ActionForDelegateInfo> _actionDelegates = new();
            private readonly Dictionary<string, List<string>> _mapsBlock = new();

            private readonly Dictionary<InputAction, int> _inputActions = new();
            private readonly Dictionary<string, bool> _axisPastValue = new();

            private bool _interrupt;
            private readonly List<ActionForDelegateInfo> _actionsForInvoke = new();
            private string _previousInvoke;
            private Dictionary<string, float> _virtualAxisValue = new();
            private Dictionary<string, Vector2> _virtualVectorValue = new();
            #endregion

            #region Properties
            public bool Enable { get; set; }
            #endregion

            #region Constructor
            public InputState(PlayerInput playerInput)
            {
                if (playerInput)
                {
                    _actions = playerInput.actions;
                }

                Enable = true;
            }
            #endregion

            #region Public Methods
            public void LateUpdate()
            {
                _previousInvoke = null;
                _interrupt = false;

                if (_actionsForInvoke.Count > 0)
                {
                    _actionsForInvoke.Sort((x, y) => x.order > y.order ? 1 : (x.order < y.order ? -1 : 0));

                    for (int i = 0; i < _actionsForInvoke.Count; i++)
                    {
                        _actionsForInvoke[i]?.action?.Invoke();
                        if (_interrupt)
                        {
                            break;
                        }
                    }

                    _actionsForInvoke.Clear();
                    _interrupt = false;
                }
            }

            #region Continuous Input
            public void ClearAixsValue()
            {
                _axisPastValue?.Clear();
            }

            public void AddVirtualAxis(in string actionString, float value, in string mapString = "")
            {
                _virtualAxisValue?.Update(actionString + mapString, value);
            }

            public void AddVirtualVector(in string actionString, Vector2 value, in string mapString = "")
            {
                _virtualVectorValue?.Update(actionString + mapString, value);
            }

            public void RemoveVirtualAxis(in string actionString, in string mapString = "")
            {
                _virtualVectorValue?.Remove(actionString + mapString);
            }

            public void RemoveVirtualVector(in string actionString, in string mapString = "")
            {
                _virtualVectorValue?.Remove(actionString + mapString);
            }

            public Vector2 GetVectorAxis(in string actionString, in bool raw = false, in string mapString = "", bool ignoreBlock = false, Func<Vector2, Vector2> filter = null)
            {
                Vector2 vector = Vector2.zero;
                if (!ignoreBlock && _interrupt)
                {
                    return vector;
                }
                if (!string.IsNullOrEmpty(mapString) && _mapsBlock.IsNotNullAndTryGetValue(mapString, out var exception) && exception != null && !exception.Contains(actionString))
                {
                    return vector;
                }


                if (actionString != null)
                {
                    if (!_virtualVectorValue.TryGetValue(actionString + mapString, out vector))
                    {
                        InputAction action;
                        if (mapString != string.Empty)
                        {
                            var mapAction = _actions.FindActionMap(mapString);
                            action = mapAction.FindAction(actionString);
                        }
                        else
                        {
                            action = _actions.FindAction(actionString);
                        }

                        if (action != null)
                        {
                            if (action.type != InputActionType.Button)
                            {
                                try
                                {
                                    vector = action.ReadValue<Vector2>();
                                }
                                catch
                                {
                                    Debug.Log("Wrong reader input");
                                    vector = Vector2.zero;
                                }
                            }
                        }
                    }

                    vector = raw ? Vector2Extend.Sign(vector) : vector;
                    vector = filter != null ? filter.Invoke(vector) : vector;
                }
                return vector;
            }

            public Vector2 GetVectorAxis(in string actionString, StateButton stateButton, in bool raw = false, in string mapString = "", bool ignoreBlock = false, Func<Vector2, Vector2> filter = null)
            {
                if (!_axisPastValue.TryGetValue(actionString, out bool pastValue))
                {
                    _axisPastValue.Add(actionString, pastValue);
                }

                Vector2 newValue = GetVectorAxis(actionString, raw, mapString, ignoreBlock, filter);
                bool result = newValue != Vector2.zero;
                _axisPastValue[actionString] = result;

                if (stateButton == StateButton.Down)
                {
                    return result && !pastValue ? newValue : Vector2.zero;
                }
                else if (stateButton == StateButton.Up)
                {
                    return !result && pastValue ? newValue : Vector2.zero;
                }
                else
                {
                    return newValue;
                }
            }

            public float GetAxis(in string actionString, in bool raw = false, in string mapString = "", bool ignoreBlock = false, Func<float, float> filter = null)
            {
                float value = 0;
                if (!ignoreBlock && _interrupt)
                {
                    return value;
                }
                if (!string.IsNullOrEmpty(mapString) && _mapsBlock.IsNotNullAndTryGetValue(mapString, out var exception) && exception != null && !exception.Contains(actionString))
                {
                    return value;
                }


                if (!_virtualAxisValue.TryGetValue(actionString, out value))
                {
                    if (actionString != null)
                    {
                        InputAction action;
                        if (mapString != string.Empty)
                        {
                            var mapAction = _actions.FindActionMap(mapString);
                            action = mapAction.FindAction(actionString);
                        }
                        else
                        {
                            action = _actions.FindAction(actionString);
                        }

                        if (action != null)
                        {
                            if (action.type != InputActionType.Button)
                            {
                                try
                                {
                                    value = action.ReadValue<float>();
                                }
                                catch
                                {
                                    Debugging.LogManager.Log("Wrong reader input");
                                    value = 0;
                                }
                            }
                        }
                    }
                }
                value = raw ? MathfExtend.Sign(value) : value;
                value = filter != null ? filter.Invoke(value) : value;
                return value;
            }

            public float GetAxis(in string actionString, StateButton stateButton, in bool raw = false, in string mapString = "", bool ignoreBlock = false, Func<float, float> filter = null)
            {
                if (!_axisPastValue.TryGetValue(actionString, out bool pastValue))
                {
                    _axisPastValue.Add(actionString, pastValue);
                }

                float newValue = GetAxis(actionString, raw, mapString, ignoreBlock, filter);
                bool result = newValue != 0;
                _axisPastValue[actionString] = result;

                if (stateButton == StateButton.Down)
                {
                    return result && !pastValue ? newValue : 0;
                }
                else if (stateButton == StateButton.Up)
                {
                    return !result && pastValue ? newValue : 0;
                }
                else
                {
                    return newValue;
                }
            }
            #endregion

            public void InvokeVirtualAction(string inputActonString, string mapAction, StateButton stateButton)
            {
                InvokeAction(inputActonString, mapAction, stateButton);
            }

            public void ChangeInterrupt(bool interrupt)
            {
                _interrupt = interrupt;
            }

            public void ChangeInterrupt(bool interrunpt, in string actionString, in StateButton stateButton, in string mapString = null)
            {
                if (actionString != null && _actions != null)
                {
                    InputAction inputAction = FindAction(actionString, mapString);

                    if (inputAction != null)
                    {
                        var map = inputAction.actionMap;
                        DelegateInfo delegateInfo = new(actionString + map, stateButton);
                        if (_actionDelegates.TryGetValue(delegateInfo, out var actionContainer))
                        {
                            actionContainer.interrunptInput = interrunpt;
                        }
                    }
                }
            }

            public void ChangeOrder(int order, in string actionString, in StateButton stateButton, in string mapString = null)
            {
                if (actionString != null && _actions != null)
                {
                    InputAction inputAction = FindAction(actionString, mapString);

                    if (inputAction != null)
                    {
                        var map = inputAction.actionMap;
                        DelegateInfo delegateInfo = new(actionString + map, stateButton);
                        if (_actionDelegates.TryGetValue(delegateInfo, out var actionContainer))
                        {
                            actionContainer.order = order;
                        }
                    }
                }
            }

            public void PerformedHandle(in string actionString, in Action actionDown, in Action actionUp, in ActionEvent actionEvent, in string mapString = null, in int order = 0, in bool interrupt = false)
            {
                PerformedHandle(actionString, actionDown, actionEvent, StateButton.Down, mapString, order, interrupt);
                PerformedHandle(actionString, actionUp, actionEvent, StateButton.Up, mapString, order, interrupt);
            }

            public void PerformedHandle(in string actionString, in Action action, in ActionEvent actionEvent, StateButton stateButton = StateButton.Down, in string mapString = null, in int order = 0, in bool interrupt = false)
            {
                 if (actionString != null && action != null && _actions != null)
                {
                    InputAction inputAction = FindAction(actionString, mapString);

                    if (inputAction != null)
                    {
                        var map = inputAction.actionMap.name;

                        DelegateInfo delegateInfo = new(actionString + map, stateButton);
                        bool isFound = _actionDelegates.TryGetValue(delegateInfo, out var actionContainer);

                        if (actionEvent == ActionEvent.Add)
                        {
                            if (_inputActions.ContainsKey(inputAction))
                            {
                                _inputActions[inputAction]++;
                            }
                            else
                            {
                                _inputActions.Add(inputAction, 1);
                                inputAction.performed += InvokeAction;
                                inputAction.canceled += InvokeAction;
                            }


                            if (!isFound)
                            {
                                actionContainer = new ActionForDelegateInfo();
                                _actionDelegates.Add(delegateInfo, actionContainer);
                            }
                            actionContainer.AddAction(action);
                            actionContainer.order = order;
                        }
                        else if (isFound)
                        {
                            if (_inputActions.ContainsKey(inputAction))
                            {
                                _inputActions[inputAction]--;
                                if (_inputActions[inputAction] == 0)
                                {
                                    _inputActions.Remove(inputAction);
                                    inputAction.performed -= InvokeAction;
                                    inputAction.canceled -= InvokeAction;
                                }
                            }


                            actionContainer.RemoveAction(action);

                            if (actionContainer.IsNull())
                            {
                                _actionDelegates.Remove(delegateInfo);
                            }
                        }

                        ChangeInterrupt(interrupt, actionString, stateButton, mapString);
                    }
                }
            }

            public void BlockMap(in string map, LockEnum blockState, string[] actionsException = null)
            {
                if (_mapsBlock != null)
                {
                    if (blockState == LockEnum.Lock)
                    {
                        _mapsBlock.Update(map, new List<string>(actionsException));
                    }
                    else
                    {
                        _mapsBlock.Remove(map);
                    }
                }
            }
            #endregion

            #region Private Methods
            private void InvokeAction(CallbackContext context)
            {
                if (context.action != null)
                {
                    string inputActonString = context.action.name;
                    var mapAction = context.action.actionMap.name;
                    StateButton stateButton = context.control.IsPressed() && !context.canceled ? StateButton.Down : StateButton.Up;
                    InvokeAction(inputActonString, mapAction, stateButton);
                }
            }

            private void InvokeAction(string inputActonString, string mapAction, StateButton stateButton)
            {
                if (_previousInvoke != null && _previousInvoke == inputActonString)
                {
                    return;
                }
                _previousInvoke = inputActonString;

                if (!Enable || _interrupt)
                {
                    return;
                }

                if (_mapsBlock.IsAlmostSpecificCount() && _mapsBlock.TryGetValue(mapAction, out var exception) && exception != null && !exception.Contains(inputActonString))
                {
                    return;
                }


                DelegateInfo delegateInfo = new(inputActonString + mapAction, stateButton);

                if (_actionDelegates.TryGetValue(delegateInfo, out var actionContainer))
                {
                    if (actionContainer.interrunptInput)
                    {
                        _actionsForInvoke.Clear();
                        _interrupt = true;
                    }

                    _actionsForInvoke.Add(actionContainer);
                }
            }

            private InputAction FindAction(in string actionString, in string mapString = null)
            {
                if (mapString == null || mapString == string.Empty)
                {
                    return _actions.FindAction(actionString);
                }
                else
                {
                    var newMap = _actions.FindActionMap(mapString);
                    if (newMap != null)
                    {
                        return newMap.FindAction(actionString);
                    }
                }
                return null;
            }
            #endregion
        }

        #region Inspector Fields
        [SerializeField]
        private PlayerInput playerInput = null;
        [SerializeField]
        private string UIMap = "UI";

        [SerializeField]
        [ReadOnly]
        [Tooltip("Device")]
        private InputDeviceEnum currentInputDevice = InputDeviceEnum.Null;

        [SerializeField]
        [ReadOnly]
        [Tooltip("Device")]
        private int numberPlayer = -1;

        [SerializeField]
        [ReadOnly]
        [Tooltip("Device")]
        public string _currentMap = string.Empty;
        #endregion

        #region Private Fields
        private InputState _inputState = null;
        #endregion

        #region Properties
        /// <summary>
        /// The current Input Device
        /// </summary>
        public InputDeviceEnum CurrentInputDevice { get { return currentInputDevice; } }

        public PlayerInput PlayerInput { get { return playerInput; } }

        public int NumberPlayer { get { return numberPlayer; } }
        #endregion

        #region Unity Callbacks
        protected override void PearlAwake()
        {
            _inputState = new InputState(playerInput);
            PearlEventsManager.AddAction<string>(ConstantStrings.newSceneEvent, OnChangeScene);
        }

        protected void Reset()
        {
            playerInput = GetComponent<PlayerInput>();
        }

        protected override void PearlStart()
        {
            OnChangeController(playerInput);

            if (playerInput && playerInput.currentActionMap != null)
            {
                playerInput.onControlsChanged += OnChangeController;
                _currentMap = playerInput.currentActionMap.name;
            }


            AddUIModule(InputManager.InputSystemUI);
        }

        protected void LateUpdate()
        {
            _inputState?.LateUpdate();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (playerInput)
            {
                playerInput.onControlsChanged -= OnChangeController;
            }

            PearlEventsManager.RemoveAction<string>(ConstantStrings.newSceneEvent, OnChangeScene);
        }
        #endregion

        #region Public Methods

        #region Axis

        public void AddVirtualAxis(in string actionString, float value, in string mapString = "")
        {
            _inputState?.AddVirtualAxis(actionString, value, mapString);
        }

        public void AddVirtualVector(in string actionString, Vector2 value, in string mapString = "")
        {
            _inputState?.AddVirtualVector(actionString, value, mapString);
        }

        public void RemoveVirtualAxis(in string actionString, in string mapString = "")
        {
            _inputState?.RemoveVirtualAxis(actionString, mapString);
        }

        public void RemoveVirtualVector(in string actionString, in string mapString = "")
        {
            _inputState?.RemoveVirtualVector(actionString, mapString);
        }

        public void ClearAixsValue()
        {
            _inputState?.ClearAixsValue();
        }

        public Vector2 GetVectorAxis(in string actionString, StateButton stateButton, in bool raw = false, in string mapString = "", bool ignoreBlock = false, Func<Vector2, Vector2> filter = null)
        {
            if (_inputState != null)
            {
                return _inputState.GetVectorAxis(actionString, stateButton, raw, mapString, ignoreBlock, filter);
            }
            return Vector2.zero;
        }

        public Vector2 GetVectorAxis(in string actionString, in bool raw = false, in string mapString = "", bool ignoreBlock = false, Func<Vector2, Vector2> filter = null)
        {
            if (_inputState != null)
            {
                return _inputState.GetVectorAxis(actionString, raw, mapString, ignoreBlock, filter);
            }
            return Vector2.zero;
        }

        public float GetAxis(in string actionString, StateButton stateButton, in bool raw = false, in string mapString = "", bool ignoreBlock = false, Func<float, float> filter = null)
        {
            if (_inputState != null)
            {
                return _inputState.GetAxis(actionString, stateButton, raw, mapString, ignoreBlock, filter);
            }

            return 0;
        }

        public float GetAxis(in string actionString, in bool raw = false, in string mapString = "", bool ignoreBlock = false, Func<float, float> filter = null)
        {
            if (_inputState != null)
            {
                return _inputState.GetAxis(actionString, raw, mapString, ignoreBlock, filter);
            }

            return 0;
        }
        #endregion

        #region Button
        public void InvokeVirtualAction(string inputActonString, string mapAction, StateButton stateButton)
        {
            _inputState?.InvokeVirtualAction(inputActonString, mapAction, stateButton);
        }

        public void PerformedHandle(in InputInfo inputInfo, in Action actionDown, in Action actionUp, in ActionEvent actionEvent, in int order = 0, in bool interrupt = false)
        {
            PerformedHandle(inputInfo.nameInput, actionDown, actionUp, actionEvent, inputInfo.nameMap, order, interrupt);
        }

        public void PerformedHandle(in string actionString, in Action actionDown, in Action actionUp, in ActionEvent actionEvent, in string mapString = null, in int order = 0, in bool interrupt = false)
        {
            if (_inputState != null && actionString != null)
            {
                _inputState.PerformedHandle(actionString.CamelCase(), actionDown, actionUp, actionEvent, mapString, order, interrupt);
            }
        }

        public void ChangeInterrupt(bool interrupt, in string actionString, in StateButton stateButton, in string mapString = null)
        {
            if (_inputState != null)
            {
                _inputState.ChangeInterrupt(interrupt, actionString, stateButton, mapString);
            }
        }

        public void ChangeInterrupt(bool interrupt)
        {
            if (_inputState != null)
            {
                _inputState.ChangeInterrupt(interrupt);
            }
        }

        public void ChangeOrder(int order, in string actionString, in StateButton stateButton, in string mapString = null)
        {
            if (_inputState != null)
            {
                _inputState. ChangeOrder(order, actionString, stateButton, mapString);
            }
        }

        public void PerformedHandle(in string actionString, in Action action, in ActionEvent actionEvent, StateButton stateButton = StateButton.Down, in string mapString = null, in int order = 0, in bool interrupt = false)
        {
            if (_inputState != null && actionString != null)
            {
                _inputState.PerformedHandle(actionString.CamelCase(), action, actionEvent, stateButton, mapString, order, interrupt);
            }
        }

        public void PerformedHandle(in InputInfo inputInfo, in Action action, in ActionEvent actionEvent, StateButton stateButton = StateButton.Down, in int order = 0, in bool interrupt = false)
        {
            PerformedHandle(inputInfo.nameInput, action, actionEvent, stateButton, inputInfo.nameMap, order, interrupt);
        }
        #endregion

        #region Map
        public void SetSwitchMap(in string newMapName, bool UIEnable = true, List<string> mapForAbilitate = null, List<string> mapForDisabilitate = null)
        {
            if (_inputState != null && newMapName != null && playerInput.actions.IsNotNull(out var actions))
            {
                var newMap = actions.FindActionMap(newMapName.CamelCase());
                if (newMap != null)
                {
                    playerInput.currentActionMap = newMap;
                    _currentMap = playerInput.currentActionMap.name;
                }


                if (UIEnable)
                {
                    actions.FindActionMap(UIMap)?.Enable();
                }

                if (mapForAbilitate != null)
                {
                    foreach (string mapString in mapForAbilitate)
                    {
                        actions.FindActionMap(mapString)?.Enable();
                    }
                }

                if (mapForDisabilitate != null)
                {
                    foreach (string mapString in mapForDisabilitate)
                    {
                        actions.FindActionMap(mapString)?.Disable();
                    }
                }
            }
        }

        public void BlockMap(in string map, LockEnum lockState, params string[] actionsException)
        {
            _inputState.BlockMap(map, lockState, actionsException);
        }
        #endregion

        #endregion

        #region Private Methods
        private void OnChangeScene(string _)
        {
            ClearAixsValue();
        }

        //Via reflection
        private void SetNumberPlayer(int numberPlayerParam)
        {
            numberPlayer = numberPlayerParam;
            gameObject.name = "InputPlayer " + numberPlayer;
        }

        //Via reflection
        private void ActivePlayerInput(bool active)
        {
            if (_inputState != null)
            {
                _inputState.Enable = active;
            }

            if (!active)
            {
                PearlInvoke.WaitForMethod(0.001f, DeactiveInput, TimeType.Unscaled);
            }
            else
            {
                PearlInvoke.StopTimer(DeactiveInput);
                playerInput.ActivateInput();
            }
        }

        private void DeactiveInput()
        {
            playerInput.DeactivateInput();
        }

        private void AddUIModule(InputSystemUIInputModule UIInputModule)
        {
            if (playerInput)
            {
                playerInput.uiInputModule = UIInputModule;
            }
        }
        private InputDeviceEnum CurrentInputDeviceInternal()
        {
            if (playerInput != null && playerInput.devices.IsNotNull(out var devices))
            {
                foreach (InputDevice device in devices)
                {
                    string deviceName = device.displayName;
                    if (deviceName.Contains("Keyboard"))
                    {
                        return InputDeviceEnum.Keyboard;
                    }
                    else if (deviceName.Contains("Controller"))
                    {
                        return InputDeviceEnum.Xbox;
                    }
                    else
                    {
                        return InputDeviceEnum.Other;
                    }
                }
            }
            return InputDeviceEnum.Null;
        }

        private void OnChangeController(PlayerInput playerInput)
        {
            currentInputDevice = CurrentInputDeviceInternal();
            PearlEventsManager.CallEvent(ConstantStrings.ChangeInputDevice, PearlEventType.Normal, currentInputDevice, numberPlayer);
        }
        #endregion
    }
}
