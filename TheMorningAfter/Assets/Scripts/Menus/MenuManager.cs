using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MenuManager
{
    /// <summary>
    /// Gos to menu.
    /// </summary>
    /// <param name="menuName">Menu name.</param>
    public static void GoToMenu(MenuName menuName)
    {
        switch (menuName)
        {
            case MenuName.Main:
                SceneManager.LoadScene("MainMenu");
                break;
            case MenuName.Pause:
                Object.Instantiate(Resources.Load("Menus/PauseMenu"));
                break;
            case MenuName.Help:
                Object.Instantiate(Resources.Load("Menus/HelpMenu"));
                break;
            case MenuName.Quit:
                Application.Quit();
                break;
            case MenuName.GameOver:
                Object.Instantiate(Resources.Load("Menus/GameOverMenu"));
                break;
        }
    }
}
