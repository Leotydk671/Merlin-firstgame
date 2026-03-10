using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

//---地图坐标显示单例：Start时读取当前坐标显示在UI上，并根据是否安全地带设置面板颜色---
public class PositionShow : MonoBehaviour
{
    //---全局单例引用---
    public static PositionShow PS = null;
    //---展示(x,y)坐标的文本组件---
    public TextMeshProUGUI position_text = null;
    //---地图坐标面板背景图---
    public UnityEngine.UI.Image position_panel = null;

    //---初始化单例---
    void Awake()
    {
        PS = this;
    }

    //---展示当前坐标文字，安全地带覆盖为绿色，战斗地带覆盖为红色---
    void Start()
    {
        if(position_text != null)
        {
            Tuple<int, int> pos = TheGlobalManager.TGM.GetCurrentPosition();
            position_text.text = "（" + pos.Item1+'，' + pos.Item2 + "）";
        }
        if(!TheGlobalManager.TGM.IsSafe())
        {
            position_panel.color = new Color(0.96f, 0.56f, 0.57f, 0.3f);  
        }
        else
        {
            position_panel.color = new Color(0.52f, 0.96f, 0.51f, 0.3f);
        }
    }

}
