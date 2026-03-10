using UnityEngine;

using TMPro;

//---数值文本管理：将浮点数属性值格式化为保留1位小数的字符串并显示在TMP文本组件上---
public class ValueManager : MonoBehaviour
{
    //---目标TMP文本组件---
    public TMP_Text TextValue;

    //---将浮点数格式化为"0.0"格式并更新文本---
    public void UpdateValue(float CurrentValue)
    {
        TextValue.text = CurrentValue.ToString("0.0");
    }
}
