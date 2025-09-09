using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Projects.Games.Garden
{
    public class GardenGameManager : MonoBehaviour
    {
        [Header("Game References")]
        [SerializeField] private GardenGameController gameController;
        [SerializeField] private GardenEndGameScreen endGameScreen;
        
        [Header("Start Screen UI")]
        [SerializeField] private GameObject startScreen;
        [SerializeField] private Button startGameButton;
        [SerializeField] private TextMeshProUGUI instructionsText;
        [SerializeField] private TextMeshProUGUI bestScoreText;
        
        [Header("Game Statistics")]
        [SerializeField] private string bestScoreKey = "GardenBestScore";
        
        private int bestScore = 0;
        private int totalMoneyEarned = 0;
        
        private void Awake()
        {
            if (gameController == null)
                gameController = FindFirstObjectByType<GardenGameController>();
                
            if (endGameScreen == null)
                endGameScreen = FindFirstObjectByType<GardenEndGameScreen>();
        }
        
        private void Start()
        {
            LoadGameStatistics();
            SetupUI();
            ShowStartScreen();
        }
        
        private void OnEnable()
        {
            GardenGameController.OnGameEnded += OnGameEnded;
        }
        
        private void OnDisable()
        {
            GardenGameController.OnGameEnded -= OnGameEnded;
        }
        
        private void SetupUI()
        {
            if (startGameButton != null)
            {
                startGameButton.onClick.AddListener(StartGame);
            }
            
            UpdateInstructionsText();
            UpdateBestScoreDisplay();
        }
        
        private void ShowStartScreen()
        {
            if (startScreen != null)
                startScreen.SetActive(true);
        }
        
        private void HideStartScreen()
        {
            if (startScreen != null)
                startScreen.SetActive(false);
        }
        
        private void UpdateInstructionsText()
        {
            if (instructionsText != null)
            {
                instructionsText.text = " Welcome to the Garden Game! \n\n" +
                                        " Rules:\n" +
                                        "• Click on the buttons that appear\n" +
                                        "• Each button gives points\n" +
                                        "• The buttons disappear after a few seconds\n" +
                                        "• The more points - the more coins!\n\n" +
                                        " You have 30 seconds!\n" +
                                        "Good luck! ";
            }
        }
        
        private void UpdateBestScoreDisplay()
        {
            if (bestScoreText != null)
            {
                if (bestScore > 0)
                {
                    bestScoreText.text = $" Best Score: {bestScore}\n Total Earned: {totalMoneyEarned}";
                }
                else
                {
                    bestScoreText.text = " No records yet\n Start playing!";
                }
            }
        }
        
        private void StartGame()
        {
            HideStartScreen();
            
            if (gameController != null)
            {
                gameController.StartGame();
            }
        }
        
        private void OnGameEnded(int score, int moneyEarned)
        {
            LoadGameStatistics();
            // Обновляем статистику
            bool newRecord = false;
            
            if (score > bestScore)
            {
                bestScore = score;
                newRecord = true;
            }
            
            Wallet.Instance.AddMoney(moneyEarned);
            
            SaveGameStatistics();
        
            UpdateBestScoreDisplay();
         
            // Логируем результат
            Debug.Log($"Garden Game Ended - Score: {score}, Money Earned: {moneyEarned}, New Record: {newRecord}");
        }
        
        private void LoadGameStatistics()
        {
            bestScore = PlayerPrefs.GetInt(bestScoreKey, 0);
        }
        
        private void SaveGameStatistics()
        {
            PlayerPrefs.SetInt(bestScoreKey, bestScore);
            
            PlayerPrefs.Save();
        }
        
        // Публичные методы для внешнего управления
        public void ResetStatistics()
        {
            bestScore = 0;
            totalMoneyEarned = 0;
            SaveGameStatistics();
            UpdateBestScoreDisplay();
        }
        
        public int GetBestScore() => bestScore;
        public int GetTotalMoneyEarned() => totalMoneyEarned;
        
        private void OnDestroy()
        {
            if (startGameButton != null)
                startGameButton.onClick.RemoveAllListeners();
        }
    }
}
