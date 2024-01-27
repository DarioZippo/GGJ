using System;
using UnityEngine;

namespace Pearl
{
    [DisallowMultipleComponent]
    public sealed class PearlObject : MonoBehaviour
    {
        #region Inspector Fields
        [SerializeField]
        private bool disactiveAtStart = false;

        [Header("Don't destroy load")]
        /// <summary>
        //  If the Boolean is true, the GameObject will be not destroy at load
        /// </summary>
        [SerializeField]
        private bool dontDestroyLoad = false;

        /// <summary>
        //  If the Boolean is true, the GameObject will be unique and each of its clone will be eliminated in the scene
        /// </summary>
        [ConditionalField("@dontDestroyLoad"), SerializeField]
        private bool isUnique = false;

        [Header("Application")]
        [SerializeField]
        private bool deleteInWebGL;
        [SerializeField]
        private bool deleteInMobile;
        #endregion

        #region Private Field
        private bool _fakeDestroyed;
        #endregion

        #region Event
        public event Action DestroyHandler;
        public event Action<GameObject> DisactiveHandler;
        #endregion

        #region Properties
        public bool FakeDestroyed { get { return _fakeDestroyed; } }
        #endregion

        #region Unity Callbacks
        private void Awake()
        {
            if (deleteInWebGL)
            {
                DeleteInWeb();
            }

            if (deleteInMobile)
            {
                DeleteInMobile();
            }

            if (isUnique && AmIClone())
            {
                GameObject.DestroyImmediate(gameObject);
                return;
            }

            if (dontDestroyLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        private void Start()
        {
            if (disactiveAtStart)
            {
                gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            DestroyHandler?.Invoke();
        }

        private void OnDisable()
        {
            DisactiveHandler?.Invoke(gameObject);
        }
        #endregion

        #region Public Methods
        public void FakeDestroy(bool destroy)
        {
            _fakeDestroyed = destroy;

            var components = transform.GetComponentsInHierarchy<Component>();
            foreach (var component in components)
            {
                if (component is IReset iReset)
                {
                    if (destroy)
                    {
                        iReset.DisableElement();
                    }
                    else
                    {
                        iReset.ResetElement();
                    }
                }


                if (component is Behaviour behavour)
                {
                    behavour.enabled = !destroy;
                }
                else if (component is Collider collider)
                {
                    collider.enabled = !destroy;
                }
            }
        }

        public void Destroy()
        {
            GameObjectExtend.DestroyExtend(gameObject);
        }
        #endregion

        #region Private
        private void DeleteInWeb()
        {
            if (GameManager.IsWebGL())
            {
                GameObject.Destroy(gameObject);
            }
        }

        private void DeleteInMobile()
        {
            if (GameManager.IsMobile())
            {
                GameObject.Destroy(gameObject);
            }
        }

        private bool AmIClone()
        {
            PearlObject[] objs = GameObject.FindObjectsByType<PearlObject>(FindObjectsSortMode.None);

            if (objs == null)
            {
                return true;
            }

            objs = Array.FindAll(objs, x => x.name == name);
            return objs == null || objs.Length > 1;
        }
        #endregion
    }
}
