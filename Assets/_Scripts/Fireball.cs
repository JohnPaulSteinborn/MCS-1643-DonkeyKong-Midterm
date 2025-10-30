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
    [Range(0f, 1f)] public float climbChance = 0.01f;

    private float climbTargetY; // ladder top
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        results = new Collider2D[4];
        direction = Vector2.right;

        // Ignore ladder collisions so the fireball can overlap triggers
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Ladder"), gameObject.layer, true);
    }

    private void Update()
    {
        CheckCollision();

        if (climbing)
        {
            rb.velocity = new Vector2(0, climbSpeed);

            // Stop climbing once at or above ladder top
            if (transform.position.y >= climbTargetY)
            {
                StopClimbing();
                FlipDirection();
            }
        }
        else
        {
            // Move horizontally while on ground or falling
            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);

            // Occasionally decide to climb
            if (canClimb && grounded && Random.value < climbChance)
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

        // Disable collision with ground while climbing
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Ground"), gameObject.layer, true);

        // Snap to center of the ladder
        Collider2D ladder = Physics2D.OverlapBox(transform.position, col.bounds.size, 0f, 1 << LayerMask.NameToLayer("Ladder"));
        if (ladder != null)
        {
            Bounds b = ladder.bounds;
            climbTargetY = b.max.y;
            transform.position = new Vector2(ladder.bounds.center.x, transform.position.y);
        }
        else
        {
            climbTargetY = transform.position.y + 2.0f;
        }
    }

    private void StopClimbing()
    {
        climbing = false;
        rb.gravityScale = 1;

        // Slightly nudge the fireball above the top of the ladder/platform
        transform.position = new Vector2(transform.position.x, transform.position.y + 0.3f);

        // Re-enable ground collision after a short delay (one frame)
        StartCoroutine(ReenableGroundCollisionNextFrame());
    }

    // Coroutine to ensure physics updates before re-enabling collision
    private System.Collections.IEnumerator ReenableGroundCollisionNextFrame()
    {
        yield return null; // wait one frame
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Ground"), gameObject.layer, false);
    }

    private void FlipDirection()
    {
        direction.x *= -1;
        transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Reverse direction if hitting walls or obstacles
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") ||
            collision.gameObject.CompareTag("Obstacle"))
        {
            FlipDirection();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (col == null) return;
        Gizmos.color = climbing ? Color.yellow : Color.cyan;
        Vector2 size = col.bounds.size;
        size.y += 0.1f;
        size.x /= 2f;
        Gizmos.DrawWireCube(transform.position, size);
    }
}
