using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance; // 单例模式
    public Text scoreText; // 引用ScoreText
    private int score = 0; // 初始分数

    void Awake()
    {
        // 创建单例模式实例
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 显示初始分数
        UpdateScoreText();
    }

    // 击杀敌人时调用此方法
    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText();
    }

    // 更新UI文本
    private void UpdateScoreText()
    {
        scoreText.text = " " + score + " ";
    }
    void Update()
    {
        
    }
}
