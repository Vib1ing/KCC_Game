using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Stopwatch : MonoBehaviour
{
    float currentTime;
    public int startMinutes;
    public TextMeshProUGUI currentTimeText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentTime = startMinutes * 60;
    }

    // Update is called once per frame
    void Update()
    {
        currentTimeText.text = currentTime.ToString();
    }
}
