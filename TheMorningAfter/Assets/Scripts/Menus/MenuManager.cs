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
    public static GameObject GoToMenu(MenuName menuName)
    {
        switch (menuName)
        {
            case MenuName.Main:
                SceneManager.LoadScene("MainMenu");
                break;
            case MenuName.Pause:
                return Object.Instantiate(Resources.Load("Menus/PauseMenu")) as GameObject;
            case MenuName.Help:
                return Object.Instantiate(Resources.Load("Menus/HelpMenu")) as GameObject;
            case MenuName.Quit:
                Application.Quit();
                break;
            case MenuName.GameOver:
                return Object.Instantiate(Resources.Load("Menus/GameOverMenu")) as GameObject;
        }

        return null;
    }
}
