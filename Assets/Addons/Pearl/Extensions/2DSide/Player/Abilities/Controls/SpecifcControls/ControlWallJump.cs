#if PEARL_2DSIDE

using Pearl;
using UnityEngine;

namespace Pearl.Side2D
{
    [RequireComponent(typeof(CharacterWallJump))]
    public class ControlWallJump : ControlAbility<CharacterWallJump>
    {
        [SerializeField]
        private string nameJumpInput = "Jump";

        protected override void CreateInput()
        {
            RegistryAction(nameJumpInput, OnPressButtonJump, StateButton.Down);
        }

        private void OnPressButtonJump()
        {
            if (ability != null)
            {
                ability.Walljump();
            }
        }
    }
}

#endif
