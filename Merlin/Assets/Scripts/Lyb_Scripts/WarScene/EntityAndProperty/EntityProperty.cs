using UnityEngine;


//---角色属性组件：存储血量/速度/护甲/法抗等属性的初始值和当前值，管理状态（Situation枚举），通过UpdataAll()汇总更新所有UI显示---
public class EntityProperty : MonoBehaviour
{
    // 属性初始值
    public float Blood;  // 原始血量
    private float Speed;  // 原始移动速度
    private float FrontDefense;  // 原始前护甲
    private float BehindDefense;  // 原始后护甲
    private float[] MagicDefense = new float[5];  // 原始法抗

    // 公有变量
    // 背景图片
    // public Image EntityBackground;
    //---当前血量，由子弹命中时扣减---
    public float CurrentBlood;
    //---当前移动速度，可受状态影响（如Frozen降速）---
    public float CurrentSpeed;
    public float CurrentFrontDefense;
    public float CurrentBehindDefense;
    //---当前法术抗性数组（0=通用，1=火，2=水，3=风，4=土）---
    public float[] CurrentMagicDfense = new float[5];  // 法抗:火、水、风、土
    //---角色状态枚举：共10种状态---
    public enum situation
    {
        Nothing,  // 无
        Toxic,  // 中毒
        Burn,  // 灼烧
        Wet,  // 潮湿
        Frozen,  // 冰冻
        Easyhurt,  // 易伤
        Magicshield,  // 法抗护盾
        Physicshield,  // 物理护盾
        Fast,  // 迅捷
        Death  // 死亡
    }
    //---当前状态和状态剩余时间的元组---
    public (situation , float) CurrentSituation;
    //---状态持续总时长，默认2秒---
    public float SituationContinuousTime = 2.0f;
    public BarManager ShowBlood;
    public BarManager ShowSpeed;
    public BarManager ShowFrontDefense;
    public BarManager ShowBehindDefense;
    //---法抗数值UI显示组件数组---
    public ValueManager[] ShowMagicDefense = new ValueManager[5];
    //---状态图标UI显示组件---
    public SituationManager ShowSituation;


    //---从CharacterStatusConfig读取初始值并设置所有属性当前值，同时设置初始状态为Nothing---
    public void Initialization(CharacterStatusConfig ConcreteInformation) 
    {
        CurrentBlood = Blood = ConcreteInformation.Blood;
        Debug.Log("initial blood is " + CurrentBlood);
        CurrentSpeed = Speed = ConcreteInformation.Speed;
        CurrentFrontDefense = FrontDefense = ConcreteInformation.FrontDefense;
        CurrentBehindDefense = BehindDefense = ConcreteInformation.BehindDefense;
        for (int i=0; i<=4; i++) CurrentMagicDfense[i] = MagicDefense[i] = ConcreteInformation.MagicDefense[i];
        CurrentSituation = (situation.Nothing , SituationContinuousTime);

    }

    //---更新血条UI，血量归零时设置状态为Death---
    public void UpdateBlood() 
    {
        ShowBlood.UpdateBar(CurrentBlood);
        //ShowMovingBlood.UpdateBar(CurrentBlood);
        
        if (CurrentBlood <= 0)
            CurrentSituation.Item1 = situation.Death;
    }
    public void UpdateSpeed() 
    {
        ShowSpeed.UpdateBar(CurrentSpeed);
    }
    public void UpdateFrontDefense() 
    {
        ShowFrontDefense.UpdateBar(CurrentFrontDefense);
    }
    public void UpdateBehindDefense() 
    {
        ShowBehindDefense.UpdateBar(CurrentBehindDefense);
    }
    public void UpdateMagicDefense() 
    {
        for (int i=1; i<=4; i++)
            ShowMagicDefense[i].UpdateValue(CurrentMagicDfense[i]);
    }
    //---更新状态图标UI，状态剩余时间归零后重置速度并清除状态---
    public void UpdateSituation() 
    {
        //Sprite BackGroundSprite = EntityBackground.GetComponent<Sprite>();

        int SituationIndex = (int)CurrentSituation.Item1;
        CurrentSituation.Item2 -= Time.smoothDeltaTime;
        // HelpTime = CurrentSituation.Item2;
        ShowSituation.UpdateSituation(SituationIndex , CurrentSituation.Item2);
        if (CurrentSituation.Item2 <= 0)
        {
            CurrentSpeed = Speed;
            CurrentSituation.Item2 = SituationContinuousTime;
            CurrentSituation.Item1 = situation.Nothing;
            //BackGroundSprite = null;
        }

        // 更新背景图片
        /*
        if (CurrentSituation.Item1!=EntityProperty.situation.Nothing && EntityBackground.sprite==null)
        {
            switch(CurrentSituation.Item1)
            {
                case EntityProperty.situation.Frozen:
                    {
                        EntityBackground.sprite = Resources.Load<Sprite>("Textures/FrozenIce");
                    }
                    break;
                default:
                    break;
            }
        }
        */
    }
    //---汇总调用所有属性的UI更新方法---
    public void UpdataAll()
    {
        UpdateBlood();
        UpdateSpeed();
        UpdateFrontDefense();
        UpdateBehindDefense();
        UpdateMagicDefense();
        UpdateSituation();
    }
}
