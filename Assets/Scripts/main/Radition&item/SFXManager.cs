using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    [SerializeField] private AudioClip spacePressSound; // ���¿ո����Ч
    [SerializeField] private AudioClip spaceReleaseSound; // �ɿ��ո����Ч
    [SerializeField] private AudioClip walkSound; // ��·��Ч
    [SerializeField] private AudioClip doorOpenSound; // ������Ч
    [SerializeField] private AudioClip pickupSound; // ʰȡ��Ч����غͻ��ܹ��ã�
    [SerializeField] private AudioClip radiationAlarmSound; // ���侯����Ч
    [SerializeField] private float sfxVolume = 0.3f; // ͨ����Ч����
    [SerializeField] private float walkVolume = 0.7f; // ��·��Ч����
    [SerializeField] private float radiationAlarmVolume = 0.5f; // ���侯������

    private AudioSource sfxAudioSource; // ���ڿո�������š�ʰȡ��Ч
    private AudioSource walkAudioSource; // ������·��Ч
    private AudioSource radiationAudioSource; // ���ڷ��侯����Ч
    private bool sfxEnabled = true; // ������Ч�Ƿ�����
    private float radiationAlarmTimer = 0f; // �������ż�ʱ��
    private float radiationAlarmInterval = 0f; // ��ǰ�������

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
        source.loop = false; // ������Ч��ѭ�������������
        source.playOnAwake = false;
        source.volume = radiationAlarmVolume;
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

        // ���·��侯����ʱ��
        if (radiationAlarmInterval > 0)
        {
            radiationAlarmTimer -= Time.deltaTime;
            if (radiationAlarmTimer <= 0 && radiationAlarmSound != null)
            {
                radiationAudioSource.PlayOneShot(radiationAlarmSound);
                radiationAlarmTimer = radiationAlarmInterval; // ���ü�ʱ��
            }
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

    // ����ʰȡ��Ч
    public void PlayPickupSound()
    {
        if (!sfxEnabled || pickupSound == null) return;

        sfxAudioSource.PlayOneShot(pickupSound);
    }

    // ����/����������Ч
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

    // ���ŷ��侯����Ч�����ݷ���ֵ�������ż��
    public void PlayRadiationAlarm(float radiation, float maxRadiation)
    {
        if (!sfxEnabled || radiationAlarmSound == null) return;

        // �������ֵ�ӽ� 0��ֹͣ����
        if (radiation <= 0.1f)
        {
            StopRadiationAlarm();
            return;
        }

        // ���㲥�ż������ 1�루�ͷ��䣩�� 0.05�루�߷��䣩��ʹ��ƽ�����߼��ٱ仯
        float normalizedRadiation = Mathf.Clamp(radiation / maxRadiation, 0f, 1f);
        float curveFactor = normalizedRadiation * normalizedRadiation; // ƽ�����ߣ��߷���ʱ�仯����
        radiationAlarmInterval = Mathf.Lerp(0.5f, 0.05f, curveFactor);

        // ��ʼ������¼�ʱ����ȷ��������Ӧ�¼��
        if (radiationAlarmTimer <= 0)
        {
            radiationAudioSource.PlayOneShot(radiationAlarmSound);
            radiationAlarmTimer = radiationAlarmInterval;
        }
        else
        {
            // �������ֵ�仯���𲽵�����ʱ������Ӧ�¼��
            radiationAlarmTimer = Mathf.Min(radiationAlarmTimer, radiationAlarmInterval);
        }
    }

    // ֹͣ���侯����Ч
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