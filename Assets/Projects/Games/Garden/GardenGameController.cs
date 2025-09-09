using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;

namespace Projects.Games.Garden
{
    public class GardenGameController : MonoBehaviour
    {
        [Header("Game Settings")]
        [SerializeField] private float gameDuration = 30f;
        [SerializeField] private int pointsPerClick = 10;
        [SerializeField] private float spawnRate = 1f;
        [SerializeField] private int maxButtons = 5;
        [SerializeField] private float buttonLifetime = 3f;
        
        [Header("Spawn Area")]
        [SerializeField] private RectTransform spawnArea;
        [SerializeField] private float buttonSpacing = 100f;
        
        [Header("UI References")]
        [SerializeField] private GameObject clickableButtonPrefab;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private GameObject gameUI;
        [SerializeField] private GameObject endGameScreen;
        
        [Header("Rewards")]
        [SerializeField] private int baseMoneyReward = 50;
        [SerializeField] private float moneyPerPoint = 0.5f;
        
        // Game State
        private int currentScore = 0;
        private float timeRemaining;
        private bool isGameActive = false;
        private List<ClickableButton> activeButtons = new List<ClickableButton>();
        private Coroutine spawnCoroutine;
        private PlayerCurrency playerCurrency;
        
        // Events
        public static event Action<int, int> OnGameEnded; // score, money earned
        public static event Action<int> OnScoreChanged;
        
        private void Awake()
        {
            playerCurrency = FindFirstObjectByType<PlayerCurrency>();
        }
        
        private void Start()
        {
            InitializeGame();
        }
        
        private void Update()
        {
            if (isGameActive)
            {
                UpdateTimer();
            }
        }
        
        public void StartGame()
        {
            isGameActive = true;
            currentScore = 0;
            timeRemaining = gameDuration;
            
            // Очищаем старые кнопки
            ClearAllButtons();
            
            // Показываем игровой UI
            if (gameUI != null) gameUI.SetActive(true);
            if (endGameScreen != null) endGameScreen.SetActive(false);
            
            // Начинаем спавн кнопок
            spawnCoroutine = StartCoroutine(SpawnButtons());
            
            UpdateUI();
        }
        
        private void InitializeGame()
        {
            if (endGameScreen != null) endGameScreen.SetActive(false);
            if (gameUI != null) gameUI.SetActive(true);
            
            UpdateUI();
        }
        
        private void UpdateTimer()
        {
            timeRemaining -= Time.deltaTime;
            
            if (timeRemaining <= 0)
            {
                EndGame();
            }
            
            UpdateUI();
        }
        
        private void UpdateUI()
        {
            if (scoreText != null)
            {
                scoreText.text = $"Score: {currentScore}";
            }
            
            if (timerText != null)
            {
                int seconds = Mathf.CeilToInt(timeRemaining);
                timerText.text = $"Time: {seconds}с";
            }
        }
        
        private IEnumerator SpawnButtons()
        {
            while (isGameActive && timeRemaining > 0)
            {
                if (activeButtons.Count < maxButtons)
                {
                    SpawnButton();
                }
                
                yield return new WaitForSeconds(1f / spawnRate);
            }
        }
        
        private void SpawnButton()
        {
            if (clickableButtonPrefab == null || spawnArea == null) return;
            
            Vector2 spawnPosition = GetRandomSpawnPosition();
            
            GameObject buttonObj = Instantiate(clickableButtonPrefab, spawnArea);
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            
            if (buttonRect != null)
            {
                buttonRect.anchoredPosition = spawnPosition;
            }
            
            ClickableButton clickableButton = buttonObj.GetComponent<ClickableButton>();
            if (clickableButton != null)
            {
                clickableButton.Initialize(pointsPerClick, buttonLifetime, this);
                activeButtons.Add(clickableButton);
            }
        }
        
        private Vector2 GetRandomSpawnPosition()
        {
            if (spawnArea == null) return Vector2.zero;
            
            Rect rect = spawnArea.rect;
            Vector2 position;
            int attempts = 0;
            const int maxAttempts = 10;
            
            do
            {
                position = new Vector2(
                    UnityEngine.Random.Range(rect.xMin + buttonSpacing/2, rect.xMax - buttonSpacing/2),
                    UnityEngine.Random.Range(rect.yMin + buttonSpacing/2, rect.yMax - buttonSpacing/2)
                );
                attempts++;
            }
            while (IsPositionTooClose(position) && attempts < maxAttempts);
            
            return position;
        }
        
        private bool IsPositionTooClose(Vector2 position)
        {
            foreach (ClickableButton button in activeButtons)
            {
                if (button == null) continue;
                
                RectTransform buttonRect = button.GetComponent<RectTransform>();
                if (buttonRect != null)
                {
                    float distance = Vector2.Distance(position, buttonRect.anchoredPosition);
                    if (distance < buttonSpacing)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        
        public void OnButtonClicked(ClickableButton button, int points)
        {
            if (!isGameActive) return;
            
            currentScore += points;
            OnScoreChanged?.Invoke(currentScore);
            
            RemoveButton(button);
            UpdateUI();
        }
        
        public void RemoveButton(ClickableButton button)
        {
            if (activeButtons.Contains(button))
            {
                activeButtons.Remove(button);
            }
            
            if (button != null && button.gameObject != null)
            {
                Destroy(button.gameObject);
            }
        }
        
        private void ClearAllButtons()
        {
            foreach (ClickableButton button in activeButtons)
            {
                if (button != null && button.gameObject != null)
                {
                    Destroy(button.gameObject);
                }
            }
            activeButtons.Clear();
        }
        
        private void EndGame()
        {
            isGameActive = false;
            timeRemaining = 0;
            
            if (spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
            }
            
            ClearAllButtons();
            
            // Рассчитываем награду
            int moneyEarned = Mathf.RoundToInt(baseMoneyReward + (currentScore * moneyPerPoint));
            
            // Добавляем деньги игроку
            if (playerCurrency != null)
            {
                playerCurrency.AddMoney(moneyEarned);
            }
            
            // Показываем экран окончания игры
            ShowEndGameScreen(moneyEarned);
            
            OnGameEnded?.Invoke(currentScore, moneyEarned);
        }
        
        private void ShowEndGameScreen(int moneyEarned)
        {
            if (gameUI != null) gameUI.SetActive(false);
            if (endGameScreen != null) endGameScreen.SetActive(true);
            
            // Найдем и обновим компонент экрана окончания игры
            GardenEndGameScreen endScreen = endGameScreen.GetComponent<GardenEndGameScreen>();
            if (endScreen != null)
            {
                endScreen.ShowResults(currentScore, moneyEarned);
            }
        }
        
        public void RestartGame()
        {
            StartGame();
        }
        
        public int GetCurrentScore() => currentScore;
        public float GetTimeRemaining() => timeRemaining;
        public bool IsGameActive() => isGameActive;
    }
}
