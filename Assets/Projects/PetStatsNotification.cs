using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace Projects
{
    public class PetStatsNotification : MonoBehaviour
    {
        [Header("Notification UI")]
        [SerializeField] private GameObject notificationPanel;
        [SerializeField] private TextMeshProUGUI notificationText;
        [SerializeField] private Image notificationIcon;
        [SerializeField] private Button closeButton;
        
        [Header("Notification Settings")]
        [SerializeField] private float notificationDuration = 3f;
        [SerializeField] private bool autoCloseNotifications = true;
        
        [Header("Icons")]
        [SerializeField] private Sprite hungerIcon;
        [SerializeField] private Sprite thirstIcon;
        [SerializeField] private Sprite cleanlinessIcon;
        [SerializeField] private Sprite sleepIcon;
        
        [Header("Colors")]
        [SerializeField] private Color criticalColor = Color.red;
        [SerializeField] private Color improvedColor = Color.green;
        
        private Coroutine _currentNotificationCoroutine;
        
        private void OnEnable()
        {
            PetStatsUI.OnCriticalStat += ShowCriticalNotification;
            PetStatsUI.OnStatImproved += ShowImprovedNotification;
        }
        
        private void OnDisable()
        {
            PetStatsUI.OnCriticalStat -= ShowCriticalNotification;
            PetStatsUI.OnStatImproved -= ShowImprovedNotification;
        }
        
        private void Start()
        {
            if (notificationPanel != null)
                notificationPanel.SetActive(false);
                
            if (closeButton != null)
                closeButton.onClick.AddListener(CloseNotification);
        }
        
        private void ShowCriticalNotification(PetStatsUI.StatType statType)
        {
            string message = GetCriticalMessage(statType);
            Sprite icon = GetStatIcon(statType);
            ShowNotification(message, icon, criticalColor);
        }
        
        private void ShowImprovedNotification(PetStatsUI.StatType statType)
        {
            string message = GetImprovedMessage(statType);
            Sprite icon = GetStatIcon(statType);
            ShowNotification(message, icon, improvedColor);
        }
        
        private void ShowNotification(string message, Sprite icon, Color color)
        {
            if (notificationPanel == null) return;
            
            // Останавливаем предыдущее уведомление
            if (_currentNotificationCoroutine != null)
            {
                StopCoroutine(_currentNotificationCoroutine);
            }
            
            // Настраиваем уведомление
            if (notificationText != null)
            {
                notificationText.text = message;
                notificationText.color = color;
            }
            
            if (notificationIcon != null && icon != null)
            {
                notificationIcon.sprite = icon;
                notificationIcon.color = color;
            }
            
            // Показываем уведомление
            notificationPanel.SetActive(true);
            
            // Автоматически скрываем через время
            if (autoCloseNotifications)
            {
                _currentNotificationCoroutine = StartCoroutine(HideNotificationAfterDelay());
            }
        }
        
        private IEnumerator HideNotificationAfterDelay()
        {
            yield return new WaitForSeconds(notificationDuration);
            CloseNotification();
        }
        
        private void CloseNotification()
        {
            if (_currentNotificationCoroutine != null)
            {
                StopCoroutine(_currentNotificationCoroutine);
                _currentNotificationCoroutine = null;
            }
            
            if (notificationPanel != null)
                notificationPanel.SetActive(false);
        }
        
        private string GetCriticalMessage(PetStatsUI.StatType statType)
        {
            switch (statType)
            {
                case PetStatsUI.StatType.Hunger:
                    return "⚠️ Питомец очень голоден!\nПора покормить!";
                case PetStatsUI.StatType.Thirst:
                    return "⚠️ Питомец хочет пить!\nДайте воды!";
                case PetStatsUI.StatType.Cleanliness:
                    return "⚠️ Питомец грязный!\nПора помыть!";
                case PetStatsUI.StatType.Sleep:
                    return "⚠️ Питомец устал!\nНужен отдых!";
                default:
                    return "⚠️ Внимание требуется!";
            }
        }
        
        private string GetImprovedMessage(PetStatsUI.StatType statType)
        {
            switch (statType)
            {
                case PetStatsUI.StatType.Hunger:
                    return "✅ Голод утолен!\nПитомец доволен!";
                case PetStatsUI.StatType.Thirst:
                    return "✅ Жажда утолена!\nПитомец освежился!";
                case PetStatsUI.StatType.Cleanliness:
                    return "✅ Питомец чистый!\nОн выглядит отлично!";
                case PetStatsUI.StatType.Sleep:
                    return "✅ Питомец отдохнул!\nЭнергия восстановлена!";
                default:
                    return "✅ Состояние улучшено!";
            }
        }
        
        private Sprite GetStatIcon(PetStatsUI.StatType statType)
        {
            switch (statType)
            {
                case PetStatsUI.StatType.Hunger:
                    return hungerIcon;
                case PetStatsUI.StatType.Thirst:
                    return thirstIcon;
                case PetStatsUI.StatType.Cleanliness:
                    return cleanlinessIcon;
                case PetStatsUI.StatType.Sleep:
                    return sleepIcon;
                default:
                    return null;
            }
        }
    }
}
