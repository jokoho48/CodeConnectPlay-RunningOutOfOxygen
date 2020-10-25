namespace RoboRyanTron.SceneReference
{
#region

    using System;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.SceneManagement;

#endregion

    /// <summary>
    ///     Class used to serialize a reference to a scene asset that can be used
    ///     at runtime in a build, when the asset can no longer be directly
    ///     referenced. This caches the scene name based on the SceneAsset to use
    ///     at runtime to load.
    /// </summary>
    [Serializable]
    public class SceneReference : ISerializationCallbackReceiver
    {
#if UNITY_EDITOR
        public SceneAsset Scene;
#endif

        [SerializeField] private bool sceneEnabled;

        [SerializeField] private int sceneIndex = -1;

        [Tooltip("The name of the referenced scene. This may be used at runtime to load the scene.")]
        public string SceneName;

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (Scene != null)
            {
                var sceneAssetPath = AssetDatabase.GetAssetPath(Scene);
                var sceneAssetGUID = AssetDatabase.AssetPathToGUID(sceneAssetPath);

                EditorBuildSettingsScene[] scenes =
                    EditorBuildSettings.scenes;

                sceneIndex = -1;
                for (var i = 0; i < scenes.Length; i++)
                    if (scenes[i].guid.ToString() == sceneAssetGUID)
                    {
                        sceneIndex = i;
                        sceneEnabled = scenes[i].enabled;
                        if (scenes[i].enabled)
                            SceneName = Scene.name;
                        break;
                    }
            }
            else
            {
                SceneName = "";
            }
#endif
        }

        public void OnAfterDeserialize()
        {
        }

        private void ValidateScene()
        {
            if (string.IsNullOrEmpty(SceneName))
                throw new SceneLoadException("No scene specified.");

            if (sceneIndex < 0)
                throw new SceneLoadException("Scene " + SceneName + " is not in the build settings");

            if (!sceneEnabled)
                throw new SceneLoadException("Scene " + SceneName + " is not enabled in the build settings");
        }

        public void LoadScene(LoadSceneMode mode = LoadSceneMode.Single)
        {
            ValidateScene();
            SceneManager.LoadScene(SceneName, mode);
        }

        public AsyncOperation LoadSceneAsync(LoadSceneMode mode = LoadSceneMode.Single)
        {
            ValidateScene();
            return SceneManager.LoadSceneAsync(SceneName, mode);
        }

        /// <summary>
        ///     Exception that is raised when there is an issue resolving and
        ///     loading a scene reference.
        /// </summary>
        public class SceneLoadException : Exception
        {
            public SceneLoadException(string message) : base(message)
            {
            }
        }
    }
}