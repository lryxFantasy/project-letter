using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    [SerializeField] private AudioClip spacePressSound; // 按下空格键音效
    [SerializeField] private AudioClip spaceReleaseSound; // 松开空格键音效
    [SerializeField] private AudioClip walkSound; // 走路音效
    [SerializeField] private AudioClip doorOpenSound; // 开门音效
    [SerializeField] private AudioClip pickupSound; // 拾取音效（电池和护盾共用）
    [SerializeField] private AudioClip radiationAlarmSound; // 辐射警报音效
    [SerializeField] private float sfxVolume = 0.3f; // 通用音效音量
    [SerializeField] private float walkVolume = 0.7f; // 走路音效音量
    [SerializeField] private float radiationAlarmVolume = 0.5f; // 辐射警报音量

    private AudioSource sfxAudioSource; // 用于空格键、开门、拾取音效
    private AudioSource walkAudioSource; // 用于走路音效
    private AudioSource radiationAudioSource; // 用于辐射警报音效
    private bool sfxEnabled = true; // 控制音效是否启用
    private float radiationAlarmTimer = 0f; // 警报播放计时器
    private float radiationAlarmInterval = 0f; // 当前警报间隔

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
        sfxAudioSource = gameObject.AddComponent<AudioSource>();
        walkAudioSource = gameObject.AddComponent<AudioSource>();
        radiationAudioSource = gameObject.AddComponent<AudioSource>();
        ConfigureSFXAudioSource(sfxAudioSource);
        ConfigureWalkAudioSource(walkAudioSource);
        ConfigureRadiationAudioSource(radiationAudioSource);
    }

    private void ConfigureSFXAudioSource(AudioSource source)
    {
        source.loop = false;
        source.playOnAwake = false;
        source.volume = sfxVolume;
    }

    private void ConfigureWalkAudioSource(AudioSource source)
    {
        source.clip = walkSound;
        source.loop = true;
        source.playOnAwake = false;
        source.volume = walkVolume;
    }

    private void ConfigureRadiationAudioSource(AudioSource source)
    {
        source.loop = false; // 警报音效不循环，按间隔播放
        source.playOnAwake = false;
        source.volume = radiationAlarmVolume;
    }

    void Update()
    {
        if (!sfxEnabled) return;

        // 检测空格键按下和松开
        if (Input.GetKeyDown(KeyCode.Space) && spacePressSound != null)
        {
            sfxAudioSource.PlayOneShot(spacePressSound);
        }
        if (Input.GetKeyUp(KeyCode.Space) && spaceReleaseSound != null)
        {
            sfxAudioSource.PlayOneShot(spaceReleaseSound);
        }

        // 更新辐射警报计时器
        if (radiationAlarmInterval > 0)
        {
            radiationAlarmTimer -= Time.deltaTime;
            if (radiationAlarmTimer <= 0 && radiationAlarmSound != null)
            {
                radiationAudioSource.PlayOneShot(radiationAlarmSound);
                radiationAlarmTimer = radiationAlarmInterval; // 重置计时器
            }
        }
    }

    // 播放或停止走路音效
    public void SetWalkSound(bool isWalking)
    {
        if (!sfxEnabled || walkSound == null) return;

        if (isWalking && !walkAudioSource.isPlaying)
        {
            walkAudioSource.Play();
        }
        else if (!isWalking && walkAudioSource.isPlaying)
        {
            walkAudioSource.Stop();
        }
    }

    // 播放开门音效
    public void PlayDoorOpenSound()
    {
        if (!sfxEnabled || doorOpenSound == null) return;

        sfxAudioSource.PlayOneShot(doorOpenSound);
    }

    // 播放拾取音效
    public void PlayPickupSound()
    {
        if (!sfxEnabled || pickupSound == null) return;

        sfxAudioSource.PlayOneShot(pickupSound);
    }

    // 启用/禁用所有音效
    public void SetSFXEnabled(bool enabled)
    {
        sfxEnabled = enabled;
        if (!enabled)
        {
            if (walkAudioSource.isPlaying)
            {
                walkAudioSource.Stop();
            }
            if (radiationAudioSource.isPlaying)
            {
                radiationAudioSource.Stop();
            }
            radiationAlarmTimer = 0f;
            radiationAlarmInterval = 0f;
        }
    }

    // 播放辐射警报音效，根据辐射值调整播放间隔
    public void PlayRadiationAlarm(float radiation, float maxRadiation)
    {
        if (!sfxEnabled || radiationAlarmSound == null) return;

        // 如果辐射值接近 0，停止警报
        if (radiation <= 0.1f)
        {
            StopRadiationAlarm();
            return;
        }

        // 计算播放间隔，从 1秒（低辐射）到 0.05秒（高辐射），使用平方曲线加速变化
        float normalizedRadiation = Mathf.Clamp(radiation / maxRadiation, 0f, 1f);
        float curveFactor = normalizedRadiation * normalizedRadiation; // 平方曲线，高辐射时变化更快
        radiationAlarmInterval = Mathf.Lerp(0.5f, 0.05f, curveFactor);

        // 初始化或更新计时器，确保快速响应新间隔
        if (radiationAlarmTimer <= 0)
        {
            radiationAudioSource.PlayOneShot(radiationAlarmSound);
            radiationAlarmTimer = radiationAlarmInterval;
        }
        else
        {
            // 如果辐射值变化，逐步调整计时器以适应新间隔
            radiationAlarmTimer = Mathf.Min(radiationAlarmTimer, radiationAlarmInterval);
        }
    }

    // 停止辐射警报音效
    public void StopRadiationAlarm()
    {
        if (radiationAudioSource.isPlaying)
        {
            radiationAudioSource.Stop();
        }
        radiationAlarmTimer = 0f;
        radiationAlarmInterval = 0f;
    }
}