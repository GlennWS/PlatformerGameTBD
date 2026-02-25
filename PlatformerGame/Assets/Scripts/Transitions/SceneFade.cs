using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SceneFade : MonoBehaviour
{
    [SerializeField] private Image faderImage;

    [Header("Settings")]
    [SerializeField] private Color fadeColor = Color.black;
    [SerializeField] private float fadeDuration = 0.5f;

    private void Awake()
    {
        if (faderImage == null)
        {
            Debug.LogError("SceneFade is missing a reference to the Fader Image component.");
            return;
        }

        Canvas rootCanvas = faderImage.GetComponentInParent<Canvas>();

        if (rootCanvas != null)
        {
            rootCanvas.transform.SetParent(null);
            DontDestroyOnLoad(rootCanvas.gameObject);
        }
    }

    private void Start()
    {
        if (faderImage != null)
        {
            StartCoroutine(FadeToClear());
        }
    }

    public IEnumerator FadeToBlack()
    {
        if (faderImage == null) yield break;

        faderImage.gameObject.SetActive(true);
        faderImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0f);

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            faderImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
            yield return null;
        }

        faderImage.color = fadeColor;
    }

    public IEnumerator FadeToClear()
    {
        if (faderImage == null) yield break;

        faderImage.gameObject.SetActive(true);
        faderImage.color = fadeColor;

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            faderImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
            yield return null;
        }

        faderImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0f);
        faderImage.gameObject.SetActive(false);
    }
}