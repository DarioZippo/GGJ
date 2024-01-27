#if PEARL_2DSIDE

using Pearl;
using UnityEngine;
using UnityEngine.Windows;

namespace Pearl.Side2D
{
    [RequireComponent(typeof(CharacterWallClinging))]
    public class ControllWallClinging : ControlAbility<CharacterWallClinging>
    {
        [SerializeField]
        private bool isVector = false;
        [SerializeField, ConditionalField("!@isVector")]
        private string nameMovementXInput = "HorizontalMove";
        [SerializeField, ConditionalField("!@isVector")]
        private string nameMovementYInput = "VerticalMove";
        [SerializeField, ConditionalField("@isVector")]
        private string nameMovementInput = "Movement";

        [SerializeField]
        private bool useButtonForColumn = false;
        [SerializeField, ConditionalField("@useButtonForColumn")]
        private string nameButtonForFlipColumn = "Use";

        private Vector2 pastValue = Vector2.zero;

        protected override void CreateInput()
        {
            if (useButtonForColumn)
            {
                RegistryAction(nameButtonForFlipColumn, OnPressUse, StateButton.Down);
            }
        }

        public override void UpdateInput()
        {
            Vector2 movement = isVector ? GetVectorInput(nameMovementInput) : Vector2.zero;

            float aux = !isVector ? GetFloatInput(nameMovementXInput, StateButton.Down) : movement.x;


            if (!useButtonForColumn && aux != 0 && pastValue.x == 0)
            {
                ability.CheckFlipColumn(aux > 0);
            }

            ability.HorizontalMovement = aux;

            aux = !isVector ? GetFloatInput(nameMovementYInput) : movement.y;
            ability.VerticalMovement = aux;

            pastValue = movement;
        }

        private void OnPressUse()
        {
            Debug.Log("e");
            ability.FlipColumn();
        }
    }
}

#endif