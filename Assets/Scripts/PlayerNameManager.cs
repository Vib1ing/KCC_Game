using System;
using Unity.Services.Authentication;
using UnityEngine;
using TMPro;

public class PlayerNameManager : MonoBehaviour
{
    public TMP_InputField nameInput;
    public TMP_Text currentNameText;
    public static string playerName;

    void Awake()
    {
        // Ensure the player name is loaded from PlayerPrefs
        playerName = PlayerPrefs.GetString("PlayerName", "Guest");
        loadName();
    }

    public async void setName()
    {
        if (string.IsNullOrWhiteSpace(nameInput.text))
        {
            Debug.LogWarning("Player name cannot be empty.");
            return;
        }
        else if (nameInput.text.Length > 20)
        {
            Debug.LogWarning("Player name cannot exceed 20 characters.");
            return;
        }
        playerName = nameInput.text.Trim();
        PlayerPrefs.SetString("PlayerName", playerName);
        loadName();
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

    void loadName()
    {
        currentNameText.text = $"Current Name: {playerName}";
    }
}
