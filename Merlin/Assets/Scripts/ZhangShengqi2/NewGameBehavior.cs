
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//---新游戏主画面输入入口：Button1新建游戏载入InputScene，Button2尝试读屘载入WarScene---
public class NewGameBehavior : MonoBehaviour
{
    //---新建游戏按钮---
    public Button button1;
    //---读屘进入游戏按钮---
    public Button button2;
    //---初始化：查找Panel内按钮并注册监听---
    void Start()
    {
        button1 = transform.Find("Panel/Button1").GetComponent<Button>();
        button2 = transform.Find("Panel/Button2").GetComponent<Button>();

        button1.onClick.AddListener(OnNewButtonClicked);
        button2.onClick.AddListener(OnLoadButtonClicked);
    }

    //---新游戏按钮回调：载入InputScene进入种子输入页面---
    void OnNewButtonClicked()
    {
        SceneManager.LoadScene("InputScene");
    }

    //---读屘按钮回调：尝试读取存档，成功则载入WarScene，失败则打印日志---
    void OnLoadButtonClicked()
    {
        if(TheGlobalManager.TGM.TrytoLoad())
        {
            SceneManager.LoadScene("WarScene");
        }
        else
        {
            Debug.Log("Cant load the scene");
        }
    }
}
