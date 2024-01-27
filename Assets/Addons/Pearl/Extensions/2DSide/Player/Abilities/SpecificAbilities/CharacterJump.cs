#if PEARL_2DSIDE

using Pearl.Input;
using UnityEngine;
using Pearl;
using static Pearl.Side2D.CharacterJump;
using System;

namespace Pearl.Side2D
{
    [Serializable]
    public class JumpParameters
    {
        [Header("Jump")]

        [Tooltip("the maximum number of jumps allowed (0 : no jump, 1 : normal jump, 2 : double jump, etc...)"), SerializeField]
        public int numberOfJumps = 2;
        [SerializeField]
        public bool useHeigh = true;
        [Tooltip("defines how high the character can jump"), ConditionalField("@useHeigh"), SerializeField]
        public float jumpHeigh = 1f;
        [Tooltip("defines how power the character can jump"), ConditionalField("!@useHeigh"), SerializeField]
        public float jumpPower = 1f;
        [SerializeField]
        public ResetJump typeResetJump = ResetJump.Grounded;
        [SerializeField]
        public bool firstJumpMustBeInGround = true;


        [Tooltip("if true, the jump duration/height will be proportional to the duration of the button's press"), SerializeField]
        public bool jumpIsProportionalToThePressTime = true;
        [SerializeField, Tooltip("the amount by which we'll modify the current speed when the jump button gets released"), ConditionalField("@jumpIsProportionalToThePressTime")]
        public float jumpReleaseForceFactor = 2f;
        [SerializeField, Tooltip("the minimum time in the air allowed when jumping - this is used for pressure controlled jumps"), ConditionalField("@jumpIsProportionalToThePressTime")]
        public float jumpMinimumAirTime = 0.1f;

        [Header("Collisions")]

        [SerializeField, Tooltip("duration (in seconds) we need to disable collisions when jumping off a moving platform")]
        public float MovingPlatformsJumpCollisionOffDuration = 0.05f;
        [SerializeField, Tooltip("duration (in seconds) we need to disable collisions when jumping down a 1 way platform")]
        public float OneWayPlatformsJumpCollisionOffDuration = 0.3f;

        [Header("Quality of Life")]

        /// a timeframe during which, after leaving the ground, the character can still trigger a jump
        [Tooltip("a timeframe during which, after leaving the ground, the character can still trigger a jump"), ConditionalField("@firstJumpMustBeInGround")]
        public float coyoteTime = 0f;
        /// if the character lands, and the jump button's been pressed during that InputBufferDuration, a new jump will be triggered 
        [Tooltip("if the character lands, and the jump button's been pressed during that InputBufferDuration, a new jump will be triggered")]
        public float inputBufferDuration = 0f;
    }

    public class CharacterJump : CharacterAbilityWithOverride<JumpParameters>
    {
        public enum ResetJump { Grounded, Other }

        private int _numberOfJumpsLeft = 0;
        protected float _initTimeJump = 0;
        protected float _lastTimeGrounded = 0f;
        protected bool _inInputBuffer = false;
        protected float _initTimeInputBuffer = 0f;

        public int NumberOfJumps { set { _overrideParameters.numberOfJumps = value; } }
        public bool UseHeigh { get { return _overrideParameters.useHeigh; } }

        public float JumpForce 
        { 
            get 
            {
                return _overrideParameters.useHeigh ? _overrideParameters.jumpHeigh : _overrideParameters.jumpPower;
            } 
        }

        protected override void Start()
        {
            base.Start();

            RipristinateNumberJump();
        }

        public override void UpdateAbility()
        {
            RestoreNumberJump();

            // we store the last timestamp at which the character was grounded
            if (controller.State.IsGrounded)
            {
                _lastTimeGrounded = Time.time;
            }

            if (_inInputBuffer && EvaluateJumpConditions())
            {
                if (Time.time - _initTimeInputBuffer <= _overrideParameters.inputBufferDuration)
                {
                    _initTimeJump = Time.time;
                    JumpFromMovingPlatform();
                    Jump();
                }

                _inInputBuffer = false;
            }
        }

