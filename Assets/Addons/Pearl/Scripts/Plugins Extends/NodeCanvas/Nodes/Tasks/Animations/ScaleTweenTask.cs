#if NODECANVAS

using Pearl.Tweens;
using UnityEngine;

namespace Pearl.NodeCanvas.Tasks
{
    [ParadoxNotion.Design.Category("Pearl/Tweens")]
    public class ScaleTweenTask : TweenTask<Vector3, Transform>
    {
        [SerializeField]
        private bool multiplyValue = false;

        protected override TweenContainer CreateTween(in Transform container, in float timeOrVelocity)
        {
            if (container != null && newValue != null)
            {
                return container.ScaleTween(multiplyValue, timeOrVelocity, true, AxisCombined.XYZ, functionEnum.value, changeMode.value, newValue.value);
            }
            return null;
        }

        protected override void UseDefaultValue(in TweenContainer tween)
        {
            if (useDefaultValue != null && useDefaultValue.value && defaultValue != null && functionEnum != null)
            {
                tween.SetInitValue(defaultValue.value);
            }
        }
    }
}
#endif