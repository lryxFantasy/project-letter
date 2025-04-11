using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    [SerializeField] private AudioClip spacePressSound; // 按下空格键音效
    [SerializeField] private AudioClip spaceReleaseSound; // 松开空格键音效
    [SerializeField] private AudioClip walkSound; // 走路音效
    [SerializeField] private AudioClip doorOpenSound; // 开门音效
    [SerializeField] private float sfxVolume = 0.7f; // 通用音效音量
    [SerializeField] private float walkVolume = 0.5f; // 走路音效音量（稍低）

    private AudioSource sfxAudioSource; // 用于空格键和开门音效
    private AudioSource walkAudioSource; // 用于走路音效
    private bool sfxEnabled = true; // 控制音效是否启用

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
        ConfigureSFXAudioSource(sfxAudioSource);
        ConfigureWalkAudioSource(walkAudioSource);
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

    // 启用/禁用所有音效
    public void SetSFXEnabled(bool enabled)
    {
        sfxEnabled = enabled;
        if (!enabled && walkAudioSource.isPlaying)
        {
            walkAudioSource.Stop(); // 禁用时停止走路音效
        }
    }
}