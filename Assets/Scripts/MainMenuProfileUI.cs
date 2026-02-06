using TMPro;
using UnityEngine;

public class MainMenuProfileUI : MonoBehaviour
{
    public TextMeshProUGUI playerNameText;

    void Start()
    {
        if (ProfileManager.Instance.currentProfile != null)
        {
            playerNameText.text = "Welcome, " +
                ProfileManager.Instance.currentProfile.playerName;
        }
    }
}
