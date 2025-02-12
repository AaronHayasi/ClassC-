using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
    public GameObject PlayerBullet;
    public GameObject BulletPosition01;
    public GameObject BulletPosition02;
    public GameObject ExplosionGo;
    public GameManagerScript GameManager;


    //生命
    public int maxLives = 3; // PlayerHealthUI
    public Image[] lifeImages; // PlayerHealthUI
    public float respawnTime = 2.0f; // PlayerHealthUI
    public Transform respawnPoint; // PlayerHealthUI
    public float flashDuration = 2.0f; // PlayerHealthUI
    public float flashInterval = 0.3f; // PlayerHealthUI
    private int currentLives; // PlayerHealthUI
    private bool isRespawning = false; // PlayerHealthUI
    private SpriteRenderer spriteRenderer; // PlayerHealthUI
    private Renderer renderer3D; // PlayerHealthUI
    private bool isDead;



    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        currentLives = maxLives;
        UpdateLifeUI();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage()
    {
        if (currentLives > 0 && !isRespawning)
        {
            currentLives--;
            UpdateLifeUI();

            if (currentLives <= 0 && !isDead)
            {
                isDead = true;
                gameObject.SetActive(false);
                GameManager.gameOver();
                GameOver();
            }
            else
            {
                StartCoroutine(Respawn());
            }
            StartCoroutine(FlashEffect());
        }
    }
    private IEnumerator FlashEffect()
    {
        float elapsedTime = 0f;

        while (elapsedTime < flashDuration)
        {
            if (spriteRenderer != null) // PlayerHealthUI
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
            }

            yield return new WaitForSeconds(flashInterval);
            elapsedTime += flashInterval;
        }

        // PlayerHealthUI
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }
        else if (renderer3D != null)
        {
            renderer3D.enabled = true;
        }
    }

    // PlayerHealthUI
    void UpdateLifeUI()
    {
        for (int i = 0; i < lifeImages.Length; i++)
        { 

            lifeImages[i].enabled = i < currentLives;
        }
    }

    
    void GameOver()
    {
        GameObject explosion = (GameObject)Instantiate(ExplosionGo);
        explosion.transform.position = transform.position;
        Destroy(gameObject);
    }

    
    private IEnumerator Respawn()
    {
        isRespawning = true;
        
        
        yield return new WaitForSeconds(respawnTime);

        
        transform.position = respawnPoint.position;

        Debug.Log("Player Respawned");
        isRespawning = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {

            //play laser sound effect
            GetComponent<AudioSource>().Play();


            GameObject bullet01 = (GameObject) Instantiate (PlayerBullet);
            bullet01.transform.position = BulletPosition01.transform.position;

            GameObject bullet02 = (GameObject) Instantiate (PlayerBullet);
            bullet02.transform.position = BulletPosition02.transform.position;
        }
        float x = Input.GetAxisRaw("Horizontal");//the value will be -1,0 or 1 (for left, no input, and right)
        float y = Input.GetAxisRaw("Vertical"); // the value will be -1,0 or 1 (down, on input, and up)

        Vector2 direction = new Vector2(x, y).normalized;

        Move (direction); //set player's position

     
    }

    void Move(Vector2 direction)
    {

     
        //screen limits player movement 
        Vector2 min = Camera.main.ViewportToWorldPoint(new Vector2 (0,0));
        Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

        max.x = max.x - 0.225f;
        min.x = min.x + 0.225f;

        max.y = max.y - 0.285f;
        min.y = min.y + 0.285f;

        Vector2 pos = transform.position;
        pos += direction * speed * Time.deltaTime;

        pos.x = Mathf.Clamp(pos.x, min.x, max.x);
        pos.y = Mathf.Clamp(pos.y, min.y, max.y);

        transform.position = pos;
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
         if ((collision.tag == "Enemy") || (collision.tag == "EnemyBullet") || (collision.tag == "Boss") || (collision.tag == "BossBullet")) //detect collisition of the player ship with enemy or bullet
        {

            TakeDamage();
        }
    }

}
