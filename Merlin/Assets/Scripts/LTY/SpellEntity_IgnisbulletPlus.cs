using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//---强化火元素子弹：比普通火弹速度更快、伤害略高，同样施加灬烧状态---
public class SpellEntity_IgnisbulletPlus : SpellEntity, LogicalOperators
{
    //---初始化强化火元素子弹属性：速度、0.15、伤害、12.0---
    public SpellEntity_IgnisbulletPlus()
    {
        Sprite_Location = "Textures/Bullets/IgnisbulletPlus";
        entity_types = Entity_Type.Bullet;
        basespeed = 0.15f;
        Blood_dec_ = 12.0f;
        Defence_dec_ = 0;

    }

    public override float Blood_dec
    {
        get => Blood_dec_;
        set => Blood_dec_ = value;
    }

    public override float Defence_dec
    {
        get => Defence_dec_;
        set => Defence_dec_ = value;
    }
    //public byte Element_type = 1<<1; //Aqua

    //---命中逻辑：根据火法抗计算伤害，设置灬烧状态，销毁子弹---
    public override void Hit(Collider2D Colliding_bullet, BulletBase bb)
    {
        Debug.Log("Fuck!");
        Enemy enemy = Colliding_bullet.gameObject.GetComponent<Enemy>();
        if(enemy != null)
        {
            enemy.Status.CurrentBlood -= Blood_dec * (1-enemy.Status.CurrentMagicDfense[1]);
            if (enemy.Status.CurrentSituation.Item1==EntityProperty.situation.Nothing || enemy.Status.CurrentSituation.Item2<=0)
                enemy.Status.CurrentSituation.Item1 = EntityProperty.situation.Burn;
            bb.Destroy_bullet();
        }
    }

}
