using Pearl.Tweens;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Pearl
{
    public class BlackPageManager : PearlBehaviour, ISingleton
    {
        #region Inspector Field
        [SerializeField]
        private Image container = null;
        #endregion

        #region Event
        public static event Action OnFinish;
        #endregion

        #region Privat fields
        private TweenContainer _tween;
        #endregion

        #region Static
        public static void AppearBlackPage(float time = 0, TypeSibilling sibilling = TypeSibilling.Last, int positionChild = 0, bool restorePage = false)
        {
            if (Singleton<BlackPageManager>.GetIstance(out var blackManager))
            {
                if (blackManager)
                {
                    blackManager.Appear(time, sibilling, positionChild, restorePage);
                }
            }
        }

        public static void DisappearBlackPage(float time = 0, TypeSibilling sibilling = TypeSibilling.Last, int positionChild = 0, bool restorePage = false)
        {
            if (Singleton<BlackPageManager>.GetIstance(out var blackManager))
            {
                if (blackManager)
                {
                    blackManager.Disappear(time, sibilling, positionChild, restorePage);
                }
            }
        }
        #endregion

        #region UnityCallbacks
        private void Reset()
        {
            container = GetComponent<Image>();
        }

        protected override void PearlAwake()
        {
            _tween = TweensExtend.FadeTween(container, 0, false, ChangeModes.Time);
            _tween.OnComplete += OnComplete;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (_tween != null)
            {
                _tween.OnComplete -= OnComplete;
                _tween.Kill();
            }
        }
        #endregion

        #region Public Methods
        public void OnComplete(TweenContainer tween, float delay)
        {
            OnFinish?.Invoke();
        }

        public void Appear(in float time = 0, TypeSibilling sibilling = TypeSibilling.Last, int positionChild = 0, bool restorePage = false)
        {
            transform.SetSibilling(sibilling, positionChild);

            if (restorePage && container != null)
            {
                container.SetAlpha(0);
            }

            if (_tween != null)
            {
                _tween.Stop();
                _tween.Duration = time;
                _tween.FinalValues = ArrayExtend.CreateArray(Vector4.one);
                _tween.Play(true);
            }
        }

        public void Disappear(in float time = 0, TypeSibilling sibilling = TypeSibilling.Last, int positionChild = 0, bool restorePage = false)
        {
            transform.SetSibilling(sibilling, positionChild);

            if (restorePage && container != null)
            {
                container.SetAlpha(1);
            }

            if (_tween != null)
            {
                _tween.Stop();
                _tween.Duration = time;
                _tween.FinalValues = ArrayExtend.CreateArray(Vector4.zero);
                _tween.Play(true);
            }
        }
        #endregion

    }
}
