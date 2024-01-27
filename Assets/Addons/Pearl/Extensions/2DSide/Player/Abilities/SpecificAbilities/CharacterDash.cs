#if PEARL_2DSIDE

using Pearl;
using System;
using System.Collections;
using UnityEngine;
using static Pearl.Side2D.CharacterDash;

namespace Pearl.Side2D
{
    [Serializable]
    public class DashParameters
    {
        [Header("Dash")]
        /// the duration of dash (in seconds)
        [Tooltip("the duration of dash (in seconds)"), SerializeField]
        public float dashDistance = 3f;

        /// the force of the dash
        [Tooltip("the force of the dash"), SerializeField]
        public float dashForce = 40f;

        [Tooltip("the duration of dash (in seconds)"), SerializeField]
        public bool interruptDash = true;

        /// if this is true, forces will be reset on dash exit (killing inertia)
        [Tooltip("if this is true, forces will be reset on dash exit (killing inertia)"), SerializeField]
        public bool resetForcesOnExit = true;

        [Header("Direction")]

        [SerializeField]
        public bool useDirectionCharacter = false;

        /// the dash's aim properties
        //[Tooltip("the dash's aim properties")]
        //[SerializeField]
        //[ConditionalField("@!useDirectionCharacter")]
        //private MMAim aim;

        [SerializeField]
        public bool limit2Direction = false;

        /// the minimum amount of input required to apply a direction to the dash
        [Tooltip("the minimum amount of input required to apply a direction to the dash"), SerializeField]
        public float minimumInputThreshold = 0.1f;

        [SerializeField]
        public float timeForSospensionAfterDash = 0.05f;

        /// if this is true, the character will flip when dashing and facing the dash's opposite direction
        [Tooltip("if this is true, the character will flip when dashing and facing the dash's opposite direction"), SerializeField]
        public bool flipCharacterIfNeeded = true;

        /// if this is true, will prevent the character from dashing into the ground when already grounded
        [Tooltip("if this is true, will prevent the character from dashing into the ground when already grounded"), SerializeField]
        public bool autoCorrectTrajectory = true;

        [Header("Cooldown")]
        /// the duration of the cooldown between 2 dashes (in seconds)
        [Tooltip("the duration of the cooldown between 2 dashes (in seconds)")]
        [SerializeField]
        public float dashCooldown = 0.5f;

        /// the method used to reset the number of dashes left, only if dashes are not infinite
        [Tooltip("the method used to reset the number of dashes left, only if dashes are not infinite"), SerializeField]
        public SuccessiveDashResetMethods successiveDashResetMethod = SuccessiveDashResetMethods.Grounded;

        /// when in time reset mode, the duration, in seconds, after which the amount of dashes left gets reset, only if dashes are not infinite
        [Tooltip("when in time reset mode, the duration, in seconds, after which the amount of dashes left gets reset, only if dashes are not infinite"),
            ConditionalField("@successiveDashResetMethod == Time"), SerializeField]
        public float timerForResetNumberDash = 2f;

        [Header("Uses")]
        /// the amount of successive dashes a character can perform, only if dashes are not infinite
        [Tooltip("the amount of successive dashes a character can perform, only if dashes are not infinite"), SerializeField]
        public int numberDash = 1;


    }

    public class CharacterDash : CharacterAbilityWithOverride<DashParameters>
    {
        public enum SuccessiveDashResetMethods { Grounded, Time, ScriptExtern }

        protected float _cooldownTimeStamp = 0;
        protected float _startTime;
        protected Vector3 _initialPosition;
        protected Vector2 _dashDirection;
        protected float _distanceTraveled = 0f;
        protected bool _shouldKeepDashing = true;
        protected float _slopeAngleSave = 0f;
        protected bool _dashEndedNaturally = true;
        protected IEnumerator _dashCoroutine;
        protected float _lastDashAt = 0f;
        private bool _isDashing = false;
        private int _currentNumberDash = 1;

