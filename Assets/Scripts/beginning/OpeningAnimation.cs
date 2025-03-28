using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement; // ��� SceneManager ֧��

public class OpeningAnimation : MonoBehaviour
{
    public TextMeshProUGUI textDisplay; // ������ʾ�������ֵĵ�һ TextMeshPro ����
    public SpriteRenderer[] backgroundImages; // ����ͼ���飬��˳���Ӧÿ������
    public AudioSource backgroundMusic; // �����������
    public float fadeDuration = 2f; // ���뵭��ʱ�䣨���ֺ����ֹ��ã�
    public float typingSpeed = 0.05f; // ���ֻ��ٶ�
    public float displayDuration = 3f; // ÿ�����ֺͱ�����ʾ�ĳ���ʱ��
    public float moveDistance = 5f; // �����������ƶ��ľ���
    public float musicMaxVolume = 0.5f; // �����������������0 �� 1��

    private Vector2 defaultTextPosition; // �洢����Ĭ��λ��
    private string[] storySegments = new string[]
    {
        "�ܶ����Ժ󣬵����ǻ����ǳ�����̫���ĺƽ�ʱ��\n��������2111���Ǹ�Ѱ��������ҹ����",
        "2111�꣬3��16�գ�ս���ĵ�20����ͷ��\n��¥�޺�ƹ�͸�����ص��Ʋ㣬�����ڳ����С�¥��䳵ˮ�������ɣ��������г�������Ϥ�Ĺ�����У�����һ������Ĳ�����Զ����ս����",
        "û���˼ǵ�ս������ο�ʼ���ݺ����ļ��أ����죬ս��ʧ����һ����̫�������˴���Ϊ���������ĵ�������ͼ���ŶԷ�������ͨѶ��\n��������̫�����ӵ�������Ӧ�����������޿���صĻ�����˵���Ԩ��",
        "ǿ�ҵķ���籩ϯ�����ȫ��ͨѶϵͳ������������������ʧ������Ӵ�ʧȥ��̫���ıӻ���",
        "��һ�¼���ʷ�ơ���ʾ¼ս������",
        "�����Ի͵Ķ��б�üž����������޷������������ߣ�ֻ���������ڱη���ķ����С�\n����������������ï�ܵ�ֲ������û������ĸ������֡���",
        "���ţ�������ԭʼ�Ľ�����ʽ��Ϊ���������ǵ�Ψһ�ֶΡ�\nFANLU-317������ȥ��۵����ļ���AI�����ˣ�����Ϊ����֮�乵ͨ�����������������Ǵ�����д�ż���һ����ż���AI�����������������䣬��ͬһ�����ε��������ѷָ��������ٶ�����һ��",
        "�����ǵĹ��·����� ����ʾ¼ս��������30���",
        "һ��ƫԶ��ɽ�塪���Ż�塣"
    };

    void Start()
    {
        // ��ʼ�����������б���ͼ������͸����Ϊ 0
        foreach (SpriteRenderer bg in backgroundImages)
        {
            bg.enabled = true;
            Color color = bg.color;
            color.a = 0f;
            bg.color = color;
            bg.transform.localPosition = new Vector3(-moveDistance, 0, 0);
        }
        // ��ʼ������͸����Ϊ 0 ����¼Ĭ��λ��
        Color textColor = textDisplay.color;
        textColor.a = 0f;
        textDisplay.color = textColor;
        defaultTextPosition = textDisplay.rectTransform.anchoredPosition;

        // ��ʼ����������
        if (backgroundMusic != null)
        {
            backgroundMusic.volume = musicMaxVolume; // ��ʼ����Ϊ 0
            backgroundMusic.Play(); // ��ʼ����
        }

        StartCoroutine(PlaySequence());
    }

