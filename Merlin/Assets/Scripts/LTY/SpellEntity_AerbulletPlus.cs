using System;
using UnityEngine;

//---强化风元素子弹：发射后散射范围极大，存活时间长，并且小幅逐帧减速，不销毁子弹实现持续伤害---
public class SpellEntity_AerbulletPlus : SpellEntity, LogicalOperators
{
    //---初始化强化风元素子弹属性：速度、0.1、伤害、7.0---
    public SpellEntity_AerbulletPlus()
    {
        Sprite_Location = "Textures/Bullets/AerbulletPlus";
        entity_types = Entity_Type.Bullet;
        basespeed = 0.1f;
        Blood_dec_ = 7.0f;
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

    //---发射时：将间隔时间设为1.2秒，将子弹的山顺散射参数增大到5.0并延长存活时间---
    public override void Emit(BulletBase bb)
    {
        intervaltime = 1.2f;
        //basespeed = basespeed * 0.7f;
        
        if(bb is EntityBullet eb)
        {
            if(Time.time - TestBulletManager.last_bullet_time < eb.intervaltime)
                return ;
            eb.skpara = 5.0f;
            eb.alive_distance_time = 4.0f;
        }
    }

    //---每帧处理：小幅减速，模拟散弹在空中慢慢扩散---
    public override void Process(BulletBase bb)
    {
        if(bb is EntityBullet eb)
        {
            eb.Speed -= eb.Speed * 0.003f;
        }
    }

    //---命中逻辑：根据敌人风法抗计算伤害，不销毁子弹（实现持续/流弹伤害）---
    public override void Hit(Collider2D Colliding_bullet, BulletBase bb)
    {
        Debug.Log("Fuck!");
        Enemy enemy = Colliding_bullet.gameObject.GetComponent<Enemy>();
        if(enemy != null)
        {
            enemy.Status.CurrentBlood -= Blood_dec * (1-enemy.Status.CurrentMagicDfense[3]);
            //bb.Destroy_bullet();
        }
    }
}
