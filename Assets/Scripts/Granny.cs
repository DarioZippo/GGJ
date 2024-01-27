using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

public class Granny : MonoBehaviour
{
    public float speed;

    // Update is called once per frame
    void Update()
    {
        if (IsOutOfBoundaries(transform.position.x))
        {
            transform.Rotate(Vector3.up, 180);
        }
        transform.Translate(speed * Vector3.right * Time.deltaTime);
    }

    bool IsOutOfBoundaries(float xPosition){
        return (xPosition <= SpecificLevelManager.GetSpecificIstance().boundaries[0].position.x || 
            xPosition >= SpecificLevelManager.GetSpecificIstance().boundaries[1].position.x);
    }
}