        public bool IsDashing { get { return _isDashing; } }
        public int SuccessiveDashAmount { set { _overrideParameters.numberDash = value; } }

        protected override void Awake()
        {
            base.Awake();

            _overrideParameters = DefaultParameters;
        }

        protected override void Start()
        {
            base.Start();

            //aim.Initialization();
            _currentNumberDash = _overrideParameters.numberDash;
        }


        public override void UpdateAbility()
        {
            // If the character is dashing, we cancel the gravity
            if (_isDashing)
            {
                controller.GravityActive(false);
            }

            // we reset our slope tolerance if dash didn't end naturally
            if (!_dashEndedNaturally && !_isDashing)
            {
                _dashEndedNaturally = true;
                controller.Parameters.MaximumSlopeAngle = _slopeAngleSave;
            }

            HandleAmountOfDashesLeft();
        }

        public void StartDash(Vector2 vector)
        {
            if (!DashConditions())
            {
                return;
            }

            InitiateDash(vector);
        }

        /// <summary>
        /// initializes all parameters prior to a dash and triggers the pre dash feedbacks
        /// </summary>
        public virtual void InitiateDash(Vector2 vector)
        {
            _isDashing = true;

            // we initialize our various counters and checks
            _startTime = Time.time;
            _dashEndedNaturally = false;
            _initialPosition = this.transform.position;
            _distanceTraveled = 0;
            _shouldKeepDashing = true;
            _cooldownTimeStamp = Time.time + _overrideParameters.dashCooldown;
            _lastDashAt = Time.time;
            _currentNumberDash -= 1;

            NewControllerState(statePlayer);

            // we prevent our character from going through slopes
            _slopeAngleSave = controller.Parameters.MaximumSlopeAngle;
            controller.Parameters.MaximumSlopeAngle = 0;

            ComputeDashDirection(vector);
            CheckFlipCharacter();

            //if (!controller) return;
            //controller.ResetForce();

            // we launch the boost corountine with the right parameters
            _dashCoroutine = Dash();
            StartCoroutine(_dashCoroutine);
        }

        /// <summary>
        /// Coroutine used to move the player in a direction over time
        /// </summary>
        protected virtual IEnumerator Dash()
        {
            // we keep dashing until we've reached our target distance or until we get interrupted
            while (_distanceTraveled < _overrideParameters.dashDistance
                && _shouldKeepDashing
                && _isDashing)
            {
                _distanceTraveled = Vector3.Distance(_initialPosition, this.transform.position);


                // if we collide with something on our left or right (wall, slope), we stop dashing, otherwise we apply horizontal force
                if (   (controller.State.IsCollidingLeft && _dashDirection.x < 0f)
                    || (controller.State.IsCollidingRight && _dashDirection.x > 0f)
                    || (controller.State.IsCollidingAbove && _dashDirection.y > 0f)
                    || (controller.State.IsCollidingBelow && _dashDirection.y < 0f))
                {
                    _shouldKeepDashing = false;
                    StopDash();
                }
                else
                {
                    controller.SetForce(_dashDirection * _overrideParameters.dashForce);
                }
                yield return null;
            }

            StopDash();
        }

        public virtual void InterruptDash()
        {
            if (_overrideParameters.interruptDash)
            {
                StopDash();
            }
        }

        /// <summary>
        /// Stops the dash coroutine and resets all necessary parts of the character
        /// </summary>
        public virtual void StopDash()
        {
            // once the boost is complete, if we were dashing, we make it stop and start the dash cooldown
            if (_isDashing)
            {
                if (_dashCoroutine != null)
                {
                    StopCoroutine(_dashCoroutine);
                }

                // once our dash is complete, we reset our various states
                controller.DefaultParameters.MaximumSlopeAngle = _slopeAngleSave;
                controller.Parameters.MaximumSlopeAngle = _slopeAngleSave;


                PearlInvoke.WaitForMethod(_overrideParameters.timeForSospensionAfterDash, WaitAMomntWithGravityZero);


                _dashEndedNaturally = true;

                // we reset our forces
                if (_overrideParameters.resetForcesOnExit)
                {
                    controller.ResetForce();
                }


                _isDashing = false;
            }
        }

