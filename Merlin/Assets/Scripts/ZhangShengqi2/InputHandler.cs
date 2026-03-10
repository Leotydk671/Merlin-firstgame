using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//---整数种子输入处理器：读取用户输入的整数种子，调用SetNewWorld设定全局种子并载入LeadScene---
public class IntegerInputHandler : MonoBehaviour
{
    // 引用输入字段
    public TMP_InputField inputField;
    // 引用按钮
    public Button submitButton;
    //---用户输入的整数种子（静态共享）---
    public static int userInputInteger;

    //---初始化：注册提交按钮监听---
    void Start()
    {
        // 为按钮的点击事件添加监听
        submitButton.onClick.AddListener(OnSubmitButtonClicked);
    }

    //---提交按钮回调：解析输入整数作为种子，无效则随机种子，设置后载入LeadScene---
    void OnSubmitButtonClicked()
    {
        // 获取输入字段的文本
        string input = inputField.text;
        int globalseed = UnityEngine.Random.Range(0, 1999999999);       // 尝试将输入的字符串转换为整数
        if (int.TryParse(input, out userInputInteger))
        {
            TheGlobalManager.TGM.SetNewWorld(userInputInteger);
            Debug.Log("用户输入的整数是: " + userInputInteger);
            SceneManager.LoadScene("LeadScene");
            // 转换成功，打印用户输入的整数
            
        }
        else
        {
            TheGlobalManager.TGM.SetNewWorld(globalseed);
            SceneManager.LoadScene("LeadScene");
            // 转换失败，提示用户输入无效
            //Debug.Log("输入无效，请输入一个有效的整数。");
        }
    }

}