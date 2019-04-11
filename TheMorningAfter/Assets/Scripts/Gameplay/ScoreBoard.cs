using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Displays the game state in a user-readable form
/// </summary>
public class ScoreBoard : MonoBehaviour
{
    private int livesRemaining; // number of lives remaining
    private GameObject[] livesSprites; // used to indicate number of lives remaining

    private int itemsCollected; // number of items collected
    private Text itemsCollectedText; // used to indicate the number of items collected
    private Text messageText; // used to indicate the name of the last item collected
    private bool showMessage; // indicates whether the name of the last item collected should be displayed

    GameState gameState; // the state of the game

    // Use this for initialization
    void Start()
    {
        // ensure that the canvas is being displayed on the main camera
        GetComponent<Canvas>().worldCamera = Camera.main;

        gameState = Camera.main.GetComponent<GameState>();

        // display a sprite for each remaining life
        UpdateLifeSprites(GameConstants.MaxLives);

        EventManager.AddPowerUpListener(CollectItem);
        EventManager.AddItemCollectedListener(CollectItem);

        itemsCollectedText = GameObject.FindGameObjectWithTag(GameConstants.ITEMSCOLLECTEDTEXT).GetComponent<Text>();
        itemsCollectedText.text = createItemsCollectedText();

        messageText = GameObject.FindGameObjectWithTag(GameConstants.MESSAGETEXT).GetComponent<Text>();
        messageText.enabled = false;
        showMessage = false;

        Logger.Log("Time = " + gameState.InGameTime.ToShortTimeString());
        GameObject.FindGameObjectWithTag(GameConstants.TIMETEXT).GetComponent<TimeController>().CurrentTime = gameState.InGameTime;
    }

    private void Update()
    {
        // if the number of items collected has changed, update the display
        if (this.itemsCollected != gameState.ItemsCollected)
        {
            this.itemsCollected = gameState.ItemsCollected;
            itemsCollectedText.text = createItemsCollectedText();
        }

        // if the number of lives remaining has changed, update the display
        if (this.livesRemaining != gameState.LivesRemaining)
        {
            this.livesRemaining = gameState.LivesRemaining;
            Logger.Log("UPDATE LIVES DISPLAY:" + this.livesRemaining);

            // destroy current life sprites
            if (livesSprites != null && livesSprites.Length > 0)
            {
                for(int i = 0; i < livesSprites.Length; i++)
                {
                    Destroy(livesSprites[i]);
                }
            }

            // create new life sprites
            if (livesRemaining > 0)
                UpdateLifeSprites(this.livesRemaining);
        }

        // if the name of the most recent item collected is being shown
        if (showMessage)
        {
            // slowly reduce the transparancy of the text so that 
            // it gradually disappears.
            Color color = messageText.color;
            if (color.a > 0f)
            {
                color.a -= 0.001f; // bigger number; quicker disappearance
                messageText.color = color;

                if (color.a <= 0f)
                {
                    messageText.enabled = false;
                    showMessage = false;
                }
            }
        }
    }

    private void UpdateLifeSprites(int remainingLives)
    {
        // display a sprite for each remaining life
        livesSprites = new GameObject[remainingLives];
        float lifeSpriteX = -8.15f; // x position on camera
        for (int i = 0; i < remainingLives; i++)
        {
            GameObject lifeSprite = UnityEngine.Object.Instantiate(Resources.Load("Objects/LifeSprite")) as GameObject;

            // set sprite position
            Vector3 position = new Vector3(lifeSpriteX, -4.38f, 0f);
            lifeSpriteX = lifeSpriteX + 0.70f;
            lifeSprite.transform.position = position;

            livesSprites[i] = lifeSprite;
        }
    }

    private string createItemsCollectedText()
    {
        return "Items Collected: " + itemsCollected + " of " + GameConstants.TotalTreasures;
    }

    /// <summary>
    /// Player has collected an item, which invalidates the previous room state
    /// </summary>
    /// <param name="itemID">Item identifier.</param>
    public void CollectItem(string itemID)
    {
        this.showMessage = true;
        this.messageText.text = "You Got " + SplitCamelCase(itemID);
        this.messageText.color = Color.white;
        this.messageText.enabled = true;
    }

    /// <summary>
    /// Converts item ids into a more user-readable form
    /// </summary>
    /// <returns>The camel case.</returns>
    /// <param name="input">Input.</param>
    public static string SplitCamelCase(string input)
    {
        return System.Text.RegularExpressions.Regex.Replace(input, "([A-Z])", " $1").Trim();
    }
}
