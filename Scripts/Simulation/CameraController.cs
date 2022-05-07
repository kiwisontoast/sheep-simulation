using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 20f;
    public float rotateSpeed = 40f;


    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 pos = transform.position;
        if (Input.GetKey(KeyCode.W))
        {
            pos += transform.forward * panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            pos -= transform.forward * panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            pos += transform.right * panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            pos -= transform.right * panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.E))
        {
            pos += transform.up * panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            pos -= transform.up * panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Rotate(-rotateSpeed*Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Rotate(rotateSpeed * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(0, -rotateSpeed * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.X))
        {
            transform.rotation = new Quaternion(0, transform.rotation.y,0,transform.rotation.w);
        }

        transform.position = pos;
    }
}
