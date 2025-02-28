using UnityEngine;

public class Circle : MonoBehaviour
{
    public float movSpeed = 10;
    float speedX, speedY;
    Rigidbody2D rb;

    private GameObject currentTeleporter;

    public Canvas canvas;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        canvas.enabled = false;
    }

    void Update()
    {
        Movement();

        if (currentTeleporter != null)
        {
            transform.position = currentTeleporter.GetComponent<Teleporter>().getDestination().position;
        }
    }

    void Movement()
    {
        speedX = Input.GetAxisRaw("Horizontal") * movSpeed;
        speedY = Input.GetAxisRaw("Vertical") * movSpeed;
        rb.linearVelocity = new Vector2(speedX, speedY);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Teleporter"))
        {
            currentTeleporter = collision.gameObject;
        }
        if (collision.CompareTag("NewThing")){
            canvas.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Teleporter"))
        {
            if (collision.gameObject == currentTeleporter)
            {
                currentTeleporter = null;
            }
        }
        canvas.enabled = false;
    }
}
