using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour, ISaveable
{
    public float HealthMax = 200f;
    public float ArmourMax = 100f;

    public float Health = 100f;
    public float Armour = 50f;

    public TextMeshProUGUI txtHealth, txtArmour;

    void Start()
    {
        UpdateText();
    }

    public void AddHealth(float amount)
    {
        Health = Mathf.Clamp(Health + amount, 0f, HealthMax);
        UpdateText();
    }

    public void AddArmour(float amount)
    {
        Armour = Mathf.Clamp(Armour + amount, 0f, ArmourMax);
        UpdateText();
    }

    public void TakeDamage(float damage)
    {
        float shake = Mathf.Clamp(damage/100, 0.1f, 1f);
        GetComponent<PlayerLook>().ShakeScreen(shake, 0.2f);

        float armourDmg = 0;
        float healthDmg = damage;

        //Armour absorbs 50% of damage if present
        if (Armour > 0)
        {
            armourDmg = damage / 2;
            healthDmg = damage / 2;

            //Don't reduce armour below 0, any excess damage is transferred straight to hp;
            if (armourDmg > Armour)
            {
                float excess = armourDmg - Armour;
                healthDmg += excess;
                armourDmg -= excess;
            }

        }

        //Receive damage
        Armour -= armourDmg;
        Health -= healthDmg;

        //Restart once dead
        if (Health <= 0f)
        {
            Die();
        }

        //Update UI
        UpdateText();
    }

    public void UpdateText()
    {
        //Update text
        txtHealth.text = "HEALTH " + (int)Mathf.Ceil(Health);
        txtArmour.text = "ARMOUR " + (int)Mathf.Ceil(Armour);
    }

    void Update()
    {
        
    }

    public void Die()
    {
        //Go to death scene
        SceneManager.LoadScene("DeathScene");

        /*
        //Reload checkpoint
        GameObject gc = GameObject.FindGameObjectWithTag("GameController");

        if (gc != null)
        {
            gc.GetComponent<GameManager>().LoadGame();
        }
        else
        {
            Restart();
        }
        */
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadFromSaveData(SaveData a_SaveData)
    {
        transform.position = a_SaveData.m_PlayerData.Pos;
        Health = a_SaveData.m_PlayerData.Health;
        Armour = a_SaveData.m_PlayerData.Armour;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        UpdateText();
    }

    public void PopulateSaveData(SaveData a_SaveData)
    {
        a_SaveData.m_PlayerData.Pos = transform.position;
        a_SaveData.m_PlayerData.Health = Health;
        a_SaveData.m_PlayerData.Armour = Armour;
    }

}
