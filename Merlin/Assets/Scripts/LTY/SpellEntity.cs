using UnityEngine;
//---所有法术实体（子弹/范围）的抽象基类，同时实现LogicalOperators接口，定义子弹属性与行为模板---
public abstract class SpellEntity : SpellComponentBase, LogicalOperators
{   
    //---实体类型枚举：Meta=未定义，Bullet=投射物，Range=范围型---
    public enum Entity_Type{Meta, Bullet, Range}

    //---当前实体类型---
    public Entity_Type entity_types = Entity_Type.Meta;
    //---子弹使用的贴图路径（Resources目录下的相对路径）---
    public string Sprite_Location = null;
    
    //---子弹基础飞行速度---
    public float basespeed = 0.1f;
    //---基础血量伤害值（实际伤害由子类覆盖）---
    public float Blood_dec_ = 1.0f;
    //---防御削减值（暂未使用）---
    public float Defence_dec_ = 0;

    //---持续时间（暂未在基类中使用）---
    public float sustaintime = 1.5f;

    //---同一子弹的最小发射间隔时间，防止连射---
    public float intervaltime = 0.3f;
    
    //---构造函数：将组件类型标记为Entity---
    protected SpellEntity()
    {
        spelltype = SpellComponentType.Entity;
    }

    //---血量伤害属性（子类必须实现getter/setter）---
    public abstract float Blood_dec
    {
        get;
        set;
    }
    //---防御削减属性（子类必须实现getter/setter）---
    public abstract float Defence_dec
    {
        get;
        set;
    }

    //---子弹发射时调用：默认实现为根据方向旋转子弹朝向---
    public virtual void Emit(BulletBase bb)
    {
        if(bb is EntityBullet eb)
        {
            Vector2 or = new Vector2(1,0);
            Vector2 thisvec = new Vector2(eb.original_up.x, eb.original_up.y);
            eb.transform.Rotate(0, 0, Vector2.SignedAngle(or, thisvec));
            Debug.Log("Rotate angle" + Vector2.SignedAngle(or, thisvec) + "  is " + thisvec.x + " " + thisvec.y);
        }

    }
    //---子弹每帧处理逻辑（子类可覆盖以实现特殊运动轨迹）---
    public virtual void Process(BulletBase bb){}
    //---子弹命中时调用（子类实现具体伤害逻辑）---
    public virtual void Hit(Collider2D Colliding_bullet, BulletBase bb){}
    //---子弹爆炸/消散时调用（子类可覆盖）---
    public virtual void Blast(){}

    //---获取基础速度，供外部查询---
    public float GetSpeed()
    {
        return basespeed;
    }
}
