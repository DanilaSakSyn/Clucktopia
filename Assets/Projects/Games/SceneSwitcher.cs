using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Projects.Games
{
    public class SceneSwitcher : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Button switchSceneButton;

        [Header("Scene Settings")]
        [SerializeField] private string targetSceneName;

        private void Start()
        {
            if (switchSceneButton != null)
            {
                switchSceneButton.onClick.AddListener(SwitchScene);
            }
        }

        private void SwitchScene()
        {
            if (!string.IsNullOrEmpty(targetSceneName))
            {
                SceneManager.LoadScene(targetSceneName);
            }
            else
            {
                Debug.LogError("Target scene name is not set.");
            }
        }
    }
}
