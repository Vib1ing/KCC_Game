using UnityEngine;

public class enemyPatrolWithProjectile : MonoBehaviour
{
    public Transform[] patrolPoints;
    public int targetPoint;
    public float speed;
    public Transform safeArea;

    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 5f;
    public float projectileSize = 0.5f;
    public float fireRate = 2f;
    public float spawnRadius = 1f;
    public float projectileLifetime = 5f;

    private float nextFireTime;

    void Start()
    {
        targetPoint = 0;
        nextFireTime = Time.time + fireRate;

        if (patrolPoints.Length == 0)
        {
            Debug.LogError("No patrol points assigned to " + gameObject.name);
        }
    }

    void Update()
    {
        Patrol();

        // Fire projectiles at intervals
        if (Time.time >= nextFireTime)
        {
            SpawnProjectile();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        if (Vector3.Distance(transform.position, patrolPoints[targetPoint].position) < 0.1f)
        {
            IncrementTargetPoint();
        }
        transform.position = Vector3.MoveTowards(transform.position, patrolPoints[targetPoint].position, speed * Time.deltaTime);
    }

    void IncrementTargetPoint()
    {
        if (patrolPoints.Length == 0) return;
        targetPoint = (targetPoint + 1) % patrolPoints.Length;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Enemy touched player! Sending to safe area.");
            collision.transform.position = safeArea.position;
        }
    }

    void SpawnProjectile()
    {
        // Generate a random offset around the enemy
        Vector2 randomOffset = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 spawnPosition = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0);

        // Instantiate the projectile
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        projectile.transform.localScale = Vector3.one * projectileSize;

        // Add Rigidbody2D if not already present
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = projectile.AddComponent<Rigidbody2D>();
        }

        rb.gravityScale = 0;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Set projectile velocity
        Vector2 direction = (spawnPosition - transform.position).normalized;
        rb.linearVelocity = direction * projectileSpeed;

        // Add CircleCollider2D if not already present
        CircleCollider2D collider = projectile.GetComponent<CircleCollider2D>();

        if (collider == null)
        {
            collider = projectile.AddComponent<CircleCollider2D>();
        }

        collider.isTrigger = false; // Allow bouncing

        // Assign bouncing physics
        PhysicsMaterial2D bounceMaterial = new PhysicsMaterial2D { bounciness = 1f, friction = 0f };
        collider.sharedMaterial = bounceMaterial;

        // Attach a script to handle collisions with the player
        projectile.AddComponent<ProjectileHandler>().safeArea = safeArea;

        // Destroy projectile after some time
        Destroy(projectile, projectileLifetime);
    }
}

public class ProjectileHandler : MonoBehaviour
{
    public Transform safeArea;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Projectile hit player! Sending to safe area.");
            collision.transform.position = safeArea.position;
        }
    }

}
