using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public Vector3[] housePositions;
    public Vector3[] housePlayerPositions;
    public bool isIndoors = false;
    public int currentHouseIndex = -1;
    private Vector3 mapPosition;
    public Vector3 lastPlayerMapPosition;
    private Vector3 teleportPosition = new Vector3(-7.3f, -2.5f, -6.1f);
    private bool isTransitioning = false;
    private bool isInitialExit = true;

    void Start()
    {
        if (housePositions == null || housePositions.Length == 0)
        {
            housePositions = new Vector3[6];
            housePositions[0] = new Vector3(100, 0, -10);
            housePositions[1] = new Vector3(200, 0, -10);
            housePositions[2] = new Vector3(300, 0, -10);
            housePositions[3] = new Vector3(400, 0, -10);
            housePositions[4] = new Vector3(500, 0, -10);
            housePositions[5] = new Vector3(600, 0, -10);
        }

        if (housePlayerPositions == null || housePlayerPositions.Length == 0)
        {
            housePlayerPositions = new Vector3[6];
            housePlayerPositions[0] = new Vector3(100, 0, 0);
            housePlayerPositions[1] = new Vector3(200, 0, 0);
            housePlayerPositions[2] = new Vector3(300, 0, 0);
            housePlayerPositions[3] = new Vector3(400, 0, 0);
            housePlayerPositions[4] = new Vector3(500, 0, 0);
            housePlayerPositions[5] = new Vector3(600, 0, 0);
        }

        mapPosition = transform.position;
        isIndoors = true;
        lastPlayerMapPosition = teleportPosition;
    }

    void LateUpdate()
    {
        if (target != null && !isIndoors)
        {
            Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z) + offset;
            transform.position = targetPosition;
            lastPlayerMapPosition = target.position;
        }
    }

    public void EnterHouse(int houseIndex)
    {
        if (houseIndex >= 0 && houseIndex < housePositions.Length)
        {
            if (!isTransitioning)
            {
                StartCoroutine(FadeToHouse(houseIndex));
                if (SFXManager.Instance != null)
                {
                    SFXManager.Instance.PlayDoorOpenSound(); // 进入房屋时播放开门音效
                }
            }
        }
    }

    public void ExitHouse()
    {
        if (isIndoors && !isTransitioning)
        {
            StartCoroutine(FadeToMap());
            if (SFXManager.Instance != null)
            {
                SFXManager.Instance.PlayDoorOpenSound(); // 进入房屋时播放开门音效
            }
        }
    }

    private IEnumerator FadeToHouse(int houseIndex)
    {
        isTransitioning = true;
        yield return StartCoroutine(FadeManager.Instance.FadeToBlack(() =>
        {
            transform.position = housePositions[houseIndex];
            target.position = housePlayerPositions[houseIndex];
            isIndoors = true;
            currentHouseIndex = houseIndex;
        }));
        isTransitioning = false;
    }

    private IEnumerator FadeToMap()
    {
        isTransitioning = true;
        yield return StartCoroutine(FadeManager.Instance.FadeToBlack(() =>
        {
            isIndoors = false;
            if (isInitialExit)
            {
                target.position = teleportPosition;
                isInitialExit = false;
            }
            else if (currentHouseIndex == 1)
            {
                target.position = teleportPosition;
                Debug.Log($"Teleporting to: {teleportPosition}");
            }
            else
            {
                target.position = lastPlayerMapPosition;
                target.position = new Vector3(target.position.x, target.position.y - 1f, target.position.z);
            }
            transform.position = new Vector3(target.position.x, target.position.y, transform.position.z) + offset;
            currentHouseIndex = -1;
        }));
        isTransitioning = false;
    }

    public bool IsIndoors()
    {
        return isIndoors;
    }
}