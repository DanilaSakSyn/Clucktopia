using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace Projects
{
    public class PetStatsUI : MonoBehaviour
    {
        [Header("Hunger UI")]
        [SerializeField] private Slider hungerSlider;
        [SerializeField] private TextMeshProUGUI hungerText;
        [SerializeField] private Image hungerFillImage;
        
        [Header("Thirst UI")]
        [SerializeField] private Slider thirstSlider;
        [SerializeField] private TextMeshProUGUI thirstText;
        [SerializeField] private Image thirstFillImage;
        
        [Header("Cleanliness UI")]
        [SerializeField] private Slider cleanlinessSlider;
        [SerializeField] private TextMeshProUGUI cleanlinessText;
        [SerializeField] private Image cleanlinessFillImage;
        
        [Header("Sleep UI")]
        [SerializeField] private Slider sleepSlider;
        [SerializeField] private TextMeshProUGUI sleepText;
        [SerializeField] private Image sleepFillImage;
        
        [Header("Colors")]
        [SerializeField] private Color highStatColor = Color.green;
        [SerializeField] private Color mediumStatColor = Color.yellow;
        [SerializeField] private Color lowStatColor = Color.red;
        [SerializeField] private Color criticalStatColor = new Color(0.8f, 0f, 0f); // Темно-красный
        
        [Header("Dependencies")]
        [SerializeField] private PetStats petStats;
        
        [Header("Display Settings")]
        [SerializeField] private bool showPercentage = true;
        [SerializeField] private bool showDecimalPlaces;
        [SerializeField] private float updateFrequency = 0.1f; // Обновление каждые 0.1 секунды
        
        // События для уведомления о критическом состоянии
        public static event Action<StatType> OnCriticalStat;
        public static event Action<StatType> OnStatImproved;
        
        public enum StatType
        {
            Hunger,
            Thirst,
            Cleanliness,
            Sleep
        }
        
        private float _lastUpdateTime;
        private bool[] _wasCritical = new bool[4]; // Для отслеживания критического состояния
        
        private void Awake()
        {
            if (petStats == null)
                petStats = FindFirstObjectByType<PetStats>();
        }
        
        private void Start()
        {
            InitializeUI();
        }
        
        private void Update()
        {
            // Обновляем UI с заданной частотой для оптимизации
            if (Time.time - _lastUpdateTime >= updateFrequency)
            {
                UpdateStatsDisplay();
                _lastUpdateTime = Time.time;
            }
        }
        
        private void InitializeUI()
        {
            // Настройка слайдеров
            SetupSlider(hungerSlider);
            SetupSlider(thirstSlider);
            SetupSlider(cleanlinessSlider);
            SetupSlider(sleepSlider);
            
            // Первоначальное обновление
            UpdateStatsDisplay();
        }
        
        private void SetupSlider(Slider slider)
        {
            if (slider != null)
            {
                slider.minValue = 0f;
                slider.maxValue = 100f;
                slider.wholeNumbers = !showDecimalPlaces;
            }
        }
        
        private void UpdateStatsDisplay()
        {
            if (petStats == null) return;
            
            // Обновляем каждую характеристику
            UpdateStatUI(hungerSlider, hungerText, hungerFillImage, petStats.Hunger, "", StatType.Hunger, 0);
            UpdateStatUI(thirstSlider, thirstText, thirstFillImage, petStats.Thirst, "", StatType.Thirst, 1);
            UpdateStatUI(cleanlinessSlider, cleanlinessText, cleanlinessFillImage, petStats.Cleanliness, "", StatType.Cleanliness, 2);
            UpdateStatUI(sleepSlider, sleepText, sleepFillImage, petStats.Sleep, "", StatType.Sleep, 3);
        }
        
        private void UpdateStatUI(Slider slider, TextMeshProUGUI text, Image fillImage, float statValue, string statName, StatType statType, int criticalIndex)
        {
            if (slider == null) return;
            
            // Обновляем значение слайдера
            slider.value = statValue;
            
            // Обновляем текст
            if (text != null)
            {
                string displayText = GetStatDisplayText(statValue, statName);
                text.text = displayText;
            }
            
            // Обновляем цвет
            Color statColor = GetStatColor(statValue);
            if (fillImage != null)
            {
                fillImage.color = statColor;
            }
            
            // Проверяем критическое состояние
            CheckCriticalState(statValue, statType, criticalIndex);
        }
        
        private string GetStatDisplayText(float statValue, string statName)
        {
            if (showPercentage)
            {
                if (showDecimalPlaces)
                {
                    return $"{statValue:F1}";
                }
                else
                {
                    return $"{Mathf.RoundToInt(statValue)}";
                }
            }
            else
            {
                if (showDecimalPlaces)
                {
                    return $"{statValue:F1}/100";
                }
                else
                {
                    return $"{Mathf.RoundToInt(statValue)}/100";
                }
            }
        }
        
        private Color GetStatColor(float statValue)
        {
            if (statValue <= 10f)
                return criticalStatColor;
            else if (statValue <= 25f)
                return lowStatColor;
            else if (statValue <= 50f)
                return mediumStatColor;
            else
                return highStatColor;
        }
        
        private void CheckCriticalState(float statValue, StatType statType, int criticalIndex)
        {
            bool isCritical = statValue <= 10f;
            
            // Если стат стал критическим
            if (isCritical && !_wasCritical[criticalIndex])
            {
                _wasCritical[criticalIndex] = true;
                OnCriticalStat?.Invoke(statType);
                Debug.LogWarning($"{statType} критически низкий: {statValue:F1}");
            }
            // Если стат улучшился из критического состояния
            else if (!isCritical && _wasCritical[criticalIndex])
            {
                _wasCritical[criticalIndex] = false;
                OnStatImproved?.Invoke(statType);
                Debug.Log($"{statType} улучшился из критического состояния: {statValue:F1}");
            }
        }
        
        // Публичные методы для настройки отображения
        public void SetShowPercentage(bool show)
        {
            showPercentage = show;
        }
        
        public void SetShowDecimalPlaces(bool show)
        {
            showDecimalPlaces = show;
            SetupSlider(hungerSlider);
            SetupSlider(thirstSlider);
            SetupSlider(cleanlinessSlider);
            SetupSlider(sleepSlider);
        }
        
        public void SetUpdateFrequency(float frequency)
        {
            updateFrequency = Mathf.Max(0.01f, frequency);
        }
        
        // Методы для получения текущих значений
        public float GetHunger() => petStats != null ? petStats.Hunger : 0f;
        public float GetThirst() => petStats != null ? petStats.Thirst : 0f;
        public float GetCleanliness() => petStats != null ? petStats.Cleanliness : 0f;
        public float GetSleep() => petStats != null ? petStats.Sleep : 0f;
        
        // Метод для принудительного обновления UI
        public void ForceUpdateDisplay()
        {
            UpdateStatsDisplay();
        }
    }
}
