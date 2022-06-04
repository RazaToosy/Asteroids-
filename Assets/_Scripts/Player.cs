using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Bullet bulletPreFab;
    [SerializeField] private float thrustSpeed = 1.0f;
    [SerializeField] private float turnSpeed = 1.0f;
    [SerializeField] private float respawnTime = 3.0f;
    [SerializeField] private float respawnInvulnerabilityTime = 3.0f;
    private bool _thrusting;
    private float _turnDirection;
    private Rigidbody2D _rigidbody;

    public Action OnRespawn;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        OnRespawn += Respawn;
        GameManager.OnPlayerRespawn = OnRespawn;
    }


    // Update is called once per frame
    private void Update()
    {
        _thrusting = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            _turnDirection = 1.0f;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            _turnDirection = -1.0f;
        }
        else
        {
            _turnDirection = 0.0f;
        }
        
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    private void FixedUpdate()  //Physics goes here
    {
        if (_thrusting)
        {
            _rigidbody.AddForce(this.transform.up * thrustSpeed);
        }

        if (_turnDirection != 0.0f)
        {
            _rigidbody.AddTorque(_turnDirection * turnSpeed);
        }
    }

    private void Shoot()
    {
        var transform1 = this.transform;
        var bullet = Instantiate(bulletPreFab, transform1.position, transform1.rotation);
        bullet.Project(this.transform.up);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Asteroid"))
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = 0.0f;
            this.gameObject.SetActive(false);
            PlayerDied();
        }
    }
    
    public void PlayerDied()
    {
        int? lives = GameManager.OnPlayExplosion?.Invoke(this.gameObject, this.transform.position);
        if (lives > 0) Invoke(nameof(Respawn), respawnTime);  
    }

    private void Respawn()
    
    {        
        this.transform.position = Vector3.zero;
        this.gameObject.layer = LayerMask.NameToLayer("IgnoreCollisions");
        this.gameObject.SetActive(true);
        Invoke(nameof(TurnOnCollisions), respawnInvulnerabilityTime);
    }

    private void TurnOnCollisions()
    {
        this.gameObject.layer = LayerMask.NameToLayer("Player");
    }
}
