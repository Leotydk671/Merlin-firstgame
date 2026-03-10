using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//---退出按钮设置：提供PauseAndExit()方法，调用Application.Quit()退出游戏---
public class ExitButtonSetting : MonoBehaviour
{
    // Start is called before the first frame update
    /*
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    */

    public void PauseAndExit()
    {
        Debug.Log("Merlin exit!");
        Application.Quit();
    }
}
