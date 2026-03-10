using UnityEngine;
using TMPro;
using UnityEngine.UI;

//---Panel自动绑定EnemyScript SO：Start时将敌人名/说明/图片分别赋値给子对象的UI组件---
public class PanelToScriptableObjectLinker : MonoBehaviour
{
    //---绑定的敌人信息ScriptableObject---
    public EnemyScript enemyScript;

    //---初始化：将enemyScript的名称/说明/图片赋值给Panel内对应的Text和Image组件---
    void Start()
    {
        if (enemyScript != null)
        {
            // 获取当前 GameObject（即 Panel）下的所有 TextMeshProUGUI 组件
            TextMeshProUGUI[] textComponents = GetComponentsInChildren<TextMeshProUGUI>();
            if (textComponents.Length >= 2)
            {
                // 将可脚本化对象的字符串赋值给 TextMeshProUGUI 组件显示
                textComponents[0].text = enemyScript.EnemyName;
                textComponents[1].text = enemyScript.EnemyExplanaion;

                // 也可以将 TextMeshProUGUI 组件的文本同步到可脚本化对象的字符串变量
                //enemyScript.text1 = textComponents[0].text;
                //enemyScript.text2 = textComponents[1].text;
            }
            else
            {
                Debug.LogError("Panel 下的 TextMeshProUGUI 组件数量不足 2 个。");
            }

            // 获取当前 GameObject（即 Panel）的直接子对象中的 Image 组件
            Image imageComponent = null;
            Transform panelTransform = transform;
            for (int i = 0; i < panelTransform.childCount; i++)
            {
                imageComponent = panelTransform.GetChild(i).GetComponent<Image>();
                if (imageComponent != null)
                {
                    break;
                }
            }

            if (imageComponent != null)
            {
                // 将可脚本化对象的 Sprite 赋值给 Image 组件显示
                imageComponent.sprite = enemyScript.EnemyImagesprite;

                // 如果需要反向同步，例如获取 Image 当前的 Sprite 赋值给可脚本化对象
                //enemyScript.EnemyImage.sprite = imageComponent.sprite;
            }
            else
            {
                Debug.LogError("Panel 的直接子对象中未找到 Image 组件。");
            }
        }
        else
        {
            Debug.LogError("EnemyScript 引用为空。");
        }
    }
}