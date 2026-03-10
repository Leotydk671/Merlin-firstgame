using UnityEngine;

//---所有子弹类型的基类，提供碰撞次数管理与销毁逻辑---
public class BulletBase : MonoBehaviour
{
    //---子弹剩余可碰撞次数，默认为1次即碰撞后立即销毁---
    public int collidetime = 1;

    //---销毁子弹方法：剩余碰撞次数为1时直接销毁，否则减少次数（用于穿透效果）---
    public void Destroy_bullet()
    {
        if(collidetime <= 1)
            Destroy(gameObject);
        else
            collidetime--;
    }



    // Start is called before the first frame update
}
