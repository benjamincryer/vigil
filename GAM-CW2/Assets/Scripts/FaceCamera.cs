using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    void Update()
    {
        //Look at camera
        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180, 0);
    }
}
