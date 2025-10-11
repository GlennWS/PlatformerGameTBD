using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SceneFade : MonoBehaviour
{
    [SerializeField] private Image faderImage;

    [Header("Settings")]
    [SerializeField] private Color fadeColor = Color.black;
    [SerializeField] private float fadeDuration = 0.5f;

    private bool isFadedIn = true;

    private void Awake()
    {
        if (faderImage == null)
        {
            Debug.LogError("SceneFader is missing a reference to the Fader Image component in the Inspector.");
            return;
        }

        Canvas rootCanvas = faderImage.GetComponentInParent<Canvas>();

        if (rootCanvas != null)
        {
            GameObject canvasRoot = rootCanvas.gameObject;
            DontDestroyOnLoad(canvasRoot);
        }
    }

    private void Start()
    {
        if (faderImage != null)
        {
            StartCoroutine(FadeOutCoroutine());
        }
    }
    public IEnumerator FadeInCoroutine()
    {
        if (faderImage == null) yield break;

        faderImage.gameObject.SetActive(true);
        faderImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0f);

        float timer = 0f;
        while (timer < fadeDuration)
        {
            if (faderImage == null) yield break;

            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            faderImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
            yield return null;
        }

        if (faderImage != null)
        {
            faderImage.color = fadeColor;
        }
        isFadedIn = true;
    }
    public IEnumerator FadeOutCoroutine()
    {
        if (faderImage == null)
        {
            isFadedIn = false;
            yield break;
        }

        faderImage.gameObject.SetActive(true);
        faderImage.color = fadeColor;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            if (faderImage == null) yield break;

            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            faderImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
            yield return null;
        }

        if (faderImage != null)
        {
            faderImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0f);
            faderImage.gameObject.SetActive(false);
        }
        isFadedIn = false;
    }
}