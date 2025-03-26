using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance { get; private set; }
    [SerializeField] private Image fadePanel;
    [SerializeField] private float fadeDuration = 0.4f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        // ��ʼ��͸��
        if (fadePanel != null) fadePanel.color = new Color(0, 0, 0, 0);
        else Debug.LogError("FadePanel δ��ֵ��");
    }

    // ���뵭����ִ�лص�
    public IEnumerator FadeToBlack(System.Action onFadeComplete = null)
    {
        if (fadePanel == null)
        {
            Debug.LogWarning("FadePanel δ���ã�ֱ��ִ�лص���");
            onFadeComplete?.Invoke();
            yield break;
        }

        yield return StartCoroutine(Fade(0f, 1f));
        onFadeComplete?.Invoke();
        yield return StartCoroutine(Fade(1f, 0f));
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;
        Color color = fadePanel.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            fadePanel.color = color;
            yield return null;
        }
        color.a = endAlpha;
        fadePanel.color = color;
    }
}