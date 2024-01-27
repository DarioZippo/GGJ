using UnityEngine;

namespace Pearl
{
    /// <summary>
    /// A class that extends the Quaternion class
    /// </summary>
    public static class QuaternionExtend
    {
        /// <summary>
        /// Returns the specific rotation from a particular direction vector
        /// </summary>
        /// <param name = "transform"> The specific component transform</param>
        public static Quaternion CalculateRotation2D(Vector2 direction, float angle0 = 0)
        {
            float angle = CalculateAngle2D(direction, false, angle0);
            return Quaternion.AngleAxis(angle, Vector3.forward);
        }

        public static Vector2 DirFromAngle(this Transform transform, float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
            {
                angleInDegrees += transform.eulerAngles.y;
            }

            return DirFromAngle(angleInDegrees);
        }

        public static Vector2 DirFromAngle(float angleInDegrees)
        {
            angleInDegrees *= Mathf.Deg2Rad;

            return new Vector2(Mathf.Cos(angleInDegrees), Mathf.Sin(angleInDegrees));
        }

        public static float CalculateAngle2D(Vector2 direction, bool positive = false,  float angle0 = 0)
        {
            angle0 %= 360;

            float angle = Vector2.SignedAngle(Vector2.right, direction);
            
            if (positive && angle < 0)
            {
                angle = 180 + (180 + angle);
            }

            angle += angle0;

            if (positive)
            {
                angle %= 360;
            }
            else if (angle > 180)
            {
                angle = -180 + (angle - 180);
            }

            return angle;
            //return (Vector2Extend.Tan(direction) * Mathf.Rad2Deg) + angle0;
        }

        public static void LookRotation2D(this Transform transform, Transform target)
        {
            Vector3 dir = target.position - transform.position;
            Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * dir;
            Quaternion rotation = Quaternion.LookRotation(Vector3.forward, rotatedVectorToTarget);
            transform.rotation = rotation;
        }

        public static void RotationByCenter(this Transform transform, Transform center, float angle)
        {
            transform.RotateAround(center.position, Vector3.forward, angle);
        }

        public static Vector3 RotationByCenter(Vector3 center, Vector3 position, float angle)
        {
            Vector3 diff = (position - center);

            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            Vector3 vectorRotation = rotation * diff;

            return center + vectorRotation;
        }
    }
}
