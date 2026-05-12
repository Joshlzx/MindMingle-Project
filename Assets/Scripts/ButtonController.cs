using UnityEngine;

public class ButtonController : MonoBehaviour
{
    private SpriteRenderer theSR;
    public Sprite defaultImage;
    public Sprite pressedImage;
    public KeyCode keyToPress;

    
    private int espHoldFrames = 0;
    private const int holdDuration = 3; 

    void Start()
    {
        theSR = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        string keyString = keyToPress.ToString();

        
        bool keyboardPressed = Input.GetKey(keyToPress); 

        
        if (ESP32InputManager.Instance.GetKeyDown(keyString))
        {
            espHoldFrames = holdDuration; 
        }
        if (espHoldFrames > 0) espHoldFrames--; 
        bool espPressed = espHoldFrames > 0;

        
        bool isPressed = keyboardPressed || espPressed;

        
        theSR.sprite = isPressed ? pressedImage : defaultImage;
    }
}