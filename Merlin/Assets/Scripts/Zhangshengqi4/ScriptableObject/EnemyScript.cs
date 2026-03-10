using UnityEngine;
using TMPro;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "EnemyScript", menuName = "ScriptableObjects/EnemyScript", order = 1)]
//---敌人信息ScriptableObject：存储敌人名称/说明文字/图片，供怪物展示面板使用---
public class EnemyScript : ScriptableObject
{
    //---敌人名称---
    public string EnemyName;

    //---敌人说明文字---
    public string EnemyExplanaion;

    //---敌人展示图片---
    public Sprite EnemyImagesprite;
}