using UnityEngine;
using System.IO.Ports;
using System.Collections.Generic;

public class ESP32InputManager : MonoBehaviour
{
    public static ESP32InputManager Instance;

    SerialPort serialPort;

    // One-shot press flags per frame
    private Dictionary<string, bool> buttonPressed = new Dictionary<string, bool>()
    {
        { "Z", false },
        { "X", false },
        { "C", false },
        { "V", false }
    };

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    void Start()
    {
        serialPort = new SerialPort("COM5", 115200); // Change COM port
        serialPort.ReadTimeout = 50;
        try
        {
            serialPort.Open();
            Debug.Log("ESP32 Connected");
        }
        catch
        {
            Debug.LogError("Failed to open COM port");
        }
    }

    void Update()
    {
        // Reset one-shot flags at the start of the frame
        List<string> keys = new List<string>(buttonPressed.Keys);
        foreach (var key in keys)
        {
            buttonPressed[key] = false;
        }

        // --- Read ESP32 serial input ---
        if (serialPort != null && serialPort.IsOpen)
        {
            try
            {
                while (serialPort.BytesToRead > 0)
                {
                    string data = serialPort.ReadLine().Trim();
                    if (!string.IsNullOrEmpty(data))
                    {
                        HandleInput(data);
                    }
                }
            }
            catch (System.TimeoutException)
            {
                // Normal if no data yet
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Serial read error: " + e.Message);
            }
        }

        // --- Check keyboard input ---
        CheckKeyboardInput();
    }

    // Handle a single ESP32 input character
    void HandleInput(string data)
    {
        switch (data)
        {
            case "Z": buttonPressed["Z"] = true; break;
            case "X": buttonPressed["X"] = true; break;
            case "C": buttonPressed["C"] = true; break;
            case "V": buttonPressed["V"] = true; break;
            default:
                Debug.LogWarning("Unknown input: " + data);
                break;
        }
    }

    // Map PC keyboard presses to the same buttons
    void CheckKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Z)) buttonPressed["Z"] = true;
        if (Input.GetKeyDown(KeyCode.X)) buttonPressed["X"] = true;
        if (Input.GetKeyDown(KeyCode.C)) buttonPressed["C"] = true;
        if (Input.GetKeyDown(KeyCode.V)) buttonPressed["V"] = true;
    }

    // Public method like Input.GetKeyDown
    public bool GetKeyDown(string key)
    {
        return buttonPressed.ContainsKey(key) && buttonPressed[key];
    }
}