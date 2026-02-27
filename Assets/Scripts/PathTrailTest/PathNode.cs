using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class PathNode : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI label;
    public Image mainCircle;

    public Image indicatorRed;
    public Image indicatorBlue;
    public Image indicatorYellow;

    [HideInInspector]
    public string nodeValue;

    private Coroutine clickCoroutine;

    void Awake()
    {
        if (mainCircle == null)
            mainCircle = GetComponent<Image>();

        // Disable all indicators by default
        indicatorRed.gameObject.SetActive(false);
        indicatorBlue.gameObject.SetActive(false);
        indicatorYellow.gameObject.SetActive(false);
    }

    public void Setup(string value)
    {
        nodeValue = value;
        if (label != null)
            label.text = value;
    }

    public void Highlight(Color color)
    {
        if (mainCircle != null)
            mainCircle.color = color;
    }

    public void ResetColor()
    {
        if (mainCircle != null)
            mainCircle.color = Color.white;

        StopClickAnimation();

        indicatorRed.gameObject.SetActive(false);
        indicatorBlue.gameObject.SetActive(false);
        indicatorYellow.gameObject.SetActive(false);
    }

    // Show colored indicator above the node with click animation
    // index: 0 = red, 1 = blue, 2 = yellow
    public void ShowIndicator(int index)
    {
        StopClickAnimation();

        indicatorRed.gameObject.SetActive(index == 0);
        indicatorBlue.gameObject.SetActive(index == 1);
        indicatorYellow.gameObject.SetActive(index == 2);

        Image active = GetActiveIndicator();
        if (active != null)
            clickCoroutine = StartCoroutine(ClickAnimation(active));
    }

    private Image GetActiveIndicator()
    {
        if (indicatorRed.gameObject.activeSelf) return indicatorRed;
        if (indicatorBlue.gameObject.activeSelf) return indicatorBlue;
        if (indicatorYellow.gameObject.activeSelf) return indicatorYellow;
        return null;
    }

    private IEnumerator ClickAnimation(Image img)
    {
        Vector3 originalScale = img.rectTransform.localScale;
        Vector3 enlarged = originalScale * 1.3f;
        float duration = 0.1f; // quick click

        // Scale up
        float t = 0f;
        while (t < duration)
        {
            img.rectTransform.localScale = Vector3.Lerp(originalScale, enlarged, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        img.rectTransform.localScale = enlarged;

        // Scale back
        t = 0f;
        while (t < duration)
        {
            img.rectTransform.localScale = Vector3.Lerp(enlarged, originalScale, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        img.rectTransform.localScale = originalScale;
    }

    private void StopClickAnimation()
    {
        if (clickCoroutine != null)
        {
            StopCoroutine(clickCoroutine);
            clickCoroutine = null;
        }

        // Reset scale of all indicators
        indicatorRed.rectTransform.localScale = Vector3.one;
        indicatorBlue.rectTransform.localScale = Vector3.one;
        indicatorYellow.rectTransform.localScale = Vector3.one;
    }
}