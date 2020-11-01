using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScene : MonoBehaviour
{
    [ReorderableList, SerializeField] private KeyCode[] codes;
    // Update is called once per frame
    void Update()
    {
        if (!codes.Any(Input.GetKeyDown)) return;
        SceneManager.LoadScene("MainMenu");
    }
}
