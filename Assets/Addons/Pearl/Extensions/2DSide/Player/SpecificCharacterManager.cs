#if PEARL_2DSIDE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

namespace Pearl.Side2D
{
    public class SpecificCharacterManager : CharacterManager
    {
        protected override void CheckState(string specificState)
        {
            if (specificState == "Dash")
            {
                SetPermission<CharacterMovementHorizontal>(false);
                SetPermission<CharacterSwim>(false);
            }
            else if (CheckExistState(specificState, "Dash", "Swimming"))
            {
                SetPermission<CharacterMovementHorizontal>(true);
                SetPermission<CharacterSwim>(true);
            }

            if (specificState == "WallClinging")
            {
                SetPermission<CharacterMovementHorizontal>(false);
            }
            else if (CheckExistState(specificState, "WallClinging"))
            {
                SetPermission<CharacterMovementHorizontal>(true);
            }

            if (specificState == "Swimming")
            {
                SetPermission<CharacterMovementHorizontal>(false);
            }
            else if (CheckExistState(specificState, "Swimming"))
            {
                SetPermission<CharacterMovementHorizontal>(true);
            }
        }

        public override bool CheckStateCondition(string specificState)
        {
            if (specificState == "Jumping")
            {
                return CurrentState != "WallClinging" && CurrentState != "WallJumping";
            }

            if (specificState == "Walking")
            {
                return CurrentState != "WallClinging" && CurrentState != "WallJumping";
            }
            return true;
        }

        protected override void CheckAllState()
        {
            CheckState(_currentState);

            if (_currentState == "Jumping" || _currentState == "Falling" || _currentState == "WallClinging")
            {
                if (characterController.State.JustGotGrounded)
                {
                    NewState(defaultState, true);
                }
            }
        }

        protected override bool IsThereConflitState(string newState)
        {
            if (newState == "Falling" || newState == "Idle" || newState == "Horizontal")
            {
                if (_currentState == "Swimming")
                {
                    return true;
                }
            }

            if (newState == "Walking")
            {
                if (_currentState == "Jumping" || _currentState == "Falling" || _currentState == "WallClinging")
                {
                    return true;
                }
            }
            else if (newState == "Idle")
            {
                if (_currentState == "Jumping" || _currentState == "Falling" || _currentState == "WallClinging")
                {
                    return true;
                }
            }

            return false;
        }
    }
}

#endif
