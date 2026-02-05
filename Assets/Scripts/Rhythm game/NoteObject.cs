using UnityEngine;
using UnityEngine.InputSystem;

public class NoteObject : MonoBehaviour
{
    public bool canBePressed;
    public KeyCode keyToPress;
    public GameObject hitEffect, goodEffect, perfectEffect, missEffect;

    // Start is called once before the first execution of Update
    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {
        // Convert KeyCode to string to match ESP32InputManager keys
        string keyString = keyToPress.ToString(); // KeyCode.Z -> "Z"

        // Check if the key is pressed either from keyboard or ESP32
        if (ESP32InputManager.Instance.GetKeyDown(keyString))
        {
            Debug.Log(gameObject.name + " triggered by " + keyString);
            if (canBePressed)
            {
                gameObject.SetActive(false);

                // Determine hit type based on y-position
                float yPos = Mathf.Abs(transform.position.y);

                if (yPos > 1f)
                {
                    Debug.Log("Hit");
                    GameManager1.instance.NormalHit();
                    Instantiate(hitEffect, transform.position, hitEffect.transform.rotation);
                }
                else if (yPos > 0.53f)
                {
                    Debug.Log("Good");
                    GameManager1.instance.GoodHit();
                    Instantiate(goodEffect, transform.position, goodEffect.transform.rotation);
                }
                else
                {
                    Debug.Log("Perfect");
                    GameManager1.instance.PerfectHit();
                    Instantiate(perfectEffect, transform.position, perfectEffect.transform.rotation);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Activator")
        {
            canBePressed = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Activator" && gameObject.activeSelf)
        {
            canBePressed = false;
            GameManager1.instance.NoteMissed();
            Instantiate(missEffect, transform.position, missEffect.transform.rotation);
        }
    }
}
