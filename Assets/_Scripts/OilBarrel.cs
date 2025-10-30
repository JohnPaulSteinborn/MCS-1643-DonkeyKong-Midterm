using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OilBarrel : MonoBehaviour
{
    public GameObject Fireball;
    public float spawnInterval = 5f;
    private bool activated = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!activated && collision.CompareTag("Obstacle"))
        {
            activated = true;
            SpawnFireball(); // Spawn the first one instantly
            InvokeRepeating(nameof(SpawnFireball), spawnInterval, spawnInterval);
        }
    }

    private void SpawnFireball()
    {
        Vector3 spawnOffset = new Vector3(1.0f, 1.0f, 0f);
        Vector3 spawnPosition = transform.position + spawnOffset;

        Instantiate(Fireball, spawnPosition, Quaternion.identity);
    }
}