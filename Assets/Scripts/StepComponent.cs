using Plugins;
using UnityEngine;
public enum MoveDirection
{
    None,
    Up,
    Down,
    Left,
    Right
}
public abstract class StepComponent : CacheBehaviour, IStepComponent
{ 
    protected virtual void Start()
    {
        Register();
    }
    private void Register()
    {
        GameManager.Instance.Register(this);
    }
    public virtual void Step(MoveDirection direction) { }
}
public interface IStepComponent
{
    void Step(MoveDirection direction); 
}

public static class UnityExtension
{
    public static Vector3 ToVector(this MoveDirection direction)
    {
        switch (direction)
        {
            case MoveDirection.Up:
                return Vector3.forward;
            case MoveDirection.Down:
                return Vector3.back;
            case MoveDirection.Left:
                return Vector3.left;
            case MoveDirection.Right:
                return Vector3.right;
            case MoveDirection.None:
                return Vector3.zero;
            default:
                return Vector3.zero;
        }
    }
    public static int ToAnimationNumber(this MoveDirection direction)
    {
        switch (direction)
        {
            case MoveDirection.Up:
                return 0;
            case MoveDirection.Down:
                return 2;
            case MoveDirection.Left:
                return 3;
            case MoveDirection.Right:
                return 1;
            default:
                return 0;
        }
    }
} 