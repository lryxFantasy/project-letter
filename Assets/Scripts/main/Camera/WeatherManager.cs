using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

public class WeatherManager : MonoBehaviour
{
    public CameraController cameraController;

    [Header("������Ч")]
    public GameObject rainEffect;
    public GameObject snowEffect;
    public GameObject radiationEffect;

    [Header("����Volume")]
    public Volume sunnyVolume;
    public Volume rainVolume;
    public Volume snowVolume;
    public Volume radiationVolume;

    public string currentWeather = "sunny";
    private GameObject currentEffect;
    private Volume currentVolume;
    private bool lastIsIndoors;

    void Start()
    {
        lastIsIndoors = cameraController.IsIndoors();
        ApplyWeather("sunny");
    }

    void Update()
    {
        bool isIndoors = cameraController.IsIndoors();
        if (isIndoors != lastIsIndoors)
        {
            lastIsIndoors = isIndoors;

            if (isIndoors)
            {
                ApplyWeather("sunny");
            }
            else
            {
                // ����������ʵļ�Ȩ���ѡ��
                string[] weathers = new string[] { "sunny", "sunny", "rain", "rain","snow", "snow", "radiation" };
                string newWeather = weathers[Random.Range(0, weathers.Length)];
                ApplyWeather(newWeather);
            }
        }
    }

    void ApplyWeather(string weather)
    {
        // �����ǰ��������
        if (currentEffect != null) currentEffect.SetActive(false);
        if (currentVolume != null) currentVolume.gameObject.SetActive(false);

        // ����������
        currentWeather = weather;
        switch (weather)
        {
            case "rain":
                currentEffect = rainEffect;
                currentVolume = rainVolume;
                break;
            case "snow":
                currentEffect = snowEffect;
                currentVolume = snowVolume;
                break;
            case "radiation":
                currentEffect = radiationEffect;
                currentVolume = radiationVolume;
                break;
            case "sunny":
            default:
                currentEffect = null;
                currentVolume = sunnyVolume;
                break;
        }

        // ����������
        if (!cameraController.IsIndoors())
        {
            if (currentEffect != null) currentEffect.SetActive(true);
        }

        if (currentVolume != null) currentVolume.gameObject.SetActive(true);
    }

}
