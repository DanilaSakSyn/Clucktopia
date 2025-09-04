using UnityEngine;

public class GameManager : MonoBehaviour
{
    private InputManager _inputManager;
    private PetStats _petStats;

    public void Initialize(InputManager inputManager, PetStats petStats)
    {
        _inputManager = inputManager;
        _petStats = petStats;
        Debug.Log("GameManager initialized with InputManager and PetStats.");
    }
}