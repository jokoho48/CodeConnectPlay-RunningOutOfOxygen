using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OnPlay()
    {
        PlayerPrefs.SetInt("Demo", 0);
        SceneManager.LoadScene("Level_00");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F9))
        {
            OnPlayDemo();
        }
    }

    public void OnPlayDemo()
    {
        PlayerPrefs.SetInt("Demo", 1);
        SceneManager.LoadScene("Level_00");
    }
    public void OnExit()
    {
        // save any game data here
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }
}
