using System;
using System.Collections.Generic;

//---单层地图层尚数据结构体：存储种子/湿度/温度/是否安全---
[Serializable]
public struct SingleScene
{
    //---这一层地图的生成种子---
    public int seed;
    //---湿度值，影响气候类型---
    public float moisture;
    //---温度值，影响气候类型---
    public float temperature;
    //---是否为安全地带（已通过的场景为true）---
    public bool safe;

}


//---存档主类：包含全局地图信息字典、最近一个安全场景坐标、当前元素持有数量---
[Serializable]
public class Save
{
    //---全局地图信息字典：(x,y)坐标 → SingleScene层尚数据---
    public Dictionary<Tuple<int,int>, SingleScene>  GlobalMapInfoTree = new Dictionary<Tuple<int,int>, SingleScene>();
    //---最近一个安全地带的坐标（读屘起始点）---
    public Tuple<int, int> LastSafeScene;
    //---四元素持有数量（火/水/土/风）---
    public Tuple<int,int,int,int> ElementNum;
}