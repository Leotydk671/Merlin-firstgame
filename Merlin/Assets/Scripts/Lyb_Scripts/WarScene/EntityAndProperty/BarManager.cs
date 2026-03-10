using UnityEngine;
using UnityEngine.UI;

//---UI血条/属性条管理：使用SmoothDamp平滑过渡fillAmount到目标值，通过UpdateBar()更新显示---
public class BarManager : MonoBehaviour
{
    //---填充条的Image组件---
    public Image fillImage; // 填充条的Image组件
    //---属性的最大值，用于计算填充比例---
    public float MaxValue = 25f; // 最大值
    //---平滑过渡时间（秒）---
    public float smoothTime = 0.3f; // 平滑过渡时间

    private float targetFillAmount; // 目标填充比例
    private float currentFillAmount; // 当前填充比例
    private float velocity = 0f; // 平滑过渡的速度变量

    //---初始化当前填充比例与fillImage保持一致---
    void Start()
    {
        currentFillAmount = fillImage.fillAmount; // 初始化当前填充比例
    }

    void Update()
    {
        // 使用Mathf.SmoothDamp进行平滑过渡
        currentFillAmount = Mathf.SmoothDamp(currentFillAmount, targetFillAmount, ref velocity, smoothTime);

        // 更新填充条的填充量
        fillImage.fillAmount = currentFillAmount;
    }

    //---外部调用：传入当前值，换算为0-1的填充比例---
    public void UpdateBar(float CurrentValue)
    {
        // 计算目标填充比例
        targetFillAmount = CurrentValue / MaxValue;
    }
}
