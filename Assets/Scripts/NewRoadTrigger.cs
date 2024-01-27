using Game;
using UnityEngine;

public class NewRoadTrigger : MonoBehaviour
{
    //int id;

    [SerializeField] GameObject currentRoad;
    [SerializeField] GameObject nextRoadAttachement;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (RoadManager.GetIstance(out var result))
            {
                var nextElement = result.GetNextRoad();
                GameObject newRoad = Instantiate(nextElement, nextRoadAttachement.transform.position, Quaternion.identity);
                
                // Ottieni le dimensioni dell'oggetto
                Vector3 size = newRoad.GetComponent<Renderer>().bounds.size;

                // Sposta newRoad di metà delle sue dimensioni sull'asse Z
                newRoad.transform.position += new Vector3(0, 0, size.z / 2);
            }
        }
    }
}