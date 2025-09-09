using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Projects.Games.Garden
{
    public class GardenGameEffects : MonoBehaviour
    {
        [Header("Visual Effects")]
        [SerializeField] private GameObject scorePopupPrefab;
        [SerializeField] private Transform effectsContainer;
        [SerializeField] private ParticleSystem backgroundParticles;
        
        [Header("Screen Effects")]
        [SerializeField] private Image screenFlash;
        [SerializeField] private Color flashColor = Color.white;
        [SerializeField] private float flashDuration = 0.1f;
        
        [Header("Combo System")]
        [SerializeField] private Text comboText;
        [SerializeField] private float comboResetTime = 2f;
        [SerializeField] private int comboMultiplier = 2;
        
        private int currentCombo = 0;
        private float lastClickTime = 0f;
        private List<GameObject> activePopups = new List<GameObject>();
        
        private void Start()
        {
            if (screenFlash != null)
            {
                Color color = screenFlash.color;
                color.a = 0f;
                screenFlash.color = color;
            }
            
            if (comboText != null)
                comboText.gameObject.SetActive(false);
                
            if (backgroundParticles != null)
                backgroundParticles.Play();
        }
        
        private void OnEnable()
        {
            GardenGameController.OnScoreChanged += OnScoreChanged;
        }
        
        private void OnDisable()
        {
            GardenGameController.OnScoreChanged -= OnScoreChanged;
        }
        
        private void Update()
        {
            UpdateCombo();
        }
        
        private void OnScoreChanged(int newScore)
        {
            UpdateComboSystem();
            CreateScreenFlash();
        }
        
        private void UpdateComboSystem()
        {
            float currentTime = Time.time;
            
            // Если прошло меньше времени сброса комбо - увеличиваем комбо
            if (currentTime - lastClickTime <= comboResetTime)
            {
                currentCombo++;
            }
            else
            {
                currentCombo = 1; // Сбрасываем до 1 (текущий клик)
            }
            
            lastClickTime = currentTime;
            
            // Показываем комбо если больше 1
            if (currentCombo > 1)
            {
                ShowCombo();
            }
        }
        
        private void UpdateCombo()
        {
            if (currentCombo > 1 && Time.time - lastClickTime > comboResetTime)
            {
                ResetCombo();
            }
        }
        
        private void ShowCombo()
        {
            if (comboText == null) return;
            
            comboText.gameObject.SetActive(true);
            comboText.text = $"КОМБО x{currentCombo}!";
            
            // Анимация текста комбо
            StopAllCoroutines();
            StartCoroutine(AnimateComboText());
        }
        
        private IEnumerator AnimateComboText()
        {
            if (comboText == null) yield break;
            
            Vector3 originalScale = comboText.transform.localScale;
            Color originalColor = comboText.color;
            
            // Увеличиваем и делаем ярче
            float duration = 0.2f;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;
                
                float scale = Mathf.Lerp(1.2f, 1f, progress);
                comboText.transform.localScale = originalScale * scale;
                
                Color color = originalColor;
                color.a = Mathf.Lerp(1f, 0.8f, progress);
                comboText.color = color;
                
                yield return null;
            }
            
            comboText.transform.localScale = originalScale;
            comboText.color = originalColor;
        }
        
        private void ResetCombo()
        {
            currentCombo = 0;
            
            if (comboText != null)
            {
                StartCoroutine(FadeOutCombo());
            }
        }
        
        private IEnumerator FadeOutCombo()
        {
            if (comboText == null) yield break;
            
            Color originalColor = comboText.color;
            float duration = 0.5f;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;
                
                Color color = originalColor;
                color.a = Mathf.Lerp(originalColor.a, 0f, progress);
                comboText.color = color;
                
                yield return null;
            }
            
            comboText.gameObject.SetActive(false);
            comboText.color = originalColor; // Возвращаем исходный цвет
        }
        
        public void CreateScorePopup(Vector3 worldPosition, int points)
        {
            if (scorePopupPrefab == null || effectsContainer == null) return;
            
            GameObject popup = Instantiate(scorePopupPrefab, effectsContainer);
            
            // Устанавливаем позицию
            if (popup.GetComponent<RectTransform>() != null)
            {
                RectTransform rectTransform = popup.GetComponent<RectTransform>();
                rectTransform.position = worldPosition;
            }
            else
            {
                popup.transform.position = worldPosition;
            }
            
            // Настраиваем текст
            Text popupText = popup.GetComponentInChildren<Text>();
            if (popupText != null)
            {
                int finalPoints = currentCombo > 1 ? points * comboMultiplier : points;
                popupText.text = currentCombo > 1 ? $"+{finalPoints} (x{currentCombo})" : $"+{finalPoints}";
                
                // Цвет в зависимости от комбо
                if (currentCombo > 1)
                {
                    popupText.color = Color.yellow;
                }
            }
            
            activePopups.Add(popup);
            StartCoroutine(AnimateScorePopup(popup));
        }
        
        private IEnumerator AnimateScorePopup(GameObject popup)
        {
            if (popup == null) yield break;
            
            Transform popupTransform = popup.transform;
            Vector3 startPosition = popupTransform.position;
            Vector3 endPosition = startPosition + Vector3.up * 100f;
            
            Text popupText = popup.GetComponentInChildren<Text>();
            Color originalColor = popupText != null ? popupText.color : Color.white;
            
            float duration = 1.5f;
            float elapsed = 0f;
            
            while (elapsed < duration && popup != null)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;
                
                // Движение вверх
                popupTransform.position = Vector3.Lerp(startPosition, endPosition, progress);
                
                // Затухание
                if (popupText != null)
                {
                    Color color = originalColor;
                    color.a = Mathf.Lerp(1f, 0f, progress);
                    popupText.color = color;
                }
                
                yield return null;
            }
            
            // Удаляем popup
            if (activePopups.Contains(popup))
            {
                activePopups.Remove(popup);
            }
            
            if (popup != null)
            {
                Destroy(popup);
            }
        }
        
        private void CreateScreenFlash()
        {
            if (screenFlash != null)
            {
                StartCoroutine(FlashScreen());
            }
        }
        
        private IEnumerator FlashScreen()
        {
            if (screenFlash == null) yield break;
            
            Color color = flashColor;
            color.a = 0.3f;
            screenFlash.color = color;
            
            float elapsed = 0f;
            
            while (elapsed < flashDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / flashDuration;
                
                color.a = Mathf.Lerp(0.3f, 0f, progress);
                screenFlash.color = color;
                
                yield return null;
            }
            
            color.a = 0f;
            screenFlash.color = color;
        }
        
        public int GetCurrentCombo() => currentCombo;
        public int GetComboMultiplier() => currentCombo > 1 ? comboMultiplier : 1;
        
        private void OnDestroy()
        {
            // Очищаем активные попапы
            foreach (GameObject popup in activePopups)
            {
                if (popup != null)
                {
                    Destroy(popup);
                }
            }
            activePopups.Clear();
        }
    }
}
