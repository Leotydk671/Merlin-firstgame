using UnityEngine;

//---简单追踪AI：每帧向Merlin标签对象方向移动，用于基础追踪类敌人---
public class EnemyAI : MonoBehaviour
{
    //---追踪移动速度---
    public float moveSpeed = 3f;
    //---玩家Transform引用---
    private Transform player;

    //---查找场景中Merlin标签对象并缓存Transform---
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Merlin").transform;
    }

    //---每帧计算指向玩家的方向并移动---
    void Update()
    {
        if(player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.Translate(direction * moveSpeed * Time.deltaTime);
        }
    }
}