using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement; // ���ڳ�����ת
using UnityEngine.Audio; // ������Ƶ���ƣ���ѡ��

public class FadeTextAndMoveImage : MonoBehaviour
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
        "��FANLU�������������ݣ��ҵ����÷�����װ�����ʱ��FANLU�����Ǵ����˼�",
        "��Ͷ���о����ɹ������˱�Яʽ������װ�ã������߳����ţ������ž�Υ�����ɡ�",
        "Ц���ڴ�����ص�����һ�Σ�����û������FANLU��",
        "�����ŵ������FANLU����������ֽҳ�������������ǵ���С�",
        "ά��������FANLU����Ƥ˵�������������ӣ���������Ŀ��ס�",
        "������˹ΪFANLUд�¡��������Ľ��졷����FANLU�ǡ����濪���Ļ�����",
        "��˿Ц����FANLU���Դ���˵FANLU�������ӵ�Ӱ�ӣ�",
        "С¬�˸�FANLU����һ���������е�FANLU��һ�����ˣ�",
        "��ܽ������д����FANLU�Ǵ�������æµ��ɫ�ʡ�",
        "�����Դǣ�ȴ���޺�FANLUʱ����˵����һ�ж�лл����",
        "��Щ�����FANLU����ֻ�ǹ��ߣ������Ż���һ���֡�",
        "������װ�������ǿ��Լ��棬�����Ƿ��������е��¶��޷������",
        "ά���к�������˹һֱ����д�ţ����Ż����˸��ӵ�ì�ܣ�",
        "������˹�ͼ�Լ�����ż���˵�Լ���������",
        "��˿��С¬�˼����ԡ��������ѡ��ķ�ʽͨ�ţ������Ƿ�Ĭ����",
        "��ܽ���ż���¼������У�����ȫ���˼�ȥ�Լ��Ļ�����",
        "�����Ǿ�������FANLU��Ϊ��ʹ�����������װ�ö���������",
        "��ΪFANLU�����������FANLU������ģ����Ȼ��˸���µ�Ŀ�ꡣ",
        "FANLU�����ڴ��У����ŵĽŲ���δͣ�£�ÿ���ű������������ڷ������б�",
        "�ڷ���֮�ϣ�FANLU�ҵ����Լ������塪���������ǹ��ߣ������Ż�Ĵ����ߡ�",
        "THE END"
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
        startY = -Screen.height / 2f + imageHeight / 2.8f; // ����Ļ�ײ���ʼ
        endY = Screen.height / 2f - imageHeight / 2.9f;    // �ƶ�����Ļ����
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