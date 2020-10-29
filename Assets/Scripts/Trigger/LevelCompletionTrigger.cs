using RoboRyanTron.SceneReference;

public class LevelCompletionTrigger : Triggerable
{
    public SceneReference nextScene;
    public override void DoTrigger()
    {
        nextScene.LoadSceneAsync();
    }
}