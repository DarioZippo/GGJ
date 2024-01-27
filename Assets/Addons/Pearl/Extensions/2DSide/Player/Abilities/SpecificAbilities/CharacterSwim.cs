#if PEARL_2DSIDE

using Pearl;
using Pearl.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pearl.Side2D
{
    public enum TypeSwim { NullGravity, Gravity }

    [Serializable]
    public class SwimParameters
    {
        [Header("Swim")]

        /// defines how high the character can jump
        [Tooltip("defines how speed the character can swim"), SerializeField]
        public float speedSwim = 6f;
        [SerializeField]
        public Transform surfacePoint = null;
        [SerializeField]
        public bool flipAtChangeDirection = true;
        [SerializeField, ConditionalField("@flipAtChangeDirection"),
            Tooltip("the threshold after which input is considered (usually 0.1f to eliminate small joystick noise)")]
        public float speedThreshold = 0.1f;
        [Tooltip("the type of swim"), SerializeField]
        public TypeSwim typeOfSwim = TypeSwim.NullGravity;


        [Header("Dive")]

        [SerializeField]
        public bool useDive = true;
        [SerializeField, ConditionalField("@useDive")]
        public float maxVerticalForceDive = 20f;
        [SerializeField, ConditionalField("@useDive")]
        public float maxSpeedDive = 40f;
        [SerializeField, ConditionalField("@useDive")]
        public float lessForce = 0.1f;
        [SerializeField, ConditionalField("@useDive")]
        public float minSpeedFallForActivateDiving = 10f;

        [Header("JumpWater")]

        [SerializeField, ConditionalField("@typeOfSwim==Gravity")]
        public float jumpForce = 1;
        [SerializeField]
        public float jumpExternForce = 3;

        [Header("timeLife")]

        [SerializeField]
        public bool useTimeLife = false;
        [SerializeField, ConditionalField("@useTimeLife")]
        public float timeFoSurvive = 20f;
        [ReadOnly, SerializeField, ConditionalField("@useTimeLife")]
        public float timeRemainPercent = 1f;

        [Header("Controller Parameters")]

        [SerializeField]
        public Side2DControllerParameters nullGravityParameters;
        [SerializeField]
        public Side2DControllerParameters gravityParameters;

        [Header("Dash")]

        [SerializeField]
        public bool useDash;
        [SerializeField, ConditionalField("@useDash")]
        public DashParameters dashParameters;
    }

    public class CharacterSwim : CharacterAbilityWithOverride<SwimParameters>, IEffectInZone
    {
        private float _currentIntensity = 0;
        private CharacterDash _characterDash;
        private Vector2 _inputSwim;
        private bool _inWater = false;
        private bool _isDiving = false;
        private bool _isJumping = false;
        private float _timeCurrentInWater = 0;

        public float TimeRemainPercent { get { return _overrideParameters.timeFoSurvive;  } }

        #region UnityCallbacks
        protected override void Start()
        {
            base.Start();

            if (manager != null && _overrideParameters.useDash)
            {
                _characterDash = manager.FindAbility<CharacterDash>();
            }
        }
        #endregion

        #region CharacterAbility
        public override void UpdateAbility()
        {
            if (_inWater)
            {

                if (_overrideParameters.useTimeLife)
                {
                    CheckTimeRemain();
                }


                if (_isDiving)
                {
                    Dive();
                }

                FlipAtDirection();
                Swim();
            }
        }

        public override void CheckNewPosition(ref Vector2 newPosition)
        {
            newPosition = Vector2.zero;
        }
        #endregion

        #region IEffectInZone
        public void OnZone(TriggerType type, GameObject zone)
        {
            if (!controller) return;

            _inWater = type == TriggerType.Enter;


            if (_inWater)
            {
                EnterWater();
            }
            else
            {
                ExitWater();
            }
        }

        private void EnterWater()
        {
            _timeCurrentInWater = 0;
            _overrideParameters.timeRemainPercent = 1;

            switch (_overrideParameters.typeOfSwim)
            {
                case TypeSwim.NullGravity:
                    controller.Parameters = _overrideParameters.nullGravityParameters;
                    break;
                case TypeSwim.Gravity:
                    controller.Parameters = _overrideParameters.gravityParameters;
                    break;
            }

            NewControllerState(statePlayer, true);
            if (_overrideParameters.useDive && Mathf.Abs(controller.Speed.y) >= _overrideParameters.minSpeedFallForActivateDiving)
            {
                SetDive();
            }


            if (_overrideParameters.useDash && _characterDash)
            {
                _characterDash.AddOverrideInfo(_overrideParameters.dashParameters);
            }


            controller.ResetForce();
        }

        private void ExitWater()
        {
            ResetOverrideDefaultState();
            controller.ResetParameters();
            ResetState(true);

            if ((_overrideParameters.typeOfSwim == TypeSwim.Gravity && _isJumping) || (_overrideParameters.typeOfSwim == TypeSwim.NullGravity && _inputSwim.y > 0))
            {
                controller.SetVerticalForce(_overrideParameters.jumpExternForce, false, true);
            }
            if (_characterDash && _overrideParameters.useDash)
            {
                _characterDash.ResetState();
            }

            _isJumping = false;
        }
        #endregion

        #region Dive
        private void SetDive()
        {
            if (!controller) return;

            _isDiving = true;
            float speed = Mathf.Abs(controller.Speed.y);

            _currentIntensity = MathfExtend.ChangeRange(speed, _overrideParameters.maxSpeedDive, _overrideParameters.maxVerticalForceDive);
            _currentIntensity = -Mathf.Min(_currentIntensity, _overrideParameters.maxVerticalForceDive);
        }

        private void Dive()
        {
            controller.SetVerticalForce(_currentIntensity, true);
            _currentIntensity = Mathf.Min(0, _currentIntensity + (_overrideParameters.lessForce * Time.deltaTime));
            if (_currentIntensity == 0)
            {
                _isDiving = false;
            }
        }
        #endregion

        #region Swim
        private void FlipAtDirection()
        {
            if (manager != null && _overrideParameters.flipAtChangeDirection && Mathf.Abs(_inputSwim.x) > _overrideParameters.speedThreshold)
            {
                SemiAxisX currentAxis = _inputSwim.x >= 0 ? SemiAxisX.Right : SemiAxisX.Left;
                manager.ChangeFacingDirection(currentAxis);
            }
        }

        private void Swim()
        {
            if (!controller) return;

            if (_overrideParameters.typeOfSwim == TypeSwim.NullGravity)
            {
                controller.SetForce(_inputSwim * _overrideParameters.speedSwim, true);
            }
            else
            {
                controller.SetHorizontalForce(_inputSwim.x * _overrideParameters.speedSwim, true);
            }


            if (_isDiving && _inputSwim.y != 0)
            {
                _isDiving = false;
            }
        }

        public void UpdateInputSwim(Vector2 input)
        {
            _inputSwim = input;
        }
        #endregion

        #region JumpWater
        public void JumpWater()
        {
            if (_inWater && _overrideParameters.typeOfSwim == TypeSwim.Gravity)
            {
                _isJumping = true;
                controller.ResetVerticalForce();
                controller.SetVerticalForce(_overrideParameters.jumpForce, false, true);

                if (_isDiving)
                {
                    _isDiving = false;
                }
            }
        }

        public void StopJumpWater()
        {
            if (_inWater && _overrideParameters.typeOfSwim != TypeSwim.Gravity)
            {
                _isJumping = false;
                controller.ResetVerticalForce();
            }
        }
        #endregion

        #region TimeLife
        public void RipristinateTime()
        {
            _timeCurrentInWater = _overrideParameters.timeFoSurvive;
        }

        private void CheckTimeRemain()
        {
            _timeCurrentInWater += Time.deltaTime;
            _overrideParameters.timeRemainPercent = Math.Max(1 - _timeCurrentInWater / _overrideParameters.timeFoSurvive, 0);
            if (_timeCurrentInWater >= _overrideParameters.timeFoSurvive)
            {
                manager.CallCharacterEvent("FinishOxigen");
            }
        }
        #endregion
    }
}

#endif
