using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using RoboRyanTron.SceneReference;

[CreateAssetMenu]
public class LevelManager : ScriptableObject
{
    private static LevelManager _instance;
    public static LevelManager Instance
    {
        get
        {
            if (_instance == null) {
                _instance = Resources.FindObjectsOfTypeAll<LevelManager>()[0];
            }
            return _instance;
        }
    }
    
    [SerializeField] private SceneReference[] levels;

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void NextLevel()
    {
        for (int i = 0; i < levels.Length; i++)
        {
            if (levels[i].SceneName == SceneManager.GetActiveScene().name)
            {
                levels[i+1].LoadScene();
                return;
            }
        }
    }
}