using UnityEngine;

//---2D圆形旋转组件：每帧绕Z转以250度/秒的速度逐帧旋转---
public class CircleRotation2D : MonoBehaviour
{
    //---旋转速度（度/秒）---
    private float rotationSpeed = 250f;

    //---每帧执行Z轴旋转---
    void Update()
    {
        // 绕 Z 轴旋转，因为在 2D 中，物体通常在 XY 平面，旋转轴为 Z 轴
        transform.Rotate(0, 0, -rotationSpeed * Time.deltaTime);
    }
}