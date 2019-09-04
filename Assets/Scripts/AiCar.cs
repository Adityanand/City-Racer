using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiCar : MonoBehaviour
{
    [Header("Path")]
    public Transform[] Waypoints;
    public int CurrentWaypoints;
    [Header("Car Setup")]
    public float MaxSteerAngle;
    public float TurnSpeed;
    public float MaxTorque;
    public float MaxBreakingTorque;
    public float CurrentSpeed;
    public float MaxSpeed;
    public Vector3 CenterOfMass;
    private float TargetSteering;
    [Header("Wheels Setup")]
    public WheelCollider WheelFL;
    public WheelCollider WheelFR;
    public WheelCollider WheelRL;
    public WheelCollider WheelRR;
    [Header("Breake SetUp")]
    public bool isBreaking;
    public Material BackLight;

    private Vector3 startPosition , speedVec;
    [Header("Speed in Km/Hr")]
    public int speed;
    [Header("Sensor")]
    public float SensorLength;
    public Vector3 FrontSensorPos;
    public float SideSensorPos;
    public float FrontSensorAngle;
    public bool Avoiding; 

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = CenterOfMass;
        CurrentWaypoints = 0;
        MaxSteerAngle = 45f;
        MaxTorque =500;
        MaxBreakingTorque = 500f;
        MaxSpeed = 1.35f;
        startPosition=transform.position;
        TurnSpeed = 20f;
        isBreaking = false;
        FrontSensorPos =new Vector3(0,.75f,2);
        SensorLength = 5f;
        SideSensorPos = .3f;
        FrontSensorAngle = 30;
        Avoiding = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ApplySteer();
        Drive();
        CheckWaypointDistance();
        LerpToSteering();
        Breaking();
        Sensor();
        speedVec=((transform.position-startPosition)/Time.deltaTime);
        speed = (int)(speedVec.magnitude*3.6f);
        startPosition = transform.position;
    }
    void ApplySteer()
    {
        if (Avoiding) return;
        Vector3 RelativeVector = transform.InverseTransformPoint(Waypoints[CurrentWaypoints].position);
        float NewSteer = (RelativeVector.x / RelativeVector.magnitude)*MaxSteerAngle;
        TargetSteering = NewSteer;
        //WheelFL.steerAngle = NewSteer;
        //WheelFR.steerAngle = NewSteer;
    }
    void Drive()
    {
        MaxTorque = Random.Range(500, 1000);
        CurrentSpeed = 2 * Mathf.PI * WheelFL.radius * WheelFL.rpm * 60 / 1000;
        if (CurrentSpeed <= MaxSpeed &&!isBreaking)
        {
            WheelFL.motorTorque = MaxTorque;
            WheelFR.motorTorque = MaxTorque;
        }
        else
        {
            Debug.Log("Car is at MaxSpeed");
            WheelFL.motorTorque = 0;
            WheelFR.motorTorque = 0;
        }
    }
    void CheckWaypointDistance()
    {
        if (Vector3.Distance(transform.position, Waypoints[CurrentWaypoints].position) <.5f)
        {
            if (CurrentWaypoints != Waypoints.Length-1)
            {
                CurrentWaypoints++;
            }
            else
            {
                CurrentWaypoints=0;
            }
        }
    }
    void Breaking()
    {
        if (isBreaking)
        {
            BackLight.EnableKeyword("_EMISSION");
            WheelRL.brakeTorque = MaxBreakingTorque;
            WheelRR.brakeTorque = MaxBreakingTorque;
        }

        else
        {
            BackLight.DisableKeyword("_EMISSION");
            WheelRL.brakeTorque = 0;
            WheelRR.brakeTorque = 0;
        }
    }
    void LerpToSteering()
    {
        WheelFL.steerAngle = Mathf.Lerp(WheelFL.steerAngle, TargetSteering, Time.deltaTime * TurnSpeed);
        WheelFR.steerAngle = Mathf.Lerp(WheelFR.steerAngle, TargetSteering, Time.deltaTime * TurnSpeed);
    }
    void Sensor()
    {
        RaycastHit hit;
        Vector3 SensorStartingPos=transform.position;
        SensorStartingPos += transform.forward * FrontSensorPos.z;
        SensorStartingPos += transform.up * FrontSensorPos.y;
        float AvoidingMultiplier=0;
        Avoiding = false;

       
       
        //front Right Sensor Position
        SensorStartingPos += transform.right*SideSensorPos;
        if (Physics.Raycast(SensorStartingPos, transform.forward, out hit, SensorLength))
        {
            if (!hit.collider.CompareTag("Road"))
            {
                Debug.DrawLine(SensorStartingPos, hit.point);
                Avoiding = true;
                AvoidingMultiplier = -1;
            }
        }
        
        //front Right Angled Sensor Position
       else if (Physics.Raycast(SensorStartingPos, Quaternion.AngleAxis(FrontSensorAngle,transform.up)*transform.forward, out hit, SensorLength))
        {
            if (!hit.collider.CompareTag("Road"))
            {
                Debug.DrawLine(SensorStartingPos, hit.point);
                Avoiding = true;
                AvoidingMultiplier = -.5f;
            }
        }
       
        //front Left Sensor Position
        SensorStartingPos -=2*transform.right *SideSensorPos;
        if (Physics.Raycast(SensorStartingPos, transform.forward, out hit, SensorLength))
        {
            if (!hit.collider.CompareTag("Road"))
            {
                Debug.DrawLine(SensorStartingPos, hit.point);
                Avoiding = true;
                AvoidingMultiplier = +1;
            }
        }
        
        //front Left Angled Sensor Position
       else if (Physics.Raycast(SensorStartingPos, Quaternion.AngleAxis(-FrontSensorAngle, transform.up) * transform.forward, out hit, SensorLength))
       {
            if (!hit.collider.CompareTag("Road"))
            {
                Debug.DrawLine(SensorStartingPos, hit.point);
                Avoiding = true;
                AvoidingMultiplier = +.5f;
            }
       }
        //front center Sensor position
        if(AvoidingMultiplier==0)
        {
            if (Physics.Raycast(SensorStartingPos, transform.forward, out hit, SensorLength))
            {
                if (!hit.collider.CompareTag("Road"))
                {
                    Debug.DrawLine(SensorStartingPos, hit.point);
                    Avoiding = true;
                }
            }
            if(hit.normal.x<0)
            {
                AvoidingMultiplier = -1;
            }
            else
            {
                AvoidingMultiplier = 1;
            }
        }
        if (Avoiding)
        {
            WheelFL.steerAngle = MaxSteerAngle*AvoidingMultiplier;
            WheelFR.steerAngle = MaxSteerAngle*AvoidingMultiplier;
        }
    }
}
