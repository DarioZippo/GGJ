#if PEARL_2DSIDE

using Pearl;
using Pearl.Input;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

namespace Pearl.Side2D
{
    public abstract class ControlAbility<T> : ControlAbilityNative where T : CharacterAbility
    {
        #region Inspector
        [SerializeField]
        protected T ability = null;
        [SerializeField]
        protected CharacterManager manager = null;

        private InputInterface _inputInterface;
        CharacterGravity _characterGravity;
        #endregion

        #region Unity Callbacks
        protected override void PearlAwake()
        {
            //ability.OnChangePermission += ChangeAbilityPermission;
        }

        protected override void PearlStart()
        {
            _inputInterface = manager != null ? InputManager.Get(manager.NumberPlayer) : null;
            _characterGravity = manager.FindAbility<CharacterGravity>();

            CreateInput();
        }

        private void Reset()
        {
            ability = GetComponent<T>();
            manager = this.GetComponentInParent<CharacterManager>(true);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            _blockInput = true;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            _blockInput = false;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            //ability.OnChangePermission -= ChangeAbilityPermission;
        }
        #endregion

        #region Input
        protected void FixInputForChangeGravity(ref Vector2 input)
        {
            if (_characterGravity != null)
            {
                if (_characterGravity.ShouldReverseInput())
                {
                    if (_characterGravity.ReverseVerticalInputWhenUpsideDown)
                    {
                        input.y = -input.y;
                    }
                    if (_characterGravity.ReverseHorizontalInputWhenUpsideDown)
                    {
                        input.x = -input.x;
                    }
                }
            }
        }

        protected void FixInputYForChangeGravity(ref float y)
        {
            if (_characterGravity != null)
            {
                if (_characterGravity.ShouldReverseInput())
                {
                    if (_characterGravity.ReverseVerticalInputWhenUpsideDown)
                    {
                        y = -y;
                    }
                }
            }
        }

        protected void FixInputXForChangeGravity(ref float x)
        {
            if (_characterGravity != null)
            {
                if (_characterGravity.ShouldReverseInput())
                {
                    if (_characterGravity.ReverseHorizontalInputWhenUpsideDown)
                    {
                        x = -x;
                    }
                }
            }
        }

        protected void RegistryAction(string nameInput, Action action, StateButton stateButton, int order = 0, bool interrupt = false)
        {
            if (_inputInterface == null)
            {
                return;
            }

            void currentAction()
            {
                if (!_blockInput)
                {
                    action?.Invoke();
                }
            }

            _inputInterface.PerformedHandle(nameInput, currentAction, ActionEvent.Add, stateButton, null, order, interrupt);

            DestroyHandler += () =>
            {
                _inputInterface.PerformedHandle(nameInput, currentAction, ActionEvent.Remove, stateButton, null, order, interrupt);
            };

        }

        protected void RegistryAction(in string nameInput, Action actionDown, Action actionUp, int order = 0, bool interrupt = false)
        {
            RegistryAction(nameInput, actionDown, StateButton.Down, order, interrupt);
            RegistryAction(nameInput, actionUp, StateButton.Up, order, interrupt);
        }

        protected float GetFloatInput(in string nameInput, StateButton stateButton, bool useRaw = false, in string mapString = "", bool ignoreBlock = false)
        {
            return _inputInterface && !_blockInput ? _inputInterface.GetAxis(nameInput, stateButton, useRaw, mapString, ignoreBlock) : 0;
        }

        protected float GetFloatInput(in string nameInput, bool useRaw = false, in string mapString = "", bool ignoreBlock = false)
        {
            return _inputInterface && !_blockInput ? _inputInterface.GetAxis(nameInput, useRaw, mapString, ignoreBlock) : 0;
        }

        protected Vector2 GetVectorInput(in string nameInput, StateButton stateButton, bool useRaw = false, in string mapString = "", bool ignoreBlock = false)
        {
            return _inputInterface && !_blockInput ? _inputInterface.GetVectorAxis(nameInput, stateButton, useRaw, mapString, ignoreBlock) : Vector2.zero;
        }

        protected Vector2 GetVectorInput(in string nameInput, bool useRaw = false, in string mapString = "", bool ignoreBlock = false)
        {
            return _inputInterface && !_blockInput ? _inputInterface.GetVectorAxis(nameInput, useRaw, mapString, ignoreBlock) : Vector2.zero;
        }

        private void ChangeAbilityPermission(bool permission)
        {
            _blockInput = !permission;
        }

        protected virtual void CreateInput()
        {

        }
        #endregion
    }
}

#endif