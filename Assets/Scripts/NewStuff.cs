using UnityEngine;

public class NewStuff : MonoBehaviour
{
    public Canvas canvas;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvas.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision){
        if (collision.CompareTag("NewThing")){
            canvas.enabled = true;
        }
    }

    private void OnTriggerExit2D(){
        canvas.enabled = false;
    }
}
