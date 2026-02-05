using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelCompleteUI : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] float displayTime = 1.5f;

    public void Show()
    {
        StartCoroutine(ShowCoroutine());
    }

    private IEnumerator ShowCoroutine()
    {
        panel.SetActive(true);
        yield return new WaitForSeconds(displayTime);
        panel.SetActive(false);
    }
}
