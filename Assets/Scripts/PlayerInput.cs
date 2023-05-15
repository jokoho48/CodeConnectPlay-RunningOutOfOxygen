using System;
using System.Linq;
using IngameDebugConsole;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class PlayerInput : MonoBehaviour
{
    private Camera _mainCamera;
    [ReorderableList] public InputData[] inputData;


    private void Start()
    {
        if (PlayerPrefs.GetInt("Demo") == 1) Destroy(this);
        _mainCamera = Camera.current;
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

        if (Input.touchSupported && Input.touchCount != 0) return;
        for (int i = 0; i < Input.touchCount; i++)
        {
            var touch = Input.GetTouch(i);
            if (touch.phase != TouchPhase.Began) return;
            var dist = float.MaxValue;
            var dir = MoveDirection.None;
            foreach (var data in inputData)
            {
                var d = Vector3.Distance(data.touchPosition, _mainCamera.ScreenToViewportPoint(touch.position));
                if (dist < d)
                {
                    dir = data.moveDirection;
                    dist = d;
                }
            }
            GameManager.Instance.ProcessInput(dir);
        }
    }
}

[Serializable]
public struct InputData
{
    [ReorderableList] public KeyCode[] keyCodes;
    public MoveDirection moveDirection;
    public Vector2 touchPosition;
    public bool ProcessInput()
    {
        return keyCodes.Any(Input.GetKeyDown);
    }
}