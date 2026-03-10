using UnityEngine;
using UnityEngine.SceneManagement;

//---巡逻边界跳图按钮：根据Merlin当前坐标象限判断方向，调用TheGlobalManager.EnterScene进入新场景---
public class ButtontoNewMap : MonoBehaviour
{
    //---判断玩家方向并尝试进入新地图，成功载入LoadScene，已访过则重进入WarScene---
    public void GotoNewMap()
    {
        if(!TheGlobalManager.TGM.IsSafe())
        {
            return;
        }
        Transform ourmerlin = PlayerController.EntityMerlin.transform;
        int direction;

        if(ourmerlin.position.x >= 0)
        {
            if(ourmerlin.position.y >= 0)
            {
                direction = 1;
            }
            else
            {
                direction = 4;
            }
        }
        else
        {
            if(ourmerlin.position.y >= 0)
            {
                direction = 2;
            }
            else
            {
                direction = 3;
            }
        }

        if(TheGlobalManager.TGM.EnterScene(direction, TheGlobalManager.TGM.getseed() + 5))
            SceneManager.LoadScene("LoadScene");
        else
            SceneManager.LoadScene("WarScene");

        

    }
}
