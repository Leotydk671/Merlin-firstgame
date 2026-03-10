using UnityEngine;

//---开始场景背景图片：游戏启动时通过CanvasGroup的alpha属性在2秒内淡入显示---
public class StartImage : MonoBehaviour
{
    //---画布组件，用于控制整体透明度---
    private CanvasGroup canvasGroup;

    //---初始化CanvasGroup并开始淡入协程---
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        StartCoroutine(FadeIn());
    }

    //---协程：在duration秒内将alpha从0线性渐变到1---
    private System.Collections.IEnumerator FadeIn()
    {
        float duration = 2f; // 渐变持续时间，单位为秒
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            yield return null; // 等待下一帧
        }

        canvasGroup.alpha = 1f; // 确保完全显示
    }
}