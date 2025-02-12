using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject enemyPrefab;    // 小型敵のPrefab
    public GameObject bossPrefab;     // ボスのPrefab
    public Transform bossSpawnPoint;  // ボスの生成位置
    public GameObject victoryPanel;   // クリア画面パネル

    public int rows = 2;              // 小型敵の行数
    public int columns = 6;           // 小型敵の列数
    public float spacingX = 1.5f;     // 小型敵のX軸間隔
    public float spacingY = 1.5f;     // 小型敵のY軸間隔
    public float moveSpeed = 2f;      // 小型敵の移動速度
    public float boundaryOffset = 0.5f; // 画面端のオフセット

    private List<GameObject> enemies = new List<GameObject>();  // 小型敵のリスト
    private bool movingRight = true;   // 初期移動方向は右
    private bool bossSpawned = false;  // ボスが生成されたかどうか

    void Start()
    {
        victoryPanel.SetActive(false);  // クリア画面を非表示にする
        SpawnEnemies();  // 小型敵を生成する
    }

    void Update()
    {
        Debug.Log($"Enemy Count: {enemies.Count}, Boss Spawned: {bossSpawned}");

        if (enemies.Count > 0)
        {
            MoveEnemies();  // 小型敵を移動させる
        }
        else if (!bossSpawned)
        {
            SpawnBoss();  // ボスを生成する
        }
    }

    void SpawnEnemies()
    {
        float totalWidth = (columns - 1) * spacingX;
        float startX = -totalWidth / 2;
        float startY = Camera.main.orthographicSize - boundaryOffset;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector2 position = new Vector2(startX + (col * spacingX), startY - (row * spacingY));
                GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity);

                Enemy enemyComponent = enemy.GetComponent<Enemy>();
                enemyComponent.enemyManager = this;
                enemies.Add(enemy);
            }
        }
    }

    void MoveEnemies()
    {
        float direction = movingRight ? 1 : -1;
        Vector3 move = new Vector3(direction * moveSpeed * Time.deltaTime, 0, 0);

        bool hitBoundary = false;

        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.transform.Translate(move);

                float screenHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
                if (Mathf.Abs(enemy.transform.position.x) + boundaryOffset >= screenHalfWidth)
                {
                    hitBoundary = true;
                }
            }
        }

        if (hitBoundary)
        {
            movingRight = !movingRight;
        }
    }

    void SpawnBoss()
    {
        Debug.Log("Spawning Boss...");
        GameObject boss = Instantiate(bossPrefab, bossSpawnPoint.position, Quaternion.identity);

        BossController bossController = boss.GetComponent<BossController>();
        if (bossController != null)
        {
            bossController.enemyManager = this;
            Debug.Log("Boss spawned successfully.");
        }

        bossSpawned = true;
    }

    public void RemoveEnemy(GameObject enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
            Destroy(enemy);
            Debug.Log($"Enemy removed. Remaining enemies: {enemies.Count}");
        }
    }

    public void OnBossDefeated()
    {
        victoryPanel.SetActive(true);
        Debug.Log("Boss defeated! Victory!");
    }
}