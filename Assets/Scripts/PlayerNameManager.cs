using System;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameManager : MonoBehaviour
{
    public InputField nameInput;
    public static string playerName;

    void Awake()
    {
        // Ensure the player name is loaded from PlayerPrefs
        playerName = PlayerPrefs.GetString("PlayerName", "Guest");
    }

    public async void setName()
    {
        if (string.IsNullOrWhiteSpace(nameInput.text))
        {
            Debug.LogWarning("Player name cannot be empty.");
            return;
        }
        playerName = nameInput.text.Trim();
        PlayerPrefs.SetString("PlayerName", playerName);
        try
        {
            await AuthenticationService.Instance.UpdatePlayerNameAsync(playerName);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to update player name: " + e.Message);
            return;
        }
        Debug.Log($"Player name set to: {playerName}");
    }
}
