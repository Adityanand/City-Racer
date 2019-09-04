using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelRotation : MonoBehaviour
{
    public WheelCollider TargetWheel;
    public Vector3 WheelPos = new Vector3();
    public Quaternion WheelRot = new Quaternion();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TargetWheel.GetWorldPose(out WheelPos,out WheelRot);
        transform.position = WheelPos;
        transform.rotation = WheelRot;
    }
}
