using Pearl.ClockManager;
using UnityEngine;

namespace Pearl
{
    public class GIFManager : MonoBehaviour, IReset
    {
        #region Inspector field
        [SerializeField]
        private GameObject referenceContainerSprite = null;
        [SerializeField]
        private bool useGIF = true;
        [SerializeField]
        private float timeGIF = 1;
        [SerializeField]
        private Sprite[] sprites = null;
        #endregion

        #region Private field
        private SpriteManager _spriteManager;
        private SimpleTimer _timer;
        private int _currentIndex = 0;
        #endregion

        #region Properties
        public bool UseGIF
        {
            set
            {
                useGIF = value;
                ResetElement();
            }
        }

        public Sprite[] Sprites
        {
            set
            {
                sprites = value;
                ResetElement();
            }
        }

        public int CurrentIndex
        {
            set
            {
                _currentIndex = value;
                if (sprites != null)
                {
                    _currentIndex = Mathf.Clamp(_currentIndex, 0, sprites.Length);
                }
                ResetElement();
            }
        }
        #endregion

        #region Unity Callbacks
        // Start is called before the first frame update
        protected void Awake()
        {
            _spriteManager = new SpriteManager(referenceContainerSprite);
            ResetElement();
        }

        protected void Reset()
        {
            referenceContainerSprite = gameObject;
        }

        protected void Start()
        {
            if (useGIF)
            {
                SetFrame();
            }
        }

        // Update is called once per frame
        protected void Update()
        {
            UpdateGIF();
        }
        #endregion

        #region IReset
        public void DisableElement()
        {
        }

        public void ResetElement()
        {
            _timer.Reset(timeGIF / sprites.Length);
            _currentIndex = 0;
        }
        #endregion

        #region Private methods
        private void UpdateGIF()
        {
            if (useGIF && sprites != null && _timer.IsFinish())
            {
                _currentIndex = MathfExtend.ChangeInCircle(_currentIndex, 1, sprites.Length);
                _timer.Reset();
                SetFrame();
            }
        }

        private void SetFrame()
        {
            if (sprites.IsAlmostSpecificCount(_currentIndex))
            {
                _spriteManager?.SetSprite(sprites[_currentIndex]);
            }
        }
        #endregion
    }
}
