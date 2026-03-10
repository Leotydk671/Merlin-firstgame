using System;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.Mathematics;

//---全局单例管理器：跨场景持久存在，负责存档/读屘、地图坐标管理、气候计算、难度计算和进入新场景逻辑---
public class TheGlobalManager : MonoBehaviour
{
    //---静态单例实例（内部）---
    private static TheGlobalManager TGM_ = null;

    //---当前地图 X 坐标---
    int current_x;
    //---当前地图 Y 坐标---
    int current_y;

    //---当前地图生成种子---
    int current_seed;
    //---当前地图温度值---
    float current_temperature;

    //---当前地图湿度值---
    float current_moisture;

    //---当前场景是否为安全地带---
    bool current_safe;

    //---是否处于战斗场景中---
    bool in_warscene = false;

    //---当前存档对象---
    private Save thissave;


    //---将当前存档对象序列化为二进制文件存到Saves/save0.txt---
    private void SaveByBin()
    {
        //Save save = GetSaveInfo();//先将需要存档的游戏信息读取过来并保存起来
        BinaryFormatter bf = new BinaryFormatter();//创建一个二进制格式化程序
        FileStream fileStream = File.Create(Application.dataPath + "/Saves" + "/save0.txt");//创建一个文件流
        bf.Serialize(fileStream, thissave);//利用二进制格式化程序的序列化方法来序列化save对象，参数：创建的文件流和需要序列化的对象
        fileStream.Close();  //关闭流
        if (!File.Exists(Application.dataPath + "/Saves" + "/save0.txt"))
        {
            Debug.Log("Cant save the game");
        }
    }


    //---从二进制文件反序列化读取Save对象，文件不存在则返回null---
    private Save LoadByBin()
    {
        if (File.Exists(Application.dataPath + "/Saves" + "/save0.txt"))
        {
            BinaryFormatter bf = new BinaryFormatter();//创建一个二进制格式化程序
            FileStream fileStream = File.Open(Application.dataPath + "/Saves" + "/save0.txt", FileMode.Open);//打开数据流
            Save save = (Save)bf.Deserialize(fileStream);//调用二进制格式化程序中的反序列化方法，将数据流反序列化为save对象并进行保存
            fileStream.Close();//关闭文件流
            return save;
        }
        else
        {
            
            Debug.Log("加载失败！！");
            return null;
        }
    }

    //---尝试读取存档，成功则恢复坐标/气候/元素数据，返回true；失败返回false---
    public bool TrytoLoad()
    {
        Save loadsave = LoadByBin();
        if(loadsave != null)
        {
            thissave = loadsave;
            Tuple<int,int> LastSafeinSave = thissave.LastSafeScene;
            current_x = LastSafeinSave.Item1;
            current_y = LastSafeinSave.Item2;
            Debug.Log("load success----x: "+ current_x + " ,y: "+ current_y );
            SingleScene ss = thissave.GlobalMapInfoTree[LastSafeinSave];
            current_moisture = ss.moisture;
            current_temperature = ss.temperature;
            current_safe = true;
            current_seed = ss.seed;
            if(thissave.ElementNum != null)
            {
                Tuple<int,int,int,int> oldata = thissave.ElementNum;
                RandomDataBehavior.savedData[0] = oldata.Item1;
                RandomDataBehavior.savedData[1] = oldata.Item2;
                RandomDataBehavior.savedData[2] = oldata.Item3;
                RandomDataBehavior.savedData[3] = oldata.Item4;
            }
            return true;
        }
        return false;
    } 


    //---初始化单例并DontDestroyOnLoad保持跨场景持久---
    void Awake()
    {
        TGM_ = this;
        DontDestroyOnLoad(TGM_.gameObject);
    }

    //---初始化存档对象---
    void Start()
    {
        Debug.Log("GlobalStart");
        thissave = new Save();
    }

    // Start is called before the first frame update

    //---TGM属性：与TheGlobalManager单例的外部访问入口---
    public static TheGlobalManager TGM
    {
        get
        {
            return TGM_;
        }
    }

