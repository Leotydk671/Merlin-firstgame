using UnityEngine;
using System.Collections;

//---敌人生成器：从Resources加载预制体，按固定间隔在区域内随机生成指定数量的敌人，生成位置需与玩家保持最小距离---
public class EnemySpawner : MonoBehaviour
{
    [Header("生成设置")]
    //---Resources文件夹下的预制体路径---
    public string prefabPath = "Enemies/BaseEnemy"; // Resources文件夹下的路径
    //---生成区域中心---
    public Vector2 spawnAreaCenter;
    //---生成区域大小---
    public Vector2 spawnAreaSize = new Vector2(20, 20);
    //---总共生成的敌人数量---
    public int totalEnemies = 10;
    //---每次生成间隔时间（秒）---
    public float spawnInterval = 3f;
    //---与玩家的最小安全距离---
    public float minDistanceFromPlayer = 5f;

    [Header("调试视图")]
    public bool showGizmos = true;
    public Color gizmoColor = Color.red;

    private Transform playerTransform;
   

    
    private GameObject enemyPrefab;
    private int spawnedCount;


    //---加载预制体资源并启动生成协程---
    void Start()
    {
        LoadEnemyPrefab();
        StartCoroutine(SpawnRoutine());
    }

    //---从Resources路径加载敌人预制体，路径无效时禁用自身---
    void LoadEnemyPrefab()
    {
        // 从Resources文件夹加载预制体
        enemyPrefab = Resources.Load<GameObject>(prefabPath);
        
        if(enemyPrefab == null)
        {
            Debug.LogError($"无法在路径 Resources/{prefabPath} 找到预制体");
            enabled = false;
            return;
        }

        // 验证预制体组件
        if(!enemyPrefab.GetComponent<EnemyAI>()) // 示例验证敌人AI组件
        {
            Debug.LogWarning("预制体缺少必要组件：EnemyAI");
        }
    }

    //---协程：每隔spawnInterval秒生成一个敌人，达到totalEnemies后停止---
    IEnumerator SpawnRoutine()
    {
        while(spawnedCount < totalEnemies)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnEnemy();
            spawnedCount++;
        }
    }

    void SpawnEnemy()
    {
        Vector2 spawnPos = GetValidSpawnPosition();
        if(spawnPos == Vector2.negativeInfinity) return;

        // 实例化预制体并保持原始设置
        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity, transform);
        ApplyOriginalPrefabSettings(enemy);
    }

    void ApplyOriginalPrefabSettings(GameObject instance)
    {
        // 保持原始激活状态
        instance.SetActive(enemyPrefab.activeSelf);
        
        // 保持所有组件状态
        MonoBehaviour[] prefabComponents = enemyPrefab.GetComponents<MonoBehaviour>();
        MonoBehaviour[] instanceComponents = instance.GetComponents<MonoBehaviour>();
        
        for(int i=0; i<prefabComponents.Length; i++)
        {
            if(i < instanceComponents.Length)
            {
                instanceComponents[i].enabled = prefabComponents[i].enabled;
            }
        }
    }

    Vector2 GetValidSpawnPosition()
    {
        int attempts = 0;
        const int maxAttempts = 30;

        while(attempts < maxAttempts)
        {
            Vector2 randomPoint = new Vector2(
                Random.Range(-spawnAreaSize.x/2, spawnAreaSize.x/2),
                Random.Range(-spawnAreaSize.y/2, spawnAreaSize.y/2)
            ) + spawnAreaCenter;

            if(playerTransform != null && 
               Vector2.Distance(randomPoint, playerTransform.position) < minDistanceFromPlayer)
            {
                attempts++;
                continue;
            }

            Collider2D[] colliders = Physics2D.OverlapCircleAll(randomPoint, 1f);
            bool isValid = true;
            foreach(Collider2D col in colliders)
            {
                if(col.CompareTag("Enemy"))
                {
                    isValid = false;
                    break;
                }
            }

            if(isValid) return randomPoint;
            attempts++;
        }

        Debug.LogWarning("未找到有效生成位置");
        return Vector2.negativeInfinity;
    }

    void OnDrawGizmos()
    {
        if(!showGizmos) return;

        Gizmos.color = gizmoColor;
        Vector3 center = new Vector3(spawnAreaCenter.x, spawnAreaCenter.y, 0);
        Vector3 size = new Vector3(spawnAreaSize.x, spawnAreaSize.y, 0.1f);
        Gizmos.DrawWireCube(center, size);
    }
        
    }
