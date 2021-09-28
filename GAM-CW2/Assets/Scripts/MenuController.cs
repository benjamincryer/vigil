using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuController : MonoBehaviour
{
    //Get stats from GameManager and set the UI accordingly
    public Button btnStart;
    public Button btnContinue;

    public TextMeshProUGUI txtStats;

    private GameManager gc;

    void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        Cursor.lockState = CursorLockMode.None;

        //Set UI
        if (txtStats != null) txtStats.text = gc.levelStats;
    }

    public void StartGameOnClick()
    {
        SceneManager.LoadScene("Level1");
    }

    public void LoadFromMenuOnClick()
    {
        gc.LoadLevelFromMenu();
    }

    public void NextLevelOnClick()
    {
        //Get id of next level
        int nextScene = gc.levelID + 1;

        SceneManager.LoadScene(nextScene);
    }

    public void LoadCheckpointOnClick()
    {
        int id = gc.levelID;
        gc.LoadLevel(id);
    }

    public void RestartLevelOnClick()
    {
        int id = gc.levelID;
        SceneManager.LoadScene(id);
    }

    public void ExitLevelOnClick()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void ExitGameOnClick()
    {
        Application.Quit();
    }
}
