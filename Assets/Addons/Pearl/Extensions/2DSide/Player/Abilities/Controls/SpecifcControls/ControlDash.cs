#if PEARL_2DSIDE

using Pearl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pearl.Side2D
{
    [RequireComponent(typeof(CharacterDash))]
    public class ControlDash : ControlAbility<CharacterDash>
    {
        [SerializeField]
        private string nameInputDash = "Dash";
        [SerializeField]
        private string nameVectorInputDirectionDash = "Move";
        
        protected override void CreateInput()
        {
            RegistryAction(nameInputDash, PressDash, ReleaseDash);
        }

        private void PressDash()
        {
            Vector2 vector = GetVectorInput(nameVectorInputDirectionDash, false);
            ability.StartDash(vector);
        }

        private void ReleaseDash()
        {
            ability.InterruptDash();
        }
    }
}

#endif