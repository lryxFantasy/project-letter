using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement; // 添加 SceneManager 支持

public class OpeningAnimation : MonoBehaviour
{
    public TextMeshProUGUI textDisplay; // 用于显示所有文字的单一 TextMeshPro 对象
    public SpriteRenderer[] backgroundImages; // 背景图数组，按顺序对应每段文字
    public AudioSource backgroundMusic; // 背景音乐组件
    public float fadeDuration = 2f; // 淡入淡出时间（文字和音乐共用）
    public float typingSpeed = 0.05f; // 打字机速度
    public float displayDuration = 3f; // 每段文字和背景显示的持续时间
    public float moveDistance = 5f; // 背景从左到右移动的距离
    public float musicMaxVolume = 0.5f; // 背景音乐最大音量（0 到 1）

    private Vector2 defaultTextPosition; // 存储文字默认位置
    private string[] storySegments = new string[]
    {
        "很多年以后，当人们回望那场吞噬太阳的浩劫时，\n都会想起2111年那个寻常至极的夜晚……",
        "2111年，3月16日，战争的第20个年头。\n高楼霓虹灯光透过厚重的云层，洒落在城市中。楼宇间车水马龙依旧，磁悬浮列车沿着熟悉的轨道滑行，新闻一如既往的播报着远方的战况。",
        "没有人记得战争是如何开始。据后世的记载，那天，战争失利的一方向太阳发射了代号为“残阳”的导弹，试图干扰对方的卫星通讯。\n这引发了太阳黑子的连锁反应，人类文明无可挽回的滑向厄运的深渊。",
        "强烈的辐射风暴席卷地球，全球通讯系统崩溃，互联网彻底消失，人类从此失去了太阳的庇护。",
        "这一事件，史称“启示录战争”。",
        "曾经辉煌的都市变得寂静，人们再无法在阳光下行走，只能蜷缩在遮蔽辐射的房屋中。\n土地上重新爬满了茂密的植被，淹没了人类的钢铁丛林……",
        "书信，人类最原始的交流方式成为了链接人们的唯一手段。\nFANLU-317——过去造价低廉的家用AI机器人，如今成为人类之间沟通的桥梁，代替着人们传递手写信件。一封封信件被AI机器人送往各个角落，如同一根无形的绳索，把分隔的世界再度连在一起。",
        "而我们的故事发生在 “启示录战争”结束30年后。",
        "一个偏远的山村——信火村。"
    };

    void Start()
    {
        // 初始化：隐藏所有背景图并设置透明度为 0
        foreach (SpriteRenderer bg in backgroundImages)
        {
            bg.enabled = true;
            Color color = bg.color;
            color.a = 0f;
            bg.color = color;
            bg.transform.localPosition = new Vector3(-moveDistance, 0, 0);
        }
        // 初始化文字透明度为 0 并记录默认位置
        Color textColor = textDisplay.color;
        textColor.a = 0f;
        textDisplay.color = textColor;
        defaultTextPosition = textDisplay.rectTransform.anchoredPosition;

        // 初始化背景音乐
        if (backgroundMusic != null)
        {
            backgroundMusic.volume = musicMaxVolume; // 初始音量为 0
            backgroundMusic.Play(); // 开始播放
        }

        StartCoroutine(PlaySequence());
    }

    void Update()
    {
        // 按 Esc 键跳过动画并跳转场景
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
            // 背景淡入并移动到中心
            yield return StartCoroutine(FadeInBackground(backgroundImages[i]));

            // 根据段落调整文字位置并选择效果
            if (i == 0 || i >= storySegments.Length - 2) // 第一段和最后两段
            {
                textDisplay.rectTransform.anchoredPosition = Vector2.zero; // 居中
                textDisplay.text = storySegments[i];
                yield return StartCoroutine(FadeInText(textDisplay));
                yield return new WaitForSeconds(displayDuration);
                yield return StartCoroutine(FadeOutText(textDisplay));
            }
            else // 其他段落使用打字机效果
            {
                textDisplay.rectTransform.anchoredPosition = defaultTextPosition; // 恢复默认位置
                textDisplay.text = ""; // 清空文字
                yield return StartCoroutine(TypeText(textDisplay, storySegments[i]));
                yield return new WaitForSeconds(displayDuration);
                yield return StartCoroutine(FadeOutText(textDisplay));
            }

            // 背景淡出并继续向右移动
            yield return StartCoroutine(FadeOutBackground(backgroundImages[i]));

            // 短暂停顿，准备下一段
            yield return new WaitForSeconds(1f);
        }

        // 音乐淡出（动画结束后）
        if (backgroundMusic != null)
        {
            yield return StartCoroutine(FadeOutMusic());
            backgroundMusic.Stop(); // 停止播放
        }

        // 动画正常结束后跳转到 "start" 场景
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
        color.a = 1f; // 打字机效果时直接可见
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
        Vector3 startPos = new Vector3(-moveDistance, 0, 0); // 从左侧开始
        Vector3 endPos = new Vector3(0, 0, 0); // 移动到中心
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;
            color.a = Mathf.Lerp(0f, 1f, t); // 淡入
            bg.color = color;
            bg.transform.localPosition = Vector3.Lerp(startPos, endPos, t); // 从左到中心
            yield return null;
        }
        color.a = 1f;
        bg.color = color;
        bg.transform.localPosition = endPos;
    }

    IEnumerator FadeOutBackground(SpriteRenderer bg)
    {
        Color color = bg.color;
        Vector3 startPos = new Vector3(0, 0, 0); // 从中心开始
        Vector3 endPos = new Vector3(moveDistance, 0, 0); // 移动到右侧
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;
            color.a = Mathf.Lerp(1f, 0f, t); // 淡出
            bg.color = color;
            bg.transform.localPosition = Vector3.Lerp(startPos, endPos, t); // 从中心到右
            yield return null;
        }
        color.a = 0f;
        bg.color = color;
        bg.transform.localPosition = new Vector3(-moveDistance, 0, 0); // 重置到左侧
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