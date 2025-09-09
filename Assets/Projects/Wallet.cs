using System;
using UnityEngine;

namespace Projects
{
    public class Wallet : MonoBehaviour
    {
        private static Wallet _instance;
        
        [Header("Wallet Settings")]
        [SerializeField] private int currentMoney = 0;
        [SerializeField] private string currencyName = "Coins";
        
        private const string MONEY_SAVE_KEY = "PlayerMoney";
        
        public static Wallet Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<Wallet>();
                    if (_instance == null)
                    {
                        GameObject walletObject = new GameObject("Wallet");
                        _instance = walletObject.AddComponent<Wallet>();
                        DontDestroyOnLoad(walletObject);
                    }
                }
                return _instance;
            }
        }
        
        public string CurrencyName
        {
            get => currencyName;
            set => currencyName = value;
        }
        
        public static event Action<int> OnMoneyChanged;
        
        public int CurrentMoney => currentMoney;
        
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                LoadMoney(); // Автоматически загружаем деньги при запуске
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            OnMoneyChanged?.Invoke(currentMoney);
        }
        
        public bool CanAfford(int amount)
        {
            return currentMoney >= amount;
        }
        
        public bool SpendMoney(int amount)
        {
            if (!CanAfford(amount))
            {
                Debug.Log($"Недостаточно {currencyName}! Нужно: {amount}  <sprite index=2>, есть: {currentMoney} <sprite index=2>");
                return false;
            }
            
            currentMoney -= amount;
            SaveMoney(); // Автоматически сохраняем после изменения
            OnMoneyChanged?.Invoke(currentMoney);
            Debug.Log($"Потрачено {amount} {currencyName}. Осталось: {currentMoney} <sprite index=2>");
            return true;
        }
        
        public void AddMoney(int amount)
        {
            if (amount < 0)
            {
                Debug.LogWarning("Попытка добавить отрицательную сумму денег!");
                return;
            }
            
            currentMoney += amount;
            SaveMoney(); // Автоматически сохраняем после изменения
            OnMoneyChanged?.Invoke(currentMoney);
            Debug.Log($"Получено {amount} {currencyName}. Всего: {currentMoney} <sprite index=2>");
        }
        
        public void SetMoney(int amount)
        {
            if (amount < 0)
            {
                Debug.LogWarning("Попытка установить отрицательную сумму денег!");
                amount = 0;
            }
            
            currentMoney = amount;
            SaveMoney(); // Автоматически сохраняем после изменения
            OnMoneyChanged?.Invoke(currentMoney);
            Debug.Log($"Деньги установлены на: {currentMoney} <sprite index=2>");
        }
        
        public void ResetMoney()
        {
            SetMoney(0);
        }
        
        /// <summary>
        /// Сохраняет текущее количество денег в PlayerPrefs
        /// </summary>
        public void SaveMoney()
        {
            PlayerPrefs.SetInt(MONEY_SAVE_KEY, currentMoney);
            PlayerPrefs.Save();
            Debug.Log($"Деньги сохранены: {currentMoney} {currencyName}");
        }
        
        /// <summary>
        /// Загружает количество денег из PlayerPrefs
        /// </summary>
        public void LoadMoney()
        {
            currentMoney = PlayerPrefs.GetInt(MONEY_SAVE_KEY, 0); // По умолчанию 0 денег
            OnMoneyChanged?.Invoke(currentMoney);
            Debug.Log($"Деньги загружены: {currentMoney} {currencyName}");
        }
        
        /// <summary>
        /// Очищает сохраненные данные о деньгах
        /// </summary>
        public void ClearSavedMoney()
        {
            PlayerPrefs.DeleteKey(MONEY_SAVE_KEY);
            PlayerPrefs.Save();
            Debug.Log("Сохраненные данные о деньгах очищены");
        }
    }
}
