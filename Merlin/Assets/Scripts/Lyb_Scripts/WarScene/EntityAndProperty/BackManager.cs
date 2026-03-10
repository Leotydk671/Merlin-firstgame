// 修改！！！！！！

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//---状态背景图管理：跟随父实体位置，根据父实体当前状态（Burn/Frozen/Toxic/Nothing）切换对应半透明背景贴图---
public class BackManager : MonoBehaviour
{
    //---被跟随的父实体---
    public Entity FatherObject;
    //---SpriteRenderer组件引用---
    private SpriteRenderer BackGround;
    //---相对父对象的偏移---
    public Vector3 offset = new Vector3(0,0.6f,0.1f);
    Color BackGroundColor;
    
    //---初始化位置和SpriteRenderer，起始时不显示任何背景---
    // Start is called before the first frame update
    void Start()
    {
        transform.position = FatherObject.transform.position + offset;
        BackGround = gameObject.GetComponent<SpriteRenderer>();
        BackGround.sprite = null;
        BackGroundColor = BackGround.color;
    }

    //---每帧根据父实体状态加载对应贴图，并保持50%透明度---
    // Update is called once per frame
    void Update()
    {
        
        switch (FatherObject.Status.CurrentSituation.Item1)
        {
            case EntityProperty.situation.Nothing:
                {
                    BackGround.sprite = null;
                }
                break;
            case EntityProperty.situation.Burn:
                {
                    BackGround.sprite = Resources.Load<Sprite>("Textures/burn_situation");
                }
                break;
            case EntityProperty.situation.Frozen:
                {
                    BackGround.sprite = Resources.Load<Sprite>("Textures/frozen_situation");
                }
                break;
            case EntityProperty.situation.Toxic:
                {
                    BackGround.sprite = Resources.Load<Sprite>("Textures/toxic_situation");
                }
                break;
            default:
             break;
        }

        BackGroundColor.a = 0.5f;
        BackGround.color = BackGroundColor;

    }
}
