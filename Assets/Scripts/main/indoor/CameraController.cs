using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public Transform target; // ��������Ŀ��
    public Vector3 offset; // ���ƫ��
    public Vector3[] housePositions; // �����ڵ����λ��
    public Vector3[] housePlayerPositions; // �����ڵ����λ��
    public bool isIndoors = false; // �Ƿ�������
    public int currentHouseIndex = -1; // ��ǰ��������
    private Vector3 mapPosition;
    public Vector3 lastPlayerMapPosition; // �������ڵ�ͼ�ϵ�λ��

    void Start()
    {
        // ��ʼ������λ������
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

        // ��ʼ������ڷ����е�λ������
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
    }

    void LateUpdate()
    {
        // ����ʱ�������
        if (target != null && !isIndoors)
        {
            Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z) + offset;
            transform.position = targetPosition;
            lastPlayerMapPosition = target.position;
        }
    }

    // ���뷿��
    public void EnterHouse(int houseIndex)
    {
        if (houseIndex >= 0 && houseIndex < housePositions.Length)
        {
            StartCoroutine(FadeToHouse(houseIndex));
        }
    }

    // �뿪����
    public void ExitHouse()
    {
        if (isIndoors)
        {
            StartCoroutine(FadeToMap());
        }
    }

    private IEnumerator FadeToHouse(int houseIndex)
    {
        yield return StartCoroutine(FadeManager.Instance.FadeToBlack(() =>
        {
            transform.position = housePositions[houseIndex];
            target.position = housePlayerPositions[houseIndex];
            isIndoors = true;
            currentHouseIndex = houseIndex;
        }));
    }

    private IEnumerator FadeToMap()
    {
        yield return StartCoroutine(FadeManager.Instance.FadeToBlack(() =>
        {
            isIndoors = false;
            currentHouseIndex = -1;
            transform.position = new Vector3(target.position.x, target.position.y, transform.position.z) + offset;
            target.position = lastPlayerMapPosition;
            target.position = new Vector3(target.position.x, target.position.y - 1f, target.position.z);
        }));
    }

    public bool IsIndoors()
    {
        return isIndoors;
    }
}