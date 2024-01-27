using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pearl;
using Pearl.Input;
using Pearl.Debugging;
using Pearl.Events;
using Game;

public class NewRoadTrigger : MonoBehaviour
{
    //int id;

    [SerializeField]GameObject currentRoad;  
    [SerializeField]GameObject nextRoadPrefab;
    [SerializeField]GameObject nextRoadAttachement;

    public void OnTriggerEnter(){
            GameObject newRoad = Instantiate(nextRoadPrefab, nextRoadAttachement.transform.position, Quaternion.identity);
    }
}