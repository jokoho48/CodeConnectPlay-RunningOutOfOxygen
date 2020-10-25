using System.Collections;
using DG.Tweening;
using UnityEngine;

public class PlayerController : StepComponent
{
    private Sequence _sequence;
    private Animator _animator;
    private static readonly int Dead = Animator.StringToHash("Dead");
    private static readonly int Dir = Animator.StringToHash("Dir");
    private static readonly int CantMove = Animator.StringToHash("cantMove");

    protected override void Start()
    {
        base.Start();
        GameManager.Instance.player = this;
        GameManager.Instance.onDeath.AddListener(Die);
        _animator = GetComponentInChildren<Animator>();
    }
    public override void Step(MoveDirection direction)
    {
        base.Step(direction);
        _animator.SetInteger(Dir, direction.ToAnimationNumber());
        Vector3 currentForward = transform.forward;
        Vector3 newForward = direction.ToVector();
        Vector3 rayPosition = transform.position + (Vector3.up / 2);
        _sequence = DOTween.Sequence();

        if (Physics.Raycast(rayPosition, newForward, 0.5f, GameManager.Instance.groundLayerMask))
        {
            _animator.SetTrigger(CantMove);
            return;
        }
        if (currentForward == newForward)
            _sequence.Append(transform.DOMove(transform.position + newForward, GameManager.Instance.stepTime).SetEase(GameManager.Instance.moveEase));
        else
        {
            float runtime = GameManager.Instance.stepTime;
            var position = transform.position;
            _sequence.Append(transform.DOLookAt(position + newForward, runtime).SetEase(GameManager.Instance.rotationEase));
            _sequence.Append(transform.DOMove(position + newForward, runtime).SetEase(GameManager.Instance.moveEase));
        }
        _sequence.Play();
        _sequence.OnComplete(() => GameManager.Instance.StepFinished());
    }

    public void StartFalling()
    {
        transform.DOMoveY(transform.position.y - GameManager.Instance.fallDistance, 3.2f).SetEase(GameManager.Instance.fallEase).OnComplete(GameManager.ReloadLevel);
        foreach (GameObject o in GameManager.Instance.fallingStopObjects)
        {
            o.SetActive(false);
        }
    }

    public void DoSuffocate()
    {
        _animator.SetInteger(Dir, -1);
    }
    private void Die()
    {
        _animator.SetBool(Dead, true);
    }
}
