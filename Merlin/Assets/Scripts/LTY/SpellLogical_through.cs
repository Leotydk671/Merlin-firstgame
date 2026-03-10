using UnityEngine;

//---穿透逻辑修饰符：子弹可多次穿透敌人，发射时将碰撞次数加一---
public class SpellLogical_through : SpellLogical, LogicalOperators
{
    //---发射时：将BulletBase的collidetime加一，实现多次穿透效果---
    public override void Emit(BulletBase bb)
    {
        bb.collidetime++;
    }
    /*public override void Hit(Collider2D Colliding_bullet, BulletBase bb)
    {
        Enemy enemy = Colliding_bullet.gameObject.GetComponent<Enemy>();
        if(enemy != null)
        {
            if(bb is EntityBullet eb)
            {
                if(eb.bullet_main_entity is SpellEntity_AerbulletPlus || eb.copy == true)
                {
                    Debug.Log("cant use this");
                    return;
                }
                if(eb.copy == false)
                {
                    Debug.Log("Double Bullet!!!!!");
                    TestBulletManager.TBM.CreateANewBullet2(eb.original_up, Colliding_bullet.gameObject.transform.position + eb.original_up.normalized*2.0f); 
                }  
                else
                {
                    Debug.Log("Double false????????");
                } 
            }
        }
    }*/
}
