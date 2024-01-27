#if PEARL_2DSIDE

using Pearl;
using System;
using UnityEngine;

namespace Pearl.Side2D
{
    [Serializable]
    public class MovementHorizontalParameters
    {
        [Header("Movement")]

        [SerializeField, Tooltip("if this is true, the character will automatically flip to face its movement direction")]
        public bool flipAtChangeDirection = true;
        [SerializeField, ConditionalField("@flipAtChangeDirection"),
            Tooltip("the threshold after which input is considered (usually 0.1f to eliminate small joystick noise)")]
        public float speedThreshold = 0.1f;
        [SerializeField, Tooltip("the speed of the character when it's walking")]
        public float speed = 1f;
        [SerializeField]
        public bool useRaw = false;
        [SerializeField]
        public bool useSlowMovement = false;
        [SerializeField, ConditionalField("@useSlowMovement")]
        public float valueForSlow = 0.2f;

        [Header("Air Movement")]

        [Range(0f, 1f), Tooltip("how much air control the player has")]
        public float AirControl = 1f;
    }

    public class CharacterMovementHorizontal : CharacterAbilityWithOverride<MovementHorizontalParameters>
    {
        private float _horizontalMovement;
        protected float _airControlDirection;

        public bool UseSlowMovement { get { return _overrideParameters.useSlowMovement; } }

        public float ValueForSlow { get { return _overrideParameters.valueForSlow; } }

        public float AirControlDirection { set { _airControlDirection = value; } }

        public float HorizontalMovement { set { _horizontalMovement = value; } }

        #region Ability
        public override void UpdateAbility()
        {
            if (CheckStateCondition())
            {
                MoveInAir();
                FlipAtDirection();
                AddForce();
            }
        }

        protected override void ResetAbility()
        {
            if (controller)
            {
                controller.SetHorizontalForce(0);
            }
        }
        #endregion

        public void SetAirControlDirection(float value)
        {
            _airControlDirection = value;
        }

        private void MoveInAir()
        {
            float inputRaw = Mathf.Sign(_horizontalMovement);

            if (_overrideParameters.AirControl < 1f && !controller.State.IsGrounded && _airControlDirection != inputRaw)
            {
                _horizontalMovement = Mathf.Lerp(0, _horizontalMovement, _overrideParameters.AirControl);
            }

            if (controller.State.IsGrounded)
            {
                SetAirControlDirection(inputRaw);
            }
        }

        private void FlipAtDirection()
        {
            if (manager != null && _overrideParameters.flipAtChangeDirection && Mathf.Abs(_horizontalMovement) > _overrideParameters.speedThreshold)
            {
                SemiAxisX currentAxis = _horizontalMovement >= 0 ? SemiAxisX.Right : SemiAxisX.Left;
                manager.ChangeFacingDirection(currentAxis);
            }
        }

        private void AddForce()
        {
            if (controller != null && manager != null)
            {
                float horizontalMovementForce = _horizontalMovement * _overrideParameters.speed;

                horizontalMovementForce = HandleFriction(horizontalMovementForce);

                if (horizontalMovementForce == 0)
                {
                    ResetState();
                }
                else
                {
                    NewControllerState(statePlayer);
                }

                controller.SetHorizontalForce(horizontalMovementForce, false, false);
            }
        }

        protected virtual float HandleFriction(float force)
        {
            if (!controller) return force;

            float friction = controller.Friction;

            // if we have a friction above 1 (mud, water, stuff like that), we divide our speed by that friction
            if (friction > 1)
            {
                force /= friction;
            }

            // if we have a low friction (ice, marbles...) we lerp the speed accordingly
            if (friction < 1 && friction > 0)
            {
                force = Mathf.Lerp(controller.Speed.x, force, Time.deltaTime * friction * 10);
            }

            return force;
        }
    }
}

#endif
