using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveTrigger : MonoBehaviour, ISaveable
{
    public Uuid id;

    public bool TriggerOnCollision = true;
    public bool TriggerOnShoot = false;
    public bool isTriggered = false;
    public bool DestroyThis = true;

    public GameObject EnemyList;
    private Object spawnPrefab;

    void Start()
    {
        spawnPrefab = Resources.Load("Prefabs/SpawnEffect");
    }

    private void EnableSpawns()
    {
        //Enable all enemies under child object
        foreach (Transform child in EnemyList.transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    private void DisableSpawns()
    {
        foreach (Transform child in EnemyList.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void PopulateSaveData(SaveData a_SaveData)
    {
        SaveData.EnemySpawnerData spawnData = new SaveData.EnemySpawnerData();
        spawnData.id = id.uuid;
        spawnData.isTriggered = isTriggered;
        a_SaveData.m_EnemySpawnerData.Add(spawnData);

        //Save each enemy's stats to the list
        foreach (Transform child in EnemyList.transform)
        {
            SaveData.EnemyData enemyData = new SaveData.EnemyData();

            Enemy e = child.gameObject.GetComponent<Enemy>();
            enemyData.id = e.id.uuid;
            enemyData.Health = e.Health;
            enemyData.Pos = e.transform.position;

            a_SaveData.m_EnemyData.Add(enemyData);
        }
    }

    public void LoadFromSaveData(SaveData a_SaveData)
    {
        foreach (SaveData.EnemySpawnerData spawnData in a_SaveData.m_EnemySpawnerData)
        {
            if (spawnData.id == id.uuid)
            {
                isTriggered = spawnData.isTriggered;

                //Spawn or despawn enemies
                if (isTriggered)
                {
                    EnableSpawns();
                }
                else
                {
                    DisableSpawns();
                }

                //Load all associated enemy save data
                foreach (Transform child in EnemyList.transform)
                {
                    Enemy e = child.gameObject.GetComponent<Enemy>();

                    foreach (SaveData.EnemyData enemyData in a_SaveData.m_EnemyData)
                    {
                        //Find enemyData entry with this enemy's id:
                        if (enemyData.id == e.id.uuid)
                        {
                            //Update stats and position
                            e.enabled = true;
                            e.transform.position = enemyData.Pos;
                            e.Health = enemyData.Health;

                            //Make dead or make alive
                            if (e.Health <= 0f)
                                e.Dead();
                            //else e.Alive();

                            break;
                        }
                    }
                }

                break;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;

        if (other.tag.Equals("Player"))
        {
            foreach (Transform child in EnemyList.transform)
            {
                //Spawn effect
                GameObject impact = Instantiate(spawnPrefab, child.position, Quaternion.identity) as GameObject;
                impact.transform.parent = child;
                Destroy(impact, 1f);
            }

            EnableSpawns();

            isTriggered = true;
        }
    }
}
