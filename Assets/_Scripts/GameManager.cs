using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem explosion;
    [SerializeField] private int score = 0;
    [SerializeField] private int lives = 3;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private GameObject gameOverTexts;

    public static Func<GameObject, Vector3, int> OnPlayExplosion;
    public static Action<int> OnScoreUpdate;
    public static Action OnPlayerRespawn;

    private void Awake()
    {
        OnPlayExplosion += playExplosion;
        OnScoreUpdate += updateScore;
        gameOverTexts.SetActive(false);
        scoreText.text = score.ToString();
        livesText.text = lives.ToString();
    }

    private void OnDestroy()
    {
        OnPlayExplosion -= playExplosion;
        OnScoreUpdate -= updateScore;
    }
    
    private void Update()
    {
        if (lives <= 0 && Input.GetMouseButtonDown(0))
        {
            NewGame();
        }
        if (Input.GetKey(KeyCode.Q))
        {
            Application.Quit();
        }
    }

    private void NewGame()
    {
        gameOverTexts.SetActive(false);
        OnPlayerRespawn?.Invoke();
        score = 0;
        lives = 3;
        livesText.text = "3";
        scoreText.text = "0";
    }
    

    private int playExplosion(GameObject sender, Vector3 position)
    {
        explosion.transform.position = position;
        explosion.Play();

        if (sender.tag == "Player")
        {
            updateLives();
        }
        
        return lives;
    }

    private void updateScore(int Score)
    {
        score += Score;
        scoreText.text = score.ToString();
    }
    
    private void updateLives()
    {
        lives--;
        if (lives <= 0)
        {
            GameOver();
        }
        livesText.text = lives.ToString();
    }
    

    private void GameOver()
    {
        gameOverTexts.SetActive(true);
    }

}
