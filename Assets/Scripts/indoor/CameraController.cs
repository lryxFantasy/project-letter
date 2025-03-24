using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // ���ǵ� Transform
    public Vector3 offset;   // �������������ǵ�ƫ����
    public Vector3[] housePositions; // �洢�������ݵ����λ��
    public Vector3[] housePlayerPositions; // �洢�������ݵ����Ǵ���λ��
    private bool isIndoors = false;  // �Ƿ��ڷ�����
    private int currentHouseIndex = -1; // ��ǰ���ݵ�������-1 ��ʾ�ڵ�ͼ��
    private Vector3 mapPosition;     // ��ͼ��Ĭ��λ��
    private Vector3 lastPlayerMapPosition; // �����ڵ�ͼ�ϵ����λ�ã������˳�ʱ���ͣ�

    void Start()
    {
        // ��ʼ������λ��
        if (housePositions == null || housePositions.Length == 0)
        {
            housePositions = new Vector3[6];
            housePositions[0] = new Vector3(100, 0, -10);  // ���� 1
            housePositions[1] = new Vector3(200, 0, -10);  // ���� 2
            housePositions[2] = new Vector3(300, 0, -10);  // ���� 3
            housePositions[3] = new Vector3(400, 0, -10);  // ���� 4
            housePositions[4] = new Vector3(500, 0, -10);  // ���� 5
            housePositions[5] = new Vector3(600, 0, -10);  // ���� 6
        }

        // ��ʼ�������ڷ����ڵĴ���λ��
        if (housePlayerPositions == null || housePlayerPositions.Length == 0)
        {
            housePlayerPositions = new Vector3[6];
            housePlayerPositions[0] = new Vector3(100, 0, 0);  // ���� 1 ����λ��
            housePlayerPositions[1] = new Vector3(200, 0, 0);  // ���� 2 ����λ��
            housePlayerPositions[2] = new Vector3(300, 0, 0);  // ���� 3 ����λ��
            housePlayerPositions[3] = new Vector3(400, 0, 0);  // ���� 4 ����λ��
            housePlayerPositions[4] = new Vector3(500, 0, 0);  // ���� 5 ����λ��
            housePlayerPositions[5] = new Vector3(600, 0, 0);  // ���� 6 ����λ��
        }

        mapPosition = transform.position; // �����ʼλ����Ϊ��ͼλ��
    }

    void LateUpdate()
    {
        if (target != null && !isIndoors)
        {
            // ����������ǣ���ͼģʽ��
            Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z) + offset;
            transform.position = targetPosition;
            lastPlayerMapPosition = target.position; // ��¼�����ڵ�ͼ�ϵ�λ��
        }
    }

    // ���뷿��
    public void EnterHouse(int houseIndex)
    {
        if (houseIndex >= 0 && houseIndex < housePositions.Length)
        {
            transform.position = housePositions[houseIndex]; // �ƶ����
            target.position = housePlayerPositions[houseIndex]; // ��������
            isIndoors = true;
            currentHouseIndex = houseIndex;
        }
    }

    // �˳�����
    public void ExitHouse()
    {
        if (isIndoors)
        {
            isIndoors = false;
            currentHouseIndex = -1;
            transform.position = new Vector3(target.position.x, target.position.y, transform.position.z) + offset;
            target.position = lastPlayerMapPosition; // �������ǻص���ͼ�ϵ����λ��
            target.position = new Vector3(target.position.x, target.position.y - 1f, target.position.z); // �����ƶ� 2 ����λ
        }
    }

    // ����Ƿ��ڷ�����
    public bool IsIndoors()
    {
        return isIndoors;
    }
}