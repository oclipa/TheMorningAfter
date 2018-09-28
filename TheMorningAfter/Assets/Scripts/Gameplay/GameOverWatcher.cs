using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
