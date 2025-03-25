using UnityEngine;

public abstract class TaskBase : MonoBehaviour
{
    public abstract string GetTaskName();
    public abstract string GetTaskObjective();
    public abstract bool IsTaskComplete();

    // 新增抽象方法，要求所有任务实现
    public abstract void DeliverLetter(string targetResident);
}