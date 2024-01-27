using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Pearl.MenuMobile
{
    public class InputMobileMenuManager : MonoBehaviour
    {
        #region Inspector Fields
        [SerializeField]
        private bool test = false;
        [SerializeField]
        private bool invisibile = false;
        #endregion

        #region Unity Callbacks
        // Start is called before the first frame update
        private void Start()
        {
            #if UNITY_EDITOR
            if (!GameManager.IsMobile() && !test)
            {
                Destroy(gameObject);
            }
#else
            if (!GameManager.IsMobile())
            {
                Destroy(gameObject);
            }
#endif

            SetAlphaMenu(0);
        }

        private void OnValidate()
        {
            if (invisibile)
            {
                SetAlphaMenu(0);
            }
            else
            {
                SetAlphaMenu(1);
            }
        }
        #endregion

        #region Private Methods
        private void SetAlphaMenu(float alpha)
        {
            var images = transform.GetComponentsInHierarchy<Image>();
            if (images != null)
            {
                foreach (Image image in images) 
                {
                    image.SetAlpha(alpha);
                }
            }
        }
        #endregion
    }
}
