using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SM_rotateThis : MonoBehaviour
{
    public float rotationSpeedX = 90;
    public float rotationSpeedY = 0;
    public float rotationSpeedZ = 0;

    void Update()
    {
        float delta = Time.deltaTime;
        transform.Rotate(rotationSpeedX * delta, rotationSpeedY * delta, rotationSpeedZ * delta);
    }
}