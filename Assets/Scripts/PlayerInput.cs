using System;
using System.Linq;
using IngameDebugConsole;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInput : MonoBehaviour
{
    [ReorderableList] public InputData[] inputData;
    [Serializable]
    public struct InputData
    {
        [ReorderableList] public KeyCode[] keyCodes;
        public MoveDirection moveDirection;
        public bool ProcessInput()
        {
            return keyCodes.Any(Input.GetKeyDown);
        }
    }
    // Update is called once per frame
    private void Update()
    {
        if (DebugLogManager.Instance.IsLogWindowVisible) return;
        foreach (var data in inputData)
        {
            if (!data.ProcessInput()) continue;
            GameManager.Instance.ProcessInput(data.moveDirection);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Escape)) SceneManager.LoadScene("MainMenu");
    }
}