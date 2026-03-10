using UnityEngine;

//---敌人攻击弹行为：沿自身up方向持续移动，存活2.5秒后自动销毁；命中Merlin时立即销毁---
public class EnemyAttackBehavior : MonoBehaviour
{
    //---游戏管理器引用，用于判断IsMoving状态---
    public GameManagerBehavior Controller = null;
    //---攻击弹移动速度---
    public float AttackSpeed = 4f;
    //记录存活时间
    private float LifeCount;

    //---初始化存活计时，获取GameManager引用，AerAttack标签的弹速加倍---
    // Start is called before the first frame update
    void Start()
    {
        LifeCount = Time.time;
        Controller = GameManagerBehavior.gm;

        if (gameObject.CompareTag("AerAttack"))
            AttackSpeed *= 1.8f;
    }

    //---每帧在IsMoving时向前移动，存活时间超限后销毁---
    // Update is called once per frame
    void Update()
    {
        if (Controller.IsMoving)
            transform.position += (AttackSpeed * Time.smoothDeltaTime) * transform.up;
        if (Time.time - LifeCount >= 2.5f)
            Destroy(transform.gameObject);
    }

    //---碰撞检测：命中Merlin标签对象时销毁自身---
    // collision
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Merlin":
                {
                    Destroy(transform.gameObject);
                }
                break;
            default:
                break;
        }
    }
}

