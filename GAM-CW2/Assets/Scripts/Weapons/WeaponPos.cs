using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPos : MonoBehaviour
{
    public float amount = 2f;
    public float maxAmount = 4f;
    public  float smooth = 3f;
    private Quaternion rot;

    public float kickback = 0.0f;

    void Start()
    {
        rot = transform.localRotation;
    }

    void Update()
    {
        //Rotate slightly towards the mouse direction
        float factorX = (Input.GetAxis("Mouse Y")) * amount;
        float factorY = -(Input.GetAxis("Mouse X")) * amount;
        float factorZ = -Input.GetAxis("Vertical") * amount;

        factorX = Mathf.Clamp(factorX, -maxAmount, maxAmount);
        factorY = Mathf.Clamp(factorY, -maxAmount, maxAmount);
        factorZ = Mathf.Clamp(factorZ, -maxAmount, maxAmount);

        Quaternion dest = Quaternion.Euler(rot.x + factorX, rot.y + factorY, rot.z + factorZ);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, dest, smooth * Time.deltaTime);

        //Add recoil if necessary, slowly reduce recoil to 0
        if (kickback != 0f) kickback = Mathf.Lerp(kickback, 0f, 10f * Time.deltaTime);
        Quaternion recoilAngle = Quaternion.Euler(-kickback, 0, 0);
        transform.localRotation = transform.localRotation * recoilAngle;
    }
}
