#if PEARL_2DSIDE

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Pearl.Side2D.Editor
{
    [CustomEditor(typeof(Side2DController))]
    [CanEditMultipleObjects]
    public class Side2DControllerInspector : UnityEditor.Editor
    {
        /// <summary>
        /// When inspecting a Controller, we add to the regular inspector some labels, useful for debugging
        /// </summary>
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("The 2Side controller class handles collisions and basic movement for your Character. Unfold the Default Parameters dropdown below to setup gravity, speed settings and slope angle and speed.", MessageType.Info);

            Side2DController controller = (Side2DController)target;
            if (controller.State != null)
            {
                EditorGUILayout.LabelField("Grounded", controller.State.IsGrounded.ToString());
                EditorGUILayout.LabelField("Falling", controller.State.IsFalling.ToString());
                EditorGUILayout.LabelField("ColliderResized", controller.State.ColliderResized.ToString());
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Colliding Left", controller.State.IsCollidingLeft.ToString());
                EditorGUILayout.LabelField("Colliding Right", controller.State.IsCollidingRight.ToString());
                EditorGUILayout.LabelField("Colliding Above", controller.State.IsCollidingAbove.ToString());
                EditorGUILayout.LabelField("Colliding Below", controller.State.IsGrounded.ToString());
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Slope Angle", controller.State.BelowSlopeAngle.ToString());
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("On a moving platform", controller.State.OnAMovingPlatform.ToString());
            }
            DrawDefaultInspector();
        }

    }
}

#endif
