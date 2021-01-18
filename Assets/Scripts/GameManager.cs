using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using IngameDebugConsole;
using NaughtyAttributes;
using Plugins;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : CacheBehaviour
{
    public static GameManager Instance;
    [BoxGroup("Ease")] public Ease moveEase;
    [BoxGroup("Ease")] public Ease rotationEase;
    [BoxGroup("Ease")] public Ease fallEase;
    [BoxGroup("Ease")] public Ease scaleEase;

    public float stepTime = 1;
    private readonly List<IStepComponent> _stepComponents = new List<IStepComponent>();
    [ReadOnly, NonSerialized] public bool stepHasFinished = true;
    [ReadOnly, NonSerialized] public PlayerController player;
    private MoveDirection _nextMoveDirection;

    [BoxGroup("Oxygen")] [SerializeField] private int oxygenAmount = 100;
    [BoxGroup("Oxygen")] public int oxygenLossPerTurn = 5;
    [BoxGroup("Oxygen")] public RectTransform oxygenBar;
    [BoxGroup("Oxygen")] public TMP_Text oxygenText;
    private Coroutine _oxygenCoroutine;

    public LayerMask groundLayerMask;
    public UnityEvent onDeath = new UnityEvent();
    [ReorderableList] public GameObject[] fallingStopObjects;
    [ReorderableList] public MonoBehaviour[] fallingStopComponents;

    public float fallDistance = 30;
    [NonSerialized] public Queue<UnityAction> StepFinishActions = new Queue<UnityAction>();

    [NonSerialized, ShowNonSerializedField]
    public bool PlayerAlive = true;

    public bool godMode;
    [SerializeField] private LevelManager levelManager;
    public void Awake()
    {
        Instance = this;
        onDeath.AddListener(() =>
        {
            PlayerAlive = false;
            Debug.Log("You Died");
        });
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
            yield return new WaitForSeconds(1);
            if (godMode) continue;
            oxygenAmount -= oxygenLossPerTurn;
            UpdateOxygenDisplay();
            if (oxygenAmount > 0) continue;
            player.DoSuffocate();
            onDeath.Invoke();
            PlayerAlive = false;
            yield return new WaitForSeconds(2);
            ReloadLevel();
            yield break;
        }
    }

    private void DoStep(MoveDirection direction)
    {
        stepHasFinished = false;
        foreach (var stepComponent in _stepComponents)
        {
            stepComponent.Step(direction);
        }
    }

    private void UpdateOxygenDisplay()
    {
        oxygenBar.DOSizeDelta(new Vector2(oxygenBar.sizeDelta.x, oxygenAmount), 1);
        oxygenText.text = $"Oxygen: {oxygenAmount}%";
    }

    public void StepFinished()
    {
        if (StepFinishActions.Count != 0)
        {
            StepFinishActions.Dequeue().Invoke();
            return;
        }

        if (!Physics.Raycast(player.transform.position, Vector3.down, out RaycastHit hit, 2f, groundLayerMask) &&
            !godMode)
        {
            onDeath.Invoke();
            return;
        }

        stepHasFinished = true;
        if (_nextMoveDirection == MoveDirection.None) return;
        DoStep(_nextMoveDirection);
        _nextMoveDirection = MoveDirection.None;
    }

    public void Register(StepComponent stepComponent)
    {
        _stepComponents.Add(stepComponent);
    }

    [ConsoleMethod("reload", "Reloads the Level")]
    public static void ReloadLevel()
    {
        LevelManager.Instance.ReloadLevel();
    }

    public void OnDrawGizmos()
    {
        if (!player) return;
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(player.transform.position, 0.25f);
        Gizmos.color = Color.white;
    }

    [ConsoleMethod("set_o2_loss", "Sets the amount of O2 Lost every second")]
    public static void SetOxygenLossPerRoundValue(int value)
    {
        Instance.oxygenLossPerTurn = value;
    }

    [ConsoleMethod("set_o2", "Sets the current amount of O2")]
    public static void SetOxygen(int value)
    {
        Instance.oxygenAmount = value;
        Instance.UpdateOxygenDisplay();
    }

    [ConsoleMethod("god", "Enables God Mode")]
    public static string GodMode()
    {
        Instance.godMode = !Instance.godMode;
        return Instance.godMode?"God Mode Enabled":"God Mode Disabled";
    }
}