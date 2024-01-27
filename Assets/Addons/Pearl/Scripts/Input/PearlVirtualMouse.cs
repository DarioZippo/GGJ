using System;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using Pearl.UI;
using Pearl.Input;
using TMPro;
using Pearl.Events;

namespace Pearl.Input
{
    //TO DO: Testing approfondito
    public class PearlVirtualMouse : PearlBehaviour, ISingleton
    {
        public enum VirtualMouseEnum { OnlyGamepad, Always };

        [SerializeField]
        private float cursorSpeedGamepad = 10f;
        [SerializeField]
        private float cursorSpeedMouse = 1f;
        [SerializeField]
        private Sprite cursorSprite = null;
        [SerializeField]
        private VirtualMouseEnum state;
        [SerializeField]
        private bool atEnable = false;

        [SerializeField]
        private string nameTagVirtualCursor = "VirtualCursor";
        [SerializeField]
        private string nameTagContainerVirtualCursor = "ContainerVirtualCursor";

        private RectTransform _container;
        private PlayerInput _playerInput;
        private RectTransform _cursorTransform;
        private RectTransform _canvasRectTransform;
        private Canvas _canvas;
        private Vector2 _sizeCursonInScreenSpace;

        private Mouse _virtualMouse;
        private bool previousMouseState;
        private Camera _mainCamera;
        private Mouse _currentMouse;
        private Rect _containerRect;

        private bool _abilitateVirtualMouse;

        private const string gamepadScheme = "Gamepad";
        private const string mouseScheme = "Keyboard&Mouse";
        private string previousControlScheme = "";

        public static RectTransform Container
        {
            set
            {
                if (Singleton<PearlVirtualMouse>.GetIstance(out var manager))
                {
                    manager._container = value;
                    if (value != null)
                    {
                        manager._containerRect = manager._container.RectTransformToScreenSpace();
                    }
                    else if (manager._canvasRectTransform != null)
                    {
                        manager._containerRect = manager._canvasRectTransform.RectTransformToScreenSpace();
                    }
                }
            }
        }

        public static VirtualMouseEnum State
        {
            set
            {
                if (Singleton<PearlVirtualMouse>.GetIstance(out var manager))
                {
                    if (manager.state != value)
                    {
                        manager.DisabilitateVirtualMouse();
                        manager.state = value;
                        manager.AbilitateVirtualMouse();
                    }
                }
            }
        }

        public static void Abilitate()
        {
            if (Singleton<PearlVirtualMouse>.GetIstance(out var manager))
            {
                manager.AbilitateVirtualMouse();
            }
        }

        public static void Disabilitate()
        {
            if (Singleton<PearlVirtualMouse>.GetIstance(out var manager))
            {
                manager.DisabilitateVirtualMouse();
            }
        }

        protected override void PearlAwake()
        {
            _mainCamera = Camera.main;
            _currentMouse = Mouse.current;        
        }


        protected override void OnEnable()
        {
            base.OnEnable();

            if (atEnable)
            {
                AbilitateVirtualMouse();
            }
        }
        protected override void OnDisable()
        {
            base.OnDisable();

            DisabilitateVirtualMouse();
        }

        public void AbilitateVirtualMouse()
        {
            if (_abilitateVirtualMouse)
            {
                return;
            }
            previousControlScheme = "";
            _canvas = GameObject.FindAnyObjectByType<Canvas>();
            if (_canvas != null && _canvas.TryGetComponent<RectTransform>(out var rect))
            {
                _canvasRectTransform = rect;
            }
            GameObject aux = GameObject.FindGameObjectWithTag(nameTagVirtualCursor);
            if (aux != null && aux.TryGetComponent<RectTransform>(out rect) && aux.TryGetComponent<Image>(out var image))
            {
                _cursorTransform = rect;
                image.sprite = cursorSprite;
                image.enabled = true;

                Rect rectCursor = _cursorTransform.RectTransformToScreenSpace();
                _sizeCursonInScreenSpace = rectCursor.size;

            }

            _playerInput = InputManager.GetPlayerInput();

            if (_canvas == null || _canvasRectTransform == null || _cursorTransform == null || _playerInput == null)
            {
                return;
            }

            _abilitateVirtualMouse = true;

            aux = GameObject.FindGameObjectWithTag(nameTagContainerVirtualCursor);
            if (aux != null && aux.TryGetComponent<RectTransform>(out rect))
            {
                Container = rect;
            }
            else
            {
                Container = null;
            }

            if (_container == null)
            {
                _container = _canvasRectTransform;
            }

            if (_virtualMouse == null)
            {
                _virtualMouse = (Mouse)InputSystem.AddDevice("VirtualMouse");
            }
            else if (!_virtualMouse.added)
            {
                InputSystem.AddDevice(_virtualMouse);
            }

            InputUser.PerformPairingWithDevice(_virtualMouse, _playerInput.user);

            if (_cursorTransform != null)
            {
                Vector2 position = _cursorTransform.anchoredPosition + new Vector2(Screen.width / 2, Screen.height / 2);
                InputState.Change(_virtualMouse.position, position);
            }

            InputSystem.onAfterUpdate += UpdateMotion;
            _playerInput.onControlsChanged += OnControlsChanged;

            if (state == VirtualMouseEnum.OnlyGamepad)
            {
                if (Gamepad.current != null)
                {
                    ShowHardwareMouse(false);
                }
                else
                {
                    ShowVirtualMouse(false);
                }
            }
            else
            {
                ShowVirtualMouse(true);
            }
        }

