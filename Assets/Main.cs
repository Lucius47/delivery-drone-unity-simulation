using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    enum States { Calibration, Ascent, Hover, Travel, Descent};
    States currentState;

    public Sensors sensors;
    Rigidbody rb;
    public float highThrustFactor;
    public float lowThrustFactor;
    public float torque;

    public Transform motor1Position;
    public Transform motor2Position;
    public Transform motor3Position;
    public Transform motor4Position;

    float droneAltitude;
    public float targetRotationX;
    public float targetAltitude;
    float angleToDestination;
    float distanceToDestination;

    public Vector3 destination = new Vector3();

    public Text verticalSpeedText;
    public Text horizontalSpeedText;
    public Text angleToDest;
    public Text distToDest;
    public Text altitudeText;
    public Text xRotationText;
    public Text orientation;

    float minThrustReqToHover;


    public GameObject propellar1;
    public GameObject propellar2;
    public GameObject propellar3;
    public GameObject propellar4;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentState = States.Ascent;
    }

    private void Update()
    {
        angleToDest.text = angleToDestination.ToString();
        distToDest.text = distanceToDestination.ToString();
        verticalSpeedText.text = rb.velocity.y.ToString();
        horizontalSpeedText.text = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude.ToString();
        altitudeText.text = sensors.AltSensor().ToString();
        xRotationText.text = sensors.Gyroscope().ToString();
        sensors.Gyroscope();
        SetOrientation();
    }

    private void FixedUpdate()
    {
        droneAltitude = sensors.AltSensor();
        PropellarRotation();
        

        distanceToDestination = destination.magnitude - sensors.GPS().magnitude;
        distToDest.text = distanceToDestination.ToString();

        SetAltitude(targetAltitude);

        StartCoroutine(sensors.Accelerometer(rb));



        if (currentState == States.Hover)
        {
            SetRotation(destination);
        }

    }

    void SetAltitude(float _targetAltitude)
    {
        if (droneAltitude <= _targetAltitude)
        {
            float thrustFactor = Mathf.InverseLerp(sensors.AltSensor(), _targetAltitude, _targetAltitude - sensors.AltSensor());
            MotorThrust(1, 1.05f + thrustFactor);
            MotorThrust(2, 1.05f + thrustFactor);
            MotorThrust(3, 1.05f + thrustFactor);
            MotorThrust(4, 1.05f + thrustFactor);
            
        }

        if (droneAltitude > _targetAltitude)
        {
            float thrustFactor2 = Mathf.InverseLerp(sensors.AltSensor(), targetAltitude, Mathf.Abs(targetAltitude - sensors.AltSensor()));
            MotorThrust(1, thrustFactor2 - 0.1f);
            MotorThrust(2, thrustFactor2 - 0.1f);
            MotorThrust(3, thrustFactor2 - 0.1f);
            MotorThrust(4, thrustFactor2 - 0.1f);
            
        }
        if (Mathf.Abs(droneAltitude - targetAltitude) < 0.1f)
        {
            currentState = States.Hover;
        }

        
    }

    /*void MotorThrustCalibration(int motorIndex, float factor)
    {
        if(motorIndex == 1)
        {
            rb.AddForceAtPosition(transform.up * factor / 4, motor1Position.position);
        }
        if (motorIndex == 2)
        {
            rb.AddForceAtPosition(transform.up * factor / 4, motor2Position.position);
        }
        if (motorIndex == 3)
        {
            rb.AddForceAtPosition(transform.up * factor / 4, motor3Position.position);
        }
        if (motorIndex == 4)
        {
            rb.AddForceAtPosition(transform.up * factor / 4, motor4Position.position);
        }
    }*/

    void MotorThrust(int motorIndex, float factor)
    {
        if (motorIndex == 1)
        {
            rb.AddForceAtPosition(transform.up * factor * GetMinimumThrustRequiredToHover() / 4, motor1Position.position);
        }
        if (motorIndex == 2)
        {
            rb.AddForceAtPosition(transform.up * factor * GetMinimumThrustRequiredToHover() / 4, motor2Position.position);
        }
        if (motorIndex == 3)
        {
            rb.AddForceAtPosition(transform.up * factor * GetMinimumThrustRequiredToHover() / 4, motor3Position.position);
        }
        if (motorIndex == 4)
        {
            rb.AddForceAtPosition(transform.up * factor * GetMinimumThrustRequiredToHover() / 4, motor4Position.position);
        }
    }


    void SetRotation(Vector3 _destination)
    {
        if (getAngleToDestination() > 0.01)
        {
            rb.AddTorque(new Vector3(0, torque, 0));
        }
        if (getAngleToDestination() < -0.01)
        {
            rb.AddTorque(new Vector3(0, -torque, 0));
        }
    }

    float getAngleToDestination()
    {
        Vector3 vectorToDestination = destination - sensors.GPS();
        angleToDestination = Vector3.SignedAngle(new Vector3(transform.forward.x, 0, transform.forward.z), vectorToDestination, transform.up);
        return angleToDestination;
    }

    float GetMinimumThrustRequiredToHover()
    {
        //float MaxThrust = 100; //The highest thrust the motors can provide.
        //float MinThrust; //The lowest thrust required to keep the drone hovering.
        //float thrust = 0;
        //for(int i = 0; i <= MaxThrust; i++)
        //{
        //    MotorThrustCalibration(1, i);
        //    MotorThrustCalibration(2, i);
        //    MotorThrustCalibration(3, i);
        //    MotorThrustCalibration(4, i);
        //    yield return new WaitForSeconds(0.05f);
        //    if(acc > 0.01f)
        //    {
        //        thrust = i;
        //        mass = thrust / acc;
        //    }
        //}
        minThrustReqToHover = rb.mass * Physics.gravity.magnitude;
        return minThrustReqToHover;
        //currentState = States.Ascent;
    }

    void PropellarRotation()
    {
        propellar1.transform.Rotate(new Vector3(0, 0, transform.up.y * 100));
        propellar2.transform.Rotate(new Vector3(0, 0, transform.up.y * 100));
        propellar3.transform.Rotate(new Vector3(0, 0, transform.up.y * 100));
        propellar4.transform.Rotate(new Vector3(0, 0, transform.up.y * 100));
    }

    void SetOrientation()
    {
        float rotationOnX = transform.rotation.eulerAngles.x;
        float rotationOnZ = transform.rotation.eulerAngles.z;
        orientation.text = "x: " + rotationOnX.ToString() + ", " + "z: " + rotationOnZ.ToString();
        if (rotationOnX > 0)
        {

        }
        if (rotationOnX < 0)
        {

        }
        if (rotationOnZ > 0)
        {

        }
        if (rotationOnZ < 0)
        {

        }
        if (rotationOnX > 0 && rotationOnZ > 0)
        {

        }
        if (rotationOnX < 0 && rotationOnZ < 0)
        {

        }
        if (rotationOnX > 0 && rotationOnZ < 0)
        {

        }
        if (rotationOnX < 0 && rotationOnZ > 0)
        {

        }
    }
}