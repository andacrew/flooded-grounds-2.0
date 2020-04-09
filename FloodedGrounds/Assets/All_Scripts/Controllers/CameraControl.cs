using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public enum RotationAxis
    {
        MouseX = 1,
        MouseY = 2
    }

    public RotationAxis axes = RotationAxis.MouseX;

    public float minVert = -45.0f;
    public float maxVert = 45.0f;
    public float sensitivity = 10.0f;

    public float rotX = 0;

    // Update is called once per frame
    void Update()
    {
        if (axes == RotationAxis.MouseX)
        {
            transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivity, 0);
        } else if (axes == RotationAxis.MouseY) {
            rotX -= Input.GetAxis("Mouse Y") * sensitivity;
            rotX = Mathf.Clamp(rotX, minVert, maxVert);

            float rotY = transform.localEulerAngles.y;

            transform.localEulerAngles = new Vector3(rotX, rotY, 0);
        }
    }
}
