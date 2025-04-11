using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    [SerializeField] private AudioClip spacePressSound; // ���¿ո����Ч
    [SerializeField] private AudioClip spaceReleaseSound; // �ɿ��ո����Ч
    [SerializeField] private AudioClip walkSound; // ��·��Ч
    [SerializeField] private AudioClip doorOpenSound; // ������Ч
    [SerializeField] private float sfxVolume = 0.7f; // ͨ����Ч����
    [SerializeField] private float walkVolume = 0.5f; // ��·��Ч�������Եͣ�

    private AudioSource sfxAudioSource; // ���ڿո���Ϳ�����Ч
    private AudioSource walkAudioSource; // ������·��Ч
    private bool sfxEnabled = true; // ������Ч�Ƿ�����

    void Awake()
    {
        // ���õ���
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

        // ��ʼ�� AudioSource
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

        // ���ո�����º��ɿ�
        if (Input.GetKeyDown(KeyCode.Space) && spacePressSound != null)
        {
            sfxAudioSource.PlayOneShot(spacePressSound);
        }
        if (Input.GetKeyUp(KeyCode.Space) && spaceReleaseSound != null)
        {
            sfxAudioSource.PlayOneShot(spaceReleaseSound);
        }
    }

    // ���Ż�ֹͣ��·��Ч
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

    // ���ſ�����Ч
    public void PlayDoorOpenSound()
    {
        if (!sfxEnabled || doorOpenSound == null) return;

        sfxAudioSource.PlayOneShot(doorOpenSound);
    }

    // ����/����������Ч
    public void SetSFXEnabled(bool enabled)
    {
        sfxEnabled = enabled;
        if (!enabled && walkAudioSource.isPlaying)
        {
            walkAudioSource.Stop(); // ����ʱֹͣ��·��Ч
        }
    }
}