        /// <summary>
        /// Handles jumping from a moving platform.
        /// </summary>
        protected virtual void JumpFromMovingPlatform()
        {
            if (controller.State.IsGrounded)
            {
                if (controller.MovingPlatformMask.Contains(controller.StandingOn.layer)
                    || controller.MovingOneWayPlatformMask.Contains(controller.StandingOn.layer))
                {
                    // we turn the boxcollider off for a few milliseconds, so the character doesn't get stuck mid air
                    StartCoroutine(controller.DisableCollisionsWithMovingPlatforms(_overrideParameters.MovingPlatformsJumpCollisionOffDuration));
                    controller.DetachFromMovingPlatform();
                }
            }
        }

        public bool JumpDownFromOneWayPlatform()
        {
            bool isJumpDownFromOneWayPlatform = controller.State.IsGrounded &&
                (controller.OneWayPlatformMask.Contains(controller.StandingOn.layer)
                || controller.MovingOneWayPlatformMask.Contains(controller.StandingOn.layer)
                || controller.StairsMask.Contains(controller.StandingOn.layer));

            if (isJumpDownFromOneWayPlatform)
            {
                // we turn the boxcollider off for a few milliseconds, so the character doesn't get stuck mid platform
                StartCoroutine(controller.DisableCollisionsWithOneWayPlatforms(_overrideParameters.OneWayPlatformsJumpCollisionOffDuration));
                controller.DetachFromMovingPlatform();
            }

            return isJumpDownFromOneWayPlatform;
        }

        public void StopJump()
        {
            if (_overrideParameters.jumpIsProportionalToThePressTime && _initTimeJump != 0 && _overrideParameters.jumpReleaseForceFactor != 0
                && Time.time - _initTimeJump >= _overrideParameters.jumpMinimumAirTime && CheckStateCondition()
                && controller.Speed.y > Mathf.Sqrt(Mathf.Abs(controller.Parameters.Gravity)))
            {
                _initTimeJump = 0;

                if (_overrideParameters.useHeigh)
                {
                    controller.SetVerticalForce(-_overrideParameters.jumpHeigh, false, true);
                }
                else
                {
                    controller.SetVerticalForce(-_overrideParameters.jumpPower);
                }
            }
        }

        public void RipristinateNumberJump()
        {
            _numberOfJumpsLeft = _overrideParameters.numberOfJumps;
        }

        public void InitializeJump()
        {
            if (EvaluateJumpConditions())
            {
                _inInputBuffer = false;
                _initTimeJump = Time.time;
                JumpFromMovingPlatform();
                Jump();
            }
            else
            {
                _inInputBuffer = true;
                _initTimeInputBuffer = Time.time;
            }
        }

        private bool EvaluateJumpConditions()
        {
            if (!manager.CheckStateCondition(statePlayer))
            {
                return false;
            }

            if (_numberOfJumpsLeft == _overrideParameters.numberOfJumps && _overrideParameters.firstJumpMustBeInGround && !controller.State.IsGrounded)
            {
                return Time.time - _lastTimeGrounded <= _overrideParameters.coyoteTime;
            }

            return _numberOfJumpsLeft >= 1;
        }

        public void LessNumberOfJumpsLeft()
        {
            _numberOfJumpsLeft--;
        }

        public void Jump()
        {
            if (controller)
            {
                controller.SetVerticalForce(JumpForce, false, true);
                _numberOfJumpsLeft = Mathf.Max(0, _numberOfJumpsLeft - 1);
                NewControllerState(statePlayer);
            }
        }

        private void RestoreNumberJump()
        {
            if (_overrideParameters.typeResetJump == ResetJump.Grounded && controller != null && controller.State.JustGotGrounded)
            {
                RipristinateNumberJump();
            }
        }
    }
}
#endif