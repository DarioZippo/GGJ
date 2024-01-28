using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FollowPlayer : MonoBehaviour
{
    public GameObject player;
    public CinemachineVirtualCamera vcam;

    void Start()
    {
        var vcam = GetComponent<CinemachineVirtualCamera>();

        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
        }
        Transform target = player.transform;
        vcam.Follow = target;
    }
}