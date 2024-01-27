#if PEARL_2DSIDE

using UnityEngine;
using System.Collections;
using Pearl.Input;
using Pearl;
using Pearl.Events;
using PlasticPipe.PlasticProtocol.Messages;
using NUnit.Framework.Internal;

namespace Pearl.Side2D
{
    /// <summary>
    /// Add this component to a Character and it'll be able to cling to walls when being in the air, 
    // facing a wall, and moving in its direction
    /// Animator parameters : WallClinging (bool)
    /// </summary>
    public class CharacterWallClinging : CharacterAbility
    {
        /// This method is only used to display a helpbox text at the beginning of the ability's inspector
        [Header("Wall Clinging")]

        /// the slow factor when wall clinging
        [Tooltip("the slow factor when wall clinging")]
        [Range(0f, 1)]
        public float WallClingingSlowFactor = 0.6f;
        /// the vertical offset to apply to raycasts for wall clinging
        [Tooltip("the vertical offset to apply to raycasts for wall clinging")]
        public float RaycastVerticalOffset = 0f;
        /// the tolerance applied to compensate for tiny irregularities in the wall (slightly misplaced tiles for example)
        [Tooltip("the tolerance applied to compensate for tiny irregularities in the wall (slightly misplaced tiles for example)")]
        public float WallClingingTolerance = 0.3f;
        [SerializeField]
        private float speedY = 0;

        [SerializeField]
        private bool useStamina = true;
        [SerializeField, ConditionalField("@useStamina")]
        private float timeMaxStamina = 2f;
        [SerializeField, ConditionalField("@useStamina")]
        private FloatEvent OnStamina;

        protected CharacterJump _characterJump;

        [Header("Automation")]

        /// if this is set to true, you won't need to press the opposite direction to wall cling, it'll be automatic anytime the character faces a wall
        [Tooltip("if this is set to true, you won't need to press the opposite direction to wall cling, it'll be automatic anytime the character faces a wall")]
        public bool InputIndependent = false;

        //protected CharacterStates.MovementStates _stateLastFrame;
        protected RaycastHit2D _raycast;
        protected WallClingingOverride _wallClingingOverride;
        protected CharacterWallJump _characterWallJump;
        private float _auxStamina = 0;
        private bool _bornout = false;
        private bool _moveUp = false;
        private bool _isColumn = false;
        private bool _isRightWall = false;
        private GameObject _collider = null;


        public float HorizontalMovement { get; set; }
        public float VerticalMovement { get; set; }

        protected override void Start()
        {
            _characterWallJump = manager.FindAbility<CharacterWallJump>();
            _characterJump = manager.FindAbility<CharacterJump>();
        }

        /// <summary>
        /// Every frame, checks if the wallclinging state should be exited
        /// </summary>
        public override void UpdateAbility()
        {
            RecoveryStamina();

            _moveUp = VerticalMovement > 0 && HorizontalMovement == 0;
            bool isWallClinging = false;

            if (!_bornout)
            {
                isWallClinging = WallClinging();
            }

            if (isWallClinging && VerticalMovement != 0 && speedY != 0)
            {
                controller.SetVerticalForce(VerticalMovement * speedY, true, false, true);
            }

            ExitWallClinging();
            WallClingingLastFrame();
        }

        public bool CheckFlipColumn(bool isRight)
        {
            if ((_isRightWall && isRight) || (!_isRightWall && !isRight))
            {
                return FlipColumn();
            }
            else if (_isColumn)
            {
                if (manager)
                {
                    manager.Flip();
                }
                ProcessExit();
            }

            return false;
        }

