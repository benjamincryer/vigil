using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour, ISaveable
{
    private AudioSource src;

    public void LoadFromSaveData(SaveData a_SaveData)
    {
        if (a_SaveData.m_MusicData.clip != null)
        {
            src.clip = a_SaveData.m_MusicData.clip;
            src.volume = a_SaveData.m_MusicData.volume;
            src.Play();
        }
    }

    public void PopulateSaveData(SaveData a_SaveData)
    {
        if (src.clip != null)
        {
            a_SaveData.m_MusicData.clip = src.clip;
            a_SaveData.m_MusicData.volume = src.volume;
        }
    }

    void Start()
    {
        src = GetComponent<AudioSource>();
    }

}
