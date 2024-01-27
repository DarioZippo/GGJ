using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewRoadTrigger : MonoBehaviour
{
    [SerializeField]GameObject currentRoad;  
    [SerializeField]GameObject nextRoadPrefab;
    [SerializeField]GameObject nextRoadAttachement;

    public void NewRoad(){
        Debug.Log("Triggered");
        GameObject newRoad = Instantiate(nextRoadPrefab, nextRoadAttachement.transform.position, Quaternion.identity);
    }
}
