using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement; // ���ڳ�����ת
using UnityEngine.Audio; // ������Ƶ���ƣ���ѡ��

public class badend : MonoBehaviour
{
    public TMP_Text displayText; // TextMeshPro ���ı����
    public float fadeDuration = 1f; // ���뵭���ĳ���ʱ��
    public float displayDuration = 3f; // ÿ�仰��ʾ�ĳ���ʱ��

    public RectTransform imageTransform; // �ƶ���ͼƬ
    public float moveDuration = 100f; // ͼƬ���µ����ƶ�����ʱ��
    private float startY; // ͼƬ��ʼ Y λ��
    private float endY;   // ͼƬ���� Y λ��

    public AudioSource backgroundMusic; // �������ֵ� AudioSource ���
    public float musicFadeOutDuration = 2f; // ���ֵ����ĳ���ʱ��

    private string[] sentences = new string[]
    {
        "FANLU��ά�������нӹ��ǰ������Կ�ף����˴����Ϸ����������ķ���ʱ����������˳���ľ������ʡ�",
        "FANLU������һ���ƾɵľ��÷�����װ���������Ȼ��ȱ��ȫ�������ڼ���˵����������һ�����ء�",
        "FANLU����Щ������ظ���������Ŀ��˲���������������ȼ�Ļ𻨡�",
        "����ͷ�о������գ����ճɹ�ƴ�ճ�һ̨��Яʽ������װ�õ�ԭ�͡�",
        "��Ϣ�����󣬴����Ƿ����ˡ�",
        "���÷������Ѽ��Ĳ��ϲ��ϸĽ���ƣ������������Яʽװ�á�",
        "���䲻���������ǵĲ����������ǵ�һ���߳����ݣ�����������Ŀ������˴�����潻̸��",
        "ά�������ż�Ϊ�����ƵĹ���֧�ܣ���һ��վֱ�����壻",
        "������˹�ͼ���ɽ�����ط꣬����ʫ�������������ڣ�",
        "��˿��С¬���ڴ�����ϣ���������ж�¡��������ѡ�����ɴ��",
        "��ܽ����������������Ƿ���û��ս�������족��",
        "Ȼ�������������߳����ţ����ŵ���������ʧ��",
        "FANLU������ģ����Ҳû�и��£��ż������տյ�����",
        "��æ�ڸĽ�װ�ã������ǳ������ػ����ɵ�ϲ���У�FANLU�������ط����������ĵĽ��䣬�⼣��FANLU����Ƥ�����ӡ�",
        "û������ҪFANLU���ţ�û���������Ǹ�����Ƥ��ͷ����",
        "FANLU����ʾ��������������·�е�΢�⽥��Ϩ��",
        "��Яʽ������װ���������ػ����ɣ����ŵ�ʱ����֮�սᡣ",
        "FANLU���������������ĵĽ��䣬�����������Ŵ�ׯ��������",
        "ֱ���紵ɢ���������źš���"
    };

    void Start()
    {
        // ȷ������Ѹ�ֵ
        if (displayText == null || imageTransform == null)
        {
            Debug.LogError("���� Inspector �и�ֵ displayText �� imageTransform��");
            return;
        }
        if (backgroundMusic == null)
        {
            Debug.LogError("���� Inspector �и�ֵ backgroundMusic��");
            return;
        }

        // ����ͼƬ����ʼ�ͽ���λ��
        float imageHeight = imageTransform.rect.height; // ͼƬ�߶�
        startY = -Screen.height / 2f + imageHeight/3f; // ����Ļ�ײ���ʼ
        endY = Screen.height / 2f - imageHeight / 4f;    // �ƶ�����Ļ����
        imageTransform.anchoredPosition = new Vector2(imageTransform.anchoredPosition.x, startY);

        // ���ű�������
        backgroundMusic.Play();

        // ��ʼ�ı����뵭����ͼƬ�ƶ�
        StartCoroutine(DisplaySentences());
        StartCoroutine(MoveImage());
    }

    IEnumerator DisplaySentences()
    {
        foreach (string sentence in sentences)
        {
            displayText.text = sentence;

            // ����
            yield return StartCoroutine(FadeIn());

            // ��ʾһ��ʱ��
            yield return new WaitForSeconds(displayDuration);

            // ����
            yield return StartCoroutine(FadeOut());
        }
    }

    IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        Color color = displayText.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            displayText.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }
    }

    IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        Color color = displayText.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            displayText.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }
    }

    IEnumerator MoveImage()
    {
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;
            float newY = Mathf.Lerp(startY, endY, t);
            imageTransform.anchoredPosition = new Vector2(imageTransform.anchoredPosition.x, newY);
            yield return null;
        }

        // ȷ������λ�þ�ȷ
        imageTransform.anchoredPosition = new Vector2(imageTransform.anchoredPosition.x, endY);

        // ͼƬ�ƶ���ɺ󵭳����ֲ���ת����
        yield return StartCoroutine(FadeOutMusic());
        SceneManager.LoadScene("start"); // ��ת�� Start ����
    }

    IEnumerator FadeOutMusic()
    {
        float elapsedTime = 0f;
        float startVolume = backgroundMusic.volume;

        while (elapsedTime < musicFadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / musicFadeOutDuration;
            backgroundMusic.volume = Mathf.Lerp(startVolume, 0f, t);
            yield return null;
        }

        // ȷ����������Ϊ 0 ��ֹͣ����
        backgroundMusic.volume = 0f;
        backgroundMusic.Stop();
    }
}