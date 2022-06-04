using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    [SerializeField] private float speed = 500.0f;
    [SerializeField] private float maxLifetime = 10.0f;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public void Project(Vector2 direction)
    {
        _rigidbody.AddForce(direction * speed);
        Destroy(this.gameObject, maxLifetime);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        Destroy(this.gameObject);
    }
    
}
