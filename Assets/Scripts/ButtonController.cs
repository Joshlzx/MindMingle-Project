using UnityEngine;

public class ButtonController : MonoBehaviour
{
    private SpriteRenderer theSR;
    public Sprite defaultImage;
    public Sprite pressedImage;
    public KeyCode keyToPress;

    // Track ESP32 presses
    private int espHoldFrames = 0;
    private const int holdDuration = 3; // frames to hold ESP32 press for visibility

    void Start()
    {
        theSR = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        string keyString = keyToPress.ToString();

        // --- Keyboard input ---
        bool keyboardPressed = Input.GetKey(keyToPress); // true while held

        // --- ESP32 input ---
        if (ESP32InputManager.Instance.GetKeyDown(keyString))
        {
            espHoldFrames = holdDuration; // start holding sprite
        }
        if (espHoldFrames > 0) espHoldFrames--; // countdown
        bool espPressed = espHoldFrames > 0;

        // --- Final pressed state ---
        bool isPressed = keyboardPressed || espPressed;

        // --- Update sprite ---
        theSR.sprite = isPressed ? pressedImage : defaultImage;
    }
}