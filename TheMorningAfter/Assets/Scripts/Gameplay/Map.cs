using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    private SpriteRenderer highlightSprite;

    private Dictionary<string, SpriteMask> spriteMasks;
    private List<string> roomIds;
    private PlayerController playerController;
    private List<SpriteRenderer> spriteRenderers;
    private GameState gameState;

    private Vector3 positionRight = new Vector3(-508.39f, -290.66f, 0f);
    private Vector3 positionLeft = new Vector3(-519.39f, -290.66f, 0f);

    // Start is called before the first frame update
    void Start()
    {
        gameState = Camera.main.GetComponent<GameState>();

        spriteMasks = new Dictionary<string, SpriteMask>();
        spriteRenderers = new List<SpriteRenderer>();

        int childCount = this.transform.childCount;
        for(int childIndex=0; childIndex < childCount; childIndex++)
        {
            GameObject child = this.transform.GetChild(childIndex).gameObject;

            if (!child.name.StartsWith("Mask"))
                spriteMasks.Add(child.name, child.GetComponent<SpriteMask>());
        }

        roomIds = Blueprints.GetRoomIDs();

        foreach (SpriteRenderer spriteRenderer in this.transform.GetComponentsInChildren<SpriteRenderer>())
        {
            if (!spriteRenderer.gameObject.name.Equals("MaskHelp"))
                spriteRenderers.Add(spriteRenderer);

            if (spriteRenderer.gameObject.name.Equals("MaskHighlight"))
                highlightSprite = spriteRenderer;
        }

        transform.localPosition = positionRight;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerController == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag(GameConstants.PLAYER);
            if (player)
                playerController = player.GetComponent<PlayerController>();
        }

        if (playerController != null)
        {
            FadeMap(playerController.PlayerState is MovingState);

            if (playerController.transform.position.x > 0f)
                transform.localPosition = positionLeft;
            else
                transform.localPosition = positionRight;
        }

        string currentRoom = gameState.CurrentRoom;

        foreach (string roomId in roomIds)
        {
            IRoom room = Blueprints.GetRoom(roomId);

            SpriteMask spriteMask = null;
            if (spriteMasks != null && room != null && spriteMasks.TryGetValue(room.GetID(), out spriteMask))
            {
                spriteMask.enabled = !room.Visited;

                if (room.GetID().Equals(currentRoom))
                {
                    highlightSprite.enabled = true;
                    highlightSprite.transform.position = spriteMask.transform.position;
                }
            }
        }
    }

    private void FadeMap(bool fade)
    {
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            Color color = sr.color;
            if (fade && color.a > 0f)
            {
                color.a -= 0.1f; // bigger number, faster fade
                sr.color = color;
            }
            else if (!fade && color.a < 1f)
            {
                color.a += 0.1f; // bigger number, faster reveal
                sr.color = color;
            }
        }
    }

    //private void ShowMap()
    //{
    //    foreach (SpriteRenderer sr in spriteRenderers)
    //    {
    //        Color color = sr.color;
    //        if (color.a <= 1f)
    //        {
    //            color.a += 0.01f; // bigger number, faster reveal
    //            sr.color = color;
    //        }
    //    }
    //}
}
