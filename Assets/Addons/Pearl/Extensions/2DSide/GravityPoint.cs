#if PEARL_2DSIDE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pearl.Side2D
{
    public class GravityPoint : MonoBehaviour
    {
        /// the distance within which objects are impact by this gravity point
        [Tooltip("the distance within which objects are impact by this gravity point")]
        public float DistanceOfEffect;

        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(this.transform.position, DistanceOfEffect);
        }
    }
}

#endif
