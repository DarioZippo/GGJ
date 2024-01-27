using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Pearl
{
    public static class ScrollRectExtend
    {
        public static void ScrollTo(this ScrollRect @this, Transform target, Axis2DCombined axis = Axis2DCombined.XY)
        {
            if (@this == null) return;

            Canvas.ForceUpdateCanvases();
            var content = @this.content;
            Vector2 offset = -@this.transform.InverseTransformPoint(target.position);
            Vector2 anchor = content.anchoredPosition;
            anchor.x = axis == Axis2DCombined.X || axis == Axis2DCombined.XY ? offset.x : anchor.x;
            anchor.y = axis == Axis2DCombined.Y || axis == Axis2DCombined.XY ?  offset.y : anchor.y;
            content.anchoredPosition = anchor;
        }
    }
}
