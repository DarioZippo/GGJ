#if PEARL_2DSIDE

using Pearl;
using UnityEngine;

namespace Pearl.Side2D
{
    [RequireComponent(typeof(CharacterSwim))]
    public class ControlSwim : ControlAbility<CharacterSwim>
    {
        [SerializeField]
        private string nameInputJumpFromSurface = "Jump";
        [SerializeField]
        private string nameInputSwim = "Swim";

        [SerializeField]
        private bool useRaw = false;

        protected override void CreateInput()
        {
            RegistryAction(nameInputJumpFromSurface, OnPressButtonJump, OnReleaseButtonJump);
        }

        public override void UpdateInput()
        {
            if (ability)
            {
                Vector2 _input = GetVectorInput(nameInputSwim, useRaw);
                FixInputForChangeGravity(ref _input);
                ability.UpdateInputSwim(_input.normalized);
            }
        }

        private void OnPressButtonJump()
        {
            if (!ability) return;

            ability.JumpWater();
        }

        private void OnReleaseButtonJump()
        {
            if (!ability) return;

            ability.StopJumpWater();
        }

        private void OnPressDown()
        {
            if (!ability) return;
        }
    }
}

#endif
