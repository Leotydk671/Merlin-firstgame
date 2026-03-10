using UnityEngine;
using UnityEngine.UI;

//---状态图标管理：根据状态索引和剩余时间加载对应状态贴图到TargetImage，剩余时间归零时显示无状态图标---
public class SituationManager : MonoBehaviour
{
    //---显示状态图标的Image组件---
    public Image TargetImage;
    public GameObject FatherEntity;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //---根据SituationIndex加载对应状态贴图；RestTime<=0时显示nothing_situation图标---
    public void UpdateSituation(int SituationIndex , float RestTime)
    {
        
        if (RestTime <= 0.0f)
        {
            Sprite SituationSprite = Resources.Load<Sprite>("Textures/nothing_situation");
            if (SituationSprite != null)
            {
                TargetImage.sprite = SituationSprite;
            }
            return;
        }

        switch(SituationIndex)
        {
            case 0: 
                {
                    Sprite SituationSprite = Resources.Load<Sprite>("Textures/nothing_situation");
                    if (SituationSprite != null)
                    {
                        TargetImage.sprite = SituationSprite;
                    }
                }
                break;
            case 1:
                {
                    Sprite SituationSprite = Resources.Load<Sprite>("Textures/toxic_situation");
                    if (SituationSprite != null)
                    {
                        TargetImage.sprite = SituationSprite;
                    }
                }
                break;
            case 2:
                {
                    Sprite SituationSprite = Resources.Load<Sprite>("Textures/burn_situation");
                    if (SituationSprite != null)
                    {
                        TargetImage.sprite = SituationSprite;
                    }
                }
                break;
            case 3:
                {
                    Sprite SituationSprite = Resources.Load<Sprite>("Textures/wet_situation");
                    if (SituationSprite != null)
                    {
                        TargetImage.sprite = SituationSprite;
                    }
                }
                break;
            case 4:
                {
                    Sprite SituationSprite = Resources.Load<Sprite>("Textures/frozen_situation");
                    if (SituationSprite != null)
                    {
                        TargetImage.sprite = SituationSprite;
                    }
                }
                break;
            case 5:
                {
                    Sprite SituationSprite = Resources.Load<Sprite>("Textures/easyhurt_situation");
                    if (SituationSprite != null)
                    {
                        TargetImage.sprite = SituationSprite;
                    }
                }
                break;
            case 6:
                {
                    Sprite SituationSprite = Resources.Load<Sprite>("Textures/magicshield_situation");
                    if (SituationSprite != null)
                    {
                        TargetImage.sprite = SituationSprite;
                    }
                }
                break;
            case 7:
                {
                    Sprite SituationSprite = Resources.Load<Sprite>("Textures/physicshield_situation");
                    if (SituationSprite != null)
                    {
                        TargetImage.sprite = SituationSprite;
                    }
                }
                break;
            case 8:
                {
                    Sprite SituationSprite = Resources.Load<Sprite>("Textures/fast_situation");
                    if (SituationSprite != null)
                    {
                        TargetImage.sprite = SituationSprite;
                    }
                }
                break;
            case 9:
                {
                    //Destroy(FatherEntity.transform.gameObject);
                }
                break;
            default:
                {
                    Sprite SituationSprite = Resources.Load<Sprite>("Textures/nothing_situation");
                    if (SituationSprite != null)
                    {
                        TargetImage.sprite = SituationSprite;
                    }
                }
                break;
        }
    }
}
