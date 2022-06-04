using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [SerializeField] private Asteroid asteroidPreFab;
    [SerializeField] private float spawnRate = 2.0f;
    [SerializeField] private int spawnAmount = 1;
    [SerializeField] private float spawnDistance = 15.0f;
    [SerializeField] private float trajectoryVariance = 15.0f;
    
    
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(spawn), this.spawnRate, this.spawnRate);    
    }

    private void spawn()
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            Vector3 spawnDirection = Random.insideUnitCircle.normalized * spawnDistance;
            Vector3 spawnPoint = this.transform.position + spawnDirection;

            float variance = Random.Range(-trajectoryVariance, trajectoryVariance);
            Quaternion rotation = Quaternion.AngleAxis(variance, Vector3.forward);

            Asteroid asteroid = Instantiate(asteroidPreFab, spawnPoint, rotation);
            asteroid.Size = Random.Range(asteroid.MinSize, asteroid.MaxSize);
            asteroid.SetTragectory(rotation * -spawnDirection);
        }
    }

}
