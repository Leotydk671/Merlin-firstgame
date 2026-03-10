using System;
using UnityEngine;
using UnityEngine.EventSystems;

//---法术槽格子：记录格子中的法术类型（子弹/修饰符），被点击时变为橙色并更新GameManager的选中状态---
public class Boxselected : MonoBehaviour, IPointerClickHandler
{
    //---格子内是否存放的是子弹类型法术---
    public bool IsBulllet = false;
    //---格子内是否存放的是修饰符类型法术---
    public bool IsComponent = false;

    //---格子内存放的法术C#类型---
    public Type SpellType = null;

    //---Awake中添加EventTrigger组件（如无）---
    private void Awake()
    {
        EventTrigger eventTrigger = gameObject.GetComponent<EventTrigger>();
        if (eventTrigger == null)
        {
            eventTrigger = gameObject.AddComponent<EventTrigger>();
        }
    }
 
    //---点击时：将之前选中格子颜色恢复白色，将自身颜色改为橙色，并更新GameManager的Selected_box---
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("EventTriggerTest OnClick");
        UnityEngine.UI.Image im = GameManagerBehavior.gm.Selected_box.GetComponent<UnityEngine.UI.Image>();
        UnityEngine.UI.Image im2 = gameObject.GetComponent<UnityEngine.UI.Image>();
        im.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        im2.color = new Color(1.0f, 0.7f, 0.4f, 1.0f);
        GameManagerBehavior.gm.Selected_box = this;
    }

}
