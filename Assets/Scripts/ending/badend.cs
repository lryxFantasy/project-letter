using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.Video; // ������Ƶ����

public class badend : MonoBehaviour
{
    public TMP_Text displayText; // TextMeshPro ���ı����
    public float fadeDuration = 1f; // ���뵭���ĳ���ʱ��
    public float displayDuration = 3f; // ÿ�仰��ʾ�ĳ���ʱ��

    public VideoPlayer videoPlayer; // ��Ƶ���������
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
        if (displayText == null)
        {
            Debug.LogError("���� Inspector �и�ֵ displayText��");
            return;
        }
        if (backgroundMusic == null)
        {
            Debug.LogError("���� Inspector �и�ֵ backgroundMusic��");
            return;
        }
        if (videoPlayer == null)
        {
            Debug.LogError("���� Inspector �и�ֵ videoPlayer��");
            return;
        }

        // ���ű�������
        backgroundMusic.Play();

        // ������Ƶ������
        videoPlayer.playOnAwake = false;
        videoPlayer.loopPointReached += OnVideoFinished; // ��Ƶ�������ʱ�����¼�

        // ��ʼ�ı����뵭������Ƶ����
        StartCoroutine(DisplaySentences());
        videoPlayer.Play();
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

    void OnVideoFinished(VideoPlayer vp)
    {
        // ��Ƶ������ɺ󵭳����ֲ���ת����
        StartCoroutine(FadeOutMusic());
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

        // ��ת�� Start ����
        SceneManager.LoadScene("start");
    }
}