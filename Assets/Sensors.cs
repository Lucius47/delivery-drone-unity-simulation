using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensors : MonoBehaviour
{
    [HideInInspector]
    public float acc;


    public Vector3 Gyroscope()
    {
        //rotationX = transform.rotation.eulerAngles.x;
        //return rotationX;
        Vector3 orientation = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        //print(orientation);
        return orientation;
    }

    public float AltSensor()
    {
        float altitude = transform.position.y;
        return altitude;
    }

    //void SpeedMeter()
    //{
        /*float pos1 = transform.position.z;
        yield return new WaitForSeconds(1);
        float pos2 = transform.position.z;
        speed = pos2 - pos1;*/
        
    //}


    public Vector3 GPS()
    {
        return new Vector3(transform.position.x, 0, transform.position.z);
    }


    public IEnumerator Accelerometer(Rigidbody rb)
    {
        float velocity1 = rb.velocity.y;
        yield return new WaitForSeconds(0.02f);
        float velocity2 = rb.velocity.y;
        acc = ((velocity2 - velocity1) / 0.02f);
        //print(acc);
    }
}
