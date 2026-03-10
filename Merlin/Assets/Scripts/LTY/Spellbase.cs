using UnityEngine;

//---挂载在场景对象上的咒文基类MonoBehaviour，定义法术类型的分类枚举---
public class Spellbase : MonoBehaviour
{
    //---法术类型枚举：Core=核心元素，Component=组件，Meta=未定义---
    public enum Spelltype{Core, Component, Meta};

    //public enum SpellCore{Ignis, Aqua, Terra, Aer, Ordo, Perditio};

    //public enum SpellComponent{Entity, Relation, Num, Logical, Property, Action}; 
    
    //---当前组件类型，默认为Meta（子类应在构造时覆盖此值）---
    protected Spelltype component_type = Spelltype.Meta;

    

}
