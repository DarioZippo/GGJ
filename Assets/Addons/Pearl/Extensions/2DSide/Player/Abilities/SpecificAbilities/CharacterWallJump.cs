#if PEARL_2DSIDE

using UnityEngine;
using System.Collections;
using Pearl.Input;
using Pearl.Side2D;
using UnityEngine.TextCore.Text;
using UnityEngine.Windows;
using Pearl.NodeCanvas.Tasks;

namespace Pearl.Side2D
{
    public class CharacterWallJump : CharacterAbility
    {
        /// the possible ways to apply force when jumping
        public enum ForceModes { AddForce, SetForce }

        /// returns true if a walljump happened this frame
        public bool WallJumpHappenedThisFrame { get; set; }

        [Header("Walljump")]
        /// if this is true, the character will be forced to flip towards the jump direction on the jump frame 
        [Tooltip("if this is true, the character will be forced to flip towards the jump direction on the jump frame")]
        public bool ForceFlipTowardsDirection = false;
        public float forceX = 10f;
        public float factorForJumpReduce = 0.5f;

        [Header("Limit")]
        /// if this is true, walljumps count as regular (non wall) jump to decrease the number of jumps left
        [Tooltip("if this is true, walljumps count as regular (non wall) jump to decrease the number of jumps left")]
        public bool ShouldReduceNumberOfJumpsLeft = true;
        /// if this is true, number of consecutive walljumps will be limited to MaximumNumberOfWalljumps 
        [Tooltip("if this is true, number of consecutive walljumps will be limited to MaximumNumberOfWalljumps")]
        public bool LimitNumberOfWalljumps = false;
        /// the maximum number of walljumps allowed
        [Tooltip("the maximum number of walljumps allowed")]
        [ConditionalField("@LimitNumberOfWalljumps")]
        public int MaximumNumberOfWalljumps = 3;
        /// the amount of walljumps left at this time 
        [Tooltip("the amount of walljumps left at this time")]
        [ConditionalField("@LimitNumberOfWalljumps")]
        [ReadOnly]
        public int NumberOfWalljumpsLeft;

        /// a delegate you can listen to to do something when a walljump happens
        public delegate void OnWallJumpDelegate();
        public OnWallJumpDelegate OnWallJump;

        protected CharacterJump _characterJump;
        protected CharacterMovementHorizontal _characterHorizontalMovement;
        // animation parameters
        protected const string _wallJumpingAnimationParameterName = "WallJumping";
        protected int _wallJumpingAnimationParameter;

        private float _wallJumpDirection;

        public float WallJumpDirection
        {
            get { return _wallJumpDirection; }
        }

        /// <summary>
        /// On start, we store our characterJump component
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            ResetNumberOfWalljumpsLeft();
        }

        protected override void Start()
        {
            _characterHorizontalMovement = manager.FindAbility<CharacterMovementHorizontal>();
            _characterJump = manager.FindAbility<CharacterJump>();
        }

        /// <summary>
        /// Resets the amount of walljumps left
        /// </summary>
        public virtual void ResetNumberOfWalljumpsLeft()
        {
            NumberOfWalljumpsLeft = MaximumNumberOfWalljumps;
        }

        /// <summary>
        /// Performs a walljump if the conditions are met when push button
        /// </summary>
        public virtual void Walljump()
        {
            WallJumpHappenedThisFrame = false;

            if (manager.CurrentCondition != CharacterManager.defaultCondition)
            {
                return;
            }

            if (LimitNumberOfWalljumps && NumberOfWalljumpsLeft <= 0)
            {
                return;
            }


            // if we're here the jump button has been pressed. If we were wallclinging, we walljump
            if (manager.CurrentState == "WallClinging")
            {
                manager.NewState(statePlayer);

                // we decrease the number of jumps left
                if (_characterJump != null && ShouldReduceNumberOfJumpsLeft)
                {
                    _characterJump.LessNumberOfJumpsLeft();
                }

                manager.ResetCondition();
                controller.GravityActive(true);
                controller.SlowFall(0f, false);

                // If the character is colliding to the right with something (probably the wall)
                _wallJumpDirection = manager.CurrentFacing == SemiAxisX.Right ? -1f : 1f;

                if (_characterHorizontalMovement != null)
                {
                    _characterHorizontalMovement.SetAirControlDirection(_wallJumpDirection);
                }


                float jump = _characterJump.JumpForce * (1 - factorForJumpReduce);
                if (_characterJump.UseHeigh)
                {
                    controller.SetVerticalForce(jump, false, true);
                }
                else
                {
                    controller.SetVerticalForce(jump);
                }


                controller.SetHorizontalForce(_wallJumpDirection * forceX);

                if (ForceFlipTowardsDirection)
                {
                    var axisX = _wallJumpDirection > 0 ? SemiAxisX.Right : SemiAxisX.Left;
                    manager.ChangeFacingDirection(axisX);
                }

                if (LimitNumberOfWalljumps)
                {
                    NumberOfWalljumpsLeft--;
                }

                //PlayAbilityStartFeedbacks();
                WallJumpHappenedThisFrame = true;
                OnWallJump?.Invoke();

                return;
            }
        }

        /// <summary>
        /// On ProcessAbility, we reset our number of wall jumps if needed
        /// </summary>
        public override void UpdateAbility()
        {
            if (controller.State.IsGrounded)
            {
                ResetNumberOfWalljumpsLeft();
            }
        }
    }
}

#endif