using UnityEngine;

public class Fireball : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D col;
    private Collider2D[] results;

    private Vector2 direction;
    private bool grounded;
    private bool climbing;
    private bool canClimb;

    public float moveSpeed = 1.0f;
    public float climbSpeed = 1.0f;
    public float climbChance = 0.01f;

    // new: track ladder top height
    private float climbTargetY;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        results = new Collider2D[4];
        direction = Vector2.right;

        // Prevent any physical blocking with ladder triggers
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Ladder"), gameObject.layer, true);
    }

    private void Update()
    {
        CheckCollision();

        if (climbing)
        {
            // move upward until top of ladder
            rb.velocity = new Vector2(0, climbSpeed);

            if (transform.position.y >= climbTargetY)
            {
                StopClimbing();
                FlipDirection();  // choose a new horizontal direction
            }
        }
        else
        {
            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);

            // Random chance to start climbing if ladder available
            if (canClimb && Random.value < climbChance)
            {
                StartClimbing();
            }
        }
    }

    private void CheckCollision()
    {
        grounded = false;
        canClimb = false;

        Vector2 size = col.bounds.size;
        size.y += 0.1f;
        size.x /= 2f;

        int amount = Physics2D.OverlapBoxNonAlloc(transform.position, size, 0.0f, results);

        for (int i = 0; i < amount; i++)
        {
            GameObject hit = results[i].gameObject;

            if (hit.layer == LayerMask.NameToLayer("Ground"))
            {
                grounded = hit.transform.position.y < (transform.position.y - 0.5f);
            }
            else if (hit.layer == LayerMask.NameToLayer("Ladder"))
            {
                canClimb = true;
            }
        }
    }

    private void StartClimbing()
    {
        climbing = true;
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;

        // find the ladder directly underneath or overlapping
        Collider2D ladder = Physics2D.OverlapBox(transform.position, col.bounds.size, 0f, 1 << LayerMask.NameToLayer("Ladder"));
        if (ladder != null)
        {
            Bounds b = ladder.bounds;
            climbTargetY = b.max.y; // top edge of the ladder
        }
        else
        {
            // fallback: climb a fixed amount if we can’t detect a ladder top
            climbTargetY = transform.position.y + 2.0f;
        }
    }

    private void StopClimbing()
    {
        climbing = false;
        rb.gravityScale = 1;
    }

    private void FlipDirection()
    {
        direction.x *= -1;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            FlipDirection();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (col == null) return;
        Gizmos.color = Color.yellow;
        Vector2 size = col.bounds.size;
        size.y += 0.1f;
        size.x /= 2f;
        Gizmos.DrawWireCube(transform.position, size);
    }
}
