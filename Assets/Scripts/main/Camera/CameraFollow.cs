using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Ŀ�����壨����� Transform
    public Vector3 offset;   // ���������������ƫ����

    void LateUpdate()
    {
        if (target != null)
        {
            // ����Ŀ��λ�ü���ƫ����
            Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z) + offset;
            // ���������λ��
            transform.position = targetPosition;
        }
    }
}