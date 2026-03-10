using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

//---元素法术信息面板：展示法术名称/说明/图标及元素消耗量，GetButton点击选中后SetButton可将法术装备到指定格子---
public class ElementPanelBehavior : MonoBehaviour
{
    //---当前面板对应的法术ScriptableObject数据---
    public Magicscript magicscript;
    private TextMeshProUGUI textComponent; // 存储 Text 组件的引用
    //---法术名称文本组件---
    public TextMeshProUGUI text1; 
    //---法术说明文本组件---
    public TextMeshProUGUI text2; 
    //---获取法术到选中格子的按钮---
    public Button GetButton = null;
    //---确认将法术装备到格子的按钮---
    public Button SetButton = null;
    //---当前面板是否已被选中（GetButton点击后为true）---
    public bool HasBeenCreate = false;
    //---显示法术图标的Image组件---
    public Image eleshow = null;



    //---初始化：绑定文本/图标组件，从GameManager获取GetButton/SetButton引用，显示四元素消耗数量和法术名称说明---
    void Awake()
    {
        text1 = transform.Find("ImageText/NameText").GetComponent<TextMeshProUGUI>();
        text2 = transform.Find("ImageText/FuncText").GetComponent<TextMeshProUGUI>();
        eleshow = transform.Find("ImageText/Elementshow").GetComponent<Image>();
        Debug.Log("Element setbutton");
        GetButton = GameManagerBehavior.gm.GetButton.GetComponent<Button>();
        SetButton = GameManagerBehavior.gm.SetButton.GetComponent<Button>();
        SetButton.interactable = false;

        for (int i=0; i<GameManagerBehavior.gm.BasicElementNum; i++)
        {
            GameObject LowerimageElement = GameManagerBehavior.gm.LowImage.transform.Find("Element"+i).gameObject;
            textComponent = LowerimageElement.GetComponentInChildren<TextMeshProUGUI>();
            textComponent.text = magicscript.ElementInfo[i].ToString();
            eleshow.sprite = magicscript.sprite;
            LowerimageElement.GetComponent<Image>().sprite = GameManagerBehavior.gm.sprites[i];
            Debug.Log("seccesful gettext");
        }  
        text1.text = magicscript.NameInfo;
        text2.text = magicscript.FuncInfo;
    }
    
    
    //---每帧更新法术图标的透明度，HasBeenCreate时完全不透明否则几乎全透明---
    void Update()
    {
        Setcolor();
    }

    //---GetButton点击事件：将HasBeenCreate置true，激活SetButton允许装备法术---
    public void OnButtonClick()
    {
        HasBeenCreate = true;
        SetButton.interactable = true;
        //Debug.Log("OnClick here");
        // 将 bool 变量设为 true
        //myBoolVariable = !myBoolVariable;
        //Debug.Log("按钮已点击，myBoolVariable 变为: " + myBoolVariable);

        //int index = GameManagerBehavior.gm.Getnum();

        //if (myBoolVariable)  
            //CreateChildImage(GameManagerBehavior.gm.Selected_box);
        //else
        //    DestroyChildImage(GameManagerBehavior.gm.Selected_box);
        
    }


    //---SetButton点击事件：先销毁再创建当前选中格子的ChildImage，更新格子法术类型---
    public void OnSetButtonClick()
    {
        DestroyChildImage(GameManagerBehavior.gm.Selected_box);
        CreateChildImage(GameManagerBehavior.gm.Selected_box);
    }



    private Transform FindChildByName(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;

            // 递归查找子对象的子对象
            Transform foundChild = FindChildByName(child, name);
            if (foundChild != null)
                return foundChild;
        }

        return null;    
    }


    //---在选中格子下查找并销毁名为ChildImage的子对象，同时清空格子状态---
    private void DestroyChildImage(Boxselected SelectedBox)
    {
            Transform childTransform = FindChildByName(SelectedBox.transform, "ChildImage");
            if(childTransform == null) return;
            GameObject.Destroy(childTransform.gameObject);  
            GameManagerBehavior.gm.ClearTheBox(SelectedBox);
    }


    //---在选中格子下创建名为ChildImage的GameObject并添加Image组件，再调用SetSpriteForChildImage赋值---
    private void CreateChildImage(Boxselected SelectedBox)
    {
        // 创建一个新的 GameObject 作为子对象
        GameObject childImageObject = new GameObject("ChildImage");
        // 设置子对象的父对象为传入的父 GameObject
        childImageObject.transform.SetParent(SelectedBox.transform, false);

        // 为子对象添加 Image 组件
        Image childImage = childImageObject.AddComponent<Image>();
        if (childImage == null)
        {
            Debug.LogError("无法为子对象添加 Image 组件。");
            return;
        }
        // 调用设置 Sprite 的方法
        SetSpriteForChildImage(childImage, SelectedBox);
    }



    //---设置ChildImage的Sprite，根据IsBullet/IsComponent更新格子的法术类型和SpellType字段---
    private void SetSpriteForChildImage(Image childImage, Boxselected SelectedBox)
    {
        // 为子 Image 的 sprite 属性赋值
        childImage.sprite = magicscript.sprite;
        RectTransform childRectTransform = childImage.GetComponent<RectTransform>();
        childRectTransform.sizeDelta *= 0.2f;

        GameManagerBehavior.gm.ClearTheBox(SelectedBox);
        if(magicscript.IsBullet)
        {
            SelectedBox.IsBulllet = true;
            Type spelltype = Type.GetType(magicscript.TypeName);
            if(spelltype == null)
                Debug.Log("Cant understand the type name");
            SelectedBox.SpellType = spelltype;
        }
        if(magicscript.IsComponent)
        {
            SelectedBox.IsComponent = true;
            Type spelltype = Type.GetType(magicscript.TypeName);
            if(spelltype == null)
                Debug.Log("Cant understand the type name");
            SelectedBox.SpellType = spelltype;
        }
        
    }



    //---根据HasBeenCreate更新eleshow透明度：选中时完全显示，未选中时几乎隐藏---
    void Setcolor()
    {
        if (HasBeenCreate)
        {
            Color mcolor = eleshow.color;
            mcolor.a = 1.0f;
            eleshow.color = mcolor;
            //Debug.Log("按钮已点击，myBoolVariable 变为: " + myBoolVariable);
        }
        else
        {
            Color mcolor = eleshow.color;
            mcolor.a = 10f/255f;
            eleshow.color = mcolor;
        }
    }
}
