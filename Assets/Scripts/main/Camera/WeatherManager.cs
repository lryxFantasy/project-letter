using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

public class WeatherManager : MonoBehaviour
{
    public CameraController cameraController;

    [Header("粒子特效")]
    public GameObject rainEffect;
    public GameObject snowEffect;
    public GameObject radiationEffect;

    [Header("后处理Volume")]
    public Volume sunnyVolume;
    public Volume rainVolume;
    public Volume snowVolume;
    public Volume radiationVolume;

    private string currentWeather = "sunny";
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
                string[] weathers = new string[] { "sunny","rain", "snow", "radiation" };
                string newWeather = weathers[Random.Range(0, weathers.Length)];
                ApplyWeather(newWeather);
            }
        }
    }

    void ApplyWeather(string weather)
    {
        // 清除当前天气粒子
        if (currentEffect != null) currentEffect.SetActive(false);
        if (currentVolume != null) currentVolume.gameObject.SetActive(false);

        // 设置新天气
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

        // 启用新天气
        if (!cameraController.IsIndoors())
        {
            if (currentEffect != null) currentEffect.SetActive(true);
        }

        if (currentVolume != null) currentVolume.gameObject.SetActive(true);
    }

}
