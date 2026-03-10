using UnityEngine;

//---气泡散弹逻辑修饰符：发射时肬然射出7颗副弹，每帧运动过程中子弹持续减速---
public class SpellLogical_bubble : SpellLogical, LogicalOperators
{
   //---发射时：设置弹道加山和存活时间，并以随机偏移方向射出7颗副弹实现散射效果---
   public override void Emit(BulletBase bb)
    {
        Debug.Log("Logical accessful");
        if(bb is EntityBullet eb)
        {
            //eb.skpara += 4.0f;
            eb.alive_distance_time = 1.2f;
            eb.intervaltime = 3.0f;
            eb.Speed = 0.05f;
            if(Time.time - TestBulletManager.last_bullet_time < eb.intervaltime)
                return ;
            if(eb.copy == false)
            {
                Debug.Log("Double Bullet!!!!!");
                TestBulletManager.TBM.CreateANewBullet2(eb.original_up+(Vector3)UnityEngine.Random.insideUnitCircle*2.0f, PlayerController.EntityMerlin.gameObject.transform.position);
                TestBulletManager.TBM.CreateANewBullet2(eb.original_up+(Vector3)UnityEngine.Random.insideUnitCircle*2.0f, PlayerController.EntityMerlin.gameObject.transform.position); 
                TestBulletManager.TBM.CreateANewBullet2(eb.original_up+(Vector3)UnityEngine.Random.insideUnitCircle*2.0f, PlayerController.EntityMerlin.gameObject.transform.position); 
                TestBulletManager.TBM.CreateANewBullet2(eb.original_up+(Vector3)UnityEngine.Random.insideUnitCircle*2.0f, PlayerController.EntityMerlin.gameObject.transform.position); 
                TestBulletManager.TBM.CreateANewBullet2(eb.original_up+(Vector3)UnityEngine.Random.insideUnitCircle*2.0f, PlayerController.EntityMerlin.gameObject.transform.position); 
                TestBulletManager.TBM.CreateANewBullet2(eb.original_up+(Vector3)UnityEngine.Random.insideUnitCircle*2.0f, PlayerController.EntityMerlin.gameObject.transform.position); 
                TestBulletManager.TBM.CreateANewBullet2(eb.original_up+(Vector3)UnityEngine.Random.insideUnitCircle*2.0f, PlayerController.EntityMerlin.gameObject.transform.position); 

            }  
            else
            {
                Debug.Log("Double false????????");
            } 
        }

        
    }



    //---每帧处理：子弹每帧微幅减速，模拟气泡在空中慢慢飘荡并硬化的功能---
    public override void Process(BulletBase bb)
    {
        if(bb is EntityBullet eb)
        {
            eb.Speed -= eb.Speed * 0.008f;
        }
    }
}
