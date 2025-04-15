using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement; // 用于场景跳转
using UnityEngine.Audio; // 用于音频控制
using UnityEngine.Video; // 用于视频播放

public class FadeTextAndMoveImage : MonoBehaviour
{
    public TMP_Text displayText; // TextMeshPro 的文本组件
    public float fadeDuration = 1f; // 淡入淡出的持续时间
    public float displayDuration = 3f; // 每句话显示的持续时间

    public VideoPlayer videoPlayer; // 视频播放器组件
    public AudioSource backgroundMusic; // 背景音乐的 AudioSource 组件
    public float musicFadeOutDuration = 2f; // 音乐淡出的持续时间

    private string[] sentences = new string[]
    {
        "当FANLU打开那座废弃房屋，找到军用防辐射装置零件时，FANLU将它们带给了简。",
        "简投入研究，成功发明了便携式防辐射装置，人们走出家门，感受着久违的自由。",
        "笑声在村子里回荡，这一次，他们没有忘记FANLU。",
        "在送信的日子里，FANLU不仅传递了纸页，更传递了他们的情感。",
        "维克托拍着FANLU的铁皮说：你这破铁罐子，比我想象的靠谱。",
        "伊莱亚斯为FANLU写下《铁与灵魂的交响》，称FANLU是“废墟开出的花”；",
        "萝丝笑着摸FANLU的脑袋，说FANLU像她孙子的影子；",
        "小卢克给FANLU画了一幅画，画中的FANLU像一个超人；",
        "伊芙在信中写道，FANLU是村子里最忙碌的色彩。",
        "简不善言辞，却在修好FANLU时低声说：这一切都谢谢你了",
        "这些点滴让FANLU不再只是工具，而是信火村的一部分。",
        "防辐射装置让人们可以见面，但他们发现书信中的温度无法替代。",
        "维克托和伊莱亚斯一直互相写信，书信缓解了父子的矛盾；",
        "伊莱亚斯和简约定用信件诉说自己的心声；",
        "萝丝和小卢克继续以“神秘朋友”的方式通信，保持那份默契；",
        "伊芙用信件记录画作灵感，并给全村人寄去自己的画作。",
        "村民们决定保留FANLU作为信使，不因防辐射装置而废弃它。",
        "简为FANLU升级了零件，FANLU的任务模块依然闪烁着新的目标。",
        "FANLU穿梭在村中，送信的脚步从未停下，每封信背后都是人们熠熠发光的情感表达。",
        "在废墟之上，FANLU找到了自己的意义——它不仅是工具，更是信火的传递者。",
        "THE END"
    };

    void Start()
    {
        // 确保组件已赋值
        if (displayText == null)
        {
            Debug.LogError("请在 Inspector 中赋值 displayText！");
            return;
        }
        if (backgroundMusic == null)
        {
            Debug.LogError("请在 Inspector 中赋值 backgroundMusic！");
            return;
        }
        if (videoPlayer == null)
        {
            Debug.LogError("请在 Inspector 中赋值 videoPlayer！");
            return;
        }

        // 播放背景音乐
        backgroundMusic.Play();

        // 设置视频播放器
        videoPlayer.playOnAwake = false;
        videoPlayer.loopPointReached += OnVideoFinished; // 视频播放完成时触发事件

        // 开始文本淡入淡出和视频播放
        StartCoroutine(DisplaySentences());
        videoPlayer.Play();
    }

    IEnumerator DisplaySentences()
    {
        foreach (string sentence in sentences)
        {
            displayText.text = sentence;

            // 淡入
            yield return StartCoroutine(FadeIn());

            // 显示一段时间
            yield return new WaitForSeconds(displayDuration);

            // 淡出
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
        // 视频播放完成后淡出音乐并跳转场景
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

        // 确保音量最终为 0 并停止播放
        backgroundMusic.volume = 0f;
        backgroundMusic.Stop();

        // 跳转到 Start 场景
        SceneManager.LoadScene("start");
    }
}