        public bool FlipColumn()
        {
            if (_isColumn && manager.CurrentState == statePlayer)
            {
                float distanceX = _collider.transform.position.x - controller.transform.position.x;

                if (manager && controller)
                {
                    controller.transform.Translate(2 * distanceX * Vector3.right);
                    manager.Flip();
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Makes the player stick to a wall when jumping
        /// </summary>
        public virtual bool WallClinging()
        {
            if ((manager.CurrentCondition != CharacterManager.defaultCondition)
                || (controller.State.IsGrounded)
                || (controller.State.ColliderResized))
            {
                return false;
            }

            if (InputIndependent || 
               (HorizontalMovement != 0 || manager.CurrentState == statePlayer))
            {
                return TestForWall() && EnterWallClinging();
            }
            return false;
        }

        private void RecoveryStamina()
        {
            if (useStamina)
            {
                OnStamina?.Invoke(1 - (_auxStamina / timeMaxStamina));

                if (controller.State.IsGrounded)
                {
                    _auxStamina = 0;
                    _bornout = false;
                }
            }
        }

        /// <summary>
        /// Casts a ray to check if we're facing a wall
        /// </summary>
        /// <returns></returns>
        protected virtual bool TestForWall()
        {
            // we then cast a ray to the direction's the character is facing, in a down diagonal.
            // we could use the controller's IsCollidingLeft/Right for that, but this technique 
            // compensates for walls that have small holes or are not perfectly flat

            //characterTransform
            Vector3 raycastOrigin = transform.position;
            Vector3 raycastDirection;
            if (manager.CurrentFacing == SemiAxisX.Right)
            {
                raycastOrigin = raycastOrigin + transform.right * controller.Width() / 2 + transform.up * RaycastVerticalOffset;
                raycastDirection = transform.right - transform.up;
                _isRightWall = true;
            }
            else
            {
                raycastOrigin = raycastOrigin - transform.right * controller.Width() / 2 + transform.up * RaycastVerticalOffset;
                raycastDirection = -transform.right - transform.up;
                _isRightWall = false;
            }

            // we cast our ray 
            _raycast = Physics2DExtend.RayCast(raycastOrigin, raycastDirection, WallClingingTolerance, controller.PlatformMask & ~(controller.OneWayPlatformMask | controller.MovingOneWayPlatformMask), controller.Parameters.DrawRaycastsGizmos, Color.black);

            // we check if the ray hit anything. If it didn't, or if we're not moving in the direction of the wall, we exit
            return _raycast;
        }

        /// <summary>
        /// Enters the wall clinging state
        /// </summary>
        protected virtual bool EnterWallClinging()
        {
            // we check for an override
            if (controller.CurrentWallCollider != null)
            {
                _wallClingingOverride = controller.CurrentWallCollider.GetComponent<WallClingingOverride>();
                _collider = controller.CurrentWallCollider;
            }
            else if (_raycast.collider != null)
            {
                _wallClingingOverride = _raycast.collider.GetComponent<WallClingingOverride>();
                _collider = _raycast.collider.gameObject;
            }


            if (_wallClingingOverride != null)
            {
                // if we can't wallcling to this wall, we do nothing and exit
                if (!_wallClingingOverride.CanWallClingToThis)
                {
                    return false;
                }

                var factor = _wallClingingOverride.slowFactorEqualPG ? WallClingingSlowFactor : _wallClingingOverride.WallClingingSlowFactor;
                controller.SlowFall(factor, true);
                _isColumn = _wallClingingOverride.IsColumn;
            }
            else
            {
                _isColumn = false;
                // we slow the controller's fall speed
                controller.SlowFall(WallClingingSlowFactor, true);
            }

            if (manager.CurrentState != statePlayer)
            {
                controller.SetVerticalForce(0);
            }

            controller.SetHorizontalForce(0);
            manager.NewState(statePlayer);
            return true;
        }

        /// <summary>
        /// If the character is currently wallclinging, checks if we should exit the state
        /// </summary>
        protected virtual void ExitWallClinging()
        {
            if (manager.CurrentState == statePlayer)
            {
                // we prepare a boolean to store our exit condition value
                bool shouldExit = false;

                if (useStamina)
                {
                    _auxStamina += Time.deltaTime;
                    if (_auxStamina >= timeMaxStamina)
                    {
                        // then we should exit
                        _auxStamina = timeMaxStamina;
                        shouldExit = true;
                        _bornout = true;
                    }
                }

                if (!_isColumn && ( (HorizontalMovement > 0 && manager.CurrentFacing == SemiAxisX.Left) || (HorizontalMovement < 0 && manager.CurrentFacing == SemiAxisX.Right)) )
                {
                    manager.Flip();
                    shouldExit = true;
                }


                if ((controller.State.IsGrounded)) // if the character is grounded
                {
                    // then we should exit
                    shouldExit = true;
                }

                // we then cast a ray to the direction's the character is facing, in a down diagonal.
                // we could use the controller's IsCollidingLeft/Right for that, but this technique 
                // compensates for walls that have small holes or are not perfectly flat
                Vector3 raycastOrigin = transform.position;
                Vector3 raycastDirection;
                if (manager.CurrentFacing == SemiAxisX.Right)
                {
                    raycastOrigin = raycastOrigin + transform.right * controller.Width() / 2 + transform.up * RaycastVerticalOffset;
                    raycastDirection = transform.right - transform.up;
                }
                else
                {
                    raycastOrigin = raycastOrigin - transform.right * controller.Width() / 2 + transform.up * RaycastVerticalOffset;
                    raycastDirection = -transform.right - transform.up;
                }

                // we check if the ray hit anything. If it didn't, or if we're not moving in the direction of the wall, we exit
                if (!InputIndependent)
                {
                    // we cast our ray 
                    RaycastHit2D hit = Physics2DExtend.RayCast(raycastOrigin, raycastDirection, WallClingingTolerance, controller.PlatformMask & ~(controller.OneWayPlatformMask | controller.MovingOneWayPlatformMask), controller.Parameters.DrawRaycastsGizmos, Color.black);
                    if (manager.CurrentFacing == SemiAxisX.Right)
                    {
                        if ((!hit) || (HorizontalMovement <= -0.1 && !_isColumn))
                        {
                            shouldExit = true;
                        }
                    }
                    else
                    {
                        if ((!hit) || (HorizontalMovement >= 0.1 && !_isColumn))
                        {
                            shouldExit = true;
                        }
                    }
                }
                else
                {
                    if (_raycast.collider == null)
                    {
                        shouldExit = true;
                    }
                }

                if (shouldExit)
                {
                    ProcessExit();
                }
            }
        }

        protected virtual void ProcessExit()
        {
            _isColumn = false;

            if (_moveUp && !_bornout)
            {
                _characterJump.Jump();
            }
            else
            {
                // if we're not wallclinging anymore, we reset the slowFall factor, and reset our state.
                controller.SlowFall(0f, false);
                // we reset the state
                manager.ResetState(true);
            }
        }

        /// <summary>
        /// This methods tests if we were wallcling previously, and if so, resets the slowfall factor and stops the wallclinging sound
        /// </summary>
        protected virtual void WallClingingLastFrame()
        {
            if ( (manager.PreviousControllerState == statePlayer)
                && (manager.CurrentState != statePlayer))
            {
                controller.SlowFall(0f, false);
            }
        }

        /// <summary>
        /// On reset ability, we cancel all the changes made
        /// </summary>
        protected override void ResetAbility()
        {
            base.ResetAbility();
        }
    }
}

#endif