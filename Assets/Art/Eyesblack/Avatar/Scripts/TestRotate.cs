using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRotate : MonoBehaviour 
{
    public float m_speed = 50;
	
	// Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))        //旋转
        {
            float value = -Input.GetAxis("Mouse X") * m_speed * Time.deltaTime;
            transform.Rotate(Vector3.up, value);
        }
    }
}
