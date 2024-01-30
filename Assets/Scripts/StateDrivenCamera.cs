using Cinemachine;
using Pearl.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StateDrivenCamera : MonoBehaviour
{
    public GameObject player;
    public CinemachineVirtualCamera vcam;

    void Start()
    {
        InputManager.PerformedHandle("Acceleration", AccelerationDown, AccelerationUp, Pearl.ActionEvent.Add);
        InputManager.PerformedHandle("DeAcceleration", DeaccelerationDown, DeaccelerationUp, Pearl.ActionEvent.Add);

        var vcam = GetComponent<CinemachineVirtualCamera>();

        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
        }
        Transform target = player.transform;
        vcam.Follow = target;
    }

    private void OnDestroy() {
        InputManager.PerformedHandle("Acceleration", AccelerationDown, AccelerationUp, Pearl.ActionEvent.Remove);
        InputManager.PerformedHandle("DeAcceleration", DeaccelerationDown, DeaccelerationUp, Pearl.ActionEvent.Remove);
    }

    private void AccelerationDown() {
        //vcam.
    }

    private void AccelerationUp() {
        //acc = false;
    }

    private void DeaccelerationDown() {
        //deAcc = true;
    }

    private void DeaccelerationUp() {
        //deAcc = false;
    }
}