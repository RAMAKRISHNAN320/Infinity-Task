using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{


    public float smoothSpeed = 20f;

    Vector3 direction;
    float distance;
    float minDst;
    float height;

    public float Distance
    {
        get
        {
            return distance;
        }
        set
        {
            distance = value;
            //Clamp max distance
            //camera was going too far
            distance = Mathf.Clamp(distance, minDst, 30f);
        }
    
    }
    public float Height
    {
        get
        {
            return height;
        }
        set
        {
            height = value;
        }
    }

    //Direction is Local Position because camera is a child the follow target
    private void Start()
    {
        direction =   transform.localPosition;
        distance = direction.magnitude;
        minDst = distance;
    }

    //Smooth update
    private void Update()
    {
        Vector3 desiredPosition = direction.normalized * distance + new Vector3(-height, 0, 0);
        Vector3 smoothedPosition = Vector3.Lerp(transform.localPosition, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.localPosition = smoothedPosition;
    }
}
