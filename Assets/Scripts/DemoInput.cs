using System;
using System.Collections.Generic;
using System.Linq;
using IngameDebugConsole;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoInput : MonoBehaviour
{
    [ReorderableList] public MoveDirection[] inputData;

    private Queue<MoveDirection> dir = new Queue<MoveDirection>();

    private void Start()
    {
        if (PlayerPrefs.GetInt("Demo") == 0) Destroy(this);
        foreach (var data in inputData)
        {
            dir.Enqueue(data);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (DebugLogManager.Instance.IsLogWindowVisible) return;
        if (Input.GetKeyDown(KeyCode.Escape)) SceneManager.LoadScene("MainMenu");
        if (GameManager.Instance.stepHasFinished && dir.Any()) 
            GameManager.Instance.ProcessInput(dir.Dequeue());
    }
}