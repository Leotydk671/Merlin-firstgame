using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//---加载WarScene的按钮处理器：Start时注册监听，点击后加载WarScene---
public class LoadSceneWithAddListener : MonoBehaviour
{
    //---触发加载场景的按钮组件---
    public Button loadSceneButton;
    //---目标场景名称---
    private string sceneName = "WarScene";

    //---初始化：为loadSceneButton注册加载场景监听---
    void Start()
    {
        // 检查按钮引用是否为空
        if (loadSceneButton != null)
        {
            // 为按钮的点击事件添加监听
            loadSceneButton.onClick.AddListener(LoadScene);
        }
        else
        {
            Debug.LogError("按钮引用未设置！");
        }
    }

    //---按钮点击回调：载入WarScene---
    void LoadScene()
    {
        // 检查场景名称是否为空
        if (!string.IsNullOrEmpty(sceneName))
        {
            // 加载指定名称的场景
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("场景名称不能为空！");
        }
    }
}