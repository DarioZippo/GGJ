using Pearl.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pearl.MenuMobile
{
    public class InputFakeManager : MonoBehaviour
    {
        #region Inspector Fields
        [SerializeField]
        private string nameAction = "Movement";
        [SerializeField]
        private string nameMap = "";
        [SerializeField]
        private bool simulateAxis = false;
        [SerializeField, ConditionalField("@simulateAxis")]
        private bool isVector = false;
        [SerializeField, ConditionalField("@simulateAxis && !@isVector")]
        private float valueSimulated = default;
        [SerializeField, ConditionalField("@simulateAxis && @isVector")]
        private Vector2 vectorSimulated = default;
        #endregion

        #region Public Methods
        public void OnPointerDown()
        {
            if (!simulateAxis)
            {
                InputManager.InvokeVirtualAction(nameAction, nameMap, StateButton.Down);
            }
            else
            {
                if (isVector)
                {
                    InputManager.AddVirtualVector(nameAction + nameMap, vectorSimulated);
                }
                else
                {
                    InputManager.AddVirtualAxis(nameAction + nameMap, valueSimulated);
                }
            }
        }

        public void OnPointerUp()
        {
            if (!simulateAxis)
            {
                InputManager.InvokeVirtualAction(nameAction, nameMap, StateButton.Up);
            }
            else
            {
                if (isVector)
                {
                    InputManager.RemoveVirtualVector(nameAction + nameMap);
                }
                else
                {
                    InputManager.RemoveVirtualAxis(nameAction + nameMap);
                }
            }
        }
        #endregion
    }
}
