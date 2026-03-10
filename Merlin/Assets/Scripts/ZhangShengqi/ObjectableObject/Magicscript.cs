using UnityEngine;

// 添加CreateAssetMenu特性，以便在编辑器中方便创建该ScriptableObject实例
[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObjects/ScriptableObject", order = 0)]
//---魔法技能ScriptableObject：存储法术名称/说明/图标/四元素消耗量/类型标识，供ElementPanelBehavior和格子系统读取---
public class Magicscript : ScriptableObject
{
    //---四元素消耗量数组（下标0~3对应火/水/土/风）---
    public int[] ElementInfo = new int[4];
    //---法术名称---
    public string NameInfo;
    //---法术功能说明文字---
    public string FuncInfo;
    //---法术图标Sprite---
    public Sprite sprite;
    //---是否已激活（保留字段）---
    public bool Isactivated;
    //---是否为子弹类型法术---
    public bool IsBullet = false;
    //---是否为法术修饰符组件类型---
    public bool IsComponent = false;
    //---对应C#类名字符串，用于反射获取法术Type---
    public string TypeName = "";
}