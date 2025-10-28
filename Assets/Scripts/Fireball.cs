using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fireball : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool climbing;
    private bool nearLadder;
    private float climbDirection = 1f;

    public float moveSpeed = 1.5f;
    public float climbSpeed = 1.0f;
    public LayerMask ladderLayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void StartClimbing(Vector3 ladderPosition)
    {
        climbing = true;
        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero;

        // Snap roughly to ladder center
        transform.position = new Vector3(ladderPosition.x, transform.position.y, transform.position.z);
    }

    private void StopClimbing()
    {
        climbing = false;
        rb.gravityScale = 1f;

        // Randomly choose new direction (left or right)
        float direction = Random.value < 0.5f ? -1f : 1f;
        moveSpeed = Mathf.Abs(moveSpeed) * direction;
    }

    private void Update()
    {
        if (climbing)
        {
            // Move upward
            rb.velocity = new Vector2(0, climbSpeed * climbDirection);
        }
        else
        {
            // Regular rolling
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
        }
        
        if (nearLadder && !climbing && Random.value < 0.001f)
        {
            StartClimbing(transform.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ladder"))
        {
            nearLadder = true;

            // Random chance to climb
            if (Random.value < 0.3f) // 30% chance
            {
                StartClimbing(collision.transform.position);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ladder"))
        {
            // Only stop climbing if there’s ground under the fireball
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, LayerMask.GetMask("Ground"));
            if (hit.collider != null)
            {
                StopClimbing();
            }
        }
    }
}