using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject bulletPrefab;    // 通常の弾丸のプレハブ
    public GameObject specialBulletPrefab;  // 特殊弾丸のプレハブ（ボス用）
    public float minFireRate = 1.5f;   // 最小発射間隔
    public float maxFireRate = 3f;     // 最大発射間隔
    public int scoreValue = 10;        //敵を倒すとポイントが増加

    public bool isBoss = false;   // この敵がボスかどうか
    public Transform playerTarget;  // プレイヤーのターゲット
    public EnemyManager enemyManager;  // EnemyManager への参照

    void Start()
    {
        // プレイヤーのターゲットを探す
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTarget = player.transform;
        }
        else
        {
            Debug.LogWarning("Player オブジェクトが見つかりません！");
        }

        // ランダムな弾丸発射のコルーチンを開始
        StartCoroutine(FireBullets());
    }

    IEnumerator FireBullets()
    {
        while (true)
        {
            float fireRate = Random.Range(minFireRate, maxFireRate);  // ランダムな発射間隔
            yield return new WaitForSeconds(fireRate);

            if (playerTarget != null)
            {
                GameObject bullet = isBoss && Random.value > 0.5f
                    ? specialBulletPrefab
                    : bulletPrefab;

                Vector2 direction = (playerTarget.position - transform.position).normalized;
                GameObject firedBullet = Instantiate(bullet, transform.position, Quaternion.identity);
                firedBullet.GetComponent<Rigidbody2D>().velocity = direction * 5f;
            }
        }
    }

    void Die()
    {
        Debug.Log("Enemy died.");
        if (isBoss)
        {
            enemyManager.OnBossDefeated();
        }
        else
        {
            enemyManager.RemoveEnemy(gameObject);
        }
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("PlayerBullet"))
        {
            Die();  // 
            ScoreManager.instance.AddScore(scoreValue);
            Destroy(col.gameObject);  
        }
    }

    
}