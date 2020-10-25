using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerInput : MonoBehaviour
{
    public void OnUp(InputAction.CallbackContext ctx)
    {
        if (!ctx.canceled) return;
        GameManager.Instance.ProcessInput(MoveDirection.Up);
    }

    public void OnDown(InputAction.CallbackContext ctx)
    {
        if (!ctx.canceled) return;
        GameManager.Instance.ProcessInput(MoveDirection.Down);
    }

    public void OnLeft(InputAction.CallbackContext ctx)
    {
        if (!ctx.canceled) return;
        GameManager.Instance.ProcessInput(MoveDirection.Left);
    }

    public void OnRight(InputAction.CallbackContext ctx)
    {
        if (!ctx.canceled) return;
        GameManager.Instance.ProcessInput(MoveDirection.Right);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            GameManager.Instance.ProcessInput(MoveDirection.Left);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            GameManager.Instance.ProcessInput(MoveDirection.Right);
        }
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            GameManager.Instance.ProcessInput(MoveDirection.Up);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            GameManager.Instance.ProcessInput(MoveDirection.Down);
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}