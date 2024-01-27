using Pearl.ClockManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Pearl.MenuMobile
{
    public enum TutorialButtonMenu { stick, menu, downRight, downRightUp, downRightLeft, upRight, upRightDown, upRightLeft, upLeft, upLeftDown, upLeftRight };

    public class MenuMobileTutorialManager : MonoBehaviour
    {
        #region Inspeector Fields
        [Header("General setting")]

        [SerializeField]
        private Color normalColor = Color.gray;
        [SerializeField]
        private Color specialColor = Color.blue;
        [SerializeField]
        private float timeForClick = 0.4f;

        [Header("Buttons")]

        [SerializeField]
        private Image stickImage = default;
        [SerializeField]
        private Image menuImage = default;

        [SerializeField]
        private Image downRightImage = default;
        [SerializeField]
        private Image downRightUpImage = default;
        [SerializeField]
        private Image downRightLeftImage = default;

        [SerializeField]
        private Image upRightImage = default;
        [SerializeField]
        private Image upRightDownImage = default;
        [SerializeField]
        private Image upRightLeftImagee = default;

        [SerializeField]
        private Image upLeftImage = default;
        [SerializeField]
        private Image upLeftDownImage = default;
        [SerializeField]
        private Image upLeftRightImage = default;
        #endregion

        #region Private Fields
        private Image _currentButton;
        private Timer _timer;
        private bool isPressing;
        #endregion

        #region Unity Callbacks
        private void Awake()
        {
            _timer = new Timer(timeForClick, false);
        }


        private void Update()
        {
            if (isPressing && _currentButton != null && _timer != null && _timer.IsFinish())
            {
                Color newColor = _currentButton.color == normalColor ? specialColor : normalColor;
                _currentButton.color = newColor;
                _timer.ResetOn();
            }
        }
        #endregion

        #region Public Methods
        public void ActiveTutorial(TutorialButtonMenu button, bool press = false)
        {
            ClearButtons();

            if (button == TutorialButtonMenu.stick && stickImage != null)
            {
                _currentButton = stickImage;
            }
            else if (button == TutorialButtonMenu.menu && menuImage != null)
            {
                _currentButton = menuImage;
            }
            else if (button == TutorialButtonMenu.downRight && menuImage != null)
            {
                _currentButton = downRightImage;
            }
            else if (button == TutorialButtonMenu.downRightLeft && menuImage != null)
            {
                _currentButton = downRightLeftImage;
            }
            else if (button == TutorialButtonMenu.downRightUp && menuImage != null)
            {
                _currentButton = downRightUpImage;
            }
            else if (button == TutorialButtonMenu.upLeft && menuImage != null)
            {
                _currentButton = upLeftImage;
            }
            else if (button == TutorialButtonMenu.upLeftDown && menuImage != null)
            {
                _currentButton = upLeftDownImage;
            }
            else if (button == TutorialButtonMenu.upLeftRight && menuImage != null)
            {
                _currentButton = upLeftRightImage;
            }
            else if (button == TutorialButtonMenu.upRight && menuImage != null)
            {
                _currentButton = upRightImage;
            }
            else if (button == TutorialButtonMenu.upRightDown && menuImage != null)
            {
                _currentButton = upRightDownImage;
            }
            else if (button == TutorialButtonMenu.upRightLeft && menuImage != null)
            {
                _currentButton = upRightLeftImagee;
            }


            _currentButton.enabled = true;
            if (press)
            {
                _currentButton.color = specialColor;
            }
            else
            {
                _timer?.ResetOn();
                isPressing = true;
            }
        }
        #endregion

        #region Private Methods
        private void ClearButtons()
        {
            _currentButton = null;
            isPressing = false;

            if (stickImage != null)
            {
                stickImage.enabled = false;
                stickImage.color = normalColor;
            }

            if (menuImage != null)
            {
                menuImage.enabled = false;
                menuImage.color = normalColor;
            }

            if (downRightImage != null)
            {
                downRightImage.enabled = false;
                downRightImage.color = normalColor;
            }

            if (downRightUpImage != null)
            {
                downRightUpImage.enabled = false;
                downRightUpImage.color = normalColor;
            }

            if (downRightLeftImage != null)
            {
                downRightLeftImage.enabled = false;
                downRightLeftImage.color = normalColor;
            }

            if (upRightImage != null)
            {
                upRightImage.enabled = false;
                upRightImage.color = normalColor;
            }

            if (upRightDownImage != null)
            {
                upRightDownImage.enabled = false;
                upRightDownImage.color = normalColor;
            }

            if (upRightLeftImagee != null)
            {
                upRightLeftImagee.enabled = false;
                upRightLeftImagee.color = normalColor;
            }

            if (upLeftImage != null)
            {
                upLeftImage.enabled = false;
                upLeftImage.color = normalColor;
            }

            if (upLeftDownImage != null)
            {
                upLeftDownImage.enabled = false;
                upLeftDownImage.color = normalColor;
            }

            if (upLeftRightImage != null)
            {
                upLeftRightImage.enabled = false;
                upLeftRightImage.color = normalColor;
            }
        }
        #endregion

    }
}
