using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ProfileSlotUI : MonoBehaviour
{
    public TextMeshProUGUI slotText;
    public Button button;
    public Button deleteButton;

    [HideInInspector]
    public PlayerProfile profile;

    private ProfileSelectManager manager;

    public void Init(PlayerProfile profile, ProfileSelectManager manager)
    {
        this.profile = profile;
        this.manager = manager;

        if (profile == null)
        {
            slotText.text = "Create New Profile";
            deleteButton.gameObject.SetActive(false);
        }
        else
        {
            slotText.text = profile.playerName;
            deleteButton.gameObject.SetActive(true);
            deleteButton.onClick.RemoveAllListeners();
            deleteButton.onClick.AddListener(OnDeleteClicked);
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => manager.OnSlotClicked(this));
    }

    private void OnDeleteClicked()
    {
        if (profile != null && manager != null)
        {
            manager.ConfirmDelete(profile);
        }
    }
}
