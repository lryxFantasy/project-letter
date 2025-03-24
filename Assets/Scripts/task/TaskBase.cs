using UnityEngine;

public abstract class TaskBase : MonoBehaviour
{
    public abstract string GetTaskName();      // ��ȡ��������
    public abstract string GetTaskObjective(); // ��ȡ����Ŀ��
    public abstract bool IsTaskComplete();     // ��������Ƿ����
}