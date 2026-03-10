using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

//---跟随血条管理：将血条UI的世界坐标转为屏幕坐标并跟随目标，并根据Cinemachine镜头的正交大小动态缩放血条大小---
public class MovingBloodManager : MonoBehaviour
{
    //---Cinemachine虚拟镜头，用于读取正交大小来缩放血条---
    public CinemachineVirtualCamera CVC;
    //---血条跟随的目标对象---
    public GameObject target; // 要跟随的目标对象
    //---血条相对目标的世界偏移量---
    public Vector3 offset = new Vector3(0, 1.6f, 0); // 血条相对于目标的偏移量
    private Camera mainCamera;

    //---初始化相机引用和Cinemachine引用，设定初始跟随位置---
    void Start()
    {
        mainCamera = Camera.main; // 获取主相机
        CVC = FindObjectOfType<CinemachineVirtualCamera>();  // 获取唯一的virtual camera
        transform.position = target.transform.position + offset;
    }

    //---每帧将目标世界坐标转为屏幕坐标更新位置，并根据镜头正交大小线性缩放血条---
    void Update()
    {
        transform.position = target.transform.position + offset;
        // 将目标的世界坐标转换为屏幕坐标，并应用偏移量
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(target.transform.position + offset);
        // 更新血条的位置
        transform.position = screenPosition;
        
        // 让血条始终朝向相机
        transform.LookAt(transform.position + mainCamera.transform.forward);

        // 让血条随着相机比例变化而变化
        float scale = CVC.m_Lens.OrthographicSize;
        scale = (float)(88.0f-6.0f*scale)/49.0f;
        transform.localScale = new Vector3 (scale,scale,1);
    }

    // 设置血量的方法，可以被其他脚本调用
    /*
    public void SetHealth(float currentHealth, float maxHealth)
    {
        Slider slider = GetComponent<Slider>();
        if (slider != null)
        {
            slider.value = currentHealth / maxHealth; // 更新血条的填充量
        }
    }
    */
}