using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 5f;        // 弾の速度
    private Vector2 direction;      // 弾の方向
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        if (direction != Vector2.zero)
        {
            rb.velocity = direction * speed;
        }
    }

    // 弾の飛行方向の設定
    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        if (rb != null)
        {
            rb.velocity = direction * speed;
        }
    }

    // 衝突イベントの処理
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player") 
        {
            Destroy(gameObject);
        }
    }
}