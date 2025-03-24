using UnityEngine;

public abstract class TaskBase : MonoBehaviour
{
    public abstract string GetTaskName();      // 获取任务名称
    public abstract string GetTaskObjective(); // 获取任务目标
    public abstract bool IsTaskComplete();     // 检查任务是否完成
}