using UnityEngine;

//---法术逻辑操作接口，定义子弹在发射、持续、命中、爆炸四个生命周期节点的回调函数签名---
public interface LogicalOperators
    {
        //---子弹创建时调用（初始化逻辑参数、可能额外创建副弹）---
        public void Emit(BulletBase bb);
        //---子弹每帧Update时调用（处理持续效果如减速）---
        public void Process(BulletBase bb);
        //---子弹碰撞到敌人时调用---
        public void Hit(Collider2D Colliding_bullet, BulletBase bb);
        //---子弹消散时调用---
        public void Blast();
    }

//---所有法术逻辑修饰符的抽象基类，提供默认空实现以防止子类必须全部实现---
public abstract class SpellLogical : SpellComponentBase, LogicalOperators
{   
    //---构造时自动将组件类型设为Logical---
    protected SpellLogical()
    {
        spelltype = SpellComponentType.Logical;
    }

    
    
    public virtual void Emit(BulletBase bb){}
    public virtual void Process(BulletBase bb){}
    public virtual void Hit(Collider2D Colliding_bullet, BulletBase bb){}
    public virtual void Blast(){}
}

