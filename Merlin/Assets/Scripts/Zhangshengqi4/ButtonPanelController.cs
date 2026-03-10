using UnityEngine;
using UnityEngine.UI;

//---多按钮面板控制器：点击按钮时隐藏所有面板，仅显示对应索引的面板---
public class ButtonPanelController : MonoBehaviour
{
    //---按钮数组---
    public Button[] buttons;

    //---对应面板数组---
    public GameObject[] panels;

    //---初始化：为每个按钮注册 ShowPanel 监听，并隐藏所有面板---
    private void Start()
    {
        // 为每个按钮添加点击事件监听器
        //button1.onClick.AddListener(() => ShowPanel(panel1));
        for (int i=0;i<buttons.Length;i++)
        {
            int index = i;
            buttons[index].onClick.AddListener(() =>ShowPanel(index));
        }
        // 初始时隐藏所有面板
        HideAllPanels();
    }

    //---隐藏所有panels中的面板---
    private void HideAllPanels()
    {
        for (int i=0;i<panels.Length;i++)
        {
            panels[i].SetActive(false);
        }
    }

    //---隐藏所有面板后显示index对应的面板---
    private void ShowPanel(int index)
    {
        // 隐藏所有面板
        HideAllPanels();
        // 显示指定的面板
        panels[index].SetActive(true);
    }
}