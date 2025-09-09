using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

namespace Projects.Games.Garden
{
    public class GardenEndGameScreen : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI finalScoreText;
        [SerializeField] private TextMeshProUGUI moneyEarnedText;
        [SerializeField] private TextMeshProUGUI congratulationsText;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private GameObject endGamePanel;
        
        [Header("Animation Settings")]
        [SerializeField] private float animationDuration = 1f;
        [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        [Header("Scene Names")]
        [SerializeField] private string mainMenuSceneName = "Main";
        [SerializeField] private string gardenGameSceneName = "GardenGame";
        
        [Header("Audio")]
        [SerializeField] private AudioSource victorySound;
        
        private GardenGameController gameController;
        
        private void Awake()
        {
            gameController = FindFirstObjectByType<GardenGameController>();
        }
        
        private void Start()
        {
            SetupButtons();
            
          
        }
        
        private void SetupButtons()
        {
            if (restartButton != null)
            {
                restartButton.onClick.AddListener(RestartGame);
            }
            
            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.AddListener(GoToMainMenu);
            }
        }
        
        public void ShowResults(int finalScore, int moneyEarned)
        {
            gameObject.SetActive(true);
            StartCoroutine(ShowResultsAnimated(finalScore, moneyEarned));
        }
        
        private IEnumerator ShowResultsAnimated(int finalScore, int moneyEarned)
        {
            // Активируем панель
            // if (endGamePanel != null)
            //     endGamePanel.SetActive(true);
            Debug.Log(gameObject.activeSelf);
            // Проигрываем звук победы
            if (victorySound != null)
                victorySound.Play();
            
            // Анимация появления панели
            yield return StartCoroutine(AnimatePanel());
            
            // Обновляем текст с результатами
            UpdateResultsText(finalScore, moneyEarned);
            
            // Анимация счетчика очков
            yield return StartCoroutine(AnimateScore(finalScore));
            
            // Анимация счетчика денег
            yield return StartCoroutine(AnimateMoney(moneyEarned));
        }
        
        private IEnumerator AnimatePanel()
        {
            if (endGamePanel == null) yield break;
            Debug.Log(gameObject.activeSelf);
            Transform panelTransform = endGamePanel.transform;
            Vector3 originalScale = panelTransform.localScale;
            float elapsed = 0f;
            
            // Начинаем с нулевого масштаба
            panelTransform.localScale = Vector3.zero;
            
            while (elapsed < animationDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / animationDuration;
                
                float scaleMultiplier = scaleCurve.Evaluate(progress);
                panelTransform.localScale = originalScale * scaleMultiplier;
                
                yield return null;
            }
            
            panelTransform.localScale = originalScale;
            Debug.Log(gameObject.activeSelf);
        }
        
        private void UpdateResultsText(int finalScore, int moneyEarned)
        {
            // Обновляем текст поздравлений
            if (congratulationsText != null)
            {
                string message = GetCongratulationsMessage(finalScore);
                congratulationsText.text = message;
            }
            
            // Изначально скрываем числовые значения
            if (finalScoreText != null)
                finalScoreText.text = "Score: 0";
                
            if (moneyEarnedText != null)
                moneyEarnedText.text = "Earned: 0 <sprite index=2>";
        }
        
        private IEnumerator AnimateScore(int targetScore)
        {
            if (finalScoreText == null) yield break;
            
            float duration = 1f;
            float elapsed = 0f;
            int currentScore = 0;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;
                
                currentScore = Mathf.RoundToInt(Mathf.Lerp(0, targetScore, progress));
                finalScoreText.text = $"Score: {currentScore}";
                
                yield return null;
            }
            
            finalScoreText.text = $"Score: {targetScore}";
        }
        
        private IEnumerator AnimateMoney(int targetMoney)
        {
            if (moneyEarnedText == null) yield break;
            
            float duration = 1f;
            float elapsed = 0f;
            int currentMoney = 0;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;
                
                currentMoney = Mathf.RoundToInt(Mathf.Lerp(0, targetMoney, progress));
                moneyEarnedText.text = $"Earned: {currentMoney} <sprite index=2>";
                
                yield return null;
            }
            
            moneyEarnedText.text = $"Earned: {targetMoney} <sprite index=2>";
        }
        
        private string GetCongratulationsMessage(int score)
        {
            if (score >= 500)
                return "🌟 Невероятно! Ты садовод-мастер! 🌟";
            else if (score >= 300)
                return "🎉 Отличная работа! 🎉";
            else if (score >= 150)
                return "👍 Хороший результат! 👍";
            else if (score >= 50)
                return "🌱 Неплохо для начала! 🌱";
            else
                return "🌿 Тренируйся больше! 🌿";
        }
        
        private void RestartGame()
        {
            if (gameController != null)
            {
                // Перезапускаем текущую игру
                gameController.RestartGame();
            }
            else
            {
                // Перезагружаем сцену игры
                SceneManager.LoadScene(gardenGameSceneName);
            }
        }
        
        private void GoToMainMenu()
        {
            // Проверяем, есть ли главная сцена
            if (Application.CanStreamedLevelBeLoaded(mainMenuSceneName))
            {
                SceneManager.LoadScene(mainMenuSceneName);
            }
            else
            {
                // Если главная сцена не найдена, загружаем первую сцену
                SceneManager.LoadScene(1);
            }
        }
        
        // Публичные методы для вызова из UI
        public void OnRestartButtonClicked()
        {
            RestartGame();
        }
        
        public void OnMainMenuButtonClicked()
        {
            GoToMainMenu();
        }
        
        private void OnDestroy()
        {
            if (restartButton != null)
                restartButton.onClick.RemoveAllListeners();
                
            if (mainMenuButton != null)
                mainMenuButton.onClick.RemoveAllListeners();
        }
    }
}
