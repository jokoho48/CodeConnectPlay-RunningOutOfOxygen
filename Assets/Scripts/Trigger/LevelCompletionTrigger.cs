using RoboRyanTron.SceneReference;

public class LevelCompletionTrigger : Triggerable
{    public override void DoTrigger()
    {
        LevelManager.Instance.NextLevel();
    }
}