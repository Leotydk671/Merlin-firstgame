using UnityEngine;

//---双弹逻辑修饰符：发射一颗主弹的同时额外射出一颗副弹，运动过程中小幅减速---
public class SpellLogical_double : SpellLogical, LogicalOperators
{
    
    //---发射时：若为主弹则额外射出一颗副弹，符合间隔时间方可触发---
    public override void Emit(BulletBase bb)
    {
        
        Debug.Log("Logical accessful");
        if(bb is EntityBullet eb)
        {
            //eb.intervaltime = eb.bullet_main_entity.intervaltime;
            if(Time.time - TestBulletManager.last_bullet_time < eb.intervaltime)
                return ;
            if(eb.copy == false)
            {
                Debug.Log("Double Bullet!!!!!");
                TestBulletManager.TBM.CreateANewBullet2(eb.original_up+(Vector3)UnityEngine.Random.insideUnitCircle, PlayerController.EntityMerlin.gameObject.transform.position); 
            }  
            else
            {
                Debug.Log("Double false????????");
            } 
        }
    }

    //---每帧处理：子弹微幅减速（比bubble慢）---
    public override void Process(BulletBase bb)
    {
        if(bb is EntityBullet eb)
        {
            eb.Speed -= eb.Speed * 0.005f;
        }
    }
}
