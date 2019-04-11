using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Shows the Game Over screen.
/// </summary>
public class GameOver : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        AudioManager.Instance.Stop();

        ShowFinalScore();
    }

    private void ShowFinalScore()
    {
        int itemsCollected = GameConstants.TotalTreasures;
        int creaturesKilled = GameConstants.TotalCreatures;
        DateTime inGameTime = DateTime.Now;
        bool bedReached = true;

        GameState gameState = Camera.main.GetComponent<GameState>();
        if (gameState.DifficultyMultiplier == GameConstants.EasyDifficulty)
            GameObject.FindGameObjectWithTag("Background").GetComponent<Image>().color = Color.gray;
        else if (gameState.DifficultyMultiplier == GameConstants.MediumDifficulty)
            GameObject.FindGameObjectWithTag("Background").GetComponent<Image>().color = Color.red;
        // else, as defined in Inspector

        itemsCollected = gameState.ItemsCollected;
        creaturesKilled = gameState.CreaturesKilled;
        inGameTime = gameState.InGameTime;
        bedReached = gameState.BedReached;

        if (itemsCollected >= GameConstants.TotalTreasures && bedReached)
        {
            AudioManager.Instance.PlayOneShot(AudioClipName.GameWon, 0.3f);

            Text finalStateText = GameObject.FindGameObjectWithTag(GameConstants.FINALSTATETEXT).GetComponent<Text>();
            finalStateText.text = "Sleep at last.\nSweet dreams.";
        }
        else
        {
            AudioManager.Instance.PlayOneShot(AudioClipName.GameLost);

            Text finalStateText = GameObject.FindGameObjectWithTag(GameConstants.FINALSTATETEXT).GetComponent<Text>();
            finalStateText.text = "YOU DIED!\nAh well, at least you are finally getting some rest.";

            Blueprints.ClearAll();
            gameState.DeleteAll();
            gameState.Save();
        }

        Text gameOverText = GameObject.FindGameObjectWithTag(GameConstants.FINALSCORETEXT).GetComponent<Text>();
        gameOverText.text = "You collected " + itemsCollected + " of " + GameConstants.TotalTreasures + " treasures.\n" +
                            "You killed " + creaturesKilled + " of " + GameConstants.TotalCreatures + " creatures.\n" +
                            "Task completed at: " + inGameTime.ToString("hh:mmtt", CultureInfo.InvariantCulture).ToLower();
    }

    public void HandleBackButtonOnClickEvent()
    {
        AudioManager.Instance.PlayOneShot(AudioClipName.Click);
        AudioManager.Instance.Stop();
        MenuManager.GoToMenu(MenuName.Main);
    }
}
