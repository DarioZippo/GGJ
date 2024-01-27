using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pearl
{
    public static class BoundsExtend
    {

        #region Encapsulated
        public static bool IsEncapsulated(this Bounds @this, List<Renderer> containers, Vector3 translationVector = default)
        {
            List<Bounds> bounds = containers.CreateList((x) => x.bounds);
            return IsEncapsulated(@this, bounds, translationVector);
        }

        public static bool IsEncapsulated(this Bounds @this, List<Collider> containers, Vector3 translationVector = default)
        {
            List<Bounds> bounds = containers.CreateList((x) => x.bounds);
            return IsEncapsulated(@this, bounds, translationVector);
        }

        public static bool IsEncapsulated(this Bounds @this, List<Collider2D> containers, Vector3 translationVector = default)
        {
            List<Bounds> bounds = containers.CreateList((x) => x.bounds);
            return IsEncapsulated(@this, bounds, translationVector);
        }

        public static bool IsEncapsulated(this Bounds @this, List<SpriteRenderer> containers, Vector3 translationVector = default)
        {
            List<Bounds> bounds = containers.CreateList((x) => x.bounds);
            return IsEncapsulated(@this, bounds, translationVector);
        }

        public static bool IsEncapsulated(this Bounds @this, Bounds container, Vector3 translationVector = default)
        {
            return IsEncapsulated(@this, ListExtend.CreateList(container), translationVector);
        }

        public static bool IsEncapsulated(this Bounds @this, List<Bounds> containers, Vector3 translationVector = default)
        {
            foreach (var bounds in containers)
            {
                if (bounds.Contains(@this.min + translationVector) && bounds.Contains(@this.max + translationVector))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region IsSaw
        public static bool IsSaw(this Bounds @this, Bounds container, Vector3 translationVector = default)
        {
            var vertex = GetVertex(@this);

            foreach (var v in vertex)
            {
                if (container.Contains(v + translationVector))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
        public static Vector3[] GetVertex(this Bounds bounds)
        {
            Vector3[] vertex = new Vector3[4];
            vertex[0] = new Vector3(bounds.max.x, bounds.max.y, bounds.center.y);
            vertex[1] = new Vector3(bounds.max.x, bounds.min.y, bounds.center.y);
            vertex[2] = new Vector3(bounds.min.x, bounds.min.y, bounds.center.y);
            vertex[3] = new Vector3(bounds.min.x, bounds.max.y, bounds.center.y);
            return vertex;
        }


        /// <summary>
        /// Returns a random point within the bounds set as parameter
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public static Vector3 RandomPointInBounds(Bounds bounds)
        {
            return new Vector3(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y), Random.Range(bounds.min.z, bounds.max.z));
        }

        public static Vector3 OppositivePointsInbounds(this Bounds bounds, Vector3 point)
        {
            Vector3 delta = bounds.center - point;
            return bounds.center + delta;

        }

        /// <summary>
        /// Gets collider bounds for an object (from Collider2D)
        /// </summary>
        /// <param name="theObject"></param>
        /// <returns></returns>
        public static Bounds GetColliderBounds(GameObject theObject)
        {
            AssertExtend.PreConditions("theObject", theObject);

            if (theObject == null)
            {
                return default;
            }

            Bounds returnBounds;

            // if the object has a collider at root level, we base our calculations on that
            if (theObject.GetComponent<Collider>() != null)
            {
                returnBounds = theObject.GetComponent<Collider>().bounds;
                return returnBounds;
            }

            // if the object has a collider2D at root level, we base our calculations on that
            if (theObject.GetComponent<Collider2D>() != null)
            {
                returnBounds = theObject.GetComponent<Collider2D>().bounds;
                return returnBounds;
            }

            // if the object contains at least one Collider we'll add all its children's Colliders bounds
            if (theObject.GetComponentInChildren<Collider>() != null)
            {
                Bounds totalBounds = theObject.GetComponentInChildren<Collider>().bounds;
                Collider[] colliders = theObject.GetComponentsInChildren<Collider>();
                foreach (Collider col in colliders)
                {
                    totalBounds.Encapsulate(col.bounds);
                }
                returnBounds = totalBounds;
                return returnBounds;
            }

            // if the object contains at least one Collider2D we'll add all its children's Collider2Ds bounds
            if (theObject.GetComponentInChildren<Collider2D>() != null)
            {
                Bounds totalBounds = theObject.GetComponentInChildren<Collider2D>().bounds;
                Collider2D[] colliders = theObject.GetComponentsInChildren<Collider2D>();
                foreach (Collider2D col in colliders)
                {
                    totalBounds.Encapsulate(col.bounds);
                }
                returnBounds = totalBounds;
                return returnBounds;
            }

            returnBounds = new(Vector3.zero, Vector3.zero);
            return returnBounds;
        }

        /// <summary>
        /// Gets bounds of a renderer
        /// </summary>
        /// <param name="theObject"></param>
        /// <returns></returns>
        public static Bounds GetRendererBounds(GameObject theObject)
        {
            AssertExtend.PreConditions("theObject", theObject);

            if (theObject == null)
            {
                return default;
            }

            Bounds returnBounds;

            // if the object has a renderer at root level, we base our calculations on that
            if (theObject.GetComponent<Renderer>() != null)
            {
                returnBounds = theObject.GetComponent<Renderer>().bounds;
                return returnBounds;
            }

            // if the object contains at least one renderer we'll add all its children's renderer bounds
            if (theObject.GetComponentInChildren<Renderer>() != null)
            {
                Bounds totalBounds = theObject.GetComponentInChildren<Renderer>().bounds;
                Renderer[] renderers = theObject.GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in renderers)
                {
                    totalBounds.Encapsulate(renderer.bounds);
                }
                returnBounds = totalBounds;
                return returnBounds;
            }

            returnBounds = new(Vector3.zero, Vector3.zero);
            return returnBounds;
        }
    }
}
