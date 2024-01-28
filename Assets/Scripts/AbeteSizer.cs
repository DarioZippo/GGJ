using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSize : MonoBehaviour
{
    public float lowestHeight = 0.5f, maxHeight = 1.2f; 

    void Start()
    {
        float randomScale = Random.Range(lowestHeight, maxHeight);
        transform.localScale = new Vector3(transform.localScale.x, randomScale, transform.localScale.z);
    }
}
