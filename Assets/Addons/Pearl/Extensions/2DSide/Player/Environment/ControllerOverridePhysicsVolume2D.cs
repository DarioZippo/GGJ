#if PEARL_2DSIDE

using IOM;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pearl;


namespace Pearl.Side2D
{
    [Serializable]
    public struct InfoControllerPhysicsVolume2D
    {
        /// the new parameters meant to override the olds ones
        [Tooltip("the new parameters meant to override the olds ones")]
        public Side2DControllerParameters ControllerParameters;

        [Header("Force Modification on Entry")]

        /// if this is true, all forces currently applied to this controller will be reset on entry
        [Tooltip("if this is true, all forces currently applied to this controller will be reset on entry")]
        public bool ResetForcesOnEntry;
        /// if this is true, forces will be multiplied by the values specified below
        [Tooltip("if this is true, forces will be multiplied by the values specified below")]
        public bool MultiplyForcesOnEntry;
        /// the force by which to multiply the current forces
        [Tooltip("the force by which to multiply the current forces")]
        public Vector2 ForceMultiplierOnEntry;
    }

    public class ControllerOverridePhysicsVolume2D : MonoBehaviour
    {
        [SerializeField]
        private InfoControllerPhysicsVolume2D parameters;


        public void Enter(GameObject obj)
        {
            Side2DController playerController = obj.GetComponent<Side2DController>();
            //playerController.ChangeControllerOverride(parameters, ActionEvent.Add);
        }

        public void Exit(GameObject obj)
        {
            Side2DController playerController = obj.GetComponent<Side2DController>();
            //playerController.ChangeControllerOverride(parameters, ActionEvent.Remove);
        }
    }
}

#endif