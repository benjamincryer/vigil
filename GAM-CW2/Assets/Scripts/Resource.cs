using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource
{
    private float Amount = 0f;
    public float MaxAmount = 0f;

    public float Get() { return Amount; }
    public void Set(float value) { Amount = value; }
    public void Add(float value) { Amount += value; }
}
