using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        AudioManager.Stop();

        showFinalScore();
    }

    private void showFinalScore()
    {
        ScoreBoard scoreBoard = GameObject.FindGameObjectWithTag(GameConstants.SCOREBOARD).GetComponent<ScoreBoard>();
        int itemsCollected = scoreBoard.ItemsCollected;
        bool bedReached = scoreBoard.BedReached;

        if (itemsCollected >= GameConstants.TotalTreasures && bedReached)
        {
            AudioManager.PlayOneShot(AudioClipName.GameWon, 0.3f);

            Text finalStateText = GameObject.FindGameObjectWithTag(GameConstants.FINALSTATETEXT).GetComponent<Text>();
            finalStateText.text = "Sleep at last.\nSweet dreams.";
        }
        else
        {
            AudioManager.PlayOneShot(AudioClipName.GameLost);

            Text finalStateText = GameObject.FindGameObjectWithTag(GameConstants.FINALSTATETEXT).GetComponent<Text>();
            finalStateText.text = "YOU DIED!\nAh well, at least you are finally getting some rest.";
        }

        Text gameOverText = GameObject.FindGameObjectWithTag(GameConstants.FINALSCORETEXT).GetComponent<Text>();
        gameOverText.text = "You collected " + itemsCollected + " of " + GameConstants.TotalTreasures + " treasures.";
    }

    public void HandleBackButtonOnClickEvent()
    {
        AudioManager.PlayOneShot(AudioClipName.Click);
        MenuManager.GoToMenu(MenuName.Main);
    }
}
