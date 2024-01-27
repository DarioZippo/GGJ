#if PEARL_2DSIDE

using UnityEngine;
using System.Collections;
using System.Diagnostics;

namespace Pearl.Side2D
{
    /// <summary>
    /// Add this component to an object with a trigger 2D collider and it'll act as a gravity zone, modifying the gravity for all Characters entering it, providing they have the CharacterGravity ability
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class GravityZone : ControllerPhysicsVolume2D
    {
        /// the angle of the gravity for this zone
        [Range(0, 360), Tooltip("the angle of the gravity for this zone")]
        public float GravityDirectionAngle = 180;
        /// the vector angle of the gravity for this zone
        public Vector2 GravityDirectionVector { get { return Vector2Extend.RotateVector2(Vector2.down, GravityDirectionAngle); } }

        /// <summary>
        /// On DrawGizmos, we draw an arrow to show the zone's current gravity direction
        /// </summary>
        protected virtual void OnDrawGizmosSelected()
        {
            //MMDebug.DebugDrawArrow(this.transform.position, GravityDirectionVector, Color.green, 3f, 0.2f, 35f);
        }
    }
}

#endif