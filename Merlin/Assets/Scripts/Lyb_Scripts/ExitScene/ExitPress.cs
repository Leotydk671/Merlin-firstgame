using UnityEngine;
using UnityEngine.SceneManagement; // 引入场景管理命名空间

//---退出场景过渡脚本：检测到任意按键后先尝试读档，再跳转到WarScene战斗场景---
public class ExitPress : MonoBehaviour
{
    //---每帧检测任意按键按下，按键时加载存档并跳转WarScene---
    void Update()
    {
        // 检测是否有任意按键被按下
        if (Input.anyKeyDown)
        {
            // 加载名为 "StartScene" 的场景
            TheGlobalManager.TGM.TrytoLoad();
            SceneManager.LoadScene("WarScene");
            Debug.Log("Loading StartScene...");
        }
    }
}
