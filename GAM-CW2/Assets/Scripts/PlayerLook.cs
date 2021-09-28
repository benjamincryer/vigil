using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public Camera cam;
    private Vector3 camPos;

    public float Sensitivity = 3.0f;
    public float TiltAmount = 3.0f;
    public float TiltSpeed = 1.0f;
    public float Clamp = 30f;

    public float ShakeAmount = 0f;
    public float ShakeDecay = 0f;

    public float effectiveSens;

    public Vector2 rotation;
    private float tilt = 0.0f;

    void Start()
    {
        effectiveSens = Sensitivity;
        camPos = cam.transform.localPosition;
    }

    void Update()
    {
        //Mouse Look
        rotation.y += Input.GetAxis("Mouse X") * effectiveSens;
        rotation.x -= Input.GetAxis("Mouse Y") * effectiveSens;
        rotation.x = Mathf.Clamp(rotation.x, -Clamp, Clamp);
        
        //Lock y-rotation between 0-360 degrees
        if(rotation.y > 360f)
        {
            rotation.y = rotation.y - 360f;
        }

        if(rotation.y < 0f)
        {
            rotation.y = 360f - rotation.y;
        }

        //Rotate entire character horizontally when turning
        transform.eulerAngles = new Vector2(0, rotation.y);

        //Rotate camera vertically when looking up or down, and tilt in direction of movement
        tilt = Mathf.Lerp(tilt, TiltAmount * -Input.GetAxis("Horizontal"), TiltSpeed * Time.deltaTime);
        cam.transform.localRotation = Quaternion.Euler(rotation.x, 0, tilt);

        //Apply any screen shake
        if (ShakeAmount > 0f)
        {
            cam.transform.localPosition = camPos + Random.insideUnitSphere * ShakeAmount;
            ShakeAmount -= ShakeDecay * Time.deltaTime;
        }
        else
        {
            ShakeAmount = 0f;
        }
        

        if (Input.GetKeyDown("escape"))
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void ShakeScreen(float amount, float time)
    {
        ShakeAmount = amount;

        //To reach 0 in t seconds, the decay rate must be:
        ShakeDecay = amount / time;
    }

}
