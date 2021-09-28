using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    public string Name;
    public int Amount;
    public int MaxCapacity;

    public void AddAmmo(int amount)
    {
        Amount += amount;
        if (Amount > MaxCapacity) Amount = MaxCapacity;
    }

    public void UseAmmo(int amount)
    {
        //If ammo not infinite:
        if (MaxCapacity != 0)
        {
            //Use ammo
            Amount -= amount;
            if (Amount < 0) Amount = 0;
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
