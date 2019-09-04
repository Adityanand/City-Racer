using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    [Header ("Wheels Setup")]
    public WheelCollider FL;
    public WheelCollider FR;
    public WheelCollider RL;
    public WheelCollider RR;

    [Header ("Car Spec")]
    public float MaxTorque;
    public float NitroTorque;
    public float MaxBrake;
    public float MaxSteering;
    public float CurrentSpeed;
    public float MaxSpeed;
    [Header ("Visual Effects")]
    public Material BackLight;
    public ParticleSystem[] Nitrox;

    private Vector3 startPosition, speedVec;
    [Header("Speed in Km/Hr")]
    public int speed;
    // Start is called before the first frame update
    void Start()
    {
        MaxBrake = 1500f;
        MaxTorque = 750f;
        MaxSpeed = 1.35f;
        NitroTorque = 500f;
        MaxSteering = 45f;
        startPosition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
      CurrentSpeed = 2 * Mathf.PI * FL.radius * FL.rpm * 60 / 1000;
        float translations = Input.GetAxis("Vertical") * MaxTorque;
        float Rotations = Input.GetAxis("Horizontal") * MaxSteering;
        var Brake = Input.GetKey(KeyCode.Space);
        var Nitro = Input.GetKey(KeyCode.LeftShift);

        if (!Brake && !Nitro && CurrentSpeed<=MaxSpeed)
        {
            RL.motorTorque = translations;
            RR.motorTorque = translations;
            RL.brakeTorque = 0;
            RR.brakeTorque = 0;
            BackLight.DisableKeyword("_EMISSION");
            foreach (var Nitros in Nitrox)
                Nitros.Stop();
        }

       if(Nitro && CurrentSpeed<=MaxSpeed)
        {
            RL.motorTorque = translations+NitroTorque;
            RR.motorTorque = translations+NitroTorque;
            foreach (var Nitros in Nitrox)
                Nitros.Play();
        }

        if(CurrentSpeed>=MaxSpeed)
        {
            RL.motorTorque = 0;
            RR.motorTorque = 0;
        }

        FL.steerAngle = Rotations;
        FR.steerAngle = Rotations;
        
        if(Brake)
        {
            BackLight.EnableKeyword("_EMISSION");
            RL.brakeTorque = MaxBrake;
            RR.brakeTorque = MaxBrake;
            RR.motorTorque = 0;
            RL.motorTorque = 0;
        }

        speedVec = ((transform.position - startPosition) / Time.deltaTime);
        speed = (int)(speedVec.magnitude * 3.6f);
        startPosition = transform.position;


    }
}
