using UnityEngine;

namespace Pearl
{
    public static class SpriteRenderExtend
    {
        public static int MaxSortingLayer { get { return 32767; } }

        public static void ChangeAlpha(this SpriteRenderer @this, float alpha)
        {
            if (@this)
            {
                Color color = @this.color;
                color.a = alpha;
                @this.color = color;
            }
        }

        public static void PositionWithForcePivot(this SpriteRenderer @this, Vector2 position, Vector2 forcePivot)
        {
            if (@this && @this.sprite != null)
            {
                Vector2 localVector = @this.sprite.pivot / @this.sprite.rect.size;
                Vector2 delta = forcePivot - localVector;
                Vector2 worldDelta = @this.size * delta;
                @this.transform.position = position - worldDelta;
            }
        }
    }
}
