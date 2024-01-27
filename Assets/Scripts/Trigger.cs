using UnityEngine;

public class Trigger : MonoBehaviour
{
    [SerializeField] GameObject currentRoad;
    [SerializeField] GameObject nextRoadPrefab;

    //Spline currentSpline;

    void Start()
    {
        //currentSpline = currentRoad.GetComponent<Spline>();
    }

    void onTriggerEnter(Collider other)
    {
        //GameObject newRoad = Instantiate(nextRoadPrefab, currentSpline.nodes[currentSpline.nodes.Count].transform.position);
        //newRoad.parent = null;
    }
}
