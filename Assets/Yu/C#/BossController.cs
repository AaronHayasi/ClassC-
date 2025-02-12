using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    public int maxHealth = 3;  
    private int currentHealth;
    private Slider healthBar;//HPUI

    public GameObject bulletType1Prefab;  
    public GameObject bulletType2Prefab;  

    public float fireRateType1 = 1f;  
    public float fireRateType2 = 2f;  

    public int scatterBulletCount = 8;  

    public EnemyManager enemyManager;  
    private bool isAlive = true;  
    private SpriteRenderer spriteRenderer;

    public Vector2 moveRange = new Vector2(5f, 3f);  
    public float moveSpeed = 2f;  
    public float moveInterval = 2f;  
    public int scoreValue = 870;//ScoreUI

    private Vector2 targetPosition;  

    void Start()
    {
        currentHealth = maxHealth;  
        healthBar = GetComponentInChildren<Slider>();//HPUI
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
            Debug.Log("HealthBar initialized");
        }
        else
        {
            Debug.LogError("HealthBar not found!");//HPUI
        }
        spriteRenderer = GetComponent<SpriteRenderer>();

        StartCoroutine(FireBulletType1()); 
        StartCoroutine(FireBulletType2()); 
        StartCoroutine(RandomMove());  
    }

    
    IEnumerator FireBulletType1()
    {
        while (isAlive)
        {
            yield return new WaitForSeconds(fireRateType1);
            ScatterShot(bulletType1Prefab);  
        }
    }

    
    IEnumerator FireBulletType2()
    {
        while (isAlive)
        {
            yield return new WaitForSeconds(fireRateType2);
            ScatterShot(bulletType2Prefab);  
        }
    }

    
    void ScatterShot(GameObject bulletPrefab)
    {
        float baseAngle = Random.Range(0f, 360f);  
        float angleStep = 360f / scatterBulletCount;  

        for (int i = 0; i < scatterBulletCount; i++)
        {
            
            float angle = baseAngle + (i * angleStep);
            Vector2 direction = GetDirectionFromAngle(angle);

            
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().velocity = direction * 5f;
        }
    }

    
    Vector2 GetDirectionFromAngle(float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    
    IEnumerator RandomMove()
    {
        while (isAlive)
        {
            float randomX = Random.Range(-moveRange.x, moveRange.x);
            float randomY = Random.Range(-moveRange.y, moveRange.y);
            targetPosition = new Vector2(randomX, randomY);

            while ((Vector2)transform.position != targetPosition)
            {
                transform.position = Vector2.MoveTowards(
                    transform.position,
                    targetPosition,
                    moveSpeed * Time.deltaTime
                );
                yield return null;  
            }

            yield return new WaitForSeconds(moveInterval);  
        }
    }

    
    public void TakeDamage(int damage)
    {
        if (!isAlive) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);//HPUI

        if (healthBar != null)
        {
            healthBar.value = currentHealth;
            Debug.Log(currentHealth);//HPUI
        }
        StartCoroutine(FlashEffect());

        Debug.Log($"Boss took {damage} damage. Current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    
    IEnumerator FlashEffect()
    {
        for (int i = 0; i < 3; i++)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
    }

    
    void Die()
    {
        if (!isAlive) return;

        isAlive = false;
        Debug.Log("Boss is dead.");

        if (enemyManager != null)
        {
            enemyManager.OnBossDefeated();  
        }

        ScoreManager.instance.AddScore(scoreValue);
        Destroy(gameObject);  
    }

    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerBullet"))
        {
            TakeDamage(1);  
            Destroy(other.gameObject);  
        }
    }
}