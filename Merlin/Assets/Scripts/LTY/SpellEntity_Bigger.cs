using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//---大型子弹实体：无元素属性，速度较慢但伤害中等，不附加状态---
public class SpellEntity_Bigger : SpellEntity , LogicalOperators
{
    //---初始化大型子弹属性：速度、0.1、伤害、8.0、直接扣血无法抗乘数---
    public SpellEntity_Bigger()
    {
        Sprite_Location = "Textures/Bullets/Biggerbullet";
        entity_types = Entity_Type.Bullet;
        basespeed = 0.1f;
        Blood_dec_ = 8.0f;
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

    //---命中逻辑：直接扣除血量，不受法抗影响，销毁子弹---
    public override void Hit(Collider2D Colliding_bullet, BulletBase bb)
    {
        Debug.Log("Fuck!");
        Enemy enemy = Colliding_bullet.gameObject.GetComponent<Enemy>();
        if(enemy != null)
        {
            enemy.Status.CurrentBlood -= Blood_dec;
            bb.Destroy_bullet();
        }
    }
}
