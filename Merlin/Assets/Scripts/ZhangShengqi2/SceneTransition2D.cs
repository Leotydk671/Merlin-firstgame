using UnityEngine;
using UnityEngine.SceneManagement;

//---定时自动载入WarScene的过场脚本：Start时启Invoke，默认5秒后执行场景跳转---
public class SceneTransition2D : MonoBehaviour
{
    //---过场延迟时间（秒）---
    public float transitionTime = 5f; // 转场时间，单位：秒
    //---目标跳转场景名---
    private string targetSceneName = "WarScene"; // 目标场景的名称

    //---启Invoke延迟调用LoadTargetScene---
    void Start()
    {
        // 调用 Invoke 方法，在指定时间后调用 LoadTargetScene 方法
        Invoke("LoadTargetScene", transitionTime);
    }

    //---执行场景切换，加载WarScene---
    void LoadTargetScene()
    {
        // 加载目标场景
        SceneManager.LoadScene(targetSceneName);
    }
}