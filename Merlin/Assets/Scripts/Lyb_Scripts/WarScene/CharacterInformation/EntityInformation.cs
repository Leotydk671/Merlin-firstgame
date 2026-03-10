using UnityEngine;

//---角色属性配置ScriptableObject：在Inspector中配置血量/速度/护甲/法抗等初始属性，供Entity.Readconfig()读取---
[CreateAssetMenu(fileName = "NewStatsConfig", menuName = "Character/Stats Config")]
public class CharacterStatusConfig : ScriptableObject 
{
    //---角色最大血量---
    public float Blood;  // 原始血量
    //---角色基础移动速度---
    public float Speed;  // 原始移动速度
    //---正面物理护甲值（0-1，减伤比例）---
    public float FrontDefense;  // 原始前护甲
    //---背面物理护甲值（0-1，减伤比例）---
    public float BehindDefense;  // 原始后护甲
    //---法术抗性数组（0=通用，1=火，2=水，3=风，4=土）---
    public float[] MagicDefense = new float[5];  // 初始法抗
    
}
