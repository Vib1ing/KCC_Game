using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Circle : MonoBehaviour
{
    public float movSpeed = 10;
    float speedX, speedY;
    Rigidbody2D rb;

    private GameObject currentTeleporter;

    public Canvas canvas;
    public Canvas canvas1;
    public Canvas canvas2;
    public Canvas canvas3;
    public Canvas canvas4;

    public Canvas nameInput;

    // Timer variables
    bool timerActive = false;
    bool isFinished = false;
    float currentTime;
    public TextMeshProUGUI currentTimeText;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        canvas.enabled = false;
        canvas1.enabled = false;
        canvas2.enabled = false;
        canvas3.enabled = false;
        canvas4.enabled = false;

        nameInput.enabled = false;

        currentTime = 0;
    }

    void Update()
    {
        Movement();

        if (currentTeleporter != null)
        {
            transform.position = currentTeleporter.GetComponent<Teleporter>().getDestination().position;
        }

        if (timerActive)
        {
            currentTime += Time.unscaledDeltaTime;
        }

        TimeSpan timeSpan = TimeSpan.FromSeconds(currentTime);
        currentTimeText.text = timeSpan.ToString(@"mm\:ss\:fff");
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
        if (collision.CompareTag("Timer"))
        {
            if (isFinished)
            {
                isFinished = false;
                currentTime = 0;
            }
            timerActive = true;
            Debug.Log("Timer started!");
        }
        if (collision.CompareTag("OtherTimer"))
        {
            timerActive = false;
            isFinished = true;
            Debug.Log($"Timer stopped! Final time: {currentTime:0.000}s");

            // Submit the final time to the leaderboard
            UGSLeaderboardManager.SubmitPlayerScore(currentTime);
        }
        if (collision.CompareTag("NewThing"))
        {
            canvas.enabled = true;
        }
        if (collision.CompareTag("Fact1"))
        {
            canvas1.enabled = true;
        }
        if (collision.CompareTag("Fact2"))
        {
            canvas2.enabled = true;
        }
        if (collision.CompareTag("Fact3"))
        {
            canvas3.enabled = true;
        }
        if (collision.CompareTag("Fact4"))
        {
            canvas4.enabled = true;
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
        canvas1.enabled = false;
        canvas2.enabled = false;
        canvas3.enabled = false;
        canvas4.enabled = false;
    }
}



