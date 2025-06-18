using UnityEngine;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public TMP_Text playButtonText;
    string playerName;
    void Start()
    {
        playerName = PlayerPrefs.GetString("PlayerName", "Guest");
        if (string.IsNullOrEmpty(playerName) || playerName == "Guest")
        {
            playButtonText.text = "Play as Guest";
        }
        else
        {
            playButtonText.text = "Play";
        }
    }
}
