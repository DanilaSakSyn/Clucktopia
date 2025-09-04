using UnityEngine;

[System.Serializable]
public class PetStats : MonoBehaviour
{
    [Range(0, 100)] public float Hunger = 100f;
    [Range(0, 100)] public float Thirst = 100f;
    [Range(0, 100)] public float Cleanliness = 100f;
    [Range(0, 100)] public float Sleep = 100f;

    public float HungerDecayRate = 0.5f;
    public float ThirstDecayRate = 0.7f;
    public float CleanlinessDecayRate = 0.3f;
    public float SleepDecayRate = 0.4f;

    private TimeManager _timeManager;

    public void Initialize(TimeManager timeManager)
    {
        _timeManager = timeManager;
        Debug.Log("PetStats initialized with TimeManager.");
    }
    private void Start()
    {
        if (_timeManager != null)
        {
            float elapsedTime = _timeManager.GetTimeSinceLastLogin();
            if (elapsedTime > 0)
            {
                DecreaseStatsOverTime(elapsedTime);
            }
        }
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;
        Hunger = Mathf.Max(0, Hunger - deltaTime * HungerDecayRate);
        Thirst = Mathf.Max(0, Thirst - deltaTime * ThirstDecayRate);
        Cleanliness = Mathf.Max(0, Cleanliness - deltaTime * CleanlinessDecayRate);
        Sleep = Mathf.Max(0, Sleep - deltaTime * SleepDecayRate);
    }

    public void Feed(float amount)
    {
        Hunger = Mathf.Min(100, Hunger + amount);
    }

    public void Drink(float amount)
    {
        Thirst = Mathf.Min(100, Thirst + amount);
    }

    public void Clean(float amount)
    {
        Cleanliness = Mathf.Min(100, Cleanliness + amount);
    }

    public void Rest(float amount)
    {
        Sleep = Mathf.Min(100, Sleep + amount);
    }

    public void ModifyStats(int hungerRestore, int thirstRestore, int cleanlinessRestore, int energyRestore)
    {
        if (hungerRestore > 0) Feed(hungerRestore);
        if (thirstRestore > 0) Drink(thirstRestore);
        if (cleanlinessRestore > 0) Clean(cleanlinessRestore);
        if (energyRestore > 0) Rest(energyRestore);
        
        Debug.Log($"Stats modified: Hunger +{hungerRestore}, Thirst +{thirstRestore}, Cleanliness +{cleanlinessRestore}, Energy +{energyRestore}");
    }

    private void DecreaseStatsOverTime(float elapsedTime)
    {
        Hunger = Mathf.Max(0, Hunger - elapsedTime * HungerDecayRate);
        Thirst = Mathf.Max(0, Thirst - elapsedTime * ThirstDecayRate);
        Cleanliness = Mathf.Max(0, Cleanliness - elapsedTime * CleanlinessDecayRate);
        Sleep = Mathf.Max(0, Sleep - elapsedTime * SleepDecayRate);
    }
}
