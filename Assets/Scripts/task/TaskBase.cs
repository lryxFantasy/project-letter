using UnityEngine;

public abstract class TaskBase : MonoBehaviour
{
    public abstract string GetTaskName();
    public abstract string GetTaskObjective();
    public abstract bool IsTaskComplete();

    // �������󷽷���Ҫ����������ʵ��
    public abstract void DeliverLetter(string targetResident);
}