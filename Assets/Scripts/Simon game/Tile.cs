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
        
        TurnOff();
    }

    public void TurnOff()
    {
        
        spriteRenderer.color = colour * 0.3f;
    }

    public void TurnOn()
    {
        spriteRenderer.color = colour;
    }

    private void OnMouseDown()
    {
        
        gameManager.PlayLightAndTone(tileId);
    }

}

