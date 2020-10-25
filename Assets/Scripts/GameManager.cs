using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using DG.Tweening;
using NaughtyAttributes;
using Plugins;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : CacheBehaviour
{
    public static GameManager Instance;
    public float stepTime = 1;
    private readonly List<IStepComponent> _stepComponents = new List<IStepComponent>();
    [ReadOnly] public bool stepHasFinished = true;
    [ReadOnly] public PlayerController player;
    public Ease moveEase;
    public Ease rotationEase;
    public Ease fallEase;
    private MoveDirection _nextMoveDirection;
    public int oxygenAmount = 100;
    public int oxygenLossPerTurn = 5;
    private Coroutine _oxygenCoroutine;
    public UnityEvent onDeath = new UnityEvent();
    public TMP_Text oxygenText;
    public LayerMask groundLayerMask;
    public float fallDistance = 30;
    public GameObject[] fallingStopObjects;
    public RectTransform oxygenBar;
    [NonSerialized, ShowNonSerializedField] public bool PlayerAlive = true;
    public void Awake()
    {
        Instance = this;
        onDeath.AddListener(() =>
        {
            PlayerAlive = false;
            Debug.Log("You Died");
        });
        // onDeath.AddListener(ReloadLevel);
    }

    public void ProcessInput(MoveDirection direction)
    {
        if (!PlayerAlive) return;
        if (!stepHasFinished)
        {
            _nextMoveDirection = direction;
            return;
        }

        if (_oxygenCoroutine == null)
        {
            _oxygenCoroutine = StartCoroutine(OxygenCoroutine());
        }
        
        DoStep(direction);
    }

    private IEnumerator OxygenCoroutine()
    {
        while (true)
        {
            oxygenAmount -= oxygenLossPerTurn;
            UpdateOxygenDisplay();
            if (oxygenAmount <= 0)
            {
                player.DoSuffocate();
                onDeath.Invoke();
                PlayerAlive = false;
                yield return new WaitForSeconds(2);
                ReloadLevel();
                yield break;
            }
            yield return new WaitForSeconds(1);
        }
    }

    private void DoStep(MoveDirection direction)
    {
        stepHasFinished = false;
        // oxygenAmount -= oxygenLossPerTurn;
        foreach (var stepComponent in _stepComponents)
        {
            stepComponent.Step(direction);
        }
    }

    public void UpdateOxygenDisplay()
    {
        oxygenBar.DOSizeDelta(new Vector2(oxygenBar.sizeDelta.x, oxygenAmount), 1);
        oxygenText.text = $"Oxygen: {oxygenAmount}%";
    }
    
    public void StepFinished()
    {
        if (!Physics.Raycast(player.transform.position, Vector3.down, out RaycastHit hit, 2f, groundLayerMask))
        {
            onDeath.Invoke();
            return;
        }
        
        stepHasFinished = true;
        if (_nextMoveDirection != MoveDirection.None)
        {
            DoStep(_nextMoveDirection);
            _nextMoveDirection = MoveDirection.None;
        }
    }
    public void Register(StepComponent stepComponent)
    {
        _stepComponents.Add(stepComponent);
    }

    public static void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnDrawGizmos()
    {
        if (player)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(player.transform.position, 0.25f);
            Gizmos.color = Color.white;            
        }
    }
}