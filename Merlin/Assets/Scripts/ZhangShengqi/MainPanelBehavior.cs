using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

//---主背包面板：显示四元素持有数量和图标，管理多个ElementPanel按钮切换展示，Setnum()验证并扣除法术元素消耗---
public class MainPanelBehavior : MonoBehaviour
{
    //---所有元素法术信息子面板数组---
    public ElementPanelBehavior[] Targetpanels;
    //---面板上方四个元素图标的Image数组---
    public Image[] Upperimages = new Image[4];
    private TextMeshProUGUI textComponent; // 存储 Text 组件的引用
    //---四元素初始持有数量（从 RandomDataBehavior.savedData 提取）---
    private int[] InitNum = new int[4];
    //---当前剩余元素数量---
    private int[] CurrNum = new int[4];
    //---切换ElementPanel显示的按钮数组---
    public Button[] buttons;


    //---初始化四元素图标和数量（从RandomDataBehavior.savedData读取），为每个按钮注册面板切换监听，初始隐藏所有子面板---
    void Start()
    {
        for (int i=0; i<4; i++)
        {
            Upperimages[i].sprite = GameManagerBehavior.gm.sprites[i];
            //InitNum[i] = UnityEngine.Random.Range(1,9);
            InitNum[i] = RandomDataBehavior.savedData[i];
            CurrNum[i] = InitNum[i];
            textComponent = Upperimages[i].GetComponentInChildren<TextMeshProUGUI>();
            textComponent.text = InitNum[i].ToString();
            GameObject LowerimageElement = GameManagerBehavior.gm.LowImage.transform.Find("Element"+i).gameObject;
            textComponent = LowerimageElement.GetComponentInChildren<TextMeshProUGUI>();
            textComponent.text = 0.ToString();
            //textComponent = Lowerimages[i].GetComponentInChildren<TextMeshProUGUI>();
            //textComponent.text = magicscript.ElementInfo[i].ToString();
        }   
        if (Targetpanels.Length != buttons.Length)
        {
            Debug.LogError("Panels and buttons count must be the same!");
            return;
        }

        // 为每个按钮添加点击事件
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i; // 为了避免闭包问题
            buttons[i].onClick.AddListener(() => ShowPanel(index));
        }

        // 初始时隐藏所有面板
        HideAllPanels();
                                 
    }
    
    //---每帧同步显示元素数量并验证法术元素消耗是否足够---
    void Update()
    {
        //for (int i=0;i<4;i++)
        //{
            //Upperimages[i].sprite = GameManagerBehavior.sprites[i];
        //}
        Updatenum();
        Setnum();
    }
    //---更新面板上方各元素数量的文本显示---
    void Updatenum()
    {
        for (int i=0; i<4; i++)
        {
            //InitNum[i] = Random.Range(1,9);
            textComponent = Upperimages[i].GetComponentInChildren<TextMeshProUGUI>();
            textComponent.text = InitNum[i].ToString();
            //textComponent = Lowerimages[i].GetComponentInChildren<TextMeshProUGUI>();
            //textComponent.text = magicscript.ElementInfo[i].ToString();
        }    
    }

    //---遍历所有ElementPanel，对HasBeenCreate的面板验证元素是否足够，足够则扣除消耗，不足则撤销HasBeenCreate---
    void Setnum()
    {
            int[] temp = new int[4];
            for (int i=0;i<4;i++)
            {
                temp[i] = InitNum[i];
            }
        for (int j=0;j<Targetpanels.Length;j++)
        {
        if (Targetpanels[j].HasBeenCreate)
        {
            
            bool flag = true;
            for (int i=0;i<4;i++)
            {
                //temp[i] = InitNum[i];
                if (InitNum[i]-Targetpanels[j].magicscript.ElementInfo[i]>=0)
                {
                    InitNum[i]-=Targetpanels[j].magicscript.ElementInfo[i];
                }
                else
                {
                    flag = false;
                    Targetpanels[j].HasBeenCreate = false;
                    Targetpanels[j].SetButton.interactable = false;
                    Debug.Log("Hey@@@@@@@");
                }
                    
            }
            if (flag)
                Updatenum();
        }
        }
        for (int i=0;i<4;i++)
{
    InitNum[i] = temp[i];
} 
    }
    //---隐藏所有子面板，展示索引index对应的子面板，并重新绑定GetButton/SetButton监听---
    private void ShowPanel(int index)
    {
        HideAllPanels();
        Targetpanels[index].gameObject.SetActive(true);

        Targetpanels[index].GetButton.onClick.RemoveAllListeners();
        Targetpanels[index].SetButton.onClick.RemoveAllListeners();

        Targetpanels[index].GetButton.onClick.AddListener(Targetpanels[index].OnButtonClick);
        Targetpanels[index].SetButton.onClick.AddListener(Targetpanels[index].OnSetButtonClick);

        if(Targetpanels[index].HasBeenCreate)
        {
            Debug.Log("Setbutton interactable true");
            Targetpanels[index].SetButton.interactable = true;
        }
        else
        {
            Targetpanels[index].SetButton.interactable = false;
            Debug.Log("Setbutton interactable false");
        }
        for (int i=0; i<GameManagerBehavior.gm.BasicElementNum; i++)
        {
            GameObject LowerimageElement = GameManagerBehavior.gm.LowImage.transform.Find("Element"+i).gameObject;
            textComponent = LowerimageElement.GetComponentInChildren<TextMeshProUGUI>();
            textComponent.text = Targetpanels[index].magicscript.ElementInfo[i].ToString();
            //eleshow.sprite = magicscript.sprite;
            //LowerimageElement.GetComponent<Image>().sprite = GameManagerBehavior.gm.sprites[i];
            Debug.Log("seccesful gettext");
        }  

    }

    //---隐藏 Targetpanels 中所有ElementPanel的GameObject---
    private void HideAllPanels()
    {
        foreach (ElementPanelBehavior panel in Targetpanels)
        {
            panel.gameObject.SetActive(false);
        }
    }
}
