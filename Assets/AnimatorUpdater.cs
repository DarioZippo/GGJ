using Pearl.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorUpdater : MonoBehaviour
{
    public Animator animator;

    private void Start() {
        animator = GetComponent<Animator>();

        InputManager.PerformedHandle("Acceleration", AccelerationDown, AccelerationUp, Pearl.ActionEvent.Add);
        InputManager.PerformedHandle("DeAcceleration", DeaccelerationDown, DeaccelerationUp, Pearl.ActionEvent.Add);
    }

    private void OnDestroy() {
        InputManager.PerformedHandle("Acceleration", AccelerationDown, AccelerationUp, Pearl.ActionEvent.Remove);
        InputManager.PerformedHandle("DeAcceleration", DeaccelerationDown, DeaccelerationUp, Pearl.ActionEvent.Remove);
    }

    private void AccelerationDown() {
        animator.SetBool("Turbo", true);
    }

    private void AccelerationUp() {
        animator.SetBool("Turbo", false);
    }

    private void DeaccelerationDown() {
        animator.SetBool("Break", true);
    }

    private void DeaccelerationUp() {
        animator.SetBool("Break", false);
    }
}
