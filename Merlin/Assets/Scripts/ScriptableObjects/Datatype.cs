using UnityEngine;

//---初始元素数量ScriptableObject：存储四元素（火/水/土/风）在进入新地图时会随机出现的四元素补充情况---
// 定义可脚本化对象类
[CreateAssetMenu(fileName = "NewData", menuName = "Datatype")]
public class StartData : ScriptableObject
{
    //---火元素（Ignis）获取数量---
    public int Ignisnum; // 要显示的内容
    //---水元素（Aqua）获取数量---
    public int Aquanum;
    //---土元素（Terra）获取数量---
    public int Terranum;
    //---风元素（Aer）获取数量---
    public int Aernum;
}