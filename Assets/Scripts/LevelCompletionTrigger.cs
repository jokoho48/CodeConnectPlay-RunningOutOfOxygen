using Plugins;
using RoboRyanTron.SceneReference;
using UnityEngine;

public class LevelCompletionTrigger : CacheBehaviour
{
    public SceneReference nextScene;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            nextScene.LoadSceneAsync();
        }
    }
    private void OnDrawGizmos()
    {
        DebugExtension.DrawBounds(collider.bounds, Color.green);
    }
}
