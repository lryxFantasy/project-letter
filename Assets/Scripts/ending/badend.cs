using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.Video; // 用于视频播放

public class badend : MonoBehaviour
{
    public TMP_Text displayText; // TextMeshPro 的文本组件
    public float fadeDuration = 1f; // 淡入淡出的持续时间
    public float displayDuration = 3f; // 每句话显示的持续时间

    public VideoPlayer videoPlayer; // 视频播放器组件
    public AudioSource backgroundMusic; // 背景音乐的 AudioSource 组件
    public float musicFadeOutDuration = 2f; // 音乐淡出的持续时间

    private string[] sentences = new string[]
    {
        "FANLU从维克托手中接过那把生锈的钥匙，打开了村子上方那座废弃的房屋时，里面堆满了尘封的军用物资。",
        "FANLU发现了一箱破旧的军用防辐射装置零件，虽然残缺不全，但对于简来说，这无疑是一座宝藏。",
        "FANLU将这些零件带回给她，她的目光瞬间亮了起来，像点燃的火花。",
        "她埋头研究了数日，最终成功拼凑出一台便携式防辐射装置的原型。",
        "消息传开后，村民们沸腾了。",
        "简用废墟中搜集的材料不断改进设计，生产出更多便携式装置。",
        "辐射不再限制人们的步伐，村民们第一次走出房屋，呼吸着室外的空气，彼此面对面交谈。",
        "维克托拄着简为他特制的骨骼支架，第一次站直了身体；",
        "伊莱亚斯和简在山坡上重逢，他的诗句终于有了听众；",
        "萝丝和小卢克在村口相认，老人终于卸下“神秘朋友”的面纱；",
        "伊芙则在阳光中完成了那幅“没有战争的明天”。",
        "然而，随着人们走出家门，书信的需求逐渐消失。",
        "FANLU的任务模块再也没有更新，信件背包空空荡荡。",
        "简忙于改进装置，村民们沉浸在重获自由的喜悦中，FANLU被静静地放在社区中心的角落，锈迹在FANLU的铁皮上蔓延。",
        "没人再需要FANLU送信，没人再提起那个“铁皮罐头”。",
        "FANLU的显示屏暗了下来，电路中的微光渐渐熄灭。",
        "便携式防辐射装置让人们重获自由，书信的时代随之终结。",
        "FANLU被废弃在社区中心的角落，静静地凝望着村庄的新生，",
        "直到风吹散了它最后的信号……"
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