using Pearl.Input;
using UnityEngine;

namespace Pearl.Examples.EventExamples
{
    public class TestInput : MonoBehaviour
    {
        [InspectorButton("ActiveInput")]
        public bool activeInput;

        [InspectorButton("DisactiveInput")]
        public bool disactiveInput;

        public void ReadDown()
        {
            Pearl.Debugging.LogManager.Log("ReadDown");
        }

        public void ReadUp()
        {
            Pearl.Debugging.LogManager.Log("ReadUp");
        }

        public void ReadVector(Vector2 vector)
        {
            Pearl.Debugging.LogManager.Log(vector);
        }

        public void ActiveInput()
        {
            InputManager.ActivePlayer(true);
        }

        public void DisactiveInput()
        {
            InputManager.ActivePlayer(false);
        }

        public void PressEvent()
        {
            Pearl.Debugging.LogManager.Log("press event");
        }

        public void DoublePressEvent()
        {
            Pearl.Debugging.LogManager.Log("double press event");
        }

        public void DetachEvent()
        {
            Pearl.Debugging.LogManager.Log("detach event");
        }

        public void EnterEvent()
        {
            Pearl.Debugging.LogManager.Log("enter event");
        }

        public void ExitEvent()
        {
            Pearl.Debugging.LogManager.Log("exit event");
        }
    }
}