    //---初始化新世界：清空地图字典、设置开始坈60,0)、初始化温湿度和元素数量并存档---
    public void SetNewWorld(int m_seed)
    {
        thissave.GlobalMapInfoTree.Clear();
        thissave.LastSafeScene = new Tuple<int,int>(0,0);
        SingleScene bornscene = new SingleScene();

        current_x = 0;
        current_y = 0;
        current_moisture = 10.0f;
        current_temperature = 10.0f;
        current_safe = false;
        current_seed = m_seed;

        bornscene.seed = m_seed;
        bornscene.moisture = 10.0f;
        bornscene.safe = true;
        bornscene.temperature = 10.0f;  
        
        thissave.GlobalMapInfoTree.Add(new Tuple<int, int>(current_x, current_y), bornscene);
        thissave.ElementNum = new Tuple<int, int, int, int>(2,2,2,2);

        SaveByBin();
    }

    //---进入新地图：保存当前地图信息，根据方向更新坐标和气候参数，已访过返回false否则true---
    public bool EnterScene(int direction, int m_seed)
    {
        in_warscene = false;
        bool is_new = false;
        Tuple<int,int> CurrentPos = new Tuple<int, int>(current_x, current_y);
        if( !thissave.GlobalMapInfoTree.ContainsKey(CurrentPos))
        {
            Debug.Log("saved a new map");
            SingleScene lastscene = new SingleScene();
            lastscene.seed = current_seed;
            lastscene.temperature = current_temperature;
            lastscene.moisture = current_moisture;
            lastscene.safe = true;

            thissave.GlobalMapInfoTree.TryAdd(CurrentPos, lastscene);
            thissave.LastSafeScene = CurrentPos;
            thissave.ElementNum = new Tuple<int, int, int, int>(RandomDataBehavior.savedData[0],
                                                                RandomDataBehavior.savedData[1],
                                                                RandomDataBehavior.savedData[2],
                                                                RandomDataBehavior.savedData[3]);
            SaveByBin();
        }
        Debug.Log("saved a already haved map");



        if(direction == 1)
        {
            current_y += 1;
            current_moisture += 0.5f;
            current_temperature -= 0.5f;
        }
        else if(direction == 2)
        {
            current_x -= 1;
            current_moisture -= 0.5f;
            current_temperature -= 0.5f;
        }
        else if(direction == 3)
        {
            current_y -= 1;
            current_moisture -= 0.5f;
            current_temperature += 0.5f;
        }
        else
        {
            current_x += 1;
            current_moisture += 0.5f;
            current_temperature += 0.5f;
        }



        CurrentPos = new Tuple<int, int>(current_x, current_y);
        if(thissave.GlobalMapInfoTree.ContainsKey(CurrentPos))
        {
            Debug.Log("Already have");
            current_safe = true;
            current_seed = thissave.GlobalMapInfoTree[CurrentPos].seed;
        }
        else
        {
            Debug.Log("New map");
            current_safe = false;
            is_new = true;
            current_seed = m_seed;
        }

        return is_new;
    }


    //---根据当前温度/湿度返回地图气候类型：1雪地/2草原/3沙漠/4森林---
    public int Climate()
    {
        if(current_temperature <= 9.0f)
        {
            if(current_moisture >= 11.0f)
            {
                return 1;   //雪地
            }
            else
                return 2;   //草原
        }
        else 
        {
            if(current_moisture <= 9.0f)
                return 3;   //沙漠
            else    
                return 4;  //森林
        }
    }

    //---返回当前地图种子---
    public int getseed()
    {
        return current_seed;
    }

    //---返回当前场景是否为安全地带---
    public bool IsSafe()
    {
        return current_safe;
    }

    //---将当前场景标记为安全---
    public void SetSafe()
    {
        current_safe = true;
    }

    //---标记当前处于战斗场景中---
    public void InWar()
    {
        in_warscene = true;
    }

    //---返回是否处于战斗场景---
    public bool IsInWar()
    {
        return in_warscene;
    }

    //---返回当前地图坐标(x,y)元组---
    public Tuple<int,int> GetCurrentPosition()
    {
        return new Tuple<int, int>(current_x, current_y);
    }

    //---根据当前坐标到原点的曼哈顿距离计算地图难度等级，返回每层敌人数量基数---
    public int GetdDifficult()
    {
        int cnum = Math.Abs(current_x) + Math.Abs(current_y);
        if(cnum <= 1)
        {
            return 4;
        }
        else if(cnum <= 3)
        {
            return 6;
        }
        else if(cnum <= 5)
        {
            return 8;
        }
        else
        {
            return 10;
        }
        
    }

}
