using UnityEngine;
using System.IO.Ports;
using System.Collections.Generic;

public class ESP32InputManager : MonoBehaviour
{
    public static ESP32InputManager Instance;

    SerialPort serialPort;

    
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
        serialPort = new SerialPort("COM5", 115200); 
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
        
        List<string> keys = new List<string>(buttonPressed.Keys);
        foreach (var key in keys)
        {
            buttonPressed[key] = false;
        }

        
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

        
        CheckKeyboardInput();
    }

    
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

    
    void CheckKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Z)) buttonPressed["Z"] = true;
        if (Input.GetKeyDown(KeyCode.X)) buttonPressed["X"] = true;
        if (Input.GetKeyDown(KeyCode.C)) buttonPressed["C"] = true;
        if (Input.GetKeyDown(KeyCode.V)) buttonPressed["V"] = true;
    }

    
    public bool GetKeyDown(string key)
    {
        return buttonPressed.ContainsKey(key) && buttonPressed[key];
    }
}