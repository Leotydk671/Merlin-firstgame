using System;
using UnityEngine;

//---子弹管理单例：负责管理当前法术配置，存储修饰符类型和主弹实体，接收鼠标点击输入并实例化子弹---
public class TestBulletManager : MonoBehaviour
{
    //---整个游戏全局唯一实例---
    public static TestBulletManager TBM = null;

    //---玩家角色游戏对象---
    public GameObject Merlin_entity = null;

    //---当前配置的修饰符类型数组，最多5个---
    Type[] operators_type = new Type[5];

    //LogicalOperators[] operators = null;

    //---当前选择的主弹实体类---
    SpellEntity main_bullet_entity = null;

    //---当前配置的修饰符数量---
    public int extension_num = 0;

    //private int global_bullet = 1; 
    public int count = 0;

    //---上次发射的全局时间戳，用于控制攻击间隔---
    // 初始攻击时间
    public static float last_bullet_time = 0;

    //---攻击间隔时间，单位秒---
    public float AttackEnemyTime = 0f;

    //---将修饰符类型数组通过反射实例化为LogicalOperators数组，同时输出主弹实体类---
    public int setoperators(ref LogicalOperators[] operators_, ref SpellEntity main_bullet_entity_)
    {
        main_bullet_entity_ = main_bullet_entity;
        operators_ = new LogicalOperators[extension_num];
        if(operators_type == null)
        {
            Debug.Log("NULL Operator_type");
            return 0;
        }
        for(int i = 0; i < extension_num; i++)
        {
            if(operators_type[i] == null)
                continue;
            var type_instance = Activator.CreateInstance(operators_type[i]);
            if(type_instance is LogicalOperators type_Instance)
            {
                operators_[i] = type_Instance;
            }
            else
            {
                Debug.Log("Invalid interface type");
            }
        }

        if(operators_ == null)
        {
            Debug.Log("FUUUCK1");
        }
        return extension_num; 
    }


    //---遍历UI法术槽格子，读取选中的子弹实体类型和修饰符类型---
    public void sequence_set(GameObject []boxes, int num)
    {
        int index = 0;
        for(int i = 0; i < num; i++)
        {
            Boxselected box = boxes[i].GetComponent<Boxselected>();
            if(box.IsBulllet)
            {
                if(Activator.CreateInstance(box.SpellType) is SpellEntity Spe)
                    main_bullet_entity = Spe;
                //main_bullet_entity = 
               
                //extension_num++;
            }
            else if(box.IsComponent)
            {
                operators_type[index] = box.SpellType;
                index++;
                extension_num++;
            }
        }
    }

    //---初始化单例，设置全局TBM引用---
    void Awake()
    {
        TBM = this;
    }



    // Start is called before the first frame update
    void Start()
    {
        Merlin_entity = PlayerController.EntityMerlin.gameObject;
        //operators = new LogicalOperators[]{new SpellLogical_Test()};
        //operators_type = new Type[]{typeof(SpellLogical_Test)};
        //bullet_type = typeof(SpellEntity_Aquabullet);
        //extension_num = 0;
    }









    //---每帧监听左键点击，在IsMoving状态下将鼠标位置作为目标创建新子弹---
    // Update is called once per frame
    void Update()
    {
        if(GameManagerBehavior.gm.IsMoving)
        {
            if(Input.GetMouseButtonDown(0))
            {
                Vector3 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if(main_bullet_entity == null)
                {
                    Debug.Log("NO BULLET");
                }
                else
                {
                    Debug.Log("here not empty");
                    /*
                    if(global_bullet == 1)   
                        main_bullet_entity = new SpellEntity_Aquabullet();
                    else
                    {
                        main_bullet_entity = new SpellEntity_Aerbullet();
                    }*/
                    CreateANewBullet(mousepos);
                    
                }
            }

        }
        
        /*if(Input.GetKeyDown(KeyCode.T))
        {
            extension_num = 1;
        }
        if(Input.GetKeyDown(KeyCode.Y))
        {
            extension_num = 0;
        }
        if(Input.GetKeyDown(KeyCode.B))
        {
            global_bullet = 0;
        }
        if(Input.GetKeyDown(KeyCode.N))
        {
            global_bullet = 1;
        }*/
    }


    //---从玩家位置创建新子弹，方向指向目标位置，copy=false表示原始子弹---
    public void CreateANewBullet(Vector3 targetpos)
    {
        Debug.Log("OOOOOOOOOOOOOO");
        Vector3 pos = Merlin_entity.transform.position;
        pos.z = 0f;
        pos += new Vector3(0, 0.6f, 0);
                    
                    //mousepos = new Vector3(0,1,0);
        targetpos.z = 0;

                    //count++;

        EntityBullet eb = EntityBullet.Create(pos, targetpos-pos, false);
                    
        if(eb.operators == null)
        {
            Debug.Log("Already failed");
        }
        else Debug.Log("Good");        
    }

    //---从指定位置创建副弹，copy=true表示该子弹是由修饰符拼接层创建的副代子弹---
    public void CreateANewBullet2(Vector3 targetpos, Vector3 pos)
    {
        /*Vector3 pos = Merlin_entity.transform.position;
        Debug.Log("double bullet pos is " + pos.x + " " + pos.y);
        pos.z = 0f;
        pos += new Vector3(0, 0.6f, 0);*/
                    
                    //mousepos = new Vector3(0,1,0);
        targetpos.z = 0;

                    //count++;

        //targetpos += (Vector3)UnityEngine.Random.insideUnitCircle ;
        EntityBullet eb = EntityBullet.Create(pos, targetpos, true);
                    
        if(eb.operators == null)
        {
            Debug.Log("Already failed");
        }
        else Debug.Log("Good");        
    }
}
