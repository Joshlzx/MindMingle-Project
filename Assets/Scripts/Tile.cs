using UnityEngine;

public class Tile : MonoBehaviour
{
    private GameManager gameManager;
    private SpriteRenderer spriteRenderer;

    private int tileId;
    private Color colour;


    public void Init(GameManager gameManager, int tileId, Color colour)
    {
        this.gameManager = gameManager;
        this.tileId = tileId;
        this.colour = colour;
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Tiles start off.
        TurnOff();
    }

    public void TurnOff()
    {
        // Darken the original colour.
        spriteRenderer.color = colour * 0.3f;
    }

    public void TurnOn()
    {
        spriteRenderer.color = colour;
    }

    private void OnMouseDown()
    {
        // Call a function on the GameManager
        gameManager.PlayLightAndTone(tileId);
    }

}

