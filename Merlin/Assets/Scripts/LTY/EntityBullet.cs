using Unity.VisualScripting;
using UnityEngine;

//---游戏中实际存在于场景的子弹实体，继承自BulletBase，负责子弹的初始化、移动、碰撞検测与生命周期管理---
public class EntityBullet : BulletBase
{
    // Start is called before the first frame update

    //public SpellEntity spellentity = null;

    //---挂载的逻辑修饰符数量---
    public int logical_num = 0;

    //---子弹已存活时间计时器---
    private float born_time = 0; 
    //---子弹最大存活时间（超过后自动销毁）---
    public float alive_distance_time = 1.0f;

    //---当前飞行速度（可被逻辑修饰符在Process中修改）---
    public float Speed = 0.05f;

    //---标记当前子弹是否为副弹（副弹不再射出副弹，防止无限递归）---
    public bool copy = false;

    //public int collietime = 1;

    //---子弹刚生时的世界坐标位置---
    public Vector3 original_position;
    //---子弹刚生时的指向向量（主弹期间会叠加随机偏差）---
    public Vector3 original_up;

    //---随机山顶偏差参数，控制弹道散射范围---
    public float skpara = 0.5f;

    //---子弹挂载的逻辑修饰符实例数组---
    public LogicalOperators[] operators = null; 

    //---该子弹对应的实体类型（带有属性、贴图等信息）---
    public SpellEntity bullet_main_entity = null;

    //---发射间隔时间，从实体中读取，防止连射---
    public float intervaltime;    

    //---静态工厂方法：在指定位置和方向创建子弹实例，绑定所有组件并从管理器芳集法术配置---
    public static EntityBullet Create(Vector3 MerlinPosition, Vector3 Bullet_up, bool Iscopy)
    {
        //Bullet_up += (Vector3)(UnityEngine.Random.insideUnitCircle * 0.3f);
        GameObject ABullet = new GameObject("ABullet");
        ABullet.tag = "Bullet";
        EntityBullet bullet_instance = ABullet.AddComponent<EntityBullet>();

        Debug.Log("Iscopy == " + Iscopy);
        bullet_instance.copy = Iscopy;
        bullet_instance.logical_num = TestBulletManager.TBM.setoperators(ref bullet_instance.operators, ref bullet_instance.bullet_main_entity);
        Bullet_up.z = 0f;
        bullet_instance.original_up = Bullet_up;

        bullet_instance.original_position = MerlinPosition;
        SpriteRenderer thisSpriteRenderer = ABullet.AddComponent<SpriteRenderer>();
        thisSpriteRenderer.sprite = Instantiate(Resources.Load<Sprite>(bullet_instance.bullet_main_entity.Sprite_Location));
        thisSpriteRenderer.sortingLayerName = "flowers";
        thisSpriteRenderer.sortingOrder = 1;

        CircleCollider2D Bullet_collider = ABullet.AddComponent<CircleCollider2D>();
        Bullet_collider.isTrigger = true;


        if(bullet_instance.operators == null)
        {
            Debug.Log("FUUUCK2");
        }

        //Debug.Log("entityid: " + bullet_instance.GetInstanceID() + "  count: " + TBM.count);
        return bullet_instance;
    }

    void Start()
    {
        transform.position = original_position;
        //transform.up = original_up;
        Speed = bullet_main_entity.basespeed;

        bullet_main_entity.Emit(this);
        intervaltime = bullet_main_entity.intervaltime;
        Debug.Log("this time internal = " + intervaltime);
        if(operators == null)
        {
            Debug.Log("Empty pointer");
        }
        else
        {
            Debug.Log("Accessful Start EntityBullrt");
            for(int i = 0; i < logical_num; i++)
            {
                if(operators[i] == null)
                {
                    Debug.Log("operators[i] is null! " + i);

                }
                else 
                {
                    operators[i].Emit(this);
                    Debug.Log("internal = "+intervaltime);
                }
            }
            
        }
        original_up += (Vector3)(UnityEngine.Random.insideUnitCircle * skpara);
        //transform.up = original_up;

        //bullet_main_entity.Emit(this);

        if(copy == false && (Time.time - TestBulletManager.last_bullet_time) < intervaltime)
        {
            Debug.Log("interval time " + bullet_main_entity.intervaltime);
            Destroy_bullet();
            return;
        }

        //bullet_main_entity.Emit(this);

        TestBulletManager.last_bullet_time = Time.time;
        born_time = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        //---每帧按original_up方向移动，调用主实体和所有逻辑修饰符的Process方法---
        //Speed = bullet_main_entity.GetSpeed();
        transform.position += original_up.normalized * Speed * Time.smoothDeltaTime * 100;
        bullet_main_entity.Process(this);
        for(int i = 0; i < logical_num; i++)
        {
            if(operators[i] == null)
            {
                Debug.Log("operators[i] is null!" + i);

            }
            else
            {
                //Debug.Log("update");
                operators[i].Process(this);
            }
        }
        
        if(Time.time - born_time >= alive_distance_time)
        {
            Destroy_bullet();
        }
    }

    //---碰撞触发器：将碰撞信息分队转发给所有逻辑修饰符和主实体的Hit方法---
    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("hhhhhhhhh");
        
        for(int i = 0; i < logical_num; i++)
        {
            if(operators[i] == null)
                continue;
            operators[i].Hit(collision, this);
        }
        
        bullet_main_entity.Hit(collision, this);
    }

}
