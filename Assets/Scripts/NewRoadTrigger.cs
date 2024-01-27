using Game;
using UnityEngine;

public class NewRoadTrigger : MonoBehaviour
{
    //int id;

    [SerializeField] GameObject currentRoad;
    [SerializeField] GameObject nextRoadAttachement;

    public void OnTriggerEnter()
    {
        //GameObject newRoad = Instantiate(nextRoadPrefab, nextRoadAttachement.transform.position, Quaternion.identity);
        if (RoadManager.GetIstance(out var result))
        {
            var nextElement = result.GetNextRoad();
            GameObject newRoad = Instantiate(nextElement, nextRoadAttachement.transform.position, Quaternion.identity);
        }
    }
}