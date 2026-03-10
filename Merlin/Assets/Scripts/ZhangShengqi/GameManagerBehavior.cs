
using UnityEngine;
using UnityEngine.UI;

//---游戏总管理器单例：负责法术槽格子管理、按气候生成敌人、Tab开背包/P暂停/按钮面板切换，以及控制IsMoving状态---
public class GameManagerBehavior : MonoBehaviour
{
    //---四元素图标Sprite（火/水/土/风）---
    public Sprite[] sprites= new Sprite[4];
    //---全局唯一实例---
    public static GameManagerBehavior gm = null; 
    //---四元素初始数量---
    public int[] InitNum = new int[4];
    //---五个法术槽格子的GameObject数组---
    public static GameObject[] Boxes = new GameObject[5];

    //---各格子内法术名称---
    public string[] Spell_inBoxes_withName = new string[5];

    public bool[] Spell_inBoxes_isBullet = new bool[5]; 
    public bool[] Spell_inBoxes_isComponent = new bool[5]; 

    //---背包UI画布对象---
    public GameObject BagCanvas;
    //---暂停UI画布对象---
    public GameObject PauseCanvas;
    //---是否允许游戏运行（非暂停非背包状态时为true）---
    public bool IsMoving = true;

    //---当前已选中的法术槽格子---
    public Boxselected Selected_box;

    //---当前地图对应的敌人总数---
    public int NumberOfEnemy;

    //---法术槽格子总数---
    public int BasicElementNum = 4;

    //---法术槽总数---
    public int SpellNum = 5;

    //---获取法术到格子的按钮---
    public GameObject GetButton = null;

    //---将格子中法术确认装备的按钮---
    public GameObject SetButton = null;

    public GameObject LowImage = null;
    //---说明文字面板UI对象---
    public GameObject InstruCanvas;
    // 用于引用按钮组件
    public Button toggleButton;
    // 用于记录Panel的当前显示状态
    private bool isPanelVisible = false;
    //---怪物信息面板UI对象---
    public GameObject MonsterCanvas;
    // 用于引用按钮组件
    public Button monsterButton;
    // 用于记录Panel的当前显示状态
    private bool isMonsterVisible = false;

    //---初始化单例、加载元素图标、获取敌人数量、查找格子和按钮---
    void Awake()
    {
        IsMoving = true;
        gm = this;
        sprites[0] = Resources.Load<Sprite>("Textures/" + "Ignis");
        if (sprites[0]!=null)
        Debug.Log("success 0");
        sprites[1] = Resources.Load<Sprite>("Textures/" + "Aqua");
        Debug.Log("success 1");
        sprites[2] = Resources.Load<Sprite>("Textures/" + "Terra");
        Debug.Log("success 2");
        sprites[3] = Resources.Load<Sprite>("Textures/" + "Aer");
        Debug.Log("success 3");
        
        NumberOfEnemy = TheGlobalManager.TGM.GetdDifficult();

        Setthoseboxes();
        SetTwoButtons();
    }

    //---初始化默认选中格子、随机初始元素数量，按气候/难度生成敌人预制体，注册面板切换按钮监听---
    void Start()
    {
        Selected_box = Boxes[0].GetComponent<Boxselected>();
        for (int i=0; i<4; i++)
        {
            InitNum[i] = UnityEngine.Random.Range(1,9);
        }
        //Setfiveboxes();
        /*for (int i=0;i<5;i++)
        {
            BoxStatus[i] = false;
        }*/

        if(!TheGlobalManager.TGM.IsSafe())
        {
            GameObject TempEnemy;
                    // 生成敌人
            int climate = TheGlobalManager.TGM.Climate();
            if(climate == 1)
            {
                for (int i=0; i<(NumberOfEnemy-1); i++)
                {
                    TempEnemy = Instantiate(Resources.Load<GameObject>("Prefabs/WaterMonster"));
                    TempEnemy = Instantiate(Resources.Load<GameObject>("Prefabs/AerMonster"));
                }
                TempEnemy = Instantiate(Resources.Load<GameObject>("Prefabs/Boss_0"));
                TempEnemy = Instantiate(Resources.Load<GameObject>("Prefabs/Boss_0"));
            }
            else if(climate == 2)
            {
                for (int i=0; i<NumberOfEnemy; i++)
                {
                    TempEnemy = Instantiate(Resources.Load<GameObject>("Prefabs/FireMonster"));
                    TempEnemy = Instantiate(Resources.Load<GameObject>("Prefabs/LittleMonster_1"));
                }
                //TempEnemy = Instantiate(Resources.Load<GameObject>("Prefabs/Boss_0"));
            }
            else if(climate == 3)
            {
                for (int i=0; i<(NumberOfEnemy-1); i++)
                {
                    TempEnemy = Instantiate(Resources.Load<GameObject>("Prefabs/RockMonster"));
                    TempEnemy = Instantiate(Resources.Load<GameObject>("Prefabs/LittleMonster_0"));
                }
                TempEnemy = Instantiate(Resources.Load<GameObject>("Prefabs/Boss_1"));
                TempEnemy = Instantiate(Resources.Load<GameObject>("Prefabs/Boss_1"));
                
            }
            else
            {
                for (int i=0; i<NumberOfEnemy; i++)
                {
                    TempEnemy = Instantiate(Resources.Load<GameObject>("Prefabs/LittleMonster_0"));
                    TempEnemy = Instantiate(Resources.Load<GameObject>("Prefabs/LittleMonster_1"));
                }
                //TempEnemy = Instantiate(Resources.Load<GameObject>("Prefabs/Boss_0"));
            }
            
            //TempEnemy = Instantiate(Resources.Load<GameObject>("Prefabs/Boss_1"));

        }
        else
            NumberOfEnemy = 0;
        toggleButton.onClick.AddListener(TogglePanel);
        monsterButton.onClick.AddListener(MonsterPanel);
    }

