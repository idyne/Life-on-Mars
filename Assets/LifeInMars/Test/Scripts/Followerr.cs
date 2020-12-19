using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class Followerr : MonoBehaviour
{
    public PathCreator pathCreator;
    public float speed = 1;
    float distanceTravelled = 0;

    private void Update()
    {
        distanceTravelled += speed * Time.deltaTime;
        transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled);
        transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled);
        print(pathCreator.path.length);
        print(distanceTravelled);
    }
}
