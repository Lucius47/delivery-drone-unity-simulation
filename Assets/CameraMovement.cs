using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform objectToFollow;
    Vector3 offset;
    void Start()
    {
        offset = objectToFollow.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = (objectToFollow.position - offset);
    }
}
