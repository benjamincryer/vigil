using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    [System.Serializable]
    public struct PlayerData
    {
        public Vector3 Pos;
        public float Health, Armour;
    }

    [System.Serializable]
    public struct LevelData
    {
        public int LevelID;
        public int Kills, Deaths;
        public float Time;
    }

    [System.Serializable]
    public struct WeaponData
    {
        public string id;
        public bool Unlocked;
        public int Ammo;
    }

    [System.Serializable]
    public struct EnemySpawnerData
    {
        public string id;
        public bool isTriggered;
    }

    [System.Serializable]
    public struct EnemyData
    {
        public string id;
        public Vector3 Pos;
        public float Health;
    }

    [System.Serializable]
    public struct PickupData
    {
        public string id;
        public bool isTriggered;
    }

    [System.Serializable]
    public struct MusicData
    {
        public AudioClip clip;
        public float volume;
    }

    public PlayerData m_PlayerData;
    public LevelData m_LevelData;
    public MusicData m_MusicData;
    public List<WeaponData> m_WeaponData = new List<WeaponData>();

    public List<PickupData> m_PickupData = new List<PickupData>();

    public List<EnemySpawnerData> m_EnemySpawnerData = new List<EnemySpawnerData>();
    public List<EnemyData> m_EnemyData = new List<EnemyData>();
    

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public void LoadFromJson(string a_Json)
    {
        JsonUtility.FromJsonOverwrite(a_Json, this);
    }
}

public interface ISaveable
{
    void PopulateSaveData(SaveData a_SaveData);
    void LoadFromSaveData(SaveData a_SaveData);
}