using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls canvas the provides mention of the weapon.
/// This is triggered by a collider that is separate from the
/// instructions collider but positioned so that the player 
/// must pass through both.  This canvas is triggered only 
/// after the instructions canvas has been triggered.
/// </summary>
public class MaidWeaponMentioner : MonoBehaviour
{
    GameState gameState;
    GameObject canvas;
    GameObject player;

    bool isGameOver;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.AddGameOverListener(gameOver);

        gameState = Camera.main.GetComponent<GameState>();
    }

    private void gameOver(int score)
    {
        isGameOver = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        player = collision.gameObject;

        // if player inside collider (and game is still active)
        if (!isGameOver && player.CompareTag(GameConstants.PLAYER))
        {
            if (canvas == null)
                canvas = Object.Instantiate(Resources.Load("Objects/MaidSpeech")) as GameObject;

            StringBuilder sb = new StringBuilder();

            // does the player still have items to collect?
            if (gameState.ItemsCollected < GameConstants.TotalTreasures)
            {
                // has the maid given the instructions but not yet mentioned the weapon?
                if (gameState.MaidGivenInstructions && !gameState.MaidMentionedWeapon)
                {
                    sb.AppendLine("By the way, thanks to your antics, you may have noticed that we have a bit of an infestation at the minute.");
                    sb.AppendLine();
                    sb.AppendLine("I believe I noticed some bug spray somewhere in the basement.  If you can find it, that may make your task a bit easier.");
                    sb.AppendLine();
                    sb.AppendLine("Be warned however; I believe it has a bit of a kick.");

                    gameState.MaidMentionedWeapon = true;
                }
            }

            canvas.GetComponentInChildren<Text>().text = sb.ToString();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // once player leaves the collider, hide all messages.
        player = collision.gameObject;
        if (player.CompareTag(GameConstants.PLAYER))
        {
            Destroy(canvas);

            player = null;
            canvas = null;
        }
    }
}
