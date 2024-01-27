#if PEARL_2DSIDE

using NodeCanvas.Tasks.Conditions;
using Pearl;
using Pearl.ClockManager;
using Pearl.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pearl.Side2D
{
    public class AbilityContainer
    {
        public CharacterAbility ability;
        public ControlAbilityNative control;
        public bool permission;

        public AbilityContainer(CharacterAbility ability, ControlAbilityNative control, bool permision)
        {
            this.ability = ability;
            this.control = control;
            this.permission = permision;
        }
    }


    [RequireComponent(typeof(AnimatorManager)), RequireComponent(typeof(Side2DController))]
    public class CharacterManager : MonoBehaviour
    {
        public event Action<string> OnCharacterEvent;

        public event Action<string, string> OnChangeState;
        public event Action<string, string> OnChangeCondition;

        public const string defaultCondition = "Normal";
        public const string defaultState = "Idle";

        [Header("References")]
        [SerializeField]
        private SpriteRenderer spriteContainer = null;
        [SerializeField]
        protected Side2DController characterController = null;
        [SerializeField]
        private AnimatorManager animatorManager = null;

        [Header("Info")]
        [SerializeField]
        private TypeControl typeControl = TypeControl.Player;
        [SerializeField]
        private SemiAxisX initFacing = SemiAxisX.Right;
        [ConditionalField("@typeControl == Player")]
        [SerializeField]
        private int player = 0;

        [SerializeField, ReadOnly]
        protected string _currentCondition;
        [SerializeField, ReadOnly]
        protected string _currentState;
        protected string _prevState;
        protected string _prevConditions;
        private SemiAxisX _currentFacing;

        public TypeControl Controller { get { return typeControl; } }
        public int NumberPlayer { get { return typeControl == TypeControl.Player ? player : -1; } }
        public Side2DController CharacterController { get { return characterController; } }
        public AnimatorManager Animator { get { return animatorManager; } }
        public string CurrentCondition { get { return _currentCondition; } }
        public string PreviousControllerState { get { return _prevState; } }
        public string CurrentState { get { return _currentState; } }
        public SemiAxisX CurrentFacing { get { return _currentFacing; } }
        public Vector2 CurrentFacingVector { get { return CurrentFacing == SemiAxisX.Right ? Vector2.right : Vector2.left; } }
        public string OverrideDefaultState { get { return _overrideDefaultState; } set { _overrideDefaultState = value; } }


        private readonly Dictionary<Type, AbilityContainer> abilities = new();
        private string _overrideDefaultState;

        #region UnityCallbacks
        private void Awake()
        {
            ResetOverrideDefaultState();
            ResetStateAndConditions();

            ChangeFacingDirection(initFacing);
        }

        private void Update()
        {
            CheckAllState();

            UpdateAbility();
        }

        protected void Reset()
        {
            spriteContainer = GetComponent<SpriteRenderer>();
            characterController = GetComponent<Side2DController>();
            animatorManager = GetComponent<AnimatorManager>();
        }
        #endregion

        #region State and Condition
        public virtual bool CheckStateCondition(string specificState)
        {
            return true;
        }
        protected virtual void CheckAllState()
        {

        }

        protected virtual void CheckState(string specificState)
        {
        }

        protected virtual bool IsThereConflitState(string newState)
        {
            return false;
        }

        public void ResetStateAndConditions(bool ignoreConflit = false)
        {
            ResetCondition(ignoreConflit);
            ResetState(ignoreConflit);
        }

        public void ResetCondition(bool ignoreConflit = false)
        {
            NewCondition(defaultCondition, ignoreConflit);
        }

        public virtual void NewCondition(string newCondition, bool ignoreConflit = false)
        {
            if (newCondition != _currentCondition)
            {
                _prevConditions = _currentCondition;
                _currentCondition = newCondition;

                OnChangeCondition?.Invoke(_currentCondition, _prevConditions);
            }
        }

        public void ResetOverrideDefaultState()
        {
            _overrideDefaultState = defaultCondition;
        }

        public void ResetState(bool ignoreConflit = false)
        {
            NewState(_overrideDefaultState, ignoreConflit);
        }

        public void NewState(string newState, bool ignoreConflit = false)
        {
            if (newState != _currentState)
            {
                if (ignoreConflit || !IsThereConflitState(newState))
                {
                    CheckState(newState);

                    _prevState = _currentState;
                    _currentState = newState;

                    OnChangeState?.Invoke(_currentState, _prevState);
                }
            }
        }
        #endregion

        #region Ability
        public void CallCharacterEvent(string ev)
        {
            OnCharacterEvent?.Invoke(ev);
        }

        public void UpdateAbility()
        {
            foreach (var abilityStruct in abilities.Values)
            {
                if (abilityStruct.permission && abilityStruct.ability && abilityStruct.control && abilityStruct.ability.Activated)
                {
                    abilityStruct.control.UpdateInput();
                    abilityStruct.ability.UpdateAbility();
                }
            }
        }

        public CharacterAbility FindAbility(Type t)
        {
            if (abilities.TryGetValue(t, out var abilityStruct))
            {
                return abilityStruct.ability;
            }
            return null;
        }

        public T FindAbility<T>() where T : CharacterAbility
        {
            if (abilities.TryGetValue(typeof(T), out var abilityStruct))
            {
                return (T)abilityStruct.ability;
            }
            return null;
        }

        public bool FindAbility<T>(out T result) where T : CharacterAbility
        {
            result = FindAbility<T>();
            return result != null;
        }

        protected void SetPermission<T>(bool permission) where T : CharacterAbility
        {
            if (abilities.TryGetValue(typeof(T), out var abilityStruct))
            {
                abilityStruct.permission = permission;
            }
        }

        public void AddAbility(CharacterAbility ability, ControlAbilityNative control)
        {
            abilities.Add(ability.GetType(), new AbilityContainer(ability, control, true));
        }

        public void RemoveAbility(CharacterAbility ability)
        {
            abilities.Remove(ability.GetType());
        }
        #endregion

        #region Facing
        public void ChangeFacingDirection(SemiAxisX newAxis)
        {
            _currentFacing = newAxis;

            if (spriteContainer)
            {
                if (newAxis == SemiAxisX.Left)
                {
                    spriteContainer.flipX = true;
                }
                else
                {
                    spriteContainer.flipX = false;
                }
            }
        }

        public void Flip()
        {
            var newFacing = _currentFacing == SemiAxisX.Right ? SemiAxisX.Left : SemiAxisX.Right;
            ChangeFacingDirection(newFacing);
        }
        #endregion

        protected bool CheckExistState(string specificState, params string[] states)
        {
            if (specificState != CurrentState)
            {
                foreach (var state in states)
                {
                    if (state == CurrentState)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

}

#endif