        public void DisabilitateVirtualMouse()
        {
            if (!_abilitateVirtualMouse)
            {
                return;
            }

            _abilitateVirtualMouse = false;

            if (_playerInput != null && _playerInput.user != null)
            {
                try
                {
                    _playerInput.user.UnpairDevice(_virtualMouse);
                }
                catch { }
            }

            if (_virtualMouse != null && _virtualMouse.added)
            {
                InputSystem.RemoveDevice(_virtualMouse);
            }

            InputSystem.onAfterUpdate -= UpdateMotion;

            _playerInput.onControlsChanged -= OnControlsChanged;

            if (_cursorTransform != null && _cursorTransform.TryGetComponent<Image>(out var imageManager))
            {
                imageManager.enabled = false;
            }
        }

        private void UpdateMotion()
        {
            if (_virtualMouse == null || !_abilitateVirtualMouse)
            {
                return;
            }

            //delta
            Vector2 deltaValue = Vector2.zero;
            if (Gamepad.current != null)
            {
                deltaValue = Gamepad.current.leftStick.ReadValue() * cursorSpeedGamepad;
            }
            if (state == VirtualMouseEnum.Always && deltaValue == Vector2.zero && _currentMouse != null)
            {
                deltaValue = _currentMouse.delta.ReadValue() * cursorSpeedMouse;
            }

            deltaValue *=  Time.deltaTime * 100f;
            InputState.Change(_virtualMouse.delta, deltaValue);
            if (Application.isFocused && state == VirtualMouseEnum.Always && _currentMouse != null)
            {
                _currentMouse.WarpCursorPosition(_virtualMouse.position.ReadValue());
            }


            //position
            Vector2 currentPosition = _virtualMouse.position.ReadValue();
            Vector2 newPosition = currentPosition + deltaValue;


            newPosition.x = Mathf.Clamp(newPosition.x, _containerRect.min.x + (_sizeCursonInScreenSpace.x / 2), _containerRect.max.x - (_sizeCursonInScreenSpace.x / 2));
            newPosition.y = Mathf.Clamp(newPosition.y, _containerRect.min.y + (_sizeCursonInScreenSpace.y / 2), _containerRect.max.y - (_sizeCursonInScreenSpace.y / 2));

            InputState.Change(_virtualMouse.position, newPosition);
            AnchorCursor(newPosition);

            bool aButtonIsPressed = false;
            if (Gamepad.current != null)
            {
                aButtonIsPressed = Gamepad.current.aButton.IsPressed();
            }
            if (state == VirtualMouseEnum.Always && !aButtonIsPressed && _currentMouse != null)
            {
                aButtonIsPressed = _currentMouse.leftButton.IsPressed();
            }

            if (previousMouseState != aButtonIsPressed)
            {
                _virtualMouse.CopyState<MouseState>(out var mouseState);
                mouseState.WithButton(MouseButton.Left, aButtonIsPressed);
                InputState.Change(_virtualMouse, mouseState);
                previousMouseState = aButtonIsPressed;
            }
        }

        private void AnchorCursor(Vector2 position)
        {
            Camera cam = _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _mainCamera;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRectTransform, position, cam, out var anchoredPosition);
            _cursorTransform.anchoredPosition = anchoredPosition;
        }

        private void OnControlsChanged(PlayerInput input)
        {
            if (_playerInput.currentControlScheme == mouseScheme && previousControlScheme != mouseScheme)
            {
                if (state == VirtualMouseEnum.Always)
                {
                    _currentMouse.WarpCursorPosition(_virtualMouse.position.ReadValue());
                }
                else
                {
                    ShowHardwareMouse(true);
                }

                previousControlScheme = mouseScheme;
            }
            else if (state == VirtualMouseEnum.OnlyGamepad && _playerInput.currentControlScheme == gamepadScheme && previousControlScheme != gamepadScheme)
            {
                ShowVirtualMouse(true);

                previousControlScheme = gamepadScheme;
            }
        }

        private void ShowVirtualMouse(bool warp)
        {
            if (_cursorTransform != null && _cursorTransform.TryGetComponent<Image>(out var imageManager))
            {
                imageManager.enabled = true;
            }

            Cursor.visible = false;
            if (warp)
            {
                InputState.Change(_virtualMouse.position, _currentMouse.position.ReadValue());
                AnchorCursor(_currentMouse.position.ReadValue());
            }
        }

        private void ShowHardwareMouse(bool warp)
        {
            if (_cursorTransform != null && _cursorTransform.TryGetComponent<Image>(out var imageManager))
            {
                imageManager.enabled = false;
            }

            Cursor.visible = true;
            if (warp)
            {
                _currentMouse.WarpCursorPosition(_virtualMouse.position.ReadValue());
            }
        }
    }
}