        private void WaitAMomntWithGravityZero()
        {
            controller.GravityActive(true);
            ResetState();
        }


        /// <summary>
        /// Checks whether or not a character flip is required, and flips the character if needed
        /// </summary>
        protected virtual void CheckFlipCharacter()
        {
            // we flip the character if needed
            if (_overrideParameters.flipCharacterIfNeeded && (Mathf.Abs(_dashDirection.x) > 0.05f))
            {
                if (manager.CurrentFacing == SemiAxisX.Right != (_dashDirection.x > 0f))
                {
                    manager.Flip();
                }
            }
        }

        /// <summary>
        /// Computes the dash direction based on the selected options
        /// </summary>
        protected virtual void ComputeDashDirection(Vector2 vector)
        {
            if (_overrideParameters.limit2Direction)
            {
                vector.y = 0;
            }

            Vector2 facing = manager.CurrentFacingVector;

            if (_overrideParameters.useDirectionCharacter)
            {
                _dashDirection = new Vector2(vector.x == 0 && !_overrideParameters.limit2Direction ? 0 : facing.x, vector.y).normalized;
            }
            else
            {
                //aim.PrimaryMovement = vector;

                if (controller)
                {
                    //aim.CurrentPosition = controller.transform.position;
                }

                //_dashDirection = aim.GetCurrentAim();
            }

            CheckAutoCorrectTrajectory();

            if (_dashDirection.magnitude < _overrideParameters.minimumInputThreshold)
            {
                _dashDirection = facing;
            }
            else
            {
                _dashDirection = _dashDirection.normalized;
            }
        }

        /// <summary>
        /// Prevents the character from dashing into the ground when already grounded and if AutoCorrectTrajectory is checked
        /// </summary>
        protected virtual void CheckAutoCorrectTrajectory()
        {
            if (_overrideParameters.autoCorrectTrajectory && controller.State.IsCollidingBelow && (_dashDirection.y < 0f))
            {
                _dashDirection.y = 0f;
            }
        }


        public virtual bool DashConditions()
        {
            // if we're in cooldown between two dashes, we prevent dash
            if (_cooldownTimeStamp > Time.time)
            {
                return false;
            }

            // if we don't have dashes left, we prevent dash
            if (_currentNumberDash <= 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if conditions are met to reset the amount of dashes left
        /// </summary>
        protected virtual void HandleAmountOfDashesLeft()
        {
            if ((_currentNumberDash >= _overrideParameters.numberDash) || (Time.time - _lastDashAt < _overrideParameters.dashCooldown))
            {
                return;
            }

            switch (_overrideParameters.successiveDashResetMethod)
            {
                case SuccessiveDashResetMethods.Time:
                    if (Time.time - _lastDashAt > _overrideParameters.timerForResetNumberDash)
                    {
                        ResetDashesNumber();
                    }
                    break;
                case SuccessiveDashResetMethods.Grounded:
                    if (controller.State.IsGrounded)
                    {
                        ResetDashesNumber();
                    }
                    break;
            }
        }

        /// <summary>
        /// A method to reset the amount of successive dashes left
        /// </summary>
        /// <param name="newAmount"></param>
        public virtual void SetNumberDash(int newAmount)
        {
            _currentNumberDash = newAmount;
        }

        public virtual void ResetDashesNumber()
        {
            SetNumberDash(_overrideParameters.numberDash);
        }

        /*
        public override void ChangeAbilityPermission(bool permission)
        {
            if (!permission)
            {
                StopDash();
                PearlInvoke.StopTimer(WaitAMomntWithGravityZero);
                RemoveControllerState(CharacterControllerState.Dashing);
            }
        }
        */
    }
}

#endif