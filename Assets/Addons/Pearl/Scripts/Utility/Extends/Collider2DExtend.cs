using UnityEngine;

namespace Pearl
{
    public static class Collider2DExtend
    {
        public static bool IsTouching(this Collider2D collider1, Collider2D collider2)
        {
            if (collider1 != null && collider2 != null)
            {
                return collider1.bounds.Intersects(collider2.bounds);
            }
            return false;
        }

        public static bool GetExitPontToCollider(out Vector2 pointSurface, Collider2D obj, Collider2D background, Vector2 movement, bool drawGizmo = false, Color color = default)
        {
            pointSurface = Vector2.zero;
            if (!background.OverlapPoint(obj.gameObject.transform.position))
            {
                return false;
            }

            Vector2 finalPoint = (Vector2) obj.transform.position + movement;

            if (background.OverlapPoint(finalPoint))
            {
                return false;
            }

            int backgroundID = background.gameObject.GetInstanceID();
            RaycastHit2D[] results = Physics2DExtend.RayCastAll(finalPoint, -movement.normalized, movement.magnitude, LayerExtend.CreateLayerMask(obj.gameObject.layer), drawGizmo, color);
            foreach (RaycastHit2D r in results)
            {
                if (r.collider.gameObject.GetInstanceID() == backgroundID)
                {
                    pointSurface = r.point;
                    return true;
                }
            }

            return false;
        }
    }
}
