using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioClip mapBGM; // ���ⱳ������
    [SerializeField] private AudioClip houseBGM; // ���ڱ�������
    [SerializeField] private float fadeDuration = 1f; // ���뵭������ʱ�䣨�룩
    [SerializeField] private CameraController cameraController; // ���� CameraController

    private AudioSource mapAudioSource;
    private AudioSource houseAudioSource;
    private bool currentIsIndoors = false;
    private bool isFading = false;

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
        mapAudioSource = gameObject.AddComponent<AudioSource>();
        houseAudioSource = gameObject.AddComponent<AudioSource>();

        // ���� AudioSource
        ConfigureAudioSource(mapAudioSource, mapBGM);
        ConfigureAudioSource(houseAudioSource, houseBGM);
    }

    void Start()
    {
        // ���� CameraController
        if (cameraController == null)
        {
            cameraController = FindObjectOfType<CameraController>();
            if (cameraController == null)
            {
                Debug.LogError("AudioManager: CameraController not found in scene!");
                return;
            }
        }

        // ���ݳ�ʼ isIndoors ״̬��������
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

        // ��� isIndoors ״̬�仯
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

    // ���ⲿ���ã�ǿ���л����֣�����������飩
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