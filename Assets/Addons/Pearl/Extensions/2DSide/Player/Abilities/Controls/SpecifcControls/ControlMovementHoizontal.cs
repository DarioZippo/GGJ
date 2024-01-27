#if PEARL_2DSIDE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pearl.Side2D
{
    [RequireComponent(typeof(CharacterMovementHorizontal))]
    public class ControlMovementHoizontal : ControlAbility<CharacterMovementHorizontal>
    {
        [SerializeField]
        private string nameMovementInput = "HorizontalMove";
        [SerializeField]
        private bool useRaw = false;
        [SerializeField]
        private bool isVector = false;
        [SerializeField]
        private string nameSlowMovementInputForKeyboard = "SlowMovement";

        private bool _isSlow = false;

        protected override void CreateInput()
        {
            if (ability && ability.UseSlowMovement)
            {
                RegistryAction(nameSlowMovementInputForKeyboard, OnPressSlowButton, OnReleaseSlowButton);
            }
        }

        public override void UpdateInput()
        {
            if (ability)
            {
                float aux;
                if (isVector)
                {
                    aux = GetVectorInput(nameMovementInput, useRaw).x;
                }
                else
                {
                    aux = GetFloatInput(nameMovementInput, useRaw);
                }
                FixInputXForChangeGravity(ref aux);
                ability.HorizontalMovement = _isSlow ? MathfExtend.Sign(aux) * ability.ValueForSlow : aux;
            }
        }

        private void OnPressSlowButton()
        {
            _isSlow = true;
        }

        private void OnReleaseSlowButton()
        {
            _isSlow = false;
        }
    }

}

#endif