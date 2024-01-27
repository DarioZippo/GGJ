using UnityEngine;

namespace Pearl.Tweens
{
    public class ScaleTweenComponent : TweenComponent<Vector3, Transform>
    {
        [SerializeField]
        private bool multiplyValue = false;

        protected override void CreateTween()
        {
            _tween = TweensExtend.ScaleTween(container.Component, multiplyValue, _valueForTween, isAutoKill, functionEnum, mode, values);
        }

        protected void Reset()
        {
            if (container != null)
            {
                container.Component = transform;
            }
        }

        public override void SetValue(Vector3 type)
        {
            if (transform != null)
            {
                transform.localScale = type;
            }
        }
    }
}