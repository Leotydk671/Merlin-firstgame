using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement;
using TMPro;

//---开始场景按钮：游戏启动后2秒内按钮逐渐淡入显示，淡入完成后才可点击，点击后跳转ChooseScene---
public class ButtonSetting : MonoBehaviour
{
    //---目标跳转场景名---
    private string sceneName = "ChooseScene"; // 目标场景名称
    //---淡入总时间（秒）---
    public float fadeDuration = 2f; // 渐变总时间（2秒）

    private Button button; // 按钮组件引用
    private Image buttonImage; // 按钮的Image组件引用
    public TMP_Text buttonText; // 按钮上的TMP_Text组件引用
    private float elapsedTime; // 已经过的时间
    private bool isFading = false; // 是否正在渐变

    //---初始化组件引用，禁用按钮并开始淡入动画---
    private void Start()
    {
        // 获取按钮组件
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
        buttonText = GetComponentInChildren<TMP_Text>(); // 获取按钮上的TMP_Text组件

        // 禁用按钮，直到渐变完成
        button.interactable = false;

        // 开始渐变
        isFading = true;
    }

    //---每帧更新按钮和文字的Alpha值实现淡入，时间到达后启用按钮交互---
    private void Update()
    {
        if (isFading)
        {
            // 计算已过时间
            elapsedTime += Time.deltaTime;

            // 使用Lerp函数从0（完全透明）渐变到1（完全不透明）
            float alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);

            // 设置按钮的Alpha值
            Color imageColor = buttonImage.color;
            imageColor.a = alpha;
            buttonImage.color = imageColor;

            // 设置按钮上TMP_Text的Alpha值
            Color textColor = buttonText.color;
            textColor.a = alpha;
            buttonText.color = textColor;

            // 如果渐变完成
            if (elapsedTime >= fadeDuration)
            {
                isFading = false;
                button.interactable = true; // 启用按钮
            }
        }
    }

    //---按钮点击事件：调用SceneManager跳转到ChooseScene---
    // 按钮点击事件：加载场景
    public void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
