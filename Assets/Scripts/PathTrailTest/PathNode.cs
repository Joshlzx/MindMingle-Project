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
    public Image indicatorGreen;

    [HideInInspector]
    public string nodeValue;

    private Coroutine pulseCoroutine;
    private Coroutine clickCoroutine;
    private Color originalColor;

    void Awake()
    {
        originalColor = mainCircle.color;
        ResetIndicators();
    }

    public void Setup(string value)
    {
        nodeValue = value;
        if (label != null)
            label.text = value;
    }

    public void Highlight(Color color)
    {
        mainCircle.color = color;
    }

    public void StartCurrentHighlight()
    {
        StopCurrentHighlight();
        pulseCoroutine = StartCoroutine(PulseEffect());
    }

    public void StopCurrentHighlight()
    {
        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
            pulseCoroutine = null;
        }
        mainCircle.color = originalColor;
    }

    IEnumerator PulseEffect()
    {
        float speed = 2f; // pulse speed

        Color baseColor = Color.yellow;       // high-contrast color
        Color pulseColor = new Color(1f, 0.6f, 0f); // orange-like for strong pulse

        Vector3 originalScale = mainCircle.rectTransform.localScale;
        Vector3 pulseScale = originalScale * 1.2f; // slightly larger for visibility

        while (true)
        {
            // Lerp color
            float t = (Mathf.Sin(Time.time * speed) + 1f) / 2f;
            mainCircle.color = Color.Lerp(baseColor, pulseColor, t);

            // Lerp scale for extra visibility
            mainCircle.rectTransform.localScale = Vector3.Lerp(originalScale, pulseScale, t);

            yield return null;
        }
    }

    public void ShowIndicator(int index)
    {
        ResetIndicators();

        indicatorRed.gameObject.SetActive(index == 0);
        indicatorBlue.gameObject.SetActive(index == 1);
        indicatorYellow.gameObject.SetActive(index == 2);
        indicatorGreen.gameObject.SetActive(index == 3);

        Image active = GetActiveIndicator();
        if (active != null)
            clickCoroutine = StartCoroutine(ClickAnimation(active));
    }

    public void ResetIndicators()
    {
        mainCircle.color = originalColor; 

        indicatorRed.gameObject.SetActive(false);
        indicatorBlue.gameObject.SetActive(false);
        indicatorYellow.gameObject.SetActive(false);
        indicatorGreen.gameObject.SetActive(false);
    }

    Image GetActiveIndicator()
    {
        if (indicatorRed.gameObject.activeSelf) return indicatorRed;
        if (indicatorBlue.gameObject.activeSelf) return indicatorBlue;
        if (indicatorYellow.gameObject.activeSelf) return indicatorYellow;
        if (indicatorGreen.gameObject.activeSelf) return indicatorGreen;
        return null;
    }

    IEnumerator ClickAnimation(Image img)
    {
        Vector3 originalScale = Vector3.one;
        Vector3 enlarged = originalScale * 1.3f;
        float duration = 0.1f;

        float t = 0f;
        while (t < duration)
        {
            img.rectTransform.localScale = Vector3.Lerp(originalScale, enlarged, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        img.rectTransform.localScale = enlarged;

        t = 0f;
        while (t < duration)
        {
            img.rectTransform.localScale = Vector3.Lerp(enlarged, originalScale, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        img.rectTransform.localScale = originalScale;
    }
}