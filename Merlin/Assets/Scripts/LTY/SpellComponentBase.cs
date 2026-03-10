//---咒文组件抽象基类，所有法术组成部分（实体、逻辑等）均继承于此---
public abstract class SpellComponentBase
{
    //---组件类型枚举：Meta=未定义，Entity=实体，Relation=关系，Num=数量，Logical=逻辑，Property=属性，Action=动作---
    public enum SpellComponentType{Meta, Entity, Relation, Num, Logical, Property, Action}; 

    //---当前组件类型，默认为Meta（未定义类型）---
    public SpellComponentType spelltype = SpellComponentType.Meta; 
    

}



