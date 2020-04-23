using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{

    public Transform target;

    public float smoothSpeed = 0.125f;
    public static Vector3 offset;

    private Vector3 desiredPosition;
    private Vector3 smoothedPosition;

    void Start()
    {
        offset = new Vector3(0, 10, 0);
    }

    void Update()
    {
        if (target)
        {
            desiredPosition = target.position + offset;
            smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        }

        if(!target)
        {
            GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");

            if(enemys.Length > 0)
                target = enemys[Random.Range(0,enemys.Length)].transform;
        }

        transform.position = smoothedPosition;
    }
}
