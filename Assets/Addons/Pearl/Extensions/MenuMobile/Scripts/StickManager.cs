using Pearl.Input;
using Pearl.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pearl.MenuMobile
{
    public class StickManager : MonoBehaviour
    {
        #region Inspector Fields
        [SerializeField]
        private string nameAction = "Movement";
        [SerializeField]
        private string nameMap = "";
        #endregion

        #region Privatee Fields
        private bool onDirection;
        private Vector2 centerPosition;
        #endregion

        #region Unity Callbacks
        private void Start()
        {
            if (TryGetComponent<RectTransform>(out var rectTransform)) 
            {
                centerPosition = rectTransform.RectTransformToScreenSpace().center;
            }
        }
        #endregion

        #region Public Methods
        // Update is called once per frame
        public void OnEnter()
        {
            onDirection = true;
        }

        public void OnExit()
        {
            onDirection = false;
            InputManager.RemoveVirtualVector(nameAction + nameMap);
        }
        #endregion

        #region Private Methods
        private void Update()
        {
            if (onDirection)
            {
                var aux = PointerExtend.GetScreenPosition();
                var direction = (aux - centerPosition).normalized;
                InputManager.AddVirtualVector(nameAction + nameMap, direction);
            }
        }
        #endregion
    }

}
