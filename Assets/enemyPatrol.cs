using UnityEngine;

public class enemyPatrol : MonoBehaviour
{
    public GameObject pointA;
    public GameObject pointB;
    public Transform safeArea; // Reference to the safe area's position
    private Rigidbody2D rb;
    private Transform currentPoint;
    public float speed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentPoint = pointB.transform;
    }

    void Update()
    {
        // Calculate direction towards the current target point
        Vector2 direction = (currentPoint.position - transform.position).normalized;

        // Set velocity in the direction of the current target point
        rb.linearVelocity = direction * speed;

        // Check if close enough to the target point and switch points
        if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f)
        {
            currentPoint = (currentPoint == pointB.transform) ? pointA.transform : pointB.transform;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the player has touched the enemy
        if (collision.CompareTag("Player"))
        {
            // Move the player back to the safe area
            collision.transform.position = safeArea.position;
        }
    }
}