    //---每帧响应Tab键切换背包/P键切换暂停，并根据各面板显示状态更新IsMoving---
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            BagCanvas.SetActive(!BagCanvas.gameObject.activeSelf);
            //Setfiveboxes();
            TestBulletManager.TBM.sequence_set(Boxes, 5);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            PauseCanvas.SetActive(!PauseCanvas.gameObject.activeSelf);
        }

        if (BagCanvas.activeSelf==true || PauseCanvas.activeSelf==true || InstruCanvas.activeSelf==true || MonsterCanvas.activeSelf==true) 
            IsMoving = false;
        else IsMoving = true;
    }

    //---切换说明(InstruCanvas)面板的显示与隐藏---
    void TogglePanel()
    {
        // 切换Panel的显示状态
        isPanelVisible = !isPanelVisible;
        InstruCanvas.SetActive(isPanelVisible);
//Setfiveboxes();
        //IsMoving = !IsMoving;
        if (isPanelVisible)
        {
            Debug.Log("PanelCanSeeEEEEE");
        }
        // 根据新的状态设置Panel的激活状态
        //panel.SetActive(isPanelVisible);
    }
    //---切换怪物信息(MonsterCanvas)面板的显示与隐藏---
    void MonsterPanel()
    {
        // 切换Panel的显示状态
        isMonsterVisible = !isMonsterVisible;
        MonsterCanvas.SetActive(isMonsterVisible);
//Setfiveboxes();
        //IsMoving = !IsMoving;
        if (isMonsterVisible)
        {
            Debug.Log("PanelCanSeeEEEEE");
        }
        // 根据新的状态设置Panel的激活状态
        //panel.SetActive(isPanelVisible);
    }
    //---在BagCanvas/Panel/AnOBject下查找box0~box4并存入静态Boxes数组---
    void Setthoseboxes()
    {
        if(BagCanvas == null)
        {
            Debug.Log("Cant find! BagCanvas is null");
            return;
        }

        for(int i = 0; i < SpellNum; i++)
        {
            Transform getted_box = BagCanvas.transform.Find("Panel/AnOBject/box" + i);
            if(getted_box == null)
                Debug.Log("Panel/AnOBject/box" + i + " is null");
            Boxes[i] = getted_box.gameObject;
        }
    }

    //---在BagCanvas/Panel下查找GetButton/SetButton/ImageLow并缓存其引用---
    void SetTwoButtons()
    {
        if(BagCanvas == null)
        {
            Debug.Log("Cant find! BagCanvas is null");
            return;
        }
        Transform getted_getbutton = BagCanvas.transform.Find("Panel/GetButton");
        Transform getted_setbutton = BagCanvas.transform.Find("Panel/SetButton");
        Transform image_low = BagCanvas.transform.Find("Panel/ImageLow");
        if(getted_getbutton == null || getted_setbutton == null)
        {
            Debug.Log("GetButton/Setbutton");
        }
        if(image_low == null)
        {
            Debug.Log("ImageLow is null");
        }
        GetButton = getted_getbutton.gameObject;
        SetButton = getted_setbutton.gameObject;
        LowImage = image_low.gameObject;
    }


    //---清除指定格子的IsBullet/IsComponent/SpellType状态，使其回到空槽---
    public void ClearTheBox(Boxselected box)
    {
        box.IsBulllet = false;
        box.IsComponent = false;
        box.SpellType = null;
    }
}
