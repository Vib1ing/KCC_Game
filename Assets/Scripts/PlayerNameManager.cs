using System;
using Unity.Services.Authentication;
using UnityEngine;
using TMPro;
using System.ComponentModel;
using UnityEngine.Rendering;

public class PlayerNameManager : MonoBehaviour
{
    public TMP_InputField nameInput;
    public TMP_Text currentNameText;
    public static string playerName;

    public static PlayerNameManager Instance;

    async void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Ensure the player name is loaded from PlayerPrefs
        playerName = PlayerPrefs.GetString("PlayerName", "Guest");
        loadName();
        try
        {
            await AuthenticationService.Instance.UpdatePlayerNameAsync(playerName);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to update player name: " + e.Message);
        }
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
        nameInput.text = string.Empty;

        string censoredName = await ProfanityDetector.Censor(playerName);

        if (!string.IsNullOrWhiteSpace(censoredName) && censoredName != playerName)
        {
            ProfanityMenu.Instance.Show(censoredName);
        }
        else
        {
            updateName();
        }
    }

    private async void updateName()
    {
        PlayerPrefs.SetString("PlayerName", playerName);
        loadName();
        try
        {
            await AuthenticationService.Instance.UpdatePlayerNameAsync(playerName);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to update player name: " + e.Message);
        }
        Debug.Log($"Player name set to: {playerName}");
    }

    public void updateName(string newName)
    {
        playerName = newName;
        updateName();
    }

    void loadName()
    {
        currentNameText.text = $"Current Name: {playerName}";
    }
}
