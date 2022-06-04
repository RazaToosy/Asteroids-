using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Random = UnityEngine.Random;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private float size = 1.0f;
    [SerializeField] private float minSize = 0.5f;
    [SerializeField] private float maxSize = 1.5f;
    [SerializeField] private float speed = 50.0f;
    [SerializeField] private float maxLifeTime = 30.0f;
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody;


    public float Size
    {
        get => size;
        set => size = value;
    }

    public float MinSize
    {
        get => minSize;
    }

    public float MaxSize
    {
        get => maxSize;
    }

    public float MaxLifeTime
    {
        get => maxLifeTime;
        set => maxLifeTime = value;
    }


    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        _spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
        var transformAsteroid = this.transform;
        transformAsteroid.eulerAngles = new Vector3(0.0f, 0.0f, Random.value * 360.0f);
        transformAsteroid.localScale = Vector3.one * this.Size;
        _rigidbody.mass = Size;
    }

    public void SetTragectory(Vector2 direction)
    {
        _rigidbody.AddForce(direction * speed);
        Destroy(this.gameObject, MaxLifeTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            if ((this.Size * 0.5f) >= this.MinSize)
            {
                CreateSplit();
                CreateSplit();
            }
            AsteroidDestroyed();
            //FindObjectOfType<GameManager>().AsteroidDestroyed(this);
            Destroy(this.gameObject);
        }
    }
    
    public void AsteroidDestroyed()
    {
        //asteroid.transform.position = asteroid.transform.position;
        int? lives = GameManager.OnPlayExplosion?.Invoke(this.gameObject, this.transform.position);

        int score = 0;

        if (size < 0.75f) score = 100;
        else if (size < 1.2f) score = 50;
        else score = 25;
        
        GameManager.OnScoreUpdate?.Invoke(score);
    }

    private void CreateSplit()
    {
        Vector2 position = this.transform.position;
        position += Random.insideUnitCircle * 0.5f;
        Asteroid half = Instantiate(this, position, this.transform.rotation);
        half.Size = this.Size * 0.5f;
        half.SetTragectory(Random.insideUnitCircle.normalized * this.speed);
    }
}
