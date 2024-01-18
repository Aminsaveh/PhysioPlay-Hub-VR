using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private enum DifficultyLevel
    {
        Easy,
        Medium,
        Hard
    }

    [SerializeField] private DifficultyLevel currentDifficulty;
    public EasyGame easy;
    public MediumGame medium;
    public HardGame hard;

    void Start()
    {
        easy.enabled = false;
        medium.enabled = false;
        hard.enabled = false;
        SetDifficulty(currentDifficulty);
    }

    void SetDifficulty(DifficultyLevel difficulty)
    {
        easy.enabled = difficulty == DifficultyLevel.Easy;
        medium.enabled = difficulty == DifficultyLevel.Medium;
        hard.enabled = difficulty == DifficultyLevel.Hard;

        // Add any additional logic you need when changing difficulty
    }
}
