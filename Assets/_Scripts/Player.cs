using System;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Bullet bulletPreFab;
    [SerializeField] private float thrustSpeed = 1.0f;
    [SerializeField] private float turnSpeed = 1.0f;
    [SerializeField] private float respawnTime = 3.0f;
    [SerializeField] private float respawnInvulnerabilityTime = 3.0f;
    [SerializeField] private float rotationMomentum = 5.0f;
    [SerializeField] private float turnDirection;
    [SerializeField] private float angle;
    private bool _thrusting;
    private Rigidbody2D _rigidbody;

    public Action OnRespawn;

    private Vector2 lastClickedPos;
    private bool _isMoving;

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
            turnDirection = 1.0f;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            turnDirection = -1.0f;
        }
        else if (Input.GetMouseButton(0))
        {
            //2 options
            //Both do the same!
            
            //To use mouse location for the player to goto
            //GetPlayerToGoToMouseLocation();
            
            //To use with moving in direction of the mouse
            lastClickedPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _isMoving = true;
            ControlWithMouse();
        }
        else
        {
            turnDirection = 0.0f;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }

    }

    private void FixedUpdate()  //Physics goes here
    {
        if (_thrusting || Input.GetMouseButton(0))
        {
            _rigidbody.AddForce(this.transform.up * thrustSpeed);
        }

        if (turnDirection != 0.0f)
        {
            _rigidbody.AddTorque(turnDirection * turnSpeed);
        }
    }

    /// <summary>
    /// Makes the player move to the point in the 2d world. Left click and keep depressed to have the player follow
    /// the cursor
    /// </summary>
    private void GetPlayerToGoToMouseLocation()
    {
        var mousePositionInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var playerdirection = transform.up.normalized;
        var positionFromMouseToPlayer = mousePositionInWorld - transform.position;
        if (Vector3.Angle(transform.right, positionFromMouseToPlayer - playerdirection) > 90f) turnDirection = 1.0f;
        else turnDirection = -1.0f;
        Debug.DrawRay(transform.position, transform.up.normalized * 100, Color.green);
        Debug.DrawRay(transform.position, positionFromMouseToPlayer, Color.red);
    }

    /// <summary>
    /// Makes the player move to the point in the 2d world. Left click and keep depressed to have the player follow
    /// the cursor
    /// </summary>
    private void ControlWithMouse() // For click on point and rotate and move player in this direction
    {
        if (_isMoving && (Vector2) transform.position != lastClickedPos)
        {
            float step = thrustSpeed * Time.deltaTime;
            transform.position = Vector2.Lerp(transform.position, lastClickedPos, step);

            var direction = lastClickedPos - (Vector2)transform.position;

            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
            Quaternion angleAxis = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, angleAxis, Time.deltaTime * rotationMomentum);
        }else _isMoving = false;
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
