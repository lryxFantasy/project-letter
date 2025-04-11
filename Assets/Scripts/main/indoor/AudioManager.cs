using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioClip mapBGM; // 屋外背景音乐
    [SerializeField] private AudioClip houseBGM; // 屋内背景音乐
    [SerializeField] private float fadeDuration = 1f; // 淡入淡出持续时间（秒）
    [SerializeField] private CameraController cameraController; // 引用 CameraController

    private AudioSource mapAudioSource;
    private AudioSource houseAudioSource;
    private bool currentIsIndoors = false;
    private bool isFading = false;

    void Awake()
    {
        // 设置单例
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 初始化 AudioSource
        mapAudioSource = gameObject.AddComponent<AudioSource>();
        houseAudioSource = gameObject.AddComponent<AudioSource>();

        // 配置 AudioSource
        ConfigureAudioSource(mapAudioSource, mapBGM);
        ConfigureAudioSource(houseAudioSource, houseBGM);
    }

    void Start()
    {
        // 查找 CameraController
        if (cameraController == null)
        {
            cameraController = FindObjectOfType<CameraController>();
            if (cameraController == null)
            {
                Debug.LogError("AudioManager: CameraController not found in scene!");
                return;
            }
        }

        // 根据初始 isIndoors 状态播放音乐
        currentIsIndoors = cameraController.IsIndoors();
        if (currentIsIndoors)
        {
            houseAudioSource.volume = 1f;
            houseAudioSource.Play();
            mapAudioSource.volume = 0f;
        }
        else
        {
            mapAudioSource.volume = 1f;
            mapAudioSource.Play();
            houseAudioSource.volume = 0f;
        }
    }

    void Update()
    {
        if (isFading) return;

        // 检测 isIndoors 状态变化
        bool newIsIndoors = cameraController.IsIndoors();
        if (newIsIndoors != currentIsIndoors)
        {
            currentIsIndoors = newIsIndoors;
            if (currentIsIndoors)
            {
                StartCoroutine(FadeBGM(mapAudioSource, houseAudioSource));
            }
            else
            {
                StartCoroutine(FadeBGM(houseAudioSource, mapAudioSource));
            }
        }
    }

    private void ConfigureAudioSource(AudioSource source, AudioClip clip)
    {
        source.clip = clip;
        source.loop = true;
        source.playOnAwake = false;
        source.volume = 0f;
    }

    private System.Collections.IEnumerator FadeBGM(AudioSource fadeOutSource, AudioSource fadeInSource)
    {
        isFading = true;

        if (!fadeInSource.isPlaying)
        {
            fadeInSource.Play();
        }

        float time = 0f;
        float startFadeOutVolume = fadeOutSource.volume;
        float startFadeInVolume = fadeInSource.volume;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = time / fadeDuration;

            fadeOutSource.volume = Mathf.Lerp(startFadeOutVolume, 0f, t);
            fadeInSource.volume = Mathf.Lerp(startFadeInVolume, 1f, t);

            yield return null;
        }

        fadeOutSource.volume = 0f;
        fadeInSource.volume = 1f;
        fadeOutSource.Stop();

        isFading = false;
    }

    // 供外部调用，强制切换音乐（用于特殊剧情）
    public void ForceSwitchBGM(bool toIndoors)
    {
        if (toIndoors == currentIsIndoors) return;

        currentIsIndoors = toIndoors;
        if (toIndoors)
        {
            StartCoroutine(FadeBGM(mapAudioSource, houseAudioSource));
        }
        else
        {
            StartCoroutine(FadeBGM(houseAudioSource, mapAudioSource));
        }
    }
}