using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Watches for Game Over and reacts accordingly
/// </summary>
public class GameOverWatcher : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        EventManager.AddGameOverListener(GameOver);
    }

    void GameOver(int score)
    {
        MenuManager.GoToMenu(MenuName.GameOver);
    }
}