    void Update()
    {
        // �� Esc ��������������ת����
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StopAllCoroutines();
            if (backgroundMusic != null) backgroundMusic.Stop();
            SceneManager.LoadScene("start");
        }
    }

    IEnumerator PlaySequence()
    {


        for (int i = 0; i < storySegments.Length; i++)
        {
            // �������벢�ƶ�������
            yield return StartCoroutine(FadeInBackground(backgroundImages[i]));

            // ���ݶ����������λ�ò�ѡ��Ч��
            if (i == 0 || i >= storySegments.Length - 2) // ��һ�κ��������
            {
                textDisplay.rectTransform.anchoredPosition = Vector2.zero; // ����
                textDisplay.text = storySegments[i];
                yield return StartCoroutine(FadeInText(textDisplay));
                yield return new WaitForSeconds(displayDuration);
                yield return StartCoroutine(FadeOutText(textDisplay));
            }
            else // ��������ʹ�ô��ֻ�Ч��
            {
                textDisplay.rectTransform.anchoredPosition = defaultTextPosition; // �ָ�Ĭ��λ��
                textDisplay.text = ""; // �������
                yield return StartCoroutine(TypeText(textDisplay, storySegments[i]));
                yield return new WaitForSeconds(displayDuration);
                yield return StartCoroutine(FadeOutText(textDisplay));
            }

            // �������������������ƶ�
            yield return StartCoroutine(FadeOutBackground(backgroundImages[i]));

            // ����ͣ�٣�׼����һ��
            yield return new WaitForSeconds(1f);
        }

        // ���ֵ���������������
        if (backgroundMusic != null)
        {
            yield return StartCoroutine(FadeOutMusic());
            backgroundMusic.Stop(); // ֹͣ����
        }

        // ����������������ת�� "start" ����
        SceneManager.LoadScene("start");
    }

    IEnumerator FadeInText(TextMeshProUGUI textObj)
    {
        Color color = textObj.color;
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime*2;
            color.a = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            textObj.color = color;
            yield return null;
        }
        color.a = 1f;
        textObj.color = color;
    }

    IEnumerator FadeOutText(TextMeshProUGUI textObj)
    {
        Color color = textObj.color;
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            textObj.color = color;
            yield return null;
        }
        color.a = 0f;
        textObj.color = color;
    }

    IEnumerator TypeText(TextMeshProUGUI textObj, string fullText)
    {
        Color color = textObj.color;
        color.a = 1f; // ���ֻ�Ч��ʱֱ�ӿɼ�
        textObj.color = color;

        foreach (char letter in fullText.ToCharArray())
        {
            textObj.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    IEnumerator FadeInBackground(SpriteRenderer bg)
    {
        Color color = bg.color;
        Vector3 startPos = new Vector3(-moveDistance, 0, 0); // ����࿪ʼ
        Vector3 endPos = new Vector3(0, 0, 0); // �ƶ�������
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;
            color.a = Mathf.Lerp(0f, 1f, t); // ����
            bg.color = color;
            bg.transform.localPosition = Vector3.Lerp(startPos, endPos, t); // ��������
            yield return null;
        }
        color.a = 1f;
        bg.color = color;
        bg.transform.localPosition = endPos;
    }

    IEnumerator FadeOutBackground(SpriteRenderer bg)
    {
        Color color = bg.color;
        Vector3 startPos = new Vector3(0, 0, 0); // �����Ŀ�ʼ
        Vector3 endPos = new Vector3(moveDistance, 0, 0); // �ƶ����Ҳ�
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;
            color.a = Mathf.Lerp(1f, 0f, t); // ����
            bg.color = color;
            bg.transform.localPosition = Vector3.Lerp(startPos, endPos, t); // �����ĵ���
            yield return null;
        }
        color.a = 0f;
        bg.color = color;
        bg.transform.localPosition = new Vector3(-moveDistance, 0, 0); // ���õ����
    }



    IEnumerator FadeOutMusic()
    {
        float elapsedTime = 0f;
        float startVolume = backgroundMusic.volume;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            backgroundMusic.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / fadeDuration);
            yield return null;
        }
        backgroundMusic.volume = 0f;
    }
}