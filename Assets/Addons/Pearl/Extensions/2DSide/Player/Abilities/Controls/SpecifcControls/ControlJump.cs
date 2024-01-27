#if PEARL_2DSIDE

using Pearl;
using UnityEngine;

namespace Pearl.Side2D
{
    [RequireComponent(typeof(CharacterJump))]
    public class ControlJump : ControlAbility<CharacterJump>
    {
        [SerializeField]
        private string nameJumpInput = "Jump";
        [SerializeField]
        private bool useJumpBottonForDown = false;
        [SerializeField, ConditionalField("@useJumpBottonForDown")]
        private string nameDownJumpInput = "Down";

        private bool _jumpButtonPressed = false;
        private bool _jumpButtonReleased = true;
        private bool _downButtonPressed = false;

        protected override void CreateInput()
        {
            RegistryAction(nameJumpInput, OnPressButtonJump, OnReleaseButtonJump);
            RegistryAction(nameDownJumpInput, OnPressDown, OnReleaseDown);
        }

        private void OnPressDown()
        {
            _downButtonPressed = true;

            if (!useJumpBottonForDown || _jumpButtonPressed)
            {
                ability.JumpDownFromOneWayPlatform();
            }
        }

        private void OnReleaseDown()
        {
            _downButtonPressed = false;
        }

        private void OnPressButtonJump()
        {
            ability.InitializeJump();
            _jumpButtonPressed = true;

            if (useJumpBottonForDown && (!_downButtonPressed || !ability.JumpDownFromOneWayPlatform()))
            {
                ability.InitializeJump();
                _jumpButtonPressed = true;
            }
        }

        private void OnReleaseButtonJump()
        {
            _jumpButtonPressed = false;
            _jumpButtonReleased = true;
        }

        public override void UpdateInput()
        {
            if (_jumpButtonReleased && !_jumpButtonPressed)
            {
                  ability.StopJump();
            }

            _jumpButtonReleased = false;
        }
    }
}

#endif
