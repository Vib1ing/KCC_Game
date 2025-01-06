using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float dirX, dirY;
    public int speed;
    public Transform safeArea;

    private new Rigidbody2D rb { get { return GetComponent<Rigidbody2D>() ?? default(Rigidbody2D); } }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dirX = Random.Range(-1f, 1f);
        dirY = Random.Range(-1f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    void Movement()
    {
        if (rb)
        {
            Vector2 vel = new Vector2(dirX * speed, dirY * speed);
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, vel, Time.deltaTime*10f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.position = safeArea.position;
        }

        if (collision.CompareTag("SafeArea"))
        {
            dirX *= -1;
        }
        
        if (collision.CompareTag("Wall"))
        {
            dirY *= -1;
        }
    }
}
