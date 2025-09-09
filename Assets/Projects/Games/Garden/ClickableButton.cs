using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace Projects.Games.Garden
{
    public class ClickableButton : MonoBehaviour
    {
        [Header("Visual Settings")]
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI pointsText;
        [SerializeField] private Image buttonImage;
        [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0.8f, 1, 1.2f);
        [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.Linear(0, 1, 1, 0);
        
        [Header("Effects")]
        [SerializeField] private Color[] buttonColors = { Color.green, Color.yellow, Color.red, Color.blue };
        [SerializeField] private ParticleSystem clickEffect;
        [SerializeField] private AudioSource clickSound;
        
        private int pointsValue;
        private float lifetime;
        private GardenGameController gameController;
        private bool isClicked = false;
        private Coroutine lifetimeCoroutine;
        
        private void Awake()
        {
            if (button == null)
                button = GetComponent<Button>();
            
            if (pointsText == null)
                pointsText = GetComponentInChildren<TextMeshProUGUI>();
            
            if (buttonImage == null)
                buttonImage = GetComponent<Image>();
        }
        
        public void Initialize(int points, float buttonLifetime, GardenGameController controller)
        {
            pointsValue = points;
            lifetime = buttonLifetime;
            gameController = controller;
            
            // Настраиваем кнопку
            if (button != null)
            {
                button.onClick.AddListener(OnClick);
            }
            
            // Отображаем очки
            if (pointsText != null)
            {
                pointsText.text = $"+{points}";
            }
            
            // Случайный цвет кнопки
            if (buttonImage != null && buttonColors.Length > 0)
            {
                Color randomColor = buttonColors[Random.Range(0, buttonColors.Length)];
                buttonImage.color = randomColor;
            }
            
            // Запускаем анимацию появления
            StartCoroutine(SpawnAnimation());
            
            // Запускаем таймер жизни кнопки
            lifetimeCoroutine = StartCoroutine(LifetimeTimer());
        }
        
        private void OnClick()
        {
            if (isClicked || gameController == null) return;
            
            isClicked = true;
            
            // Останавливаем таймер жизни
            if (lifetimeCoroutine != null)
            {
                StopCoroutine(lifetimeCoroutine);
            }
            
            // Эффекты клика
            PlayClickEffects();
            
            // Уведомляем контроллер
            gameController.OnButtonClicked(this, pointsValue);
            
            // Анимация исчезновения
            StartCoroutine(ClickAnimation());
        }
        
        private void PlayClickEffects()
        {
            // Звуковой эффект
            if (clickSound != null)
            {
                clickSound.Play();
            }
            
            // Эффект частиц
            if (clickEffect != null)
            {
                clickEffect.Play();
            }
        }
        
        private IEnumerator SpawnAnimation()
        {
            if (transform == null) yield break;
            
            Vector3 originalScale = transform.localScale;
            float duration = 0.3f;
            float elapsed = 0f;
            
            // Начинаем с маленького размера
            transform.localScale = Vector3.zero;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;
                
                float scaleMultiplier = scaleCurve.Evaluate(progress);
                transform.localScale = originalScale * scaleMultiplier;
                
                yield return null;
            }
            
            transform.localScale = originalScale;
        }
        
        private IEnumerator ClickAnimation()
        {
            float duration = 0.2f;
            float elapsed = 0f;
            Vector3 originalScale = transform.localScale;
            Color originalColor = buttonImage != null ? buttonImage.color : Color.white;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;
                
                // Увеличиваем размер
                transform.localScale = originalScale * (1f + progress * 0.5f);
                
                // Убираем прозрачность
                if (buttonImage != null)
                {
                    Color color = originalColor;
                    color.a = 1f - progress;
                    buttonImage.color = color;
                }
                
                if (pointsText != null)
                {
                    Color textColor = pointsText.color;
                    textColor.a = 1f - progress;
                    pointsText.color = textColor;
                }
                
                yield return null;
            }
            
            // Удаляем кнопку через контроллер
            if (gameController != null)
            {
                gameController.RemoveButton(this);
            }
        }
        
        private IEnumerator LifetimeTimer()
        {
            float elapsed = 0f;
            Color originalColor = buttonImage != null ? buttonImage.color : Color.white;
            
            while (elapsed < lifetime && !isClicked)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / lifetime;
                
                // Постепенное мигание когда время заканчивается
                if (progress > 0.7f)
                {
                    float blinkSpeed = Mathf.Lerp(2f, 8f, (progress - 0.7f) / 0.3f);
                    float alpha = 0.5f + 0.5f * Mathf.Sin(Time.time * blinkSpeed);
                    
                    if (buttonImage != null)
                    {
                        Color color = originalColor;
                        color.a = alpha;
                        buttonImage.color = color;
                    }
                }
                
                yield return null;
            }
            
            // Если время истекло и кнопку не нажали
            if (!isClicked)
            {
                StartCoroutine(ExpireAnimation());
            }
        }
        
        private IEnumerator ExpireAnimation()
        {
            float duration = 0.3f;
            float elapsed = 0f;
            Vector3 originalScale = transform.localScale;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;
                
                // Уменьшаем размер
                transform.localScale = originalScale * (1f - progress);
                
                // Убираем прозрачность
                if (buttonImage != null)
                {
                    Color color = buttonImage.color;
                    color.a = 1f - progress;
                    buttonImage.color = color;
                }
                
                yield return null;
            }
            
            // Удаляем кнопку через контроллер
            if (gameController != null)
            {
                gameController.RemoveButton(this);
            }
        }
        
        private void OnDestroy()
        {
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
            }
        }
    }